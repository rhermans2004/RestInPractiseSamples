using System;
using System.Runtime.Serialization;
using System.ServiceModel.Syndication;
using Restbucks.WcfRestToolkit.Http.HeaderValues;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;
using Restbucks.WcfRestToolkit.Utility;

namespace Restbucks.OrderFulfillment.IntegrationTests.Utility
{
    public class Cashier
    {
        private readonly string name;
        private readonly Uri uri;

        public Cashier(string name, Uri uri)
        {
            this.name = name;
            this.uri = uri;
        }

        public MemberWrapper SubmitOrder(Order order)
        {
            Member member = new Member
                            {
                                Title = SyndicationContent.CreatePlaintextContent("order"),
                                LastUpdatedTime = DateTimeOffset.Now,
                                Content = new XmlSyndicationContent(MediaTypes.Restbucks.TypeAndSubtype, order, null as DataContractSerializer)
                            };
            member.Authors.Add(new SyndicationPerson {Name = name});
            member.Draft = DraftStatus.Yes;

            Atom10ItemFormatter formatter = member.GetAtom10Formatter();

            Response response = Request.ForPost(uri, MediaTypes.AtomEntry.TypeAndSubtype, formatter).Execute();
            Atom10ItemFormatter<Member> newFormatter = new Atom10ItemFormatter<Member>();
            newFormatter.ReadFrom(response.EntityBody.CreateReader());

            return new MemberWrapper((Member) newFormatter.Item, response.StatusCode, EntityTag.Parse( response.ETag));
        }

        public Response UpdateOrder(MemberWrapper memberWrapper)
        {
            Member member = memberWrapper.Member;
            
            return Request.ForPut(member.GetEditLink().GetAbsoluteUri(), MediaTypes.AtomEntry.TypeAndSubtype, memberWrapper.EntityTag, new Atom10ItemFormatter<Member>(member)).Execute();
        }

        public MemberWrapper GetLatestVersionOfOrder(Member member)
        {
            Response response = Request.ForGet(member.GetEditLink().GetAbsoluteUri()).Execute();

            Atom10ItemFormatter<Member> formatter = new Atom10ItemFormatter<Member>();
            formatter.ReadFrom(response.EntityBody.CreateReader());

            return new MemberWrapper((Member)formatter.Item, response.StatusCode, EntityTag.Parse(response.ETag));
        }

        public Response CancelOrder(MemberWrapper memberWrapper)
        {
            return Request.ForDelete(memberWrapper.Member.GetEditLink().GetAbsoluteUri()).Execute();
        }
    }
}