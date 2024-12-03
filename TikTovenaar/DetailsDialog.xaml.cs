using System.Net.Http;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace TikTovenaar
{
    /// <summary>
    /// Interaction logic for DetailsDialog.xaml
    /// </summary>
    public partial class DetailsDialog : Window
    {
        private string _word;
        public DetailsDialog(string word)
        {
            InitializeComponent();
            _word = word;

            titleText.Text = "Betekenis van het woord: '" + _word + "'";
            subTitleText.Text = "Even geduld...";

            // Get the meaning and category of the word
            GetMeaningAndCategoryFromWord(_word).ContinueWith(task =>
            {
                Dispatcher.Invoke(() =>
                {
                    subTitleText.Text = task.Result.meaning;
                });
            });
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public static async Task<(string category, string meaning)> GetMeaningAndCategoryFromWord(string word)
        {
            string apiUrl = $"https://nl.wiktionary.org/w/api.php?action=query&titles={word}&prop=revisions&rvprop=content&format=json";
            try
            {
                {
                    string response = await GetApiResponse(apiUrl);

                    // Parse the JSON response
                    JObject json = JObject.Parse(response);

                    // Extract the content of the site
                    var pages = json["query"]?["pages"];
                    if (pages != null)
                    {
                        foreach (var page in pages)
                        {
                            var content = page.First["revisions"]?[0]?["*"];
                            if (content != null)
                            {
                                (string category, string meaning) = ExtractMeaningAndCategoryFromWikitext(content.ToString());
                                return (category, meaning);
                            }
                            else
                            {
                                return ("Geen categorie gevonden", "Geen betekenis gevonden");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return ("Geen categorie gevonden", "Geen betekenis gevonden");
        }

        static (string category, string meaning) ExtractMeaningAndCategoryFromWikitext(string wikitext)
        {
            foreach (string line in wikitext.Split('\n'))
            {
                if (line.Trim().StartsWith("#"))
                {
                    // format
                    // #{{meubel|nld}} een meestal rechthoekig, soms rond meubelstuk met poten, bedoeld om dingen op te zetten of te leggen 
                    // or
                    // # een meestal rechthoekig, soms rond meubelstuk met poten, bedoeld om dingen op te zetten of te leggen

                    if (!line.Trim().Contains("{{"))
                    {
                        return ("No category found", line.Trim().Replace("#", ""));
                    }
                    else
                    {
                        int startCategory = line.Trim().IndexOf("{{") + 2;  // Skip over the {{
                        int endCategory = line.Trim().IndexOf("|");
                        int startMeaning = line.Trim().IndexOf("}}") + 2;  // Skip over the }}

                        // Extract category and meaning
                        string category = line.Substring(startCategory, endCategory - startCategory).Trim();
                        string meaning = line.Substring(startMeaning).Replace("[", "").Replace("]", "").Trim();

                        return (category, meaning);
                    }
                }
            }
            return ("No category found", "No meaning found");
        }

        static async Task<string> GetApiResponse(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
