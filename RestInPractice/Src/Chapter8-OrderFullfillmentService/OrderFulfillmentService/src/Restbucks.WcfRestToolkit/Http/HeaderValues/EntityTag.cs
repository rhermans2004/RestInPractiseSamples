using System;

namespace Restbucks.WcfRestToolkit.Http.HeaderValues
{
    public class EntityTag
    {
        public static EntityTag CreateWithRandomValue()
        {
            return Parse(Guid.NewGuid().ToString("N"));
        }
        
        public static EntityTag Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string unquotedValue = value.Replace(@"""", string.Empty);

            if (string.IsNullOrEmpty(unquotedValue))
            {
                return null;
            }

            return new EntityTag(string.Format(@"""{0}""", unquotedValue));
        }

        private readonly string value;

        private EntityTag(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }

        public bool Equals(EntityTag other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.value, value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (EntityTag)) return false;
            return Equals((EntityTag) obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}