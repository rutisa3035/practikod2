using System.Text.Json;

public class HtmlHelper
{
    private readonly static HtmlHelper _instance = new HtmlHelper();

    public static HtmlHelper Instance => _instance;
    // מאפיינים לאחסון התגיות
    public string[] AllTags { get; private set; }
    public string[] SelfClosingTags { get; private set; }

    // בנאי לטעינת הנתונים מהקבצים
    public HtmlHelper()
    {
        // טענת קובץ התגיות
        //AllTags = LoadTagsFromJson("JsonFiles/allTags.json");

        AllTags = LoadTagsFromJson("JsonFiles/HtmlTags.json");

        // טענת קובץ ה-Self-Closing
        SelfClosingTags = LoadTagsFromJson("JsonFiles/HtmlVoidTags.json");
    }

    // שיטה לטעינת תגיות מקובץ JSON
    private string[] LoadTagsFromJson(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"File {fileName} not found.");
        }

        var json = File.ReadAllText(fileName);
        return JsonSerializer.Deserialize<string[]>(json);
    }

    // שיטה לבדיקת Self-Closing
    public bool IsSelfClosing(string tag)
    {
        return SelfClosingTags.Contains(tag);
    }
}




