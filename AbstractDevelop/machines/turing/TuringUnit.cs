using AbstractDevelop.machines.tape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.turing
{
    /// <summary>
    /// Представляет модуль машины Тьюринга, определяющий пару: читающая/пишущая головка и соответствующая ей лента машины Тьюринга.
    /// </summary>
    [Serializable]
    public class TuringUnit : ISerializable
    {
        private Tape _tape;

        /// <summary>
        /// Инициализирует модуль машины Тьюринга пустой лентой и читающей/пишущей головкой, установленной напротив нулевой ячейки.
        /// </summary>
        public TuringUnit()
        {
            _tape = new Tape(TapeType.MultiStated);
            Position = 0;
        }

        private TuringUnit(SerializationInfo info, StreamingContext context)
        {
            _tape = info.GetValue("t", typeof(Tape)) as Tape;
            Position = (BigInteger)info.GetValue("p", typeof(BigInteger));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("t", _tape, typeof(Tape));
            info.AddValue("p", Position, typeof(BigInteger));
        }

        /// <summary>
        /// Получает ленту модуля.
        /// </summary>
        public Tape Tape { get { return _tape; } }

        /// <summary>
        /// Получает или задает позицию читающей/пишущей головки.
        /// </summary>
        public BigInteger Position { get; set; }
        
    }
}
