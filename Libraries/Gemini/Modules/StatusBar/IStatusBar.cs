using System.Windows;
using Caliburn.Micro;
using Gemini.Modules.StatusBar.ViewModels;

namespace Gemini.Modules.StatusBar
{
	public interface IStatusBar
	{
        IObservableCollection<StatusBarItemViewModel> Items { get; }

        StatusBarItemViewModel AddItem(string message, GridLength width);
	}
}