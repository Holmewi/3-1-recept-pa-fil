using FiledRecipes.Domain;
using FiledRecipes.App.Mvp;
using FiledRecipes.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiledRecipes.Views
{
    /// <summary>
    /// 
    /// </summary>
    public class RecipeView : ViewBase, IRecipeView
    {
        // Visa recept
        public void Show(IRecipe recipe)
        {
            Console.Clear();
            Header = recipe.Name;   // Sätter metoden Header till receptets namn
            ShowHeaderPanel();      // Skriver ut panelen där headern presenteras
        }

        // Visa alla recept
        public void Show(IEnumerable<IRecipe> recipes)
        {
            throw new NotImplementedException();
        }
    }
}
