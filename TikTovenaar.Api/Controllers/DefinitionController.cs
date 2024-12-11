using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace TikTovenaar.Api.Controllers
{
    [Route("definition")]
    [ApiController]
    public class DefinitionController : ControllerBase
    {
        [HttpGet]
        public JsonResult GetDefinition([FromQuery] string word)
        {
            HttpClient client = new();
            HttpResponseMessage response = client.GetAsync($"https://nl.wiktionary.org/w/api.php?action=query&titles={word}&prop=revisions&rvprop=content&format=json").Result;
            dynamic? result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            try
            {
                dynamic pages = result!.query.pages;
                foreach (dynamic page in pages)
                {
                    try
                    {
                        string content = page.First.revisions[0]["*"].ToString();
                        foreach (string line in content.Split('\n'))
                        {
                            if (line.Trim().StartsWith('#') && line.Trim().Contains("{{"))
                            {
                                int startCategory = line.Trim().IndexOf("{{") + 2;
                                int endCategory = line.Trim().IndexOf('|');
                                int startMeaning = line.Trim().IndexOf("}}") + 2;
                                string category = line[startCategory..endCategory].Trim();
                                string meaning = line[startMeaning..].Replace("[", "").Replace("]", "").Trim();
                                return new(new { type = "success", message = new { category, meaning } });
                            }
                        }
                    }
                    catch (Exception) {}
                }
            }
            catch (Exception)
            {
                throw;
            }
            return new(new { type = "error", message = "Definition not found." });
        }
    }
}