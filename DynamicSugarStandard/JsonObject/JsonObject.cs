using DynamicSugar;

namespace DynamicSugar
{
    public class JsonObject
    {
        public string JsonFileName { get; set; }

        const string JsonFileNameProperty = "JsonFileName";

        public void Save(string jsonFileName = null)
        {
            var f = jsonFileName ?? this.JsonFileName;
            SetJsonFileNameProperty(this, f);

            JsonObjectStaticHelper.SaveToFile(this, f);
        }

        public string ToJSON()
        {
            return JsonObjectStaticHelper.Serialize(this);
        }

        private static void SetJsonFileNameProperty(object This, string f)
        {
            if (ReflectionHelper.PropertyExist(This, JsonFileNameProperty))
                ReflectionHelper.SetProperty(This, JsonFileNameProperty, f);
        }

        public static T FromFile<T>(string fileName) where T : new()
        {
            T t = JsonObjectStaticHelper.FromFile<T>(fileName);
            SetJsonFileNameProperty(t, fileName);

            return t;
        }
    }
}
