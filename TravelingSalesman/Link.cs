namespace TravelingSalesman
{
    public sealed class Link
    {
        public const double PheromoneAmount = 0.566546;
        public const double EvaporationRate = 0.01;

        public Node A { get; }
        public Node B { get; }
        public double Distance { get; }

        public double PheromoneOnLink { get; private set; }

        public Link(Node a, Node b, double distance)
        {
            A = a;
            B = b;
            Distance = distance;
        }

        public bool Same(Node a, Node b)
        {
            return (a == A && b == B) || (b == A && a == B);
        }

        public void Decay()
        {
            PheromoneOnLink -= PheromoneOnLink * EvaporationRate;
        }

        public void Compound()
        {
            PheromoneOnLink += PheromoneAmount / Distance;
        }
    }
}
