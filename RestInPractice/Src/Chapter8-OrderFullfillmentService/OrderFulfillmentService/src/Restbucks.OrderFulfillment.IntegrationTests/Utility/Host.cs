using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Restbucks.OrderFulfillment.Commands;
using Restbucks.OrderFulfillment.Model;
using Restbucks.WcfRestToolkit;

namespace Restbucks.OrderFulfillment.IntegrationTests.Utility
{
    public class Host : IDisposable
    {
        private readonly WebServiceHost svcHost;

        public Host(Uri uri)
        {
            IWindsorContainer container = new WindsorContainer();

            container.Register(Component.For(typeof (OrderFulfillmentService)).LifeStyle.Transient);
            container.Register(Component.For(typeof (IRepository)).ImplementedBy(typeof (Repository)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (IIdGenerator)).ImplementedBy(typeof (IdGenerator)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (IDateTimeProvider)).ImplementedBy(typeof (DateTimeProvider)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (CommandFactory)).ImplementedBy(typeof (CommandFactory)).LifeStyle.Singleton);

            svcHost = new WindsorWebServiceHost(typeof (OrderFulfillmentService), uri, container);
        }

        public void Start()
        {
            try
            {
                svcHost.Open();
            }
            catch (CommunicationException)
            {
                svcHost.Abort();
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                svcHost.Close();
            }
            catch (CommunicationException)
            {
                svcHost.Abort();
                throw;
            }
        }

        public void Dispose()
        {
            if (svcHost.State.Equals(CommunicationState.Closed) || svcHost.State.Equals(CommunicationState.Faulted))
            {
                return;
            }

            try
            {
                svcHost.Close();
            }
            catch (CommunicationException)
            {
                svcHost.Abort();
                throw;
            }
        }
    }
}