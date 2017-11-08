using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net.Config;
using Restbucks.OrderFulfillment.Commands;
using Restbucks.OrderFulfillment.Model;
using Restbucks.WcfRestToolkit;

namespace Restbucks.OrderFulfillment.Host
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            IWindsorContainer container = new WindsorContainer();

            container.Register(Component.For(typeof (OrderFulfillmentService)).LifeStyle.Transient);
            container.Register(Component.For(typeof (IRepository)).ImplementedBy(typeof (Repository)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (IIdGenerator)).ImplementedBy(typeof (IdGenerator)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (IDateTimeProvider)).ImplementedBy(typeof (DateTimeProvider)).LifeStyle.Singleton);
            container.Register(Component.For(typeof (CommandFactory)).ImplementedBy(typeof (CommandFactory)).LifeStyle.Singleton);

            WebServiceHost svcHost = new WindsorWebServiceHost(typeof (OrderFulfillmentService), new Uri("http://localhost/fulfillment"), container);
            
            try
            {
                svcHost.Open();

                Console.WriteLine("Service is running");
                Console.WriteLine("Press enter to quit...");
                Console.ReadLine();

                svcHost.Close();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine("An exception occurred: {0}", ex.Message);
                svcHost.Abort();
                Console.ReadLine();
            }
        }
    }
}