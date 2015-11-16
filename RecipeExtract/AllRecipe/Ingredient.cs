using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeExtract.AllRecipe
{
    public class Ingredient
    {
        public string Name { get; set; }
        public string Amount { get; set; }
        public int Id { get; set; }
        public double Grams { get; set; }
    }
}
