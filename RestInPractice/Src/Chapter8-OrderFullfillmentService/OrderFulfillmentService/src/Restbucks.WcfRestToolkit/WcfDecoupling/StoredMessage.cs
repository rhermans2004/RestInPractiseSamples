using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Restbucks.WcfRestToolkit.WcfDecoupling
{
    public class StoredMessage : IExtension<OperationContext>
    {
        private readonly Message message;

        public StoredMessage(Message message)
        {
            this.message = message;
        }

        public void Attach(OperationContext owner)
        {
            //Do nothing
        }

        public void Detach(OperationContext owner)
        {
            //Do nothing
        }

        public Message Message
        {
            get { return message; }
        }
    }
}