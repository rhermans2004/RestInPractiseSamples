using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Restbucks.WcfRestToolkit.Syndication
{
    public class AllowXmlSubTypeExtensionAttribute : Attribute, IServiceBehavior
    {
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ServiceEndpoint endpoint in serviceHostBase.Description.Endpoints)
            {
                CustomBinding binding = new CustomBinding(endpoint.Binding);
                WebMessageEncodingBindingElement bindingElement = binding.Elements.Find<WebMessageEncodingBindingElement>();
                bindingElement.ContentTypeMapper = new XmlSubTypeExtensionContentTypeMapper();
                endpoint.Binding = binding;
            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}