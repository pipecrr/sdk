using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Frontend.Components.Layout
{
    public class LayoutService : ILayoutService, INotifyPropertyChanged
    {

        public RenderFragment TopBarTitle => TopBarSetter?.TopBarTitle;
        public RenderFragment TopBarButtons => TopBarSetter?.TopBarButtons;

        public SetTopBar TopBarSetter
        {
            get => topBarSetter;
            set
            {
                if (topBarSetter == value) return;
                topBarSetter = value;
                UpdateTopBar();
            }
        }

        public void UpdateTopBar() {
            NotifyPropertyChanged(nameof(TopBarTitle));
            NotifyPropertyChanged(nameof(TopBarButtons));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private SetTopBar topBarSetter;
    }

}
