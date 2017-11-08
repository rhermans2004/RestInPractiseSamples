using System;
using System.Runtime.Serialization;
using System.ServiceModel.Syndication;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Utility
{
    public class MemberBuilder
    {
        public static Member NewMember()
        {
            return new MemberBuilder().Build();
        }

        public static Member WithoutAuthor()
        {
            MemberBuilder builder = new MemberBuilder();
            builder.author = null;
            return builder.Build();
        }

        public static Member WithoutContent()
        {
            MemberBuilder builder = new MemberBuilder();
            builder.content = null;
            return builder.Build();
        }

        private SyndicationPerson author;
        private XmlSyndicationContent content;
        private DateTimeOffset lastUpdatedDateTime;

        public MemberBuilder()
        {
            author = new SyndicationPerson("author");
            content = new XmlSyndicationContent(MediaTypes.Restbucks.TypeAndSubtype, new Order(), null as DataContractSerializer);
            lastUpdatedDateTime = DateTime.Now;
        }

        public MemberBuilder WithAuthor(string value)
        {
            author = new SyndicationPerson {Name = value};
            return this;
        }

        public MemberBuilder WithLastUpdatedDateTime(DateTimeOffset value)
        {
            lastUpdatedDateTime = value;
            return this;
        }

        public MemberBuilder WithContent(object value)
        {
            content = new XmlSyndicationContent(MediaTypes.Restbucks.TypeAndSubtype, value, null as DataContractSerializer);
            return this;
        }

        public Member Build()
        {
            Member member = new Member {LastUpdatedTime = lastUpdatedDateTime};
            if (author != null)
            {
                member.Authors.Add(author);
            }
            if (content != null)
            {
                member.Content = content;
            }
            return member;
        }
    }
}