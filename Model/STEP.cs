//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project_1.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class STEP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public STEP()
        {
            this.IMAGES = new HashSet<IMAGE>();
        }
    
        public int recipeId { get; set; }
        public int orderNumber { get; set; }
        public string content { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IMAGE> IMAGES { get; set; }
        public virtual RECIPE RECIPE { get; set; }
    }
}
