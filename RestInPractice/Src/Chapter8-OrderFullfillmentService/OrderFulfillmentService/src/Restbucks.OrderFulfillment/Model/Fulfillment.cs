using System;
using System.ServiceModel.Syndication;
using System.Xml;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;
using Restbucks.WcfRestToolkit.Utility;

namespace Restbucks.OrderFulfillment.Model
{
    public class Fulfillment
    {
        private readonly Member member;

        public Fulfillment(Member member)
        {
            this.member = member;
        }

        public Fulfillment(Guid id, DateTimeOffset createdDateTime, SyndicationContent content, Uri baseUri, string author)
        {
            member = new Member
                     {
                         Title = SyndicationContent.CreatePlaintextContent("order"),
                         Id = new UniqueId(id).ToString(),
                         LastUpdatedTime = createdDateTime,
                         EditedDateTime = createdDateTime,
                         Draft = DraftStatus.Yes,
                         Content = content
                     };
            member.Authors.Add(new SyndicationPerson {Name = author});
            member.Links.Add(new EditLink(baseUri, id).ToSyndicationLink());
        }

        public Guid Id
        {
            get
            {
                Guid id;
                new UniqueId(member.Id).TryGetGuid(out id);
                return id;
            }
        }

        public DateTimeOffset EditedDateTime
        {
            get { return member.EditedDateTime; }
            set { member.EditedDateTime = value; }
        }

        public void DoAction(Action<Member> action)
        {
            action((Member) member.Clone());
        }

        public Atom10ItemFormatter<Member> GetAtomFormatter()
        {
            return new Atom10ItemFormatter<Member>(member);
        }

        public Uri EditUri
        {
            get { return member.GetEditLink().GetAbsoluteUri(); }
        }

        public Fulfillment Edit(Member editedMember, DateTimeOffset editedDateTime)
        {
            if (member.Draft.Equals(DraftStatus.No))
            {
                throw new InvalidOperationException("Fulfillment can no longer be edited.");
            }
            
            member.EditedDateTime = editedDateTime;
            member.Draft = editedMember.Draft;
            member.Content = editedMember.Content;

            return this;
        }
    }
}