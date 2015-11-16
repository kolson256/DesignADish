using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.Text.RegularExpressions;

namespace DesignADishAdmin
{
    public partial class AdminForm : Form
    {
        public AdminForm()
        {
            InitializeComponent();
        }

        HashSet<String> ingredients = new HashSet<string>();
        HashSet<String> stopTerms = new HashSet<string>();
        Dictionary<String, int> termCounts = new Dictionary<string, int>();
        List<KeyValuePair<string, int>> sortableList;
        int currentCount;

        private void btnExtractTerms_Click(object sender, EventArgs e)
        {
            int wordCount = int.Parse(cmbWordCount.Text.Trim());

            ingredients = new HashSet<string>();
            stopTerms = new HashSet<string>();

            // PostgeSQL-style connection string
            //string connstring = String.Format("Server={0};Port={1};" +
            //    "User Id={2};Password={3};Database={4};",
            //    "localhost", "5432", "postgres",
            //    "password", "designadish");

            string connstring = String.Format("Server={0};Port={1};" +
                "User Id={2};Password={3};Database={4};",
                "75.98.175.117", "5432", "designa3_admin",
                "passW0rd", "designa3_designadish");

            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            String sql = "SELECT Name, isIngredient From Term Where Words = " + wordCount.ToString();
            using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if ((bool)reader["isIngredient"])
                            ingredients.Add(reader["Name"].ToString());
                        else
                            stopTerms.Add(reader["Name"].ToString());
                    }
                }
            }

            termCounts = new Dictionary<string, int>();
            sql = "SELECT Name From Ingredient";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    Regex pattern = new Regex("[.?!,;:(){}\"']");
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        name = pattern.Replace(name, "").ToLower();
                        string[] terms = name.Split(null);

                        string[] fullTerm = new string[wordCount];
                        int count = 0;
                        foreach (string term in terms)
                        {
                            count++;
                            if (count < wordCount)
                            {
                                fullTerm[count - 1] = term;
                                continue;
                            }
                            else if (count == wordCount)
                            {
                                fullTerm[count - 1] = term;
                            }
                            else
                            {
                                for (int i = 0; i < wordCount - 1; i++)
                                {
                                    fullTerm[i] = fullTerm[i + 1];
                                }
                                fullTerm[wordCount - 1] = term;
                            }

                            string termStr = "";
                            foreach (string str in fullTerm)
                                if (termStr == "")
                                    termStr = str;
                                else
                                    termStr += " " + str;

                            if (termCounts.ContainsKey(termStr))
                            {
                                termCounts[termStr]++;
                            }
                            else
                            {
                                termCounts[termStr] = 1;
                            }
                        }
                    }
                }
            }

            sortableList = termCounts.Where(item => item.Value > 2).ToList();

            sortableList.Sort((firstPair, nextPair) =>
            {
                return nextPair.Value.CompareTo(firstPair.Value);
            });

            currentCount = -1;
            getNextTerm();
            if (currentCount == sortableList.Count)
                lblTerm.Text = "FINISHED";
            else
            {
                KeyValuePair<string, int> firstTerm = sortableList[currentCount];
                lblTerm.Text = firstTerm.Key;
                lblCount.Text = firstTerm.Value.ToString();
            }
        }

        private void getNextTerm()
        {
            currentCount++;
            if (currentCount == sortableList.Count)
                return;

            KeyValuePair<string, int> thisTerm = sortableList[currentCount];
            while (stopTerms.Contains(thisTerm.Key) || ingredients.Contains(thisTerm.Key))
            {
                currentCount++;
                thisTerm = sortableList[currentCount];
            }
           
        }

        private void btnTermCategory_Click(object sender, EventArgs e)
        {
            int wordCount = int.Parse(cmbWordCount.Text.Trim());

            Button btn = (Button)sender;

            // PostgeSQL-style connection string
            string connstring = String.Format("Server={0};Port={1};" +
                "User Id={2};Password={3};Database={4};",
                "75.98.175.117", "5432", "designa3_admin",
                "passW0rd", "designa3_designadish");

            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string sql = "INSERT INTO term (name, words, isingredient) VALUES (@name, @words, @isingredient)";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
            {
                command.Parameters.Add(new NpgsqlParameter("name", NpgsqlTypes.NpgsqlDbType.Text));
                command.Parameters.Add(new NpgsqlParameter("words", NpgsqlTypes.NpgsqlDbType.Smallint));
                command.Parameters.Add(new NpgsqlParameter("isingredient", NpgsqlTypes.NpgsqlDbType.Boolean));

                command.Parameters["name"].Value = lblTerm.Text;
                command.Parameters["words"].Value = wordCount;

                if (btn.Text == "Ingredient")
                    command.Parameters["isingredient"].Value = true;
                else
                    command.Parameters["isingredient"].Value = false;

                command.ExecuteNonQuery();
            }
            conn.Close();

            currentCount++;
            getNextTerm();
            if (currentCount == sortableList.Count)
                lblTerm.Text = "FINISHED";
            else
            {
                KeyValuePair<string, int> nextTerm = sortableList[currentCount];
                lblTerm.Text = nextTerm.Key;
                lblCount.Text = nextTerm.Value.ToString();
            }
        }

        private void btnBuildIndex_Click(object sender, EventArgs e)
        {
            // PostgeSQL-style connection string
            string connstring = String.Format("Server={0};Port={1};" +
                "User Id={2};Password={3};Database={4};",
                "75.98.175.117", "5432", "designa3_admin",
                "passW0rd", "designa3_designadish");

            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();
            
            string sql = "DELETE FROM recipetermindex";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
            {
                command.ExecuteNonQuery();
            }

            Dictionary<int, HashSet<String>> ingredientsByRecipeId = new Dictionary<int, HashSet<String>>();
            sql = "SELECT Name, RecipeId From Ingredient";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    Regex pattern = new Regex("[.?!,;:(){}\"']");
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        int recipeid = reader.GetInt32(1);
                        name = pattern.Replace(name, "").ToLower();

                        HashSet<String> ingredients = new HashSet<string>();
                        if (ingredientsByRecipeId.ContainsKey(recipeid))
                        {
                            ingredients = ingredientsByRecipeId[recipeid];
                        }
                        else
                        {
                            ingredientsByRecipeId.Add(recipeid, ingredients);
                        }

                        ingredients.Add(name);
                    }
                }
            }

            Dictionary<String, int> termIdByName = new Dictionary<String, int>();
            sql = "SELECT id, name From term WHERE isingredient = true";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int termid = reader.GetInt32(0);
                        string name = reader.GetString(1);

                        termIdByName.Add(name, termid);
                    }
                }
            }

            Dictionary<int, HashSet<int>> termIdsByRecipeId = new Dictionary<int, HashSet<int>>();
            foreach (int recipeId in ingredientsByRecipeId.Keys)
            {
                HashSet<String> ingredients = ingredientsByRecipeId[recipeId];
                HashSet<int> termIds = new HashSet<int>();

                foreach (String ingredient in ingredients)
                {
                    string[] terms = ingredient.Split(null);

                    int maxWordCount = terms.Length;
                    if (maxWordCount > 5)
                        maxWordCount = 5;

                    for (int wordCount = 1; wordCount <= maxWordCount; wordCount++)
                    {
                        string[] fullTerm = new string[wordCount];
                        int count = 0;
                        foreach (string term in terms)
                        {
                            count++;
                            if (count < wordCount)
                            {
                                fullTerm[count - 1] = term;
                                continue;
                            }
                            else if (count == wordCount)
                            {
                                fullTerm[count - 1] = term;
                            }
                            else
                            {
                                for (int i = 0; i < wordCount - 1; i++)
                                {
                                    fullTerm[i] = fullTerm[i + 1];
                                }
                                fullTerm[wordCount - 1] = term;
                            }

                            string termStr = "";
                            foreach (string str in fullTerm)
                                if (termStr == "")
                                    termStr = str;
                                else
                                    termStr += " " + str;

                            if (termIdByName.ContainsKey(termStr))
                            {
                                int termId = termIdByName[termStr];
                                termIds.Add(termId);
                            }
                        }
                    }
                }

                termIdsByRecipeId.Add(recipeId, termIds);

                sql = "INSERT INTO recipetermindex (recipeid, termid) VALUES (@recipeid, @termid)";
                using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
                {
                    command.Parameters.Add(new NpgsqlParameter("recipeid", NpgsqlTypes.NpgsqlDbType.Integer));
                    command.Parameters.Add(new NpgsqlParameter("termid", NpgsqlTypes.NpgsqlDbType.Integer));

                    foreach (int termId in termIds)
                    {
                        command.Parameters["recipeid"].Value = recipeId;
                        command.Parameters["termid"].Value = termId;

                        command.ExecuteNonQuery();
                    }
                }
            }
            conn.Close();

            MessageBox.Show("Done");
        }
    }
}
