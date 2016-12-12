﻿using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;

namespace Gemini.Modules.StatusBar.ViewModels
{
	[Export(typeof(IStatusBar))]
	public class StatusBarViewModel : PropertyChangedBase, IStatusBar
	{
        private readonly StatusBarItemCollection _items;
	    public IObservableCollection<StatusBarItemViewModel> Items
	    {
            get { return _items; }
	    }

	    public StatusBarViewModel()
        {
            _items = new StatusBarItemCollection();
        }

	    public StatusBarItemViewModel AddItem(string message, GridLength width)
	    {
            var item = new StatusBarItemViewModel(message, width);
            Items.Add(item);

            return item;
	    }

        private class StatusBarItemCollection : BindableCollection<StatusBarItemViewModel>
        {
            protected override void InsertItemBase(int index, StatusBarItemViewModel item)
            {
                item.Index = index;
                base.InsertItemBase(index, item);
            }

            protected override void SetItemBase(int index, StatusBarItemViewModel item)
            {
                item.Index = index;
                base.SetItemBase(index, item);
            }
        }
	}
}