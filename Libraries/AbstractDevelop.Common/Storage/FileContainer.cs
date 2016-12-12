using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO.Compression;

namespace AbstractDevelop.Storage
{
    public class FileContainer
    {
        public string Path { get; }

        public JsonSerializer Serializer { get; set; }

        public T Read<T>()
        {
            bool isCompressed = false, tryOpen = true;
            Stream stream = File.OpenRead(Path);
            var result = default(T);

            while (tryOpen)
            {
                try
                {
                    using (var reader = (isCompressed ? new BsonReader(stream) as JsonReader : new JsonTextReader(new StreamReader(stream))))
                    {
                        result = Serializer.Deserialize<T>(reader);
                        break;
                    }
                }
                catch (JsonSerializationException ex)
                {
                    if (tryOpen = isCompressed = !isCompressed)
                        // открытие в сжатом потоке
                        stream = new DeflateStream(stream, CompressionLevel.Optimal);
                    else
                        // открыть файл не удалось
                        throw new FileLoadException(Path, ex);
                }
            }

            stream.Close();
            return result;
        }

        public void Write(object value, bool isCompressed)
        {
            Stream stream = File.OpenRead(Path);
            if (isCompressed)
                stream = new DeflateStream(stream, CompressionLevel.Optimal);
            try
            {
                using (var writer = (isCompressed ? new BsonWriter(stream) as JsonWriter : new JsonTextWriter(new StreamWriter(stream))))
                    Serializer.Serialize(writer, value);
            }
            catch (JsonSerializationException ex)
            {
                // открыть файл не удалось
                throw new FileLoadException(Path, ex);
            }

            stream.Close();
        }

        public FileContainer(string path)
        {
            if (path.IsValidPath() && !File.Exists(path))
                File.Create(path);

            Serializer = new JsonSerializer();
        }
    }
}
