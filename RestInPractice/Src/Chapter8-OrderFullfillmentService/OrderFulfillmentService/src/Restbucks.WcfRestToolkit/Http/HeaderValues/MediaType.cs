using System;
using System.Collections.Generic;
using System.Linq;

namespace Restbucks.WcfRestToolkit.Http.HeaderValues
{
    public class MediaType
    {
        public static MediaType Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string normalizedValue = value.ToLower().Replace(" ", string.Empty);

            if (string.IsNullOrEmpty(normalizedValue))
            {
                return null;
            }

            return new MediaType(normalizedValue);
        }

        private readonly string value;

        private MediaType(string value)
        {
            this.value = value;
        }

        public string TypeAndSubtypeAndParameters
        {
            get { return value; }
        }

        public string TypeAndSubtype
        {
            get
            {
                return (from m in Split(value)
                        select m).First();
            }
        }

        public bool IsTypeAndSubtypeMatch(MediaType mediaType)
        {
            if (mediaType == null)
            {
                return false;
            }

            return TypeAndSubtype.Equals(mediaType.TypeAndSubtype);
        }

        public bool IsTypeSubtypeAndParameterMatch(MediaType mediaType)
        {
            if (mediaType == null)
            {
                return false;
            }

            return ((from e in Split(value)
                     orderby e
                     select e).Except(from m in Split(mediaType.TypeAndSubtypeAndParameters)
                                      orderby m
                                      select m))
                .Count().Equals(0);
        }

        public MediaType WithTypeAndSubtypeOnly()
        {
            return Parse(TypeAndSubtype);
        }

        private static IEnumerable<string> Split(string s)
        {
            return s.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}