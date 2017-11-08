using System;

namespace Restbucks.WcfRestToolkit.Syndication.AtomPub
{
    public class DraftStatus
    {
        public static DraftStatus Parse(string status)
        {
            switch (status)
            {
                case "yes":
                    return Yes;
                case "no":
                    return No;
                default:
                    throw new ArgumentException("Invalid argument. Valid values are 'yes' and 'no'.");
            }
        }

        public static readonly DraftStatus Yes = new DraftStatus("yes");
        public static readonly DraftStatus No = new DraftStatus("no");

        private readonly string value;

        private DraftStatus(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }
    }
}