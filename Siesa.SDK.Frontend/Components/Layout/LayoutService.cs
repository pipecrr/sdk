﻿using Microsoft.AspNetCore.Components;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Frontend.Components.FormManager.Views;
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
        public RenderFragment TopBarExtraButtons => TopBarSetter?.TopBarExtraButtons;
        public string StyleName => TopBarSetter?.StyleName;
        public bool HasExtraButtons => TopBarSetter != null ? TopBarSetter.HasExtraButtons : false;
        public dynamic BusinessObj => TopBarSetter?.BusinessObj;
        public bool HiddenCompaies => TopBarSetter != null ? TopBarSetter.HiddenCompaies : false;
        public bool DisableCompanies => TopBarSetter != null ? TopBarSetter.DisableCompanies: false;
        public ListView ListView => TopBarSetter != null ? TopBarSetter?.ListView : null;
        public EventCallback<E00201_Company> OnChangeCompany => TopBarSetter != null ? TopBarSetter.OnChangeCompany : new EventCallback<E00201_Company>(null, null);

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
            NotifyPropertyChanged(nameof(TopBarExtraButtons));
            NotifyPropertyChanged(nameof(StyleName));
            NotifyPropertyChanged(nameof(HasExtraButtons));
            NotifyPropertyChanged(nameof(BusinessObj));
            NotifyPropertyChanged(nameof(HiddenCompaies)); 
            NotifyPropertyChanged(nameof(DisableCompanies));
            NotifyPropertyChanged(nameof(OnChangeCompany));
            NotifyPropertyChanged(nameof(ListView));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private SetTopBar topBarSetter;
    }

}
