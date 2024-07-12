using DynamicSugar;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace DynamicSugar
{
    public class JsonObjectStaticHelper
    {
        public static T Deserialize<T>(string json) where T : new()
        {
            T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json/*, jsonSerializerSettings*/);
            return t;
        }

        public static string Serialize<T>(T o, bool exportNullProperty = true, Formatting formatting = Formatting.Indented, JsonSerializerSettings jsonSerializerSettings = null) where T : new()
        {
            if (jsonSerializerSettings == null)
            {
                jsonSerializerSettings = new JsonSerializerSettings
                {
                    //PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    //ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                };
            }

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(o, formatting, jsonSerializerSettings);
            return json;
        }

        public static string SwitchToJsonExtension(string fileName)
        {
            return Path.Combine(
                Path.GetDirectoryName(fileName),
                Path.GetFileNameWithoutExtension(fileName) + ".json"
                );
        }

        public static bool BackUpFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                var bakFileName = fileName + ".bak";
                if (File.Exists(bakFileName))
                {
                    try
                    {
                        File.Delete(bakFileName);
                    }
                    catch
                    {
                        return false;
                    }
                }
                try
                {
                    File.Move(fileName, fileName + ".bak");
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public static bool SaveToFile<T>(T o, string fileName) where T : new()
        {
            const string __lastModifiedPropertyName = "__LastModified";
            if (ReflectionHelper.PropertyExist(o, __lastModifiedPropertyName))
                ReflectionHelper.SetProperty(o, __lastModifiedPropertyName, DateTime.Now);

            fileName = SwitchToJsonExtension(fileName);
            var json = Serialize<T>(o);
            if (!BackUpFile(fileName))
                return false;

            File.WriteAllText(fileName, json, Encoding.UTF8);
            return true;
        }

        public static T FromFile<T>(string fileName) where T : new()
        {
            fileName = SwitchToJsonExtension(fileName);
            if (!File.Exists(fileName))
                throw new ArgumentException($"file:{fileName} not found");
            var json = File.ReadAllText(fileName);
            var o = Deserialize<T>(json);
            return o;
        }
    }
}
