using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AbstractDevelop
{
    public class CodeFile
    {
        #region [Свойства]

        public DirectoryInfo ParentFolder { get { return TargetFile.Directory; } }

        public FileInfo TargetFile { get; set; }

        public Stream Content { get; set; }

        #endregion [Свойства]

        #region [Методы]

        /// <summary>
        /// Открывает файл в режиме mode с доступом access
        /// </summary>
        /// <param name="mode">Режим открытия файла</param>
        /// <param name="access">Ограничение доступа после открытия файла</param>
        public void Open(FileMode mode = FileMode.Create, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.ReadWrite)
        {
            try
            {
                if (Content != default(Stream) && Content.Length != 0)
                {
                    Content.FlushAsync();
                    Content.Close();
                }
            }
            finally
            {
                if (!ParentFolder.Exists) ParentFolder.Create();
                Content = TargetFile.Open(mode, access, share);
            }
        }

        public StreamWriter CreateWriter() => new StreamWriter(Content) { AutoFlush = true };

        public StreamReader CreateReader() => new StreamReader(Content);
        /// <summary>
        /// Сохраняет объект в файл при помощи formatter
        /// </summary>
        /// <param name="formatter">Объект, выводящий содержимое</param>
        /// <param name="value">Объект, содержимое которого необходимо вывести</param>
        public void Serialize(IFormatter formatter, object value) => formatter.Serialize(Content, value);

        /// <summary>
        /// Загружает сериализуемый объект из файла
        /// </summary>
        /// <typeparam name="TargetType">Ожидаемый тип объекта после десериализации</typeparam>
        /// <param name="formatter">Объект, производящий десериализацию</param>
        public TargetType Deserialize<TargetType>(IFormatter formatter) where TargetType : class
            => (formatter.Deserialize(Content) as TargetType);

        #region [Сохранение и загрузка]

        /// <summary>
        /// Сохраняет изменения в проассоциированный файл
        /// </summary>
        public async Task Save()
        {
            if (Content is FileStream)
                await Content.FlushAsync();
            else
            {
                try
                {
                    if (!ParentFolder.Exists) ParentFolder.Create();
                    var output = File.OpenWrite(TargetFile.FullName);
                    await Content.CopyToAsync(output);
                    Content.Close();
                    // замена исходного потока на созданный файловый поток
                    Content = output;
                }
                catch (Exception ex) { throw new Exception($"Ошибка сохранения файла {TargetFile.Name}", ex); }
            }
        }

        /// <summary>
        /// Загружает файлы по указанных путям
        /// </summary>
        /// <param name="shouldOpen">Требуется ли открывать файлы (создавать файловые потоки) при загрузке</param>
        /// <param name="sourceFiles">Список файлов для загрузки</param>
        public static List<CodeFile> LoadFrom(bool shouldOpen, params string[] sourceFiles)
            => sourceFiles.ToList().ConvertAll(path => new CodeFile(path, shouldOpen));

        #endregion [Сохранение и загрузка]

        #endregion [Методы]

        internal CodeFile(string path, bool shouldOpen)
        {
            TargetFile = new FileInfo(path);
            if (shouldOpen) Open();
        }

        public static implicit operator string(CodeFile source) => (source?.TargetFile?.FullName ?? string.Empty);
    }
}