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
    
    public partial class IMAGE
    {
        public int recipeId { get; set; }
        public int stepOrderNumber { get; set; }
        public string imageSource { get; set; }
    
        public virtual STEP STEP { get; set; }
    }
}
