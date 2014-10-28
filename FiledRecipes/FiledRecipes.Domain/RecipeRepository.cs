using System;
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
            // 1. Skapar en lista med referenser till receptobjektet
            List<IRecipe> recipeList = new List<IRecipe>();

            // Sätter den uppräkningsbara typens status till odefinierad
            RecipeReadStatus status = RecipeReadStatus.Indefinite;

            

            // 2. Öppnar filen med recept och listar hela textfilen
            try
            {
                using (StreamReader reader = new StreamReader(@"App_Data/Recipes.txt"))
                {
                    // Variabel som ska hantera rader i textdokumentet
                    string line;

                    while ((line = reader.ReadLine()) != null)  // Så länge dokumentet inte är slut eller tomt
                    {

                        if (line == "")     // 3.a. Om raden är tom
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
                            Recipe recipe = new Recipe(line);
                            if (status == RecipeReadStatus.New)
                            {
                                recipeList.Add(recipe);
                            }
                            else if (status == RecipeReadStatus.Ingredient)
                            {
                                string[] split = line.Split(new char[] { ';' }, StringSplitOptions.None);
                                foreach (string value in split)
                                {
                                    recipeList.Add(recipe);     // Lägger till dokumentets rader för den uppräkningsbara typen "Ingredient"
                                }
                                if (split.Length > 3)
                                {
                                    throw new FileFormatException();
                                }
                            }
                            else if (status == RecipeReadStatus.Instruction)
                            {
                                recipeList.Add(recipe);
                            }
                        }
                        */


                        // Går i genom rad för rad för att dela upp sektioner
                        switch (line)
                        {
                            case SectionRecipe:     // 3.b. Om det finns en section med recept
                                status = RecipeReadStatus.New;
                                continue;
                            case SectionIngredients:        // 3.c. Om det finns en section med ingredienser
                                Console.WriteLine("Ingredienser");  // Tillfällig rubrik
                                Console.BackgroundColor = ConsoleColor.DarkBlue;  // Testing
                                Console.ForegroundColor = ConsoleColor.White;
                                status = RecipeReadStatus.Ingredient;
                                continue;
                            case SectionInstructions:       // 3.d. Om det finns en section med instructioner
                                Console.ResetColor(); // Testing
                                Console.WriteLine("Instruktioner");  // Tillfällig rubrik
                                status = RecipeReadStatus.Instruction;
                                continue;
                        }

                        // Skapar ett nytt object från den icke abstrakta klassen som hanterar recept
                        Recipe recipe = new Recipe(line);

                        // 3.e. Går i genom och modifierar statustyperne för recept, ingredienser och instruktioner
                        switch (status)
                        {
                            case RecipeReadStatus.New:                      // 3.e.i Statusen är satt till receptets namn
                                recipeList.Add(recipe);                     // 3.e.i.1. Lägger till dokumentets rader för den uppräkningsbara typen "New"
                                break;
                            case RecipeReadStatus.Ingredient:               // 3.e.ii Statusen är satt till receptets ingredienser
                                string[] split = line.Split(new string[] 
                                { ";" }, StringSplitOptions.None);          // 3.e.ii.1 Delar upp raden i delar
                                if (split.Length > 3)                       // 3.e.ii.2 Skicka exception om det finns fler delar än tre
                                {
                                    throw new FileFormatException();
                                }  
                                Ingredient ingredient = new Ingredient();   // 3.e.ii.3 Skapa ett ingrediensobjekt och initera det med tre delar
                                ingredient.Amount = split[0];
                                ingredient.Measure = split[1];
                                ingredient.Name = split[2];
                                recipeList.Add(recipe);                     // 3.e.ii.4 Lägger till dokumentets rader från den uppräkningsbara typen "Ingredient"
                                break;
                            case RecipeReadStatus.Instruction:              // 3.e.iii Statusen är satt till receptets instruktioner
                                recipeList.Add(recipe);                     // 3.e.iii.1 Lägger till rader för den uppräkningsbara typen "Instruction"
                                break;
                            case RecipeReadStatus.Indefinite:               // 3.e.iv Kasta ett undantag om något är fel
                                throw new FileFormatException();
                        }

                        recipeList.OrderBy(recipeNameOrder => recipeNameOrder.Name).ToList();      // 4. Ska sortera listan efter receptens namn
                        //IEnumerable<Recipe> recipeOrder = recipeList.OrderBy(recipeNameOrder => recipeNameOrder.Name).ToList();


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
