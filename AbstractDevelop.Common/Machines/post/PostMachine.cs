using System.Linq;
using System.Numerics;

namespace AbstractDevelop.Machines
{
    /// <summary>
    /// Кодирует возможные операции машины Поста.
    /// </summary>
    public enum PostOperationId : byte
    {
        /// <summary>
        /// Сместить каретку влево.
        /// </summary>
        Left,
        /// <summary>
        /// Сместить каретку вправо.
        /// </summary>
        Right,
        /// <summary>
        /// Стереть метку.
        /// </summary>
        Erase,
        /// <summary>
        /// Установить метку.
        /// </summary>
        Place,
        /// <summary>
        /// Условный переход.
        /// </summary>
        Decision,
        /// <summary>
        /// Завершить выполнение программы.
        /// </summary>
        Stop
    }

    /// <summary>
    /// Реализация машины Поста
    /// </summary>
    public class PostMachine :
        AbstractMachine<PostOperationId, BigInteger, Tape<bool>>
    {
        /// <summary>
        /// Лента с даннымми
        /// </summary>
        /// <remarks>
        /// Рекомендуется использовать кеширование при многократном доступе к данному ресурсу
        /// </remarks> 
        public Tape<bool> Tape { get; } = new Tape<bool>();

        public override string Name => "Машина Поста";

        public PostMachine(bool debugMode = true) : base()
        {
            Operations.Definitions.
                Add(PostOperationId.Stop,       "s", 0, (args) => Stop(StopReason.Result)).

                Add(PostOperationId.Left,       "l", 1, (args) => Tape.Position--).
                Add(PostOperationId.Right,      "r", 1, (args) => Tape.Position++).
                Add(PostOperationId.Place,      "p", 1, (args) => Tape.Current = true).
                Add(PostOperationId.Erase,      "e", 1, (args) => Tape.Current = false).
                
                Add(PostOperationId.Decision,   "j", 2, (args) => Tape.Position = args[Tape.Current? 1 : 0]);

            OperationPostprocess += (operation) =>
            {
                // переход к команде, указанной в аргументе 
                // (кроме операции условного перехода и останова)
                if (Operations.Definitions[operation.Id].ArgumentCount == 1)
                    Operations.Goto((int)operation.Args[0]);
            };

            Tape.DebugInfo.Enabled = debugMode;
            Tape.DebugInfo.SituationHandler += (o, n, info) =>  // o = old, n = new
            {                                                   // info = TapeDebugInfo
                if (o == n) info.Output.WriteLine(n == true? 
                    "Попытка установки метки в отмеченную ячейку" :
                    "Попытка стирания несуществующей метки");
            };
        }
    }
}