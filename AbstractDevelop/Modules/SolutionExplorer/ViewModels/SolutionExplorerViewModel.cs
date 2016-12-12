using Gemini.Modules.Toolbox.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemini.Framework.Services;
using Gemini.Modules.Toolbox.Services;
using System.ComponentModel.Composition;
using Gemini.Modules.Toolbox;
using Gemini.Framework;

namespace AbstractDevelop.Modules.SolutionExplorer.ViewModels
{
    [Export(typeof(IToolbox))]
    public class SolutionExplorerViewModel :
        Tool, IToolbox
    {
        public override PaneLocation PreferredLocation
            => PaneLocation.Right;

        public override string DisplayName
            => "Solution Explorer";
        
    }
}
