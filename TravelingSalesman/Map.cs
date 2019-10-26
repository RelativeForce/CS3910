using System.Collections.Generic;
using System.Linq;

namespace TravelingSalesman
{
    public sealed class Map
    {
        public IList<Node> Nodes { get; }
        public ISet<Link> Links { get; }

        public Map(string[] fileText)
        {
            Nodes = new List<Node>();
            Links = new HashSet<Link>();

            var nodeData = fileText.Select(s => s.Split(',')).Where(d => d.Length == 3);

            foreach (var data in nodeData)
            {
                AddNode(data[0], double.Parse(data[1]), double.Parse(data[2]));
            }

            foreach (var node in Nodes)
            {
                AddLinks(node);
            }
        }

        private void AddNode(string name, double x, double y)
        {
            if (Nodes.Any(n => n.Name.Equals(name)))
                return;

            Nodes.Add(new Node(name, x, y));
        }

        private void AddLinks(Node node)
        {
            foreach (var n in Nodes)
            {
                if (n == node || Links.Any(l => l.Same(node, n)))
                    continue;

                var distance = Point.Distance(node.Position, n.Position);

                var link = new Link(node, n, distance);

                Links.Add(link);

                node.Links.Add(link);
                node.LinkedTo.Add(n);

                n.LinkedTo.Add(node);
                n.Links.Add(link);
            }
        }
    }
}
