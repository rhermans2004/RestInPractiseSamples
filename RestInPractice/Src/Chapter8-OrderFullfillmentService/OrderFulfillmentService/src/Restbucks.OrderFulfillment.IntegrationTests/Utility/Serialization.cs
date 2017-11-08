using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Restbucks.OrderFulfillment.IntegrationTests.Utility
{
    public static class Serialization
    {
        public static byte[] ToByteArray(object o)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(o.GetType());
                serializer.WriteObject(stream, o);
                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream))
                {
                    string s = reader.ReadToEnd();
                    return Encoding.UTF8.GetBytes(s);
                }
            }
        }
    }
}