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
    
    public partial class Image : BaseViewModel
    {
        private int _recipeId;
        public int RecipeId { get => _recipeId; set { _recipeId = value; OnPropertyChanged(); } }

        private int _stepOrderNumber;
        public int StepOrderNumber { get => _stepOrderNumber; set { _stepOrderNumber = value; OnPropertyChanged(); } }

        private string _imageSource;
        public string ImageSource { get => _imageSource; set { _imageSource = value; OnPropertyChanged(); } }

        public virtual Step Step { get; set; }
    }
}
