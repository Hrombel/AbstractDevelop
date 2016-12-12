using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FormatterType = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;
using SettingsContainer = System.Collections.Generic.Dictionary<string, object>;

namespace AbstractDevelop
{
    [Serializable]
    public class AbstractProject
    {
        internal const string ProjectExtension = ".adp";
        private static readonly FormatterType formatter = new FormatterType();

        #region [Свойства]

        public string Name { get; set; }

        public MachineId Type { get; set; }

        public List<CodeFile> Files { get { return files; } }

        public DirectoryInfo ParentFolder { get; }

        public SettingsContainer Settings { get; set; }

        #endregion [Свойства]

        [NonSerialized]
        List<CodeFile> files;

        List<string> filePaths;

        #region [Методы]

        #region [Сохранение]

        public void Associate(IEnumerable<CodeFile> targetFiles)
        {
            if (targetFiles == null)
                throw new ArgumentNullException(nameof(targetFiles));

            foreach (var file in targetFiles)
                if (!Files.Contains(file)) Files.Add(file);
        }

        public void Associate(params CodeFile[] targetFiles) => Associate(targetFiles as IEnumerable<CodeFile>);
    

        /// <summary>
        /// Показывает, ассоциирован ли проверяемый файл с данным проектом
        /// </summary>
        /// <param name="fileName">Файл, свойство ассоциированности коотрого требуется проверить</param>
        public bool IsAssociated(string fileName)
        {
            fileName = new FileInfo(fileName).FullName;
            foreach (var file in Files)
                //HACK: неявное преобразование из CodeFile в строку, содержащую полный путь файла
                if (file == fileName) return true;
            return false;
        }

        /// <summary>
        /// Сохраняет все изменения в файлах данного проекта, за исключением указанных файлов
        /// </summary>
        /// <param name="excludeFiles">Файлы, сохранение которых не требуется</param>
        /// <returns>Возвращает задачу, описывающую асинхронное состояние сохранения</returns>
        public async Task Save(params string[] excludeFiles)
        {
            if (!ParentFolder.Exists) ParentFolder.Create();
            try
            {
                // сохранение связнных файлов
                var exclusion = excludeFiles.ToList();
                foreach (var file in Files)
                {
                    if (exclusion.Contains(file)) continue;
                    else await file.Save();
                }

                filePaths = files.ConvertAll((file => ((string)file).Replace(file.ParentFolder.FullName + "\\", "")));
                // сохранение данных о проекте в отдельный файл
                using (var output = File.Create(Path.Combine(ParentFolder.FullName, Name + ProjectExtension)))
                    formatter.Serialize(output, this);
            }
            catch (Exception ex) { throw new Exception($"Ошибка сохранения проекта {Name}", ex); }
        }

        #endregion [Сохранение]

        #region [Загрузка]

        /// <summary>
        /// Загружает проект из указанного потока
        /// </summary>
        /// <param name="source">Исходный поток с данными проекта</param>
        /// <param name="shouldCloseStream">Флаг, показывающий необходимость закрытия потока после загрузки данных</param>
        public static AbstractProject Load(Stream source, bool shouldCloseStream = false)
        {
            try
            {
                var result = formatter.Deserialize(source) as AbstractProject;
                if (shouldCloseStream) source.Close();
                return result;
            }
            catch (Exception ex) { throw new FileLoadException($"Ошибка загрузки проекта", ex); }
        }

        /// <summary>
        /// Загружает проект из указанного файла и ассоциирует этот файл с проектом.
        /// </summary>
        /// <param name="fileName">Имя файла для загрузки.</param>
        /// <returns>Загруженный проект.</returns>
        public static AbstractProject Load(string filePath) => Load(File.OpenRead(filePath), true);

        #endregion [Загрузка]

        #endregion [Методы]

        public AbstractProject(string name, MachineId type, string parentFolder, params string[] files)
        {
            Name = (name ?? "Untitled Project").CheckPath(false); // имя должно состоять только из допустимых символов
            Type = type;
            ParentFolder = new DirectoryInfo(parentFolder.CheckPath(false));

            this.files = CodeFile.LoadFrom(true, files);
            Settings = new SettingsContainer();
        }
    }
}