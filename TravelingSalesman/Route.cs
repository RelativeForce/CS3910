using System.Collections.Generic;
using System.Linq;

namespace TravelingSalesman
{
    public sealed class Route
    {
        public IList<int> Indexes;
        public IList<Node> Nodes;
        public double Length;
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

        public Route(IList<int> indexes, IList<Node> nodes, double length)
        {
            Indexes = indexes;
            Length = length;
            Nodes = nodes;
        }
    }
}
