/// <reference path="knockout-2.3.0.debug.js" />
ko.bindingHandlers.displayUang = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        $(element).html('Rp ' + valueAccessor()().toFixed(2));
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        $(element).html('Rp ' + valueAccessor()().toFixed(2));
    }
}