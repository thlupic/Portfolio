using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using System.Reflection;

namespace WebService
{
    //metode za serijalizaciju i deserijalizaciju
    public class Serialization
    {
        public string Serializer<T>(T objectToSerialize)
        {
            var seralizedString = "";
            var serializer = new JavaScriptSerializer();

            seralizedString = serializer.Serialize(objectToSerialize);

            return seralizedString;
        }

        public object Deserialize(string stringToDeserialize, Type deserializeObjectType)
        {
            var serializer = new JavaScriptSerializer();

            return serializer.Deserialize(stringToDeserialize, deserializeObjectType);
        }
    }
}
