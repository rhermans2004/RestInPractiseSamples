using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;

namespace Restbucks.OrderFulfillment
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    [ServiceKnownType(typeof (Atom10FeedFormatter))]
    [ServiceKnownType(typeof (Atom10ItemFormatter))]
    public interface IOrderFulfillmentService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/")]
        Atom10FeedFormatter<Collection> GetCollection();

        [OperationContract]
        [WebGet(UriTemplate = "/{id}")]
        Atom10ItemFormatter<Member> GetMember(string id);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/")]
        Atom10ItemFormatter<Member> CreateMember(Atom10ItemFormatter member);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{id}")]
        void UpdateMember(string id, Atom10ItemFormatter<Member> member);

        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{id}")]
        void DeleteMember(string id);
    }
}