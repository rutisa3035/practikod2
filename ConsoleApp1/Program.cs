//// See https://aka.ms/new-console-template for more information


using System.Text.RegularExpressions;
using HtmlRepresentation;
using System.Linq;
using System;
using System.IO;
using System.Net.Http;

class Program
{
    static async Task Main()
    {
        // הורדת ה-HTML
        var html = await Load("https://www.example.com");

        if (string.IsNullOrEmpty(html))
        {
            Console.WriteLine("Failed to download HTML.");
            return;
        }

        // מסנן את ה-HTML בצורה נכונה, בלי להשפיע על הרווחים
        var cleanHtml = new Regex("<(.*?)>").Replace(html, string.Empty);

        // מחלק את ה-HTML לשורות
        var htmlLines = new Regex("<(.*?)>").Split(cleanHtml)
            .Where(s => s.Length > 0)
            .ToList();

        // יצירת העץ
        HtmlTreeBuilder treeBuilder = new HtmlTreeBuilder();
        var root = treeBuilder.BuildTree(htmlLines);

        // הדפסת ה-HTML אחרי עיבוד
        Console.WriteLine(root.Render());

        // שמירת התוכן לקובץ HTML
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "output.html");
        File.WriteAllText(filePath, "<!DOCTYPE html>\n" + root.Render());

        Console.WriteLine($"HTML saved to {filePath}");

        // הצגת תגי HTML שנמצאו
        HtmlHelper htmlHelper = HtmlHelper.Instance;

        Console.WriteLine("All Tags:");
        foreach (var tag in htmlHelper.AllTags)
        {
            Console.WriteLine(tag);
        }

        Console.WriteLine("\nSelf-Closing Tags:");
        foreach (var tag in htmlHelper.SelfClosingTags)
        {
            Console.WriteLine(tag);
        }

        // בדיקת Self-Closing
        string testTag = "img";
        bool isSelfClosing = htmlHelper.IsSelfClosing(testTag);
        Console.WriteLine($"\nIs '{testTag}' a Self-Closing tag? {isSelfClosing}");
    }

    static async Task<string> Load(string url)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading HTML: {ex.Message}");
            return string.Empty;
        }
    }
}

