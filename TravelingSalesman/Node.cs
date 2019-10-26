using System.Collections.Generic;

namespace TravelingSalesman
{
    public sealed class Node
    {
        public string Name { get; }
        public Point Position { get; }
        public ISet<Link> Links { get; }
        public ISet<Node> LinkedTo { get; }

        public Node(string name, double x, double y)
        {
            Name = name;
            Links = new HashSet<Link>();
            LinkedTo = new HashSet<Node>();
            Position = new Point(x, y);
        }
    }
}
