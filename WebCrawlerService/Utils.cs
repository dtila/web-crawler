using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawlerService
{
    public static class Utils
    {
        public static string GetAppDataDirectory()
        {
            var fileInfo = new System.IO.FileInfo(Assembly.GetExecutingAssembly().Location);
            for (var folder = fileInfo.Directory;  ; folder = folder.Parent)
            {
                var appData = folder.EnumerateDirectories("App_Data").FirstOrDefault();
                if (appData != null)
                    return appData.FullName;
            }
        }
    }


    public static class SerializationUtils
    {
        private static readonly DataContractJsonSerializer CookieSerializer = new DataContractJsonSerializer(typeof(Cookie));

        public static byte[] SerializeCookie(Cookie cookie)
        {
            using (var ms = new MemoryStream())
            {
                CookieSerializer.WriteObject(ms, cookie);
                return ms.ToArray();
            }
        }

        public static Cookie DeserializeCookie(byte[] content)
        {
            using (var ms = new MemoryStream(content))
                return (Cookie)CookieSerializer.ReadObject(ms);
        }

        public static byte[] Serialize(object obj)
        {
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static object Deserialize(byte[] bytes)
        {
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream(bytes))
                return formatter.Deserialize(ms);
        }
    }
}
