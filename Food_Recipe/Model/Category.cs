//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Food_Recipe.Model
{
    using Food_Recipe.ViewModels;
    using System;
    using System.Collections.Generic;
    
    public partial class Category : BaseViewModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Category()
        {
            this.Recipes = new HashSet<Recipe>();
        }
    
        private int _id;
        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }

        private string _type;
        public string Type { get => _type; set { _type = value; OnPropertyChanged(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
