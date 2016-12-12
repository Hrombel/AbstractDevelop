using System;
using System.IO;

using AbstractDevelop.Storage.Formats;

namespace AbstractDevelop.Projects
{
    [Serializable]
    public class ProjectFile :
        IProjectEntry
    {

        #region [Свойства и Поля]

        /// <summary>
        /// Содержание элемента (строка base64)
        /// </summary>
        public object Content
        {
            get { return Convert.ToBase64String(data); }
            set
            {
                if (value is string source)
                    data = Convert.FromBase64String(source);
            }
        }

        /// <summary>
        /// Имя элемента
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Показывает длину потока данного файла
        /// </summary>
        public int Length => data.Length;

        /// <summary>
        /// Тип элемента
        /// </summary>
        public EntryType Type => EntryType.File;

        [NonSerialized]
        // набор байт, представляющий содержимое
        byte[] data;

        #endregion

        #region [Методы]

        public Stream CreateStream()
            => CopyTo(new MemoryStream()).Do(s => s.Position = 0);
     
        public Stream CopyTo(Stream target)
            => target?.Do(stream => stream.Write(data, 0, data.Length));

        public void CopyFrom(Stream source)
        {
            if (source is MemoryStream sourceData)
                data = sourceData.ToArray();
            else
            {
                using (var mem = new MemoryStream())
                {
                    var pos = source.Position;

                    source.Position = 0;
                    source.CopyTo(mem);
                    source.Position = pos;

                    data = mem.ToArray();
                }
            }
        }

        /// <summary>
        /// Загружает сериализуемый объект из файла
        /// </summary>
        /// <typeparam name="TargetType">Ожидаемый тип объекта после десериализации</typeparam>
        /// <param name="formatProvider">Объект, производящий десериализацию</param>
        public TargetType Deserialize<TargetType>(IDataFormatProvider formatProvider)
            => formatProvider.Deserialize<TargetType>(CreateStream());

        /// <summary>
        /// Сохраняет объект в файл при помощи formatter
        /// </summary>
        /// <param name="formatProvider">Объект, выводящий содержимое</param>
        /// <param name="value">Объект, содержимое которого необходимо вывести</param>
        public void Serialize<TargetType>(TargetType value, IDataFormatProvider formatProvider)
        {
            using (var stream = new MemoryStream())
            {
                formatProvider.Serialize(value, stream);
                CopyFrom(stream);
            }
        }
        public void Dispose() { }
           
        #endregion

        #region [Конструкторы и деструкторы]

        /// <summary>
        /// Конструктор по умолчанию лля элемента файла
        /// </summary>
        /// <param name="content">Содержимое файла</param>
        public ProjectFile(params byte[] content) :
            this(null, content)
        { }
     
        /// <summary>
        /// Конструктор по умолчанию лля элемента файла
        /// </summary>
        /// <param name="name">Имя файла</param>
        /// <param name="content">Содержимое файла</param>
        public ProjectFile(string name = default(string), params byte[] content)
        {
            Name = name;
            data = (content ?? new byte[0]);
        }

        /// <summary>
        /// Конструктор с возможностью создания файла из готового потока (данные копируются)
        /// </summary>
        /// <param name="name">Имя файла</param>
        /// <param name="source">Поток с исходными данными для копировния</param>
        public ProjectFile(string name, Stream source)
        {
            Name = name;
            // копирование данных
            CopyFrom(source);
        }

        /// <summary>
        /// Деструктор класса
        /// </summary>
        ~ProjectFile() { Dispose(); }

        #endregion

    }
}
