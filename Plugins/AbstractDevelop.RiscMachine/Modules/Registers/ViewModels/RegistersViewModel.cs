using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemini.Framework;
using Gemini.Modules.Toolbox;
using System.ComponentModel.Composition;
using Gemini.Framework.Services;

using RegisterCollection = System.Collections.ObjectModel.ObservableCollection<AbstractDevelop.Modules.Registers.ViewModels.RegisterItemViewModel>;

namespace AbstractDevelop.Modules.Registers.ViewModels
{
    [Export("RegisterView", typeof(IToolbox))]
    public class RegistersViewModel :
        Tool, IToolbox
    {
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public override string DisplayName
            => "Регистры RISC";//Translate.Key("RiscRegistersWindowTitle", source: Properties.Resources.ResourceManager);

        /// <summary>
        /// Отображаемые регистры
        /// </summary>
        public RegisterCollection Registers { get; set; }

        /// <summary>
        /// Предпочитаемое расположение вида
        /// </summary>
        public override PaneLocation PreferredLocation => PaneLocation.Right;
       
        [ImportingConstructor]
        public RegistersViewModel([Import(AllowDefault = true)]ICollection<IRegister> registers, IExtensibilityProvider extensibility)
        {
            Registers = registers is RegisterCollection source? source : 
                new RegisterCollection(registers?.Select(register => new RegisterItemViewModel(register)) ?? new RegisterItemViewModel[0]);

            // экспорт созданных регистров для использования в эмуляторе
            extensibility.Export(Registers as ICollection<IRegister>);
        }
    }
}
