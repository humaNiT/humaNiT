/// <reference path="../Scripts/knockout-2.3.0.debug.js" />
var EmployeeViewModel = function (employeeSignalR) {
    var self = this;
    self.employees = ko.observableArray();
    self.loading = ko.observable(true);
    self.displayMode = function (employee) {
        if (employee.Locked()) {
            return 'lock-template';
        } else {
            return employee.Edit() ? 'edit-template' : 'read-template';
        }
    }
    self.edit = function (employee) {
        employee.Edit(true);
        employeeSignalR.server.lock(employee.Id);
    }
    self.done = function (employee) {
        employee.Edit(false);
        employeeSignalR.server.unlock(employee.Id);
    }

    self.watchModel = function (model, callback) {
        for (var key in model) {
            if (model.hasOwnProperty(key) && ko.isObservable(model[key]) && key != 'Edit' && key != 'Locked') {
                self.subscribeToProperty(model, key, function (key, val) {
                    callback(model, key, val);
                });
            }
        }
    };

    self.subscribeToProperty = function(model, key, callback) {
        model[key].subscribe(function (val) {
            callback(key, val);
        });
    };

    self.modelChanged = function (model, key, val) {
        var payload = {};
        payload[key] = val;
        $.ajax({
            url: '/odata/Employees(' + model.Id + ')',
            type: 'PATCH',
            data: JSON.stringify(payload),
            contentType: 'application/json',
            dataType: 'json'
        });
    }

    $.getJSON('/odata/Employees', function (data) {
        self.employees(ko.utils.arrayMap(data.value, function (employee) {
            var obsEmployee = {
                Id: employee.Id,
                Name: ko.observable(employee.Name),
                Email: ko.observable(employee.Email),
                Salary: ko.observable(employee.Salary),
                Edit: ko.observable(false),
                Locked: ko.observable(employee.Locked)
            }
            self.watchModel(obsEmployee, self.modelChanged);
            return obsEmployee;
        }));
        self.loading(false);
    });
};

$(function () {
    var employeeSignalR = $.connection.employee;
    var viewModel = new EmployeeViewModel(employeeSignalR);

    var findEmployee = function (id) {
        return ko.utils.arrayFirst(viewModel.employees(), function (item) {
            if (item.Id == id) {
                return item;
            }
        });
    }

    employeeSignalR.client.updateEmployee = function (id, key, value) {
        var employee = findEmployee(id);
        employee[key](value);
    };

    employeeSignalR.client.lockEmployee = function (id) {
        var employee = findEmployee(id);
        employee.Locked(true);
    };

    employeeSignalR.client.unlockEmployee = function (id) {
        var employee = findEmployee(id);
        employee.Locked(false);
    };

    $.connection.hub.start().done(function () {
        ko.applyBindings(viewModel);
    })
});