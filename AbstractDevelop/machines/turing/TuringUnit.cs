using AbstractDevelop.Machines;
using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace AbstractDevelop.machines.turing
{
    /// <summary>
    /// Представляет модуль машины Тьюринга, определяющий пару: читающая/пишущая головка и соответствующая ей лента машины Тьюринга.
    /// </summary>
    [Serializable]
    public class TuringUnit : ISerializable
    {
        private Tape<char> _tape;

        /// <summary>
        /// Инициализирует модуль машины Тьюринга пустой лентой и читающей/пишущей головкой, установленной напротив нулевой ячейки.
        /// </summary>
        public TuringUnit()
        {
            //_tape = new Tape<char>()
            Position = 0;
        }

        private TuringUnit(SerializationInfo info, StreamingContext context)
        {
            _tape = info.GetValue("t", typeof(Tape<char>)) as Tape<char>;
            Position = (BigInteger)info.GetValue("p", typeof(BigInteger));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("t", _tape, typeof(Tape<char>));
            info.AddValue("p", Position, typeof(BigInteger));
        }

        /// <summary>
        /// Получает ленту модуля.
        /// </summary>
        public Tape<char> Tape { get { return _tape; } }

        /// <summary>
        /// Получает или задает позицию читающей/пишущей головки.
        /// </summary>
        public BigInteger Position { get; set; }
    }
}