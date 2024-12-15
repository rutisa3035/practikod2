
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlRepresentation; // ודאי שה-namespace מתאים לקוד שלך

class HtmlTreeBuilder
{
    private readonly HtmlHelper _htmlHelper = HtmlHelper.Instance;

    public HtmlTag BuildTree(IEnumerable<string> htmlLines)
    {
        HtmlTag root = new HtmlTag("html");
        HtmlTag current = root;

        foreach (var line in htmlLines)
        {
            string trimmedLine = line.Trim();

            if (string.IsNullOrEmpty(trimmedLine))
                continue;

            if (trimmedLine == "html/")
            {
                break;
            }

            // אם זו תגית סגירה
            if (trimmedLine.StartsWith("</"))
            {
                if (current.Parent != null)
                {
                    current = current.Parent; // חזרה להורה של current
                }
                else
                {
                    Console.WriteLine("Error: Parent is null");
                }
                continue;
            }

            // אם זו תגית פתיחה
            if (trimmedLine.StartsWith("<") && !trimmedLine.StartsWith("</"))
            {
                var match = Regex.Match(trimmedLine, @"<(\w+)(.*?)(/?)>");
                if (!match.Success) continue;

                string tagName = match.Groups[1].Value;
                string attributesString = match.Groups[2].Value;
                bool isSelfClosing = !string.IsNullOrEmpty(match.Groups[3].Value) ||
                                     _htmlHelper.IsSelfClosing(tagName);

                HtmlTag newTag = new HtmlTag(tagName);
                newTag.Parent = current; // הגדרת ה-Parent של התגית החדשה כ- current

                var attributeMatches = Regex.Matches(attributesString, @"(\w+)=""(.*?)""");
                foreach (Match attributeMatch in attributeMatches)
                {
                    string key = attributeMatch.Groups[1].Value;
                    string value = attributeMatch.Groups[2].Value;

                    if (key == "class")
                    {
                        foreach (var className in value.Split(' '))
                        {
                            newTag.AddClass(className);
                        }
                    }
                    else if (key == "id")
                    {
                        newTag.Id = value;
                    }
                    else
                    {
                        newTag.AddAttribute(key, value);
                    }
                }

                current.AddChild(newTag); // הוספת התגית החדשה כילד של current

                // אם התגית אינה סגורה, אנחנו שומרים אותה כ- current
                if (!isSelfClosing)
                {
                    current = newTag;
                }

                continue;
            }

            // הוספת תוכן לתוך current
            if (current != null)
            {
                current.InnerHtml += trimmedLine;
            }
            else
            {
                Console.WriteLine("Error: current is null");
            }
        }

        return root;
    }
}


