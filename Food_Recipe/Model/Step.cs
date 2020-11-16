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
    
    public partial class Step : BaseViewModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Step()
        {
            this.Images = new HashSet<Image>();
        }
    
        private int _recipeId;
        public int RecipeId { get => _recipeId; set { _recipeId = value; OnPropertyChanged(); } }

        private int _orderNumber;
        public int OrderNumber { get => _orderNumber; set { _orderNumber = value; OnPropertyChanged(); } }

        private string _content;
        public string Content { get => _content; set { _content = value; OnPropertyChanged(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Image> Images { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
