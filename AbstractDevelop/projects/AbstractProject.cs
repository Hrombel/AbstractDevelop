using AbstractDevelop.machines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AbstractDevelop.projects
{
    [Serializable]
    /// <summary>
    /// Представляет проект AbstractDevelop.
    /// </summary>
    public class AbstractProject
    {
        private string _name;
        private MachineId _machineId;
        private List<string> _files;

        [NonSerialized]
        private string _directory;

        /// <summary>
        /// Инициализирует новый проект с указанными начальными параметрами.
        /// </summary>
        /// <param name="name">Название проекта.</param>
        /// <param name="machine">Тип абстрактного вычислителя, под который создается проект.</param>
        /// <param name="directory">Путь к директории проекта.</param>
        public AbstractProject(string name, MachineId machine, string directory)
        {
            if (name == null)
                throw new ArgumentNullException("Название проекта не может быть неопределенным");
            if(!Enum.IsDefined(typeof(MachineId), machine))
                throw new ArgumentException("Указанный тип не существует");
            if (!CheckName(name))
                throw new ArgumentException("Название проекта некорректно. Название может содержать латинские буквы и десятичные цифры");
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentException("Путь к файлу проекта имеет неверный формат");

            _name = name;
            _machineId = machine;
            _files = new List<string>();
            _directory = directory;
        }

        /// <summary>
        /// Получает или задает словарь пользовательских настроек визуализатора.
        /// </summary>
        public Dictionary<string, bool> VisualizerSettings { get; set; }

        /// <summary>
        /// Получает имя проекта.
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Получает тип абстрактного вычислителя, к которому относится проект.
        /// </summary>
        public MachineId Machine { get { return _machineId; } }

        /// <summary>
        /// Получает директорию проекта.
        /// </summary>
        public string ProjectDirectory { get { return _directory; } }

        /// <summary>
        /// Добавляет путь указанный файл в список связей проекта.
        /// </summary>
        /// <param name="path">Путь к файлу относительно проекта.</param>
        private void AddFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Указанное имя файла имеет неверный формат");

            _files.Add(path);
        }

        /// <summary>
        /// Возвращает файлы, связанные с проектом.
        /// </summary>
        /// <returns>Массив связанных с проектом файлов.</returns>
        public string[] GetFiles()
        {
            return _files.ToArray();
        }

        /// <summary>
        /// Удаляет связанный с проектом файл по его индексу.
        /// </summary>
        /// <param name="index">Индекс файла в массиве.</param>
        public void RemoveFileAt(int index)
        {
            if (index < 0 || index >= _files.Count)
                throw new ArgumentOutOfRangeException("Указанный индекс находится вне диапазона допустимых значений");

            try
            {
                File.Delete(_directory + Path.DirectorySeparatorChar + _files[index]);
                _files.RemoveAt(index);
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Ошибка во время удаления файла: \"{0}\"", ex.Message), ex);
            }
        }

        /// <summary>
        /// Удаляет связанный с проектом файл по его имени.
        /// </summary>
        /// <param name="fileName">Имя удаляемого файла относительно проекта.</param>
        public void RemoveFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Неверный формат имени файла");

            int i = GetFileIndex(fileName);
            if (i == -1) throw new ArgumentException("Указанного файла не существует в списке связей проекта");

            RemoveFileAt(i);
        }

        /// <summary>
        /// Получает индекс файла по его имени.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <returns>Индекс файла.</returns>
        private int GetFileIndex(string fileName)
        {
            return _files.FindIndex(x => x == fileName);
        }

        /// <summary>
        /// Определяет, находится ли указанный файл в списке связей проекта.
        /// </summary>
        /// <param name="fileName">Проверяемый файл.</param>
        /// <returns>Истина, если файл присутствует в списке связей, иначе - ложь.</returns>
        public bool FileExists(string fileName)
        {
            return GetFileIndex(fileName) != -1;
        }

        /// <summary>
        /// Сохраняет указанный текст в указанный файл и при необходимости ассоциирует его с проектом.
        /// </summary>
        /// <param name="fileName">Имя файла относительно проекта.</param>
        /// <param name="data">Сохраняемый текст.</param>
        public void SaveFile(string fileName, string data)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Указанное имя файла имеет неверный формат");
            if (data == null)
                throw new ArgumentNullException("Сохраняемый текст не может быть неопределенным");

            if (!FileExists(fileName))
                AddFile(fileName);

            try
            {
                using (FileStream fs = File.Create(_directory + Path.DirectorySeparatorChar + fileName))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(data);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Сохраняет данные потока в указанный двоичный файл и при необходимости ассоциирует файл с проектом.
        /// После завершения операции поток остается открытым.
        /// </summary>
        /// <param name="fileName">Имя файла относительно проекта.</param>
        /// <param name="stream">Поток данных, сохраняемых в двоичный файл.</param>
        public void SaveFile(string fileName, MemoryStream stream)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Указанное имя файла имеет неверный формат");
            if (stream == null)
                throw new ArgumentNullException("Сохраняемый объект не может быть неопределенным");

            if (!FileExists(fileName))
                AddFile(fileName);

            try
            {
                BinaryFormatter format = new BinaryFormatter();

                using (FileStream fs = File.Create(_directory + Path.DirectorySeparatorChar + fileName))
                {
                    stream.WriteTo(fs);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Сохраняет указанный сериализируемый объект в указанный файл и при необходимости ассоциирует файл с проектом.
        /// </summary>
        /// <param name="fileName">Имя файла относительно проекта.</param>
        /// <param name="data">Сохраняемый объект.</param>
        public void SaveFile(string fileName, object data)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Указанное имя файла имеет неверный формат");
            if (data == null)
                throw new ArgumentNullException("Сохраняемый объект не может быть неопределенным");

            if (!FileExists(fileName))
                AddFile(fileName);

            try
            {
                BinaryFormatter format = new BinaryFormatter();

                using (FileStream fs = File.Create(_directory + Path.DirectorySeparatorChar + fileName))
                {
                    format.Serialize(fs, data);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Загружает указанный файл, ассоциированный с проектом, как текстовый.
        /// </summary>
        /// <param name="fileIndex">Имя ассоциированного с проектом файла.</param>
        /// <returns>Загруженный текст.</returns>
        public string LoadString(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Указанное имя файла имеет неверный формат");
            if (!FileExists(fileName))
                throw new ArgumentException("Указанный файл не ассоциирован с проектом");

            string result;
            try
            {
                using (TextReader tr = File.OpenText(_directory + Path.DirectorySeparatorChar + fileName))
                {
                    result = tr.ReadToEnd();
                }
            }
            catch (FileNotFoundException ex)
            {
                _files.RemoveAt(GetFileIndex(fileName));
                Save();
                throw new Exception("Ассоциированный с проектом файл не найден и был удален из списка зависимостей", ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Открывает файл на чтение в файловом потоке.
        /// </summary>
        /// <param name="fileName">Открываемый файл, ассоциированный с проектом.</param>
        /// <returns>Открытый на чтение файловый поток.</returns>
        public FileStream LoadBytes(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Указанное имя файла имеет неверный формат");
            if (!FileExists(fileName))
                throw new ArgumentException("Указанный файл не ассоциирован с проектом");

            FileStream result;
            try
            {
                result = File.OpenRead(_directory + Path.DirectorySeparatorChar + fileName);
            }
            catch (FileNotFoundException ex)
            {
                _files.RemoveAt(GetFileIndex(fileName));
                Save();
                throw new Exception("Ассоциированный с проектом файл не найден и был удален из списка зависимостей", ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Загружает сериализуемый объект из файла.
        /// </summary>
        /// <param name="fileName">Открываемый файл, ассоциированный с проектом.</param>
        /// <returns>Десериализованный объект.</returns>
        public object LoadObject(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Указанное имя файла имеет неверный формат");
            if (!FileExists(fileName))
                throw new ArgumentException("Указанный файл не ассоциирован с проектом");

            object result;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = File.Open(_directory + Path.DirectorySeparatorChar + fileName, FileMode.Open))
                {
                    result = formatter.Deserialize(fs);
                }
            }
            catch (FileNotFoundException ex)
            {
                _files.RemoveAt(GetFileIndex(fileName));
                Save();
                throw new Exception("Ассоциированный с проектом файл не найден и был удален из списка зависимостей", ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Проверяет название проекта на корректность.
        /// </summary>
        /// <param name="name">Проверяемое название.</param>
        /// <returns>Истина, если название кооректно, иначе - ложь.</returns>
        public static bool CheckName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;

            Regex reg = new Regex(@"[a-zA-Z\d]+");
            Match m = reg.Match(name);
            if (!m.Success) return false;

            return name.Replace(m.Value, "").Length == 0;
        }

        /// <summary>
        /// Сохраняет изменения в проассоциированный файл.
        /// </summary>
        public void Save()
        {
            try
            {
                if (!Directory.Exists(_directory))
                    Directory.CreateDirectory(_directory);

                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = File.Create(_directory + Path.DirectorySeparatorChar + Name + ".adp"))
                {
                    formatter.Serialize(fs, this);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Ошибка сохранения файла проекта: \"{0}\"", ex.Message), ex);
            }
        }

        /// <summary>
        /// Загружает проект из указанного файла и ассоциирует этот файл с проектом.
        /// </summary>
        /// <param name="fileName">Имя файла для загрузки.</param>
        /// <returns>Загруженный проект.</returns>
        public static AbstractProject LoadFrom(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("Указанное имя файла имеет неверный формат");

            AbstractProject proj = null;

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = File.Open(fileName, FileMode.Open))
                {
                    proj = formatter.Deserialize(fs) as AbstractProject;
                }
                proj._directory = Path.GetDirectoryName(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Ошибка загрузки файла проекта: \"{0}\"", ex.Message), ex);
            }

            return proj;
        }
    }
}
