﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FiledRecipes.Domain
{
    /// <summary>
    /// Holder for recipes.
    /// </summary>
    public class RecipeRepository : IRecipeRepository
    {
        /// <summary>
        /// Represents the recipe section.
        /// </summary>
        private const string SectionRecipe = "[Recept]";

        /// <summary>
        /// Represents the ingredients section.
        /// </summary>
        private const string SectionIngredients = "[Ingredienser]";

        /// <summary>
        /// Represents the instructions section.
        /// </summary>
        private const string SectionInstructions = "[Instruktioner]";

        /// <summary>
        /// Occurs after changes to the underlying collection of recipes.
        /// </summary>
        public event EventHandler RecipesChangedEvent;

        /// <summary>
        /// Specifies how the next line read from the file will be interpreted.
        /// </summary>
        private enum RecipeReadStatus { Indefinite, New, Ingredient, Instruction };

        /// <summary>
        /// Collection of recipes.
        /// </summary>
        private List<IRecipe> _recipes;

        /// <summary>
        /// The fully qualified path and name of the file with recipes.
        /// </summary>
        private string _path;

        /// <summary>
        /// Indicates whether the collection of recipes has been modified since it was last saved.
        /// </summary>
        public bool IsModified { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the RecipeRepository class.
        /// </summary>
        /// <param name="path">The path and name of the file with recipes.</param>
        public RecipeRepository(string path)
        {
            // Throws an exception if the path is invalid.
            _path = Path.GetFullPath(path);

            _recipes = new List<IRecipe>();
        }

        /// <summary>
        /// Returns a collection of recipes.
        /// </summary>
        /// <returns>A IEnumerable&lt;Recipe&gt; containing all the recipes.</returns>
        public virtual IEnumerable<IRecipe> GetAll()
        {
            // Deep copy the objects to avoid privacy leaks.
            return _recipes.Select(r => (IRecipe)r.Clone());
        }

        /// <summary>
        /// Returns a recipe.
        /// </summary>
        /// <param name="index">The zero-based index of the recipe to get.</param>
        /// <returns>The recipe at the specified index.</returns>
        public virtual IRecipe GetAt(int index)
        {
            // Deep copy the object to avoid privacy leak.
            return (IRecipe)_recipes[index].Clone();
        }

        /// <summary>
        /// Deletes a recipe.
        /// </summary>
        /// <param name="recipe">The recipe to delete. The value can be null.</param>
        public virtual void Delete(IRecipe recipe)
        {
            // If it's a copy of a recipe...
            if (!_recipes.Contains(recipe))
            {
                // ...try to find the original!
                recipe = _recipes.Find(r => r.Equals(recipe));
            }
            _recipes.Remove(recipe);
            IsModified = true;
            OnRecipesChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Deletes a recipe.
        /// </summary>
        /// <param name="index">The zero-based index of the recipe to delete.</param>
        public virtual void Delete(int index)
        {
            Delete(_recipes[index]);
        }

        /// <summary>
        /// Raises the RecipesChanged event.
        /// </summary>
        /// <param name="e">The EventArgs that contains the event data.</param>
        protected virtual void OnRecipesChanged(EventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            EventHandler handler = RecipesChangedEvent;

            // Event will be null if there are no subscribers. 
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, e);
            }
        }

        // Virtual används för att modifiera metoden från det ärvda interfacet
        // Hämta recept
        public virtual void Load()
        {
            // Skapar en lista med referenser till receptobjektet
            List<IRecipe> recipeList = new List<IRecipe>();

            // Sätter den uppräkningsbara typens status till odefinierad
            RecipeReadStatus status = RecipeReadStatus.Indefinite;

            // Öppnar filen med recept och listar hela textfilen
            try
            {
                using (StreamReader reader = new StreamReader(@"App_Data/Recipes.txt"))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {

                        if (line == "")     // Om raden är tom
                        {
                            continue;
                        }
                        /*
                        if (line == SectionRecipe)  // Om det finns en section med recept
                        {
                            status = RecipeReadStatus.New;
                        }
                        else if (line == SectionIngredients)    // Om det finns en section med ingredienser
                        {
                            status = RecipeReadStatus.Ingredient;
                        }
                        else if (line == SectionInstructions)   // Om det finns en section med instructioner
                        {
                            status = RecipeReadStatus.Instruction;
                        }
                        // Annars om det är en sträng innanför en section
                        else
                        {
                            if (status == RecipeReadStatus.New)
                            {
                                Console.WriteLine("   TEST RECEPT");
                            }
                            else if (status == RecipeReadStatus.Ingredient)
                            {
                                Console.WriteLine("   TEST INGREDIENS");
                            }
                            else if (status == RecipeReadStatus.Instruction)
                            {
                                Console.WriteLine("   TEST INSTRUKTION");
                            }
                        }
                        */
                        
                        
                        switch (line)
                        {
                            case SectionRecipe:
                                status = RecipeReadStatus.New;
                                continue;
                            case SectionIngredients:
                                Console.WriteLine("Ingredienser");  // Tillfällig rubrik
                                Console.BackgroundColor = ConsoleColor.Green;  // Testing
                                status = RecipeReadStatus.Ingredient;
                                continue;        
                            case SectionInstructions:
                                Console.ResetColor(); // Testing
                                Console.WriteLine("Instruktioner");  // Tillfällig rubrik
                                status = RecipeReadStatus.Instruction;
                                continue;
                        }

                        switch (status)
                        {
                            case RecipeReadStatus.New:
                                Console.WriteLine("   TEST RECEPT");
                                continue;
                            case RecipeReadStatus.Ingredient:
                                Console.WriteLine("   TEST INGREDIENS");
                                continue;
                            case RecipeReadStatus.Instruction:
                                Console.WriteLine("   TEST INSTRUKTION");
                                continue;
                        }
                        Console.WriteLine(line);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("FEL!\n{0} ", ex.Message);
            }
        }

        // Spara recept
        public virtual void Save()
        {
            throw new NotImplementedException();
        }

    }
}
