using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AbstractDevelop.Projects
{
    class ProjectEntryConverter :
       JsonConverter
    {
        static Type EntryEnumerableType = typeof(IEnumerable<IProjectEntry>);

        public override bool CanConvert(Type objectType)
            => objectType.BasedOn(EntryEnumerableType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Read() && reader.TokenType == JsonToken.StartObject)
                return GetSubsystem().ToArray();
            else
                return new IProjectEntry[0];

            IEnumerable<IProjectEntry> GetSubsystem()
            {
                while (reader.TokenType != JsonToken.EndArray)
                {
                    if (reader.TokenType == JsonToken.EndObject)
                    {
                        reader.Read();
                        continue;
                    }

                    dynamic jsonObject = JObject.Load(reader);
                    var projectEntry = default(IProjectEntry);

                    switch (jsonObject.Type.Value)
                    {
                        case "File":
                            projectEntry = new ProjectFile();
                            break;
                        case "Directory":
                            projectEntry = new ProjectDirectory();
                            break;
                    }

                    serializer.Populate(jsonObject.CreateReader(), projectEntry);
                    yield return projectEntry;
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (var entry in value as IEnumerable<IProjectEntry>)
            {

                JObject output = new JObject();
                switch (entry)
                {
                    case ProjectFile File:
                        output.Add("Type", nameof(File));
                        output.Add("Name", File.Name);
                        output.Add("Content", JToken.FromObject(File.Content, serializer));
                        break;
                    case ProjectDirectory Directory:
                        output.Add("Type", nameof(Directory));
                        output.Add("Name", Directory.Name);
                        output.Add("Content", JToken.FromObject(Directory.Content, serializer));
                        break;
                }

                output.WriteTo(writer);
            }

            writer.WriteEndArray();
        }
    }
}
