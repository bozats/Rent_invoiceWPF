//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfApplication3
{
    using System;
    using System.Collections.ObjectModel;
    
    public partial class kupci
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public kupci()
        {
            this.racuni = new ObservableCollection<racuni>();
        }
    
        public int idbroj { get; set; }
        public string ime { get; set; }
        public string jmbg { get; set; }
        public string adresa { get; set; }
        public string mesto { get; set; }
        public string telefon { get; set; }
        public decimal dug { get; set; }
        public decimal pot { get; set; }
        public Nullable<decimal> saldo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<racuni> racuni { get; set; }
    }
}