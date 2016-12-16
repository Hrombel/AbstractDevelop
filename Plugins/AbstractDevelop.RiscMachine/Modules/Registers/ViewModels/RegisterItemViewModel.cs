using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Modules.Registers.ViewModels
{
    public class RegisterItemViewModel :
        PropertyChangedBase, IRegister
    {
        private IRegister register;

        public RegisterItemViewModel(IRegister register)
        {
            this.register = register;
        }

        public int Index { get; }

        public string ID => $"r{Index}";

        public byte Value { get; set; }
    }
}
