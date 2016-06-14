using System;
using System.Collections;
using System.Collections.Generic;
using RegisterOperation = AbstractDevelop.Machines.Operation<AbstractDevelop.machines.regmachine.RegisterOperationId, System.Numerics.BigInteger>;

namespace AbstractDevelop.machines.regmachine
{
    /// <summary>
    /// Представляет коллекцию программ для параллельной машины с бесконечными регистрами.
    /// </summary>
    public class RegisterProgramCollection : ICollection<RegisterProgram>
    {
        private List<RegisterProgram> _list;
        private RegisterProgram _entryPoint;

        /// <summary>
        /// Инициализирует пустую коллекцию программ для параллельной машины с бесконечными регистрами.
        /// </summary>
        public RegisterProgramCollection()
        {
            _list = new List<RegisterProgram>();
            _entryPoint = null;
        }

        /// <summary>
        /// Возвращает текущую точку входа коллекции. Если такой не существует, возвращается null.
        /// </summary>
        public RegisterProgram GetEntry()
        {
            return _entryPoint;
        }

        /// <summary>
        /// Возвращает программу по ее уникальному идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор программы.</param>
        /// <returns>Программа.</returns>
        public RegisterProgram Get(int id)
        {
            if (id < 0) throw new ArgumentException("Уникальный идентификатор программы не может быть отрицательным");
            int i = _list.BinarySearch(new RegisterProgram(new RegisterOperation[0], id, "search"));
            if (i < 0) throw new ArgumentException("Программы с указанным id не существует в коллекции");

            return _list[i];
        }

        /// <summary>
        /// Добавляет элемент в коллекцию. Проверка на существование элемента производится по Id, а не по ссылке.
        /// </summary>
        /// <param name="item">Добавляемый элемент.</param>
        public void Add(RegisterProgram item)
        {
            if (item == null)
                throw new ArgumentNullException("Добавляемый предмет не может быть неопределенным");

            int i = _list.BinarySearch(item);
            if (i >= 0) throw new ArgumentException("Программа с указанным уникальным идентификатором уже существует в коллекции");

            _list.Insert(~i, item);
            if (item.IsEntry)
            {
                if (_entryPoint == null)
                    _entryPoint = item;
                else
                    throw new ArgumentException("В коллекции уже существует точка входа. Чтобы задать новую, удалите предыдущую");
            }
        }

        /// <summary>
        /// Очищает коллекцию.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// Определяет, содержит ли коллекция указанный элемент. Проверка на существование элемента производится по Id, а не по ссылке.
        /// </summary>
        /// <param name="item">Проверяемый элемент.</param>
        /// <returns>Истина, если содержится, иначе - ложь.</returns>
        public bool Contains(RegisterProgram item)
        {
            if (item == null)
                throw new ArgumentNullException("Определяемый предмет не может быть неопределенным");

            return _list.BinarySearch(item) >= 0;
        }

        public void CopyTo(RegisterProgram[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Удаляет элемент из коллекции. Проверка на существование элемента производится по Id, а не по ссылке.
        /// </summary>
        /// <param name="item">Удаляемый элемент.</param>
        /// <returns>Истина, если элемент удален, иначе - ложь.</returns>
        public bool Remove(RegisterProgram item)
        {
            if (item == null)
                throw new ArgumentNullException("Удаляемый предмет не может быть неопределенным");

            int i = _list.BinarySearch(item);
            if (i < 0) return false;

            _list.RemoveAt(i);

            if (item.IsEntry)
                _entryPoint = null;

            return true;
        }

        public IEnumerator<RegisterProgram> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (_list as IEnumerable).GetEnumerator();
        }
    }
}