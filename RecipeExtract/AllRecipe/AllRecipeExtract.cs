using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace RecipeExtract.AllRecipe
{
    public class AllRecipeExtract
    {
        public static List<Collection> collections;
        public static Dictionary<String, Collection> collectionsByUrl;
        public static Dictionary<String, Recipe> recipesByUrl;

        public static List<Collection> Collections
        {
            get
            {
                if (collections != null)
                    return collections;

                collectionsByUrl = new Dictionary<string, Collection>();
                recipesByUrl = new Dictionary<string, Recipe>();
                DateTime start = DateTime.Now;
                collections = getCollections("http://allrecipes.com/recipes/main.aspx");
                TimeSpan span = DateTime.Now - start;

                return collections;
            }
        }

        private static List<Collection> getCollections(string url)
        {
            return getCollections(url, null);
        }

        private static List<Collection> getCollections(string url, Collection parentCollection)
        {
            List<Collection> childCollections = new List<Collection>();

            var webGet = new HtmlWeb();
            var document = webGet.Load(url);

            var containerLists = from node in document.DocumentNode.Descendants()
                                 where node.Name == "ul" &&
                                 node.Attributes["id"] != null &&
                                 node.Attributes["id"].Value == "subNavGroupContainer"
                                 select node;

            foreach (HtmlNode ulNode in containerLists)
            {
                string headerText = ""; 

                foreach (HtmlNode recipeGroup in ulNode.Descendants())
                {
                    if (recipeGroup.Attributes["id"] != null && recipeGroup.Attributes["id"].Value == "subNavHeaderContainer")
                    {
                        var title = from node in recipeGroup.Descendants()
                                    where node.Name == "span" &&
                                    node.Attributes["id"] != null &&
                                    node.Attributes["id"].Value == "lblTitle"
                                    select node;

                        foreach (HtmlNode headerNode in title)
                        {
                            headerText = headerNode.InnerText;
                        }
                    }
                    else
                    {
                        var links = from node in recipeGroup.Descendants()
                                    where node.Name == "a" &&
                                    node.Attributes["id"] != null &&
                                    node.Attributes["id"].Value == "hlSubNavItem"
                                    select node;

                        foreach (HtmlNode headerNode in links)
                        {
                            Collection col = new Collection(headerNode.InnerText, headerNode.Attributes["href"].Value, parentCollection);
                            childCollections.Add(col);
                        }
                    }
                }
            }

            
            var resultsCount = from node in document.DocumentNode.Descendants()
                                where node.Name == "p" &&
                                node.Attributes["class"] != null &&
                                node.Attributes["class"].Value.Contains("searchResultsCount")
                                select node;

            long recipeCount = 0;
            foreach (HtmlNode countNode in resultsCount)
            {
                if (countNode.Attributes["class"] != null && countNode.Attributes["class"].Value.Contains("staff-picks"))
                    continue;

                foreach (HtmlNode child in countNode.ChildNodes)
                {
                    if (child.Name == "span")
                    {
                        if (child.Attributes["id"] != null && child.Attributes["id"].Value.Contains("Collections"))
                            continue;
                        
                        recipeCount = long.Parse(child.InnerText.Replace(",", ""));
                    }
                }
            }

            int numPages = 1;
            if (recipeCount > 15)
            {
                recipeCount -= 15;
                Double extraPages = (Double)recipeCount / 20.0;
                numPages += (int)Math.Ceiling(extraPages);
            }

            for (int pageNum = 1; pageNum <= numPages; pageNum++)
            {
                getRecipes(url + "?Page=" + pageNum, parentCollection);
                
                //if (recipesByUrl.Count > 10)
                //    break;
            }
            /*
            foreach (Collection collection in childCollections)
            {
                if (!collectionsByUrl.ContainsKey(collection.Url))
                {
                    collectionsByUrl.Add(collection.Url, collection);
                    collection.ChildCollections = getCollections(collection.Url, collection);
                }
            }
            */
            return childCollections;
        }

        private static void getRecipes(string url, Collection parentCollection)
        {
            var webGet = new HtmlWeb();
            var document = webGet.Load(url);

            var itemWrapper = from node in document.DocumentNode.Descendants()
                              where node.Name == "div" &&
                              node.Attributes["id"] != null &&
                              node.Attributes["id"].Value == "divGridItemWrapper"
                              select node;
            foreach (HtmlNode wrapper in itemWrapper)
            {
                var items = from node in wrapper.Descendants()
                            where node.Name == "a" &&
                            node.Attributes["class"] != null &&
                            node.Attributes["class"].Value == "title"
                            select node;
                
                foreach (HtmlNode item in items)
                {
                    Recipe recipe;
                    String recipeUrl = item.Attributes["href"].Value;
                    if (recipesByUrl.ContainsKey(recipeUrl))
                        recipe = recipesByUrl[recipeUrl];
                    else
                    {
                        recipe = new Recipe(item.InnerText, recipeUrl);
                        getRecipeContents(recipe);
                        recipesByUrl.Add(recipeUrl, recipe);
                    }

                    recipe.Collections.Add(parentCollection);
                }
            }
        }

        private static void getRecipeContents(Recipe recipe)
        {
            var webGet = new HtmlWeb();
            var document = webGet.Load(recipe.Url);

            var ratingDiv = from node in document.DocumentNode.Descendants()
                            where node.Name == "div" &&
                            node.Attributes["itemprop"] != null &&
                            node.Attributes["itemprop"].Value == "aggregateRating"
                            select node;

            foreach (HtmlNode wrapper in ratingDiv)
            {
                var ratingMeta = from node in wrapper.Descendants()
                                 where node.Name == "meta" &&
                                 node.Attributes["itemprop"] != null &&
                                 node.Attributes["itemprop"].Value == "ratingValue"
                                 select node;

                foreach (HtmlNode rating in ratingMeta)
                {
                    recipe.Rating = Double.Parse(rating.Attributes["content"].Value);
                }
            }

            var ingredients = from node in document.DocumentNode.Descendants()
                              where node.Name == "li" &&
                              node.Attributes["id"] != null &&
                              node.Attributes["id"].Value == "liIngredient"
                              select node;

            foreach (HtmlNode ingredientNode in ingredients)
            {
                Ingredient ingredient = new Ingredient();
                ingredient.Id = int.Parse(ingredientNode.Attributes["data-ingredientid"].Value);
                ingredient.Grams = Double.Parse(ingredientNode.Attributes["data-grams"].Value);

                var spans = from node in ingredientNode.Descendants()
                            where node.Name == "span" &&
                              node.Attributes["class"] != null &&
                              node.Attributes["class"].Value.Contains("ingredient-")
                            select node;

                foreach (HtmlNode span in spans)
                {
                    if (span.Attributes["class"].Value == "ingredient-name")
                        ingredient.Name = span.InnerText;
                    if (span.Attributes["class"].Value == "ingredient-amount")
                        ingredient.Amount = span.InnerText;
                }

                recipe.Ingredients.Add(ingredient);
            }

            var instructions = from node in document.DocumentNode.Descendants()
                               where node.Name == "div" &&
                               node.Attributes["itemprop"] != null &&
                               node.Attributes["itemprop"].Value == "recipeInstructions"
                               select node;

            foreach (HtmlNode instructionNode in instructions)
            {
                var instructionSpan = from node in instructionNode.Descendants()
                                      where node.Name == "span" &&
                                      node.Attributes["class"] != null &&
                                      node.Attributes["class"].Value == "plaincharacterwrap break"
                                      select node;

                foreach (HtmlNode span in instructionSpan)
                {
                    recipe.Instructions.Add(span.InnerText);
                }
            }

            var timeSpans = from node in document.DocumentNode.Descendants()
                            where node.Name == "span" &&
                            node.Attributes["id"] != null &&
                            (node.Attributes["id"].Value.Contains("HoursSpan") ||
                            node.Attributes["id"].Value.Contains("MinsSpan"))
                            select node;

            int prepTime = 0;
            int cookTime = 0;
            int readyTime = 0;
            foreach (HtmlNode span in timeSpans)
            {
                if (span.Attributes["id"].Value == "prepHoursSpan")
                {
                    string txt = span.InnerText;
                    prepTime += int.Parse(txt.Substring(0, txt.IndexOf("hr") - 1)) * 60;
                }

                if (span.Attributes["id"].Value == "prepMinsSpan")
                {
                    string txt = span.InnerText;
                    prepTime += int.Parse(txt.Substring(0, txt.IndexOf("min") - 1));
                }

                if (span.Attributes["id"].Value == "cookHoursSpan")
                {
                    string txt = span.InnerText;
                    cookTime += int.Parse(txt.Substring(0, txt.IndexOf("hr") - 1)) * 60;
                }

                if (span.Attributes["id"].Value == "cookMinsSpan")
                {
                    string txt = span.InnerText;
                    cookTime += int.Parse(txt.Substring(0, txt.IndexOf("min") - 1));
                }

                if (span.Attributes["id"].Value == "totalHoursSpan")
                {
                    string txt = span.InnerText;
                    readyTime += int.Parse(txt.Substring(0, txt.IndexOf("hr") - 1)) * 60;
                }

                if (span.Attributes["id"].Value == "totalMinsSpan")
                {
                    string txt = span.InnerText;
                    readyTime += int.Parse(txt.Substring(0, txt.IndexOf("min") - 1));
                }
            }
            recipe.PrepMinutes = prepTime;
            recipe.CookMinutes = cookTime;
            recipe.ReadyMinutes = readyTime;

            var nutrients = from node in document.DocumentNode.Descendants()
                            where node.Name == "ul" &&
                            node.Attributes["id"] != null &&
                            node.Attributes["id"].Value == "ulNutrient"
                            select node;

            foreach (HtmlNode nutrientItem in nutrients)
            {
                var nutrientNodes = from node in nutrientItem.Descendants()
                                    where node.Name == "li"
                                    select node;

                Nutrient nutrient = new Nutrient();
                foreach (HtmlNode nutrientNode in nutrientNodes)
                {
                    if (nutrientNode.Attributes["class"] != null && nutrientNode.Attributes["class"].Value == "categories")
                        nutrient.Name = nutrientNode.InnerText;
                    if (nutrientNode.Attributes["class"] != null && nutrientNode.Attributes["class"].Value == "units")
                        nutrient.Value = nutrientNode.InnerText;
                    if (nutrientNode.Attributes["class"] != null && nutrientNode.Attributes["class"].Value == "percentages" &&
                        nutrientNode.InnerText.Contains('%'))
                        nutrient.Percent = int.Parse(nutrientNode.InnerText.Substring(0, nutrientNode.InnerText.IndexOf('%')));
                }
                recipe.Nutrients.Add(nutrient);
            }
            recipe.Complete = true;
        }
    }
}
