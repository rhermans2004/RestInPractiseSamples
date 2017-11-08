using System.ServiceModel;
using System.ServiceModel.Syndication;
using Restbucks.OrderFulfillment.Commands;
using Restbucks.WcfRestToolkit;
using Restbucks.WcfRestToolkit.Syndication;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;
using Restbucks.WcfRestToolkit.WcfDecoupling;

namespace Restbucks.OrderFulfillment
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerCall)]
    [WcfDecouplingSupport]
    [AllowXmlSubTypeExtension]
    public class OrderFulfillmentService : IOrderFulfillmentService
    {
        private readonly CommandFactory commands;

        public OrderFulfillmentService(CommandFactory commands)
        {
            this.commands = commands;
        }

        public Atom10FeedFormatter<Collection> GetCollection()
        {
            return Request.Handle(
                (request, parameters) =>
                commands.GetFulfillmentCollection().Execute(request, parameters));
        }

        public Atom10ItemFormatter<Member> GetMember(string id)
        {
            return Request.Handle(
                (request, parameters) =>
                commands.GetFulfillment().Execute(request, parameters));
        }

        public Atom10ItemFormatter<Member> CreateMember(Atom10ItemFormatter member)
        {
            return RequestWithEntityBody.Handle<Atom10ItemFormatter, Atom10ItemFormatter<Member>>(
                (request, parameters) =>
                commands.CreateFulfillment().Execute(request, parameters));
        }

        public void UpdateMember(string id, Atom10ItemFormatter<Member> member)
        {
            RequestWithEntityBody.Handle<Atom10ItemFormatter<Member>>(
                (request, parameters) =>
                commands.UpdateFulfillment().Execute(request, parameters));
        }

        public void DeleteMember(string id)
        {
            Request.Handle(
                (request, parameters) =>
                commands.DeleteFulfillment().Execute(request, parameters));
        }
    }
}