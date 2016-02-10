using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.tape
{
    /// <summary>
    /// Кодирует типы лент для абстрактных вычислителей.
    /// </summary>
    public enum TapeType : byte
    { 
        /// <summary>
        /// Лента, ячейки которой имеют два состояния: наличие либо отсутствие метки.
        /// Все значения, которые записываются на такую ленту будут автоматически сведены к
        /// двум состояниям: отсутствие метки будет распознано, если подан символ, определяющий
        /// пустоту, присутствие - если будет записан любой другой символ.
        /// </summary>
        TwoStated,
        
        /// <summary>
        /// Лента, ячейки которой могут иметь более двух состояний.
        /// </summary>
        MultiStated
    }
}
