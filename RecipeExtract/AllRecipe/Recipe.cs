using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeExtract.AllRecipe
{
    public class Recipe
    {
        public int Id { get; set; }
        public bool Complete { get; set; }

        public string Title { get; set; }
        public string Url { get; set; }
        public string Html { get; set; }
        public double Rating { get; set; }

        public int PrepMinutes { get; set; }
        public int CookMinutes { get; set; }
        public int ReadyMinutes { get; set; }

        public List<Nutrient> Nutrients { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<String> Instructions { get; set; }
        public List<Collection> Collections { get; set; }

        public Recipe(string title, string url)
        {
            this.Complete = false;
            this.Title = title;
            
            if (url.Contains("http"))
                this.Url = url.Substring(0, url.IndexOf("?"));
            else
                this.Url = "http://allrecipes.com" + url.Substring(0, url.IndexOf("?"));

            this.Collections = new List<Collection>();
            this.Instructions = new List<string>();
            this.Ingredients = new List<Ingredient>();
            this.Nutrients = new List<Nutrient>();
        }
    }
}
