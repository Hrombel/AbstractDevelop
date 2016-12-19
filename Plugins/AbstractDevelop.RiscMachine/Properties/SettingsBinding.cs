using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace AbstractDevelop.Machines.Properties
{ 

    [MarkupExtensionReturnType(typeof(object))]
    public class SettingsExtension :
        MarkupExtension
    {
        [ConstructorArgument("name")]
        public string Name { get; set; }

        public SettingsExtension(string name)
            => Name = name ?? throw new ArgumentNullException(nameof(name));

        public override object ProvideValue(IServiceProvider serviceProvider)
            => Settings.Default[Name];
    }

    public class SettingBindingExtension : 
        Binding
    {
        public SettingBindingExtension()
        {
            Initialize();
        }

        public SettingBindingExtension(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            Source = Settings.Default;
            Mode = BindingMode.TwoWay;
        }
    }
}
