using AbstractDevelop.machines.post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines
{
    /// <summary>
    /// Описывает базовые возможности абстрактных машин. Любая модель абстрактной машины
    /// должна реализовывать указанный интерфейс.
    /// </summary>
    public interface IAbstractMachine
    {
        /// <summary>
        /// Производит запуск выполнения абстрактной машины.
        /// </summary>
        /// <param name="ops">Список выполняемых операций.</param>
        void Start(List<Operation> ops);

        /// <summary>
        /// Переводит абстрактную машину в режим пошагового выполнения программы.
        /// </summary>
        /// <param name="ops">Список выполняемых операций.</param>
        void StartManual(List<Operation> ops);

        /// <summary>
        /// Выполняет следующую по порядку команду для абстрактной машины.
        /// </summary>
        /// <returns>Истина, если операция выполнена успешно и не вызвала остановку машины, иначе - возникла ошибка, которая остановила машину.</returns>
        bool Forward();

        /// <summary>
        /// Останавливает работу абстрактной машины.
        /// </summary>
        void Stop();
    }
}
