using System.Runtime.Serialization;

namespace Restbucks.WcfRestToolkit.Syndication.AtomPub
{
    [DataContract(Name = "control", Namespace = Namespaces.AtomPub)]
    public class ControlExtension
    {
        [DataMember(Name = "draft")]
        private string draft;

        public DraftStatus Draft
        {
            get
            {
                if (string.IsNullOrEmpty(draft))
                {
                    return DraftStatus.No;
                }
                return DraftStatus.Parse(draft);
            }
            set { draft = value.Value; }
        }
    }
}