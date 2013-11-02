using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RealTimeSignalR.Models;
using System.Collections.Concurrent;

namespace RealTimeSignalR.Hubs
{
    [HubName("employee")]
    public class EmployeeHub : Hub
    {
        private RealTimeSignalRContext db = new RealTimeSignalRContext();
        private static ConcurrentDictionary<string, List<int>> _mapping = new ConcurrentDictionary<string, List<int>>();

        public override System.Threading.Tasks.Task OnConnected()
        {
            _mapping.TryAdd(Context.ConnectionId, new List<int>());
            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected()
        {
            foreach (var id in _mapping[Context.ConnectionId])
            {
                var employeeToPatch = db.Employees.Find(id);
                employeeToPatch.Locked = false;
                db.Entry(employeeToPatch).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                Clients.Others.unlockEmployee(id);
            }
            var list = new List<int>();
            _mapping.TryRemove(Context.ConnectionId, out list);
            return base.OnDisconnected();
        }

        public void Lock(int id)
        {
            var employeeToPatch = db.Employees.Find(id);
            employeeToPatch.Locked = true;
            db.Entry(employeeToPatch).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            Clients.Others.lockEmployee(id);
            _mapping[Context.ConnectionId].Add(id);
        }
        public void Unlock(int id)
        {
            var employeeToPatch = db.Employees.Find(id);
            employeeToPatch.Locked = false;
            db.Entry(employeeToPatch).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            Clients.Others.unlockEmployee(id);
            _mapping[Context.ConnectionId].Remove(id);
        }
    }
}