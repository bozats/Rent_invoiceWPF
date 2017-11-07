﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace WpfApplication3.ViewModel
{
    public class RacunisViewModel : ViewModelBase
    {
        private readonly DAL _dal;
        private RacuniViewModel _selectedRacuni;

        public ICommand SaveCommand => new RelayCommand(Save, CanSave);
        public ICommand DeleteCommand => new RelayCommand(Delete, CanDelete);
        public ICommand AddCommand => new RelayCommand<System.Windows.Controls.DataGrid>(Add);
        public ICommand UndoCommand => new RelayCommand(Undo, CanUndo);
        public ICommand NewInvoiceWindowCommand => new RelayCommand(NewInvoice, CanNewInvoice);
        public ICommand AddNewInvoiceCommand => new RelayCommand(AddNewInvoice);
        public ICommand EditInvoiceWindowCommand => new RelayCommand(EditInvoice, CanEditInvoice);
        public ICommand PrintInvoiceCommand => new RelayCommand(PrintInvoice, CanPrintInvoice);

        private RacuniViewModel _noviRevers;

        public RacuniViewModel NoviRevers
        {
            get { return _noviRevers; }
            set
            {
                _noviRevers = value;
                RaisePropertyChanged();
            }
        }

        public RacuniViewModel SelectedRacuni
        {
            get { return _selectedRacuni; }
            set
            {
                _selectedRacuni = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<RacuniViewModel> Racunis { get; }

        public RacunisViewModel(DAL dal, IEnumerable<KupciViewModel> kupcis, IEnumerable<RevRobaViewModel> revRobas)
        {
            _dal = dal;
            Racunis = new ObservableCollection<RacuniViewModel>(_dal.GetRacuni().Select(x => new RacuniViewModel(dal, x, kupcis, new RevRobasViewModel(revRobas.Where(rr => rr.RacuniID == x.RacuniID).ToList()))));
        }

        private void AddNewInvoice()
        {
            NoviRevers.Save();
            Racunis.Add(NoviRevers);

            NewInvoiceWindow.Window.Close();
        }

        private bool CanSave()
        {
            return Racunis.Any(x => x.Changed || x.IsDeleted);
        }

        private void Save()
        {
            foreach (var racuni in Racunis.Where(x => x.Changed || x.IsDeleted).ToArray())
            {
                racuni.Save();

                if (racuni.IsDeleted)
                    Racunis.Remove(racuni);
            }
        }

        private bool CanDelete()
        {
            if (SelectedRacuni == null)
                return false;

            if (SelectedRacuni.IsDeleted)
                return false;

            return true;
        }

        private void Delete()
        {
            if (SelectedRacuni == null)
                return;

            SelectedRacuni.IsDeleted = true;
        }

        private void Add(System.Windows.Controls.DataGrid grid)
        {
            var newItem = new RacuniViewModel(_dal);
            Racunis.Add(newItem);

            int idx;

            for (idx = 0; idx < grid.Items.Count; idx++)
            {
                if (newItem == grid.Items[idx])
                    break;
            }

            grid.SelectionUnit = DataGridSelectionUnit.Cell;
            grid.Focus();
            grid.CurrentCell = new DataGridCellInfo(grid.Items[idx], grid.Columns[0]);
            grid.BeginEdit();
            grid.SelectionUnit = DataGridSelectionUnit.FullRow;
        }

        private bool CanUndo()
        {
            if (SelectedRacuni == null)
                return false;

            if (SelectedRacuni.IsDeleted)
                return true;

            return false;
        }

        private void Undo()
        {
            if (SelectedRacuni == null)
                return;
            SelectedRacuni.IsDeleted = false;
        }

        private void NewInvoice()
        {
            NoviRevers = new RacuniViewModel(_dal);
            NewInvoiceWindow.ShowSingleWindow();
        }

        private bool CanNewInvoice()
        {
            if (NewInvoiceWindow.Window == null)
                return true;

            return false;
        }

        private void EditInvoice()
        {
            EditInvoiceWindow.ShowSingleWindow(SelectedRacuni);
        }

        private bool CanEditInvoice()
        {
            if (SelectedRacuni != null & EditInvoiceWindow.Window == null)
                return true;

            return false;
        }

        private void PrintInvoice()
        {
            
            var pivm = new PrintInvoiceViewModel();
            var printDialogWindow = new PrintPreview();

            pivm.InvoiceNumber = SelectedRacuni.Brev;
            pivm.InvoiceDate = SelectedRacuni.Datum;
            pivm.Name = SelectedRacuni.Kupci.Ime;
            pivm.Jmbg = SelectedRacuni.Kupci.Jmbg;
            pivm.City = SelectedRacuni.Kupci.Mesto;
            pivm.Address = SelectedRacuni.Kupci.Adresa;

            foreach (var e in SelectedRacuni.RevRobas.Items)
            {
                var vm = new PrintInvoiceLineViewModel();
                vm.Roba = e.Roba.Naziv;
                vm.Amount = e.Kolic;
                vm.Price = e.Cena;
                pivm.Items.Add(vm);
            }

            var path = ReportFactory.RunReport(pivm, "ReversStampa");

            var pdf = PdfiumViewer.PdfDocument.Load(path);
            printDialogWindow.pdfViewer1.Document = pdf;
            var printpdf = printDialogWindow.pdfViewer1.Document.CreatePrintDocument();
            var pd = new System.Windows.Forms.PrintDialog();
            pd.Document = printpdf;
            pd.UseEXDialog = true;
            DialogResult result = pd.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    printpdf.Print();
                }
                catch (InvalidPrinterException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
        }

        private bool CanPrintInvoice()
        {
            if (SelectedRacuni == null)
                return false;

            return true;
        }
    }
}