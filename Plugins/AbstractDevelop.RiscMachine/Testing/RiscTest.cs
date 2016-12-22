using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using static AbstractDevelop.Machines.Testing.TestSource;

namespace AbstractDevelop.Machines.Testing
{
    [DebuggerDisplay("{Title}")]
    public  class RiscTest :
        INotifyPropertyChanged
    {
        private Exception lastError;
        private TestState state;
        #region [Свойства и Поля]

        public byte[] Input { get; set; }
        public RiscMachine.InstructionBase InstructionSet { get; set; }
        public byte[] Output { get; set; }
        public string Title { get; set; }

        public TestState State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    state = value;
                    OnPropertyChanged();
                }
            }
        }

        public Exception LastError
        {
            get { return lastError; }
            set
            {
                if (lastError != value)
                {
                    lastError = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        /// <summary>
        /// Вызывает событие <see cref="PropertyChanged"/>
        /// </summary>
        /// <param name="propertyName">Имя свойства, которое было изменено</param>
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #region [Конструкторы и деструкторы]

        public RiscTest(TestEntry test, RiscTestSystem system)
        {
            Title = test.Title;
            State = default(TestState);

            Input = test.Input.Select(RiscMachine.GetNumberValue).ToArray();
            Output = test.Output.Select(RiscMachine.GetNumberValue).ToArray();

            InstructionSet = test.InstructionSet != null ? new RiscMachine.InstructionBase(system.Machine.Instructions.Definitions, test.InstructionSet) : null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
            => Title;

        #endregion
    }
}
