using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AmiBroker.Samples.YahooDataSource
{
    [XmlRoot(Namespace = "AmiBroker.Samples.YahooDataSource", IsNullable = false)]
    public class YConfiguration
    {
        public int RefreshPeriod;

        public static string GetConfigString(YConfiguration configuration)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(YConfiguration));

            Stream stream = new MemoryStream();
            serializer.Serialize(XmlWriter.Create(stream), configuration);

            stream.Seek(0, SeekOrigin.Begin);
            StreamReader streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        public static YConfiguration GetConfigObject(string config)
        {
            // if no config string, set default values
            if (string.IsNullOrEmpty(config) || config.Trim().Length == 0)
                return GetDefaultConfigObject();

            XmlSerializer serializer = new XmlSerializer(typeof(YConfiguration));
            Stream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(config));

            try
            {
                return (YConfiguration)serializer.Deserialize(stream);
            }
            catch (Exception)
            {
                return GetDefaultConfigObject();
            }
        }

        public static YConfiguration GetDefaultConfigObject()
        {
            YConfiguration defConfig = new YConfiguration();

            defConfig.RefreshPeriod = 10;

            return defConfig;
        }
    }
}
