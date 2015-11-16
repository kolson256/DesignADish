using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeExtract.AllRecipe;
using Npgsql;

namespace RecipeExtract
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                List<Collection> groups = AllRecipeExtract.Collections;
            }
            catch (Exception ex) { }

            // PostgeSQL-style connection string
            string connstring = String.Format("Server={0};Port={1};" +
                "User Id={2};Password={3};Database={4};",
                "localhost", "5432", "postgres",
                "password", "designadish");

            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            int nextId;
            using (NpgsqlCommand command = new NpgsqlCommand("select nextval('recipe_id_seq')", conn))
            {
                nextId = int.Parse(command.ExecuteScalar().ToString()) + 1;
            }

            string sql = "INSERT INTO recipe (name, url, rating, prepminutes, cookminutes, totalminutes) VALUES (@name, @url, @rating, @prepminutes, @cookminutes, @totalminutes)";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
            {
                command.Parameters.Add(new NpgsqlParameter("name", NpgsqlTypes.NpgsqlDbType.Text));
                command.Parameters.Add(new NpgsqlParameter("url", NpgsqlTypes.NpgsqlDbType.Text));
                command.Parameters.Add(new NpgsqlParameter("rating", NpgsqlTypes.NpgsqlDbType.Double));
                command.Parameters.Add(new NpgsqlParameter("prepminutes", NpgsqlTypes.NpgsqlDbType.Smallint));
                command.Parameters.Add(new NpgsqlParameter("cookminutes", NpgsqlTypes.NpgsqlDbType.Smallint));
                command.Parameters.Add(new NpgsqlParameter("totalminutes", NpgsqlTypes.NpgsqlDbType.Smallint));

                foreach (Recipe recipe in AllRecipeExtract.recipesByUrl.Values)
                {
                    if (recipe.Complete)
                    {
                        command.Parameters["name"].Value = recipe.Title;
                        command.Parameters["url"].Value = recipe.Url;
                        command.Parameters["rating"].Value = recipe.Rating;
                        command.Parameters["prepminutes"].Value = recipe.PrepMinutes;
                        command.Parameters["cookminutes"].Value = recipe.CookMinutes;
                        command.Parameters["totalminutes"].Value = recipe.ReadyMinutes;
                        command.ExecuteNonQuery();
                        recipe.Id = nextId;
                        nextId++;
                    }
                }
            }

            sql = "INSERT INTO ingredient (name, amount, externalid, grams, recipeid) VALUES (:name, :amount, :externalid, :grams, :recipeid)";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
            {
                command.Parameters.Add(new NpgsqlParameter("name", NpgsqlTypes.NpgsqlDbType.Text));
                command.Parameters.Add(new NpgsqlParameter("amount", NpgsqlTypes.NpgsqlDbType.Text));
                command.Parameters.Add(new NpgsqlParameter("externalid", NpgsqlTypes.NpgsqlDbType.Text));
                command.Parameters.Add(new NpgsqlParameter("grams", NpgsqlTypes.NpgsqlDbType.Double));
                command.Parameters.Add(new NpgsqlParameter("recipeid", NpgsqlTypes.NpgsqlDbType.Integer));

                foreach (Recipe recipe in AllRecipeExtract.recipesByUrl.Values)
                {
                    if (recipe.Complete)
                    {
                        foreach (Ingredient ingredient in recipe.Ingredients)
                        {
                            if (ingredient.Name == null)
                                continue;

                            command.Parameters["name"].Value = ingredient.Name;
                            command.Parameters["amount"].Value = ingredient.Amount;
                            command.Parameters["externalid"].Value = ingredient.Id;
                            command.Parameters["grams"].Value = ingredient.Grams;
                            command.Parameters["recipeid"].Value = recipe.Id;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }

            sql = "INSERT INTO nutrient (name, value, percent, recipeid) VALUES (:name, :value, :percent, :recipeid)";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
            {
                command.Parameters.Add(new NpgsqlParameter("name", NpgsqlTypes.NpgsqlDbType.Text));
                command.Parameters.Add(new NpgsqlParameter("value", NpgsqlTypes.NpgsqlDbType.Text));
                command.Parameters.Add(new NpgsqlParameter("percent", NpgsqlTypes.NpgsqlDbType.Smallint));
                command.Parameters.Add(new NpgsqlParameter("recipeid", NpgsqlTypes.NpgsqlDbType.Integer));

                foreach (Recipe recipe in AllRecipeExtract.recipesByUrl.Values)
                {
                    if (recipe.Complete)
                    {
                        foreach (Nutrient nutrient in recipe.Nutrients)
                        {
                            command.Parameters["name"].Value = nutrient.Name;
                            command.Parameters["value"].Value = nutrient.Value;
                            command.Parameters["percent"].Value = nutrient.Percent;
                            command.Parameters["recipeid"].Value = recipe.Id;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }

            sql = "INSERT INTO instruction (name, recipeid) VALUES (:name, :recipeid)";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
            {
                command.Parameters.Add(new NpgsqlParameter("name", NpgsqlTypes.NpgsqlDbType.Text));
                command.Parameters.Add(new NpgsqlParameter("name", NpgsqlTypes.NpgsqlDbType.Text));
                command.Parameters.Add(new NpgsqlParameter("recipeid", NpgsqlTypes.NpgsqlDbType.Integer));

                foreach (Recipe recipe in AllRecipeExtract.recipesByUrl.Values)
                {
                    if (recipe.Complete)
                    {
                        foreach (String instruction in recipe.Instructions)
                        {
                            command.Parameters["name"].Value = instruction;
                            command.Parameters["recipeid"].Value = recipe.Id;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }

            conn.Close();

            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
