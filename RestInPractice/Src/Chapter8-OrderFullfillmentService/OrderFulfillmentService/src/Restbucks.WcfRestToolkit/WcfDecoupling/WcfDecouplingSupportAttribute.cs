using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Restbucks.WcfRestToolkit.WcfDecoupling
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class WcfDecouplingSupportAttribute : Attribute, IServiceBehavior
    {
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            //Do nothing
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            //Do nothing
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            IEnumerable<DispatchRuntime> runtimes = (from ChannelDispatcher cd in serviceHostBase.ChannelDispatchers
                                                     from EndpointDispatcher e in cd.Endpoints
                                                     select e.DispatchRuntime);
            foreach (DispatchRuntime dr in runtimes)
            {
                dr.MessageInspectors.Add(new StoreMessage());
            }
        }
    }
}