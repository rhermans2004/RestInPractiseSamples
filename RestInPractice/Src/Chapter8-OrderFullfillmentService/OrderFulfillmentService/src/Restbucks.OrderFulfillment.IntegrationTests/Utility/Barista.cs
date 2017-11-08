using System;
using System.Linq;
using System.ServiceModel.Syndication;
using Restbucks.WcfRestToolkit.Http.HeaderValues;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;
using Restbucks.WcfRestToolkit.Utility;

namespace Restbucks.OrderFulfillment.IntegrationTests.Utility
{
    public class Barista
    {
        private readonly Uri uri;

        public Barista(Uri uri)
        {
            this.uri = uri;
        }

        public Collection GetFulfillmentCollection()
        {
            Response response = Request.ForGet(uri).Execute();
            Atom10FeedFormatter<Collection> formatter = new Atom10FeedFormatter<Collection>();

            formatter.ReadFrom(response.EntityBody.CreateReader());
            return (Collection) formatter.Feed;
        }

        public Member IdentifyNextOrderAwaitingFulfillment(Collection collection)
        {
            return IdentifyNextOrderAwaitingFulfillment(collection, null);
        }

        public Member IdentifyNextOrderAwaitingFulfillment(Collection collection, Member ignoreMember)
        {
            foreach (Member member in collection.Items.Reverse())
            {
                if (member.Draft.Equals(DraftStatus.Yes))
                {
                    if (ignoreMember == null || member.Id != ignoreMember.Id)
                    {
                        return member;
                    }
                }
            }
            return null;
        }

        public MemberWrapper GetLatestVersionOfOrder(Member member)
        {
            Response response = Request.ForGet(member.GetEditLink().GetAbsoluteUri()).Execute();

            Atom10ItemFormatter<Member> formatter = new Atom10ItemFormatter<Member>();
            formatter.ReadFrom(response.EntityBody.CreateReader());

            return new MemberWrapper((Member) formatter.Item, response.StatusCode, EntityTag.Parse(response.ETag));
        }

        public Response ReserveOrder(MemberWrapper memberWrapper)
        {
            Member member = memberWrapper.Member;
            member.Draft = DraftStatus.No;

            return Request.ForPut(member.GetEditLink().GetAbsoluteUri(), MediaTypes.AtomEntry.TypeAndSubtype, memberWrapper.EntityTag, new Atom10ItemFormatter<Member>(member)).Execute();
        }

        public Response CompleteOrder(MemberWrapper memberWrapper)
        {
            return Request.ForDelete(memberWrapper.Member.GetEditLink().GetAbsoluteUri()).Execute();
        }
    }
}