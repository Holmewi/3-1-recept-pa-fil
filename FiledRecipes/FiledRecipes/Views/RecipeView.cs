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

            Console.WriteLine("\nIngredienser\n");

            // För varje ingrediens (från interfacet IIngredient) i receptets del för ingredienser, som hämtas genom IRecipeView från IRecipe
            foreach (IIngredient ingredient in recipe.Ingredients)
            {
                Console.WriteLine(ingredient);
            }

            int instructionCount = 1;

            Console.WriteLine("\nInstruktioner\n");
            // För varje sträng instruktion i receptets del för instruktioner, som hämtas genom IRecipeView från IRecipe
            foreach (string instruction in recipe.Instructions)
            {

                Console.WriteLine("<{0}>\n{1}", instructionCount, instruction);
                instructionCount++;
            }

        }

        // Visa alla recept
        public void Show(IEnumerable<IRecipe> recipes)
        {
            // För varje fullständigt recept i listan med recept
            foreach (IRecipe recipe in recipes)
            {
                Show(recipe);               // Visar ett recept
                ContinueOnKeyPressed();     // Visar nästa recept via knapptryckning
            }
        }
    }
}
