


using System;
using System.Collections.Generic;

public class Selector
{
    public string TagName { get; set; }
    public string Id { get; set; }
    public List<string> Classes { get; set; }
    public Selector Parent { get; set; }
    public Selector Child { get; set; }

    public static Selector Parse(string query)
    {
        var parts = query.Split(' ');
        Selector root = null;
        Selector current = null;

        foreach (var part in parts)
        {
            var selector = new Selector { Classes = new List<string>() };
            int idIndex = part.IndexOf('#');
            int classIndex = part.IndexOf('.');

            if (idIndex != -1) selector.Id = part.Substring(idIndex + 1, (classIndex == -1 ? part.Length : classIndex) - idIndex - 1);
            if (classIndex != -1) selector.Classes.AddRange(part.Substring(classIndex + 1).Split('.'));

            if (idIndex == -1 && classIndex == -1) selector.TagName = part;

            if (root == null) root = selector;

            if (current != null)
            {
                current.Child = selector;
                selector.Parent = current;
            }

            current = selector;
        }

        return root;
    }
}


