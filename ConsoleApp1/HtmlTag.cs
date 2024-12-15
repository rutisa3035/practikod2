
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlRepresentation
{
    public class HtmlTag
    {
        // מאפיינים
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<string> Classes { get; set; } = new List<string>();
        public string InnerHtml { get; set; }
        public HtmlTag Parent { get; set; }
        public List<HtmlTag> Children { get; set; } = new List<HtmlTag>();

        // בנאי
        public HtmlTag(string name)
        {
            Name = name;
        }

        // הוספת מחלקה
        public void AddClass(string className)
        {
            if (!Classes.Contains(className))
                Classes.Add(className);
        }

        // הוספת מאפיין
        public void AddAttribute(string key, string value)
        {
            Attributes[key] = value;
        }

        // הוספת ילד
        public void AddChild(HtmlTag child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        // יצירת HTML כטקסט
        public string Render()
        {
            var sb = new StringBuilder();
            sb.Append($"<{Name}");

            if (!string.IsNullOrEmpty(Id))
                sb.Append($" id=\"{Id}\"");

            if (Classes.Count > 0)
                sb.Append($" class=\"{string.Join(" ", Classes)}\"");

            foreach (var attribute in Attributes)
                sb.Append($" {attribute.Key}=\"{attribute.Value}\"");

            sb.Append(">");

            if (!string.IsNullOrEmpty(InnerHtml))
                sb.Append(InnerHtml);

            foreach (var child in Children)
                sb.Append(child.Render());

            sb.Append($"</{Name}>");
            return sb.ToString();
        }

        // פונקציית Descendants
        public IEnumerable<HtmlTag> Descendants()
        {
            var queue = new Queue<HtmlTag>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;

                foreach (var child in current.Children)
                    queue.Enqueue(child);
            }
        }

        // פונקציית Ancestors
        public IEnumerable<HtmlTag> Ancestors()
        {
            var current = Parent;

            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

  

        public static IEnumerable<HtmlTag> FindElements(HtmlTag root, Selector selector)
        {
            if (root == null || selector == null) yield break;

            // יצירת HashSet למניעת כפילויות
            var visitedElements = new HashSet<HtmlTag>();

            // קבלת כל הצאצאים של העץ
            var descendants = root.Descendants();

            foreach (var element in descendants)
            {
                if (MatchesSelector(element, selector) && visitedElements.Add(element))
                {
                    // האלמנט עונה לקריטריונים ולא נמצא בעבר
                    yield return element;
                }

                if (selector.Child != null)
                {
                    foreach (var childElement in FindElements(element, selector.Child))
                    {
                        if (visitedElements.Add(childElement))
                            yield return childElement;
                    }
                }
            }
        }


        // בדיקת התאמת סלקטור
        private static bool MatchesSelector(HtmlTag element, Selector selector)
        {
            if (!string.IsNullOrEmpty(selector.TagName) && selector.TagName != element.Name)
                return false;

            if (!string.IsNullOrEmpty(selector.Id) && selector.Id != element.Id)
                return false;

            if (selector.Classes != null && selector.Classes.Any() &&
                !selector.Classes.All(c => element.Classes.Contains(c)))
                return false;

            return true;
        }
    }
}


