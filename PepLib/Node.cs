using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PepLib
{
    public class Node
    {
        private Node parent;

        public List<Node> Children;

        public Node()
        {
            Children = new List<Node>();
        }

        public Node Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                UpdateDepth();
            }
        }

        public string Value { get; set; }

        private void UpdateDepth()
        {
            if (Parent != null)
                Level = Parent.Level + 1;
            else
                Level = 0;

            foreach (var node in Children)
                node.Parent = this;
        }

        public int Level { get; protected set; }

        public void AddChild(Node node)
        {
            node.Parent = this;
            Children.Add(node);
        }

        public void Write(TextWriter writer)
        {
            writer.WriteLine("".PadLeft(Level * 2) + this.ToString());

            foreach (var node in Children)
                node.Write(writer);
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public class KeyNode : Node
    {
        public string Name;

        public static KeyNode Parse(Node node)
        {
            var index = node.Value.IndexOf('=');

            if (index == -1)
                return null;

            return new KeyNode()
            {
                Name = node.Value.Remove(index),
                Value = node.Value.Remove(0, index + 1).Trim()
            };
        }

        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Value);
        }
    }
}
