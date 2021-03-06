﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using System.ComponentModel;

namespace WpfApplication3.ViewModel
{
    public class RevRobasViewModel : ViewModelBase
    {

        private RevRobaViewModel _selectedRevRoba;

        public RevRobaViewModel SelectedRevRoba
        {
            get { return _selectedRevRoba; }
            set
            {
                _selectedRevRoba = value;
                RaisePropertyChanged();
            }
        }


        private RevRobaViewModel _noviRedReversa;
        public RevRobaViewModel NoviRedReversa
        {
            get { return _noviRedReversa; }
            set
            {
                _noviRedReversa = value;
                RaisePropertyChanged();
            }
        }
        
        public BindingList<RevRobaViewModel> Items { get; }


        public RevRobasViewModel(IList<RevRobaViewModel> revrobas)
        {
            Items = new BindingList<RevRobaViewModel>(revrobas);
            Items.RaiseListChangedEvents = true;
            NoviRedReversa = new RevRobaViewModel();
        }
    }
}
