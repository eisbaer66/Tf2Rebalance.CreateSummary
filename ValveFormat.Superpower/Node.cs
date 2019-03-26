using System.Collections.Generic;
using System.Diagnostics;

namespace ValveFormat.Superpower
{
    [DebuggerDisplay("{Name}")]
    public class Node
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public IList<Node> Childs { get; set; }
        public Node Parent { get; set; }

        public Node(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public Node(string name, List<Node> childs)
        {
            Name = name;
            Childs = childs;
        }

        public Node()
        {
        }
    }
}