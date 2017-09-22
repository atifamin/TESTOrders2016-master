using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace OrderForm2016.Extensions
{
    public static class ObjectExtension
    {
        public static string ToJsonString(this Object obj)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                var js = new JsonSerializer();
                js.Serialize(sw, obj);
            }

            return sb.ToString();
        }
    }
}