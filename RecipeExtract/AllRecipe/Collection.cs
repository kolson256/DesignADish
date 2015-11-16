using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeExtract.AllRecipe
{
    public class Collection
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public Collection ParentCollection { get; set; }
        public List<Collection> ChildCollections { get; set; }

        public Collection(string title, string url, Collection parentCollection)
        {
            this.Title = title;
            if (url.Contains("http"))
                this.Url = url.Substring(0, url.IndexOf("?"));
            else
                this.Url = "http://allrecipes.com" + url.Substring(0, url.IndexOf("?"));

            ChildCollections = new List<Collection>();
            this.ParentCollection = parentCollection;
        }
    }
}
