using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemini.Framework;

namespace AbstractDevelop.Modules.Registers.ViewModels
{
    public class RegistersViewModel :
        Document
    {
        public override string DisplayName
        {
            get { return "RISC Registers"; }
           
            set
            {
                base.DisplayName = value;
            }
        }

       // override 
    }
}
