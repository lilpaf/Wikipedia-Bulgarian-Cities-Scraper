using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Xml;
using HtmlAgilityPack;

const string CityInParentheses = "(град)";
const string BulgariaInParentheses = "(България)";
const string Municipality = "област";

// URL of the webpage you want to scrape
string url = "https://bg.wikipedia.org/wiki/%D0%93%D1%80%D0%B0%D0%B4%D0%BE%D0%B2%D0%B5_%D0%B2_%D0%91%D1%8A%D0%BB%D0%B3%D0%B0%D1%80%D0%B8%D1%8F";

// Create a new HtmlWeb instance
HtmlWeb web = new HtmlWeb();

// Load the HTML document from the URL
HtmlDocument doc = web.Load(url);


HtmlNode tableNode = doc.DocumentNode.SelectSingleNode("//table");

if (tableNode != null)
{
    HtmlNodeCollection linkNodes = tableNode.SelectNodes(".//a[@href]");

    if (linkNodes != null)
    {
        Console.WriteLine("Links found on the table");

        List<string> cities = new();

        for (int i = 2; i < linkNodes.Count; i++)
        {
            string titleValue = linkNodes[i].GetAttributeValue("title", string.Empty);

            if (!string.IsNullOrEmpty(titleValue) && !titleValue.Contains(Municipality, StringComparison.CurrentCultureIgnoreCase))
            {
                if (titleValue.Contains(BulgariaInParentheses) || titleValue.Contains(CityInParentheses, StringComparison.CurrentCultureIgnoreCase))
                {
                    titleValue = titleValue.Replace(BulgariaInParentheses, string.Empty);
                    titleValue = titleValue.Replace(CityInParentheses, string.Empty);

                    titleValue = titleValue.TrimEnd();
                }

                cities.Add(titleValue);
            }
        }

        JsonSerializerOptions options = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(cities, options);

        File.WriteAllText("../../../cities.json", json);

        Console.WriteLine("JSON file has been created successfully");
    }
}
