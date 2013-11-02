using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RealTimeSignalR.Models;
using System.Web.Http.OData;

namespace RealTimeSignalR.Controllers
{
    public class EntitySetControllerWithHub<THub> : EntitySetController<Employee, int>
        where THub : IHub
    {
        Lazy<IHubContext> hub = new Lazy<IHubContext>(
            () => GlobalHost.ConnectionManager.GetHubContext<THub>());

        protected IHubContext Hub
        {
            get { return hub.Value; }
        }
    }
}