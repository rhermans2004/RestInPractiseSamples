using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Castle.Windsor;
using log4net;

namespace Restbucks.WcfRestToolkit.ServiceHosting
{
    public class WindsorInstanceProvider : IInstanceProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly IWindsorContainer container;
        private readonly Type serviceType;

        public WindsorInstanceProvider(IWindsorContainer container, Type serviceType)
        {
            this.container = container;
            this.serviceType = serviceType;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            Log.DebugFormat("Getting instance of type [{0}].", serviceType.FullName);
            return container.Resolve(serviceType);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            if (instance is IDisposable)
            {
                Log.DebugFormat("Calling Dispose() on instance of type [{0}].", instance.GetType().FullName);
                ((IDisposable)instance).Dispose();
            }
        }
    }
}