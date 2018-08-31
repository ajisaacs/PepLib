using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PepLib
{
    public class IniConfig
    {
        public List<Node> Nodes;

        public IniConfig()
        {
            Nodes = new List<Node>();
        }

        private static int LeadingWhitespaceCount(string s)
        {
            for (int i = 0; i < s.Length; ++i)
                if (s[i] != ' ') return i;

            return 0;
        }

        public Node FindNode(string path)
        {
            return FindNode(path, Nodes);
        }

        private Node FindNode(string path, List<Node> nodes)
        {
            var a = path.Split('/');

            var b = nodes.FirstOrDefault(node =>
            {
                if (node is KeyNode)
                {
                    var c = node as KeyNode;
                    return c.Name.ToUpper() == a[0].ToUpper();
                }
                else
                {
                    return node.Value == a[0].Trim();
                }
            });

            string path2 = string.Empty;

            for (int i = 1; i < a.Length; ++i)
                path2 += a[i] + '/';

            if (b == null || a.Length == 1)
                return b;
            else
                return FindNode(path2.TrimEnd('/'), b.Children);
        }

        public static IniConfig Load(string file)
        {
            var doc = new IniConfig();
            var reader = new StreamReader(file);

            Node currentNode = null;
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                int spaces = LeadingWhitespaceCount(line) / 2;
                var node = new Node();
                node.Value = line.Trim();

                var keyNode = KeyNode.Parse(node);

                if (keyNode != null)
                    node = keyNode;

                int currentdepth = currentNode != null ? currentNode.Level : 0;

                if (spaces == 0)
                    doc.Nodes.Add(node);
                else if (spaces == currentdepth)
                    currentNode.Parent.AddChild(node);
                else if (spaces > currentdepth)
                    currentNode.AddChild(node);
                else if (spaces < currentdepth)
                {
                    var n = currentNode.Parent;

                    while (spaces < n.Level)
                        n = n.Parent;

                    n.Parent.AddChild(node);
                }

                currentNode = node;
            }

            reader.Close();

            return doc;
        }
    }
}
