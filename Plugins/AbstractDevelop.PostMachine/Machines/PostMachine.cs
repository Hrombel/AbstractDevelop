using System;
using System.Linq;
using System.Numerics;
using AbstractDevelop.Translation;

namespace AbstractDevelop.Machines
{
    /// <summary>
    /// Реализация машины Поста
    /// </summary>
    public class PostMachine :
        InstructionMachine<PostOperationId, BigInteger>
    {
        static Argument[] basicOperation = new Argument[1] { Argument.Optional };
     

        /// <summary>
        /// Лента с даннымми
        /// </summary>
        /// <remarks>
        /// Рекомендуется использовать кеширование при многократном доступе к данному ресурсу
        /// </remarks> 
        public Tape<bool> Tape { get; } = new Tape<bool>();

        public override ISourceTranslator Translator => throw new NotImplementedException();

        protected override ExecutionAction OnAfterStep(EventArgs args, bool breakpointsActive, bool isSucceded = true)
        {
            if (Instructions.Current.Definition.Arguments == basicOperation)
            {
                
            }

            return base.OnAfterStep(args, breakpointsActive, isSucceded);
            //Instructions.Current.Arguments.TryGet(1)

            //if (.Length > 0)

            //OperationPostprocess += (operation) =>
            //{
            //    // переход к команде, указанной в аргументе 
            //    // (кроме операции условного перехода и останова)
            //    if (Operations.Definitions[operation.Id].ArgumentCount == 1)
            //        Operations.Goto((int)operation.Args[0]);



        }

        public PostMachine()
        {
            Instructions.Definitions = new InstructionDefinitions()
            {
                // останов выполнения программы
                ["s"] = (PostOperationId.Stop, args => Stop(StopReason.Result), Argument.NoArgOperation),
                // сдвиг каретки влево\вправо
                ["l"] = (PostOperationId.Left,  args => Tape.Position--, basicOperation),
                ["r"] = (PostOperationId.Right, args => Tape.Position++, basicOperation),
                // установка\стирание метки
                ["p"] = (PostOperationId.Place, args => Tape.Current = true, basicOperation),
                ["e"] = (PostOperationId.Erase, args => Tape.Current = false, basicOperation),
                // переход по условию
                ["j"] = (PostOperationId.Erase, args => Tape.Position = (long)args[Tape.Current ? 1 : 0], Argument.TwoArgsOperation)
            };
             
            //Tape.DebugInfo.Enabled = debugMode;
            //Tape.DebugInfo.SituationHandler += (o, n, info) =>  // o = old, n = new
            //{                                                   // info = TapeDebugInfo
            //    if (o == n) info.Output.WriteLine(n == true? 
            //        "Попытка установки метки в отмеченную ячейку" :
            //        "Попытка стирания несуществующей метки");
            //};
        }
    }
}