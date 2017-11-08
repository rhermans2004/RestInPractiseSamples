using System;

namespace Restbucks.WcfRestToolkit.Utility
{
    public static class Check
    {
        public static void IsNotNull(object o, string name)
        {
            if (o == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void IsGreaterThanZero(int v, string name)
        {
            if (v <= 0)
            {
                throw new ArgumentException(string.Format("{0} must be greater than zero.", name));
            }
        }
    }
}