using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CitiBot.Main
{
    public static class ToolBox
    {
        public static void Serialize<T>(string xmlName, T t)
        {
            var serializer = new XmlSerializer(t.GetType());
            using (var writer = XmlWriter.Create(xmlName))
            {
                serializer.Serialize(writer, t);
            }
        }

        public static T Deserialize<T>(string xmlName)
        {
            if (!File.Exists(xmlName))
                return default(T);
            T t;
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = XmlReader.Create(xmlName))
            {
                t = (T)serializer.Deserialize(reader);
            }
            return t;
        }

    }
}
