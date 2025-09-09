using System.IO;
using System.Xml.Serialization;

namespace ASOFT.A00.DataAccess.Utilities
{
    public static class InternalXmlHelper
    {
        public static string ModelToXml(object value)
        {
            if (value == null)
            {
                return null;
            }

            using (var writer = new StringWriter())
            {
                var xs = new XmlSerializer(value.GetType());
                xs.Serialize(writer, value);
                return writer.ToString();
            }
        }
    }
}
