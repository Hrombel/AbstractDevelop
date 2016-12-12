using Gu.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace AbstractDevelop.Storage.Formats
{
    public class JsonFormat :
        IBinaryDataFormatProvider
    {
        public static readonly List<JsonConverter> CommonConverters = new List<JsonConverter>()
        // стандартный набор конвертаторов
        {
             new StringEnumConverter(),
             new Projects.ProjectEntryConverter()
        };

        /// <summary>
        /// Объект сериализации в формате JSON
        /// </summary>
        public JsonSerializer Serializer { get; } = JsonSerializer.Create(new JsonSerializerSettings()
        {
            Culture = Translator.CurrentCulture,
            Formatting = Formatting.Indented,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,

            Converters = CommonConverters,

            Error = ErrorHandler
        });

        static void ErrorHandler(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            // TODO: реализовать обработку ошибок вывода
            e.ErrorContext.Handled = false;
        }

        /// <summary>
        /// Показывает, является ли двоичный формат предпочтительным
        /// </summary>
        public bool PreferBinary { get; set; }

        public T Deserialize<T>(Stream source)
        {
            try
            {
                // попытка чтения данных в "чистом" формате
                try
                {
                    using (var reader = new JsonTextReader(new StreamReader(source)) { CloseInput = false })
                        return Serializer.Deserialize<T>(reader);
                }
                // попытка чтения данных в сжатом формате
                catch (JsonException ex)
                {
                    using (var reader = new BsonReader(DeflateDecompress()))
                    {
                        source.Position = 0;
                        return Serializer.Deserialize<T>(reader);
                    }
                }
            }
            catch (Exception ex) { throw new InvalidDataException(Translate.Key("JsonLoadingError"), ex); }

            BinaryReader DeflateDecompress()
                => new BinaryReader(new DeflateStream(source, CompressionMode.Decompress));
        }

        public void Serialize<T>(T data, Stream target)
        {
            JsonWriter writer;

            if (PreferBinary)
                writer = new BsonWriter(DeflateInitCompress());
            else writer = new JsonTextWriter(new StreamWriter(target));

            try
            {
                // вывод данных в контейнер 
                Serializer.Serialize(writer, data);
                writer.Close();
            }
            catch (Exception ex) { throw new InvalidDataException(Translate.Key("JsonSavingError"), ex); }

            BinaryWriter DeflateInitCompress()
              => new BinaryWriter(new DeflateStream(target, CompressionMode.Compress));
        }

        public void Dispose()
        {
            Serializer.Error -= ErrorHandler;
        }
    }
}
