using System.Collections.Generic;
using System.Linq;

namespace TravelingSalesman
{
    public sealed class Route
    {
        public IList<Node> Nodes { get; private set; }
        public double Length { get; private set; }
        public string Path {
            get {

                if (!Nodes.Any())
                    return "None";

                string output = "";

                foreach (var node in Nodes)
                {
                    output += node.Name + " ";
                }

                if(Nodes[0] != Nodes[Nodes.Count - 1])
                    output += Nodes[0].Name + " ";

                return output;
            }
        }

        public Route()
        {
            Update(new List<Node>(), -1);
        }

        public Route(IList<Node> route, double length)
        {
            Update(route, length);
        }

        public void Update(IList<Node> route, double length)
        {
            Nodes = route;
            Length = length;
        }

        public void Update(Route route)
        {
            Nodes = route.Nodes;
            Length = route.Length;
        }
    }
}
