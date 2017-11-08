using Restbucks.WcfRestToolkit.Http.HeaderValues;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;

namespace Restbucks.OrderFulfillment.IntegrationTests.Utility
{
    public class MemberWrapper
    {
        private readonly Member member;
        private readonly int statusCode;
        private readonly EntityTag entityTag;

        public MemberWrapper(Member member, int statusCode, EntityTag entityTag)
        {
            this.member = member;
            this.statusCode = statusCode;
            this.entityTag = entityTag;
        }

        public Member Member
        {
            get { return member; }
        }

        public int StatusCode
        {
            get { return statusCode; }
        }

        public EntityTag EntityTag
        {
            get { return entityTag; }
        }
    }
}