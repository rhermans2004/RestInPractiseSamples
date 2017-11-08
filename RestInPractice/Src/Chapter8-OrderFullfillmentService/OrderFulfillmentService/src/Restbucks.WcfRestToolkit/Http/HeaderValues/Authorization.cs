namespace Restbucks.WcfRestToolkit.Http.HeaderValues
{
    public class Authorization
    {
        private readonly string value;

        public Authorization(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }
    }
}