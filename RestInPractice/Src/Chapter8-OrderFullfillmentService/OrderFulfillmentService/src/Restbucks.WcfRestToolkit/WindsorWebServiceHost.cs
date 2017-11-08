using System;
using System.ServiceModel.Web;
using Castle.Windsor;
using Restbucks.WcfRestToolkit.ServiceHosting;

namespace Restbucks.WcfRestToolkit
{
    public class WindsorWebServiceHost : WebServiceHost
    {
        private readonly IWindsorContainer container;

        public WindsorWebServiceHost(Type serviceType, Uri baseAddress, IWindsorContainer container) : base(serviceType, baseAddress)
        {
            this.container = container;
        }

        protected override void OnOpening()
        {
            Description.Behaviors.Add(new WindsorServiceBehavior(container));
            base.OnOpening();
        }
    }
}