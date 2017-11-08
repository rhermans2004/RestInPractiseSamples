using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Restbucks.WcfRestToolkit.WcfDecoupling
{
    public class StoreMessage : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
            OperationContext.Current.Extensions.Add(new StoredMessage(buffer.CreateMessage()));
            request = buffer.CreateMessage();
            buffer.Close();

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            //Do nothing
        }
    }
}