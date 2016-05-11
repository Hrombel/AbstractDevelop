using AbstractDevelop.machines.tape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AbstractDevelop.Machines.Post
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
    public partial class PostMachine :
        AbstractMachine<PostOperationId, BigInteger, Tape>
    {
        /// <summary>
        /// Лента с даннымми
        /// </summary>
        /// <remarks>
        /// Рекомендуется использовать кеширование при многократном доступе к данному ресурсу
        /// </remarks> 
        public Tape Tape { get { return Storages[0]; } }

        public override string Name => "Машина Поста";

        public PostMachine() : base()
        {
            OperationPostprocess += (operation) => {
                // переход к команде, указанной в аргументе 
                // (кроме операции условного перехода)
                if (operation.Id != PostOperationId.Decision && operation.Args.Length > 0)
                    Operations.Next = operation.Args.Last();
            };

            Operations.Definitions.
                AddChain(PostOperationId.Left,      (args) => Tape.Position--).
                AddChain(PostOperationId.Right,     (args) => Tape.Position++).
                AddChain(PostOperationId.Place,     (args) => Tape.CurrentValue = 0).
                AddChain(PostOperationId.Erase,     (args) => Tape.CurrentValue = 1).
                AddChain(PostOperationId.Stop,      (args) => Stop(StopReason.Result)).
                AddChain(PostOperationId.Decision,  (args) => Tape.Position = (int)args.TryGetArg(Tape.CurrentValue));

            Tape.DebugInfo.Enabled = true;
            Tape.DebugInfo.SituationHandler += (o, n, info) =>  // o = old, n = new
            {                                                   // info = TapeDebugInfo
                if (o == n) info.Output.WriteLine(n == 1? 
                    "Попытка установки метки в отмеченную ячейку" :
                    "Попытка стирания несуществующей метки");
            };
        }
    }
}