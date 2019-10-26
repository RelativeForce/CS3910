namespace TravelingSalesman
{
    public sealed class Link
    {
        public Node A { get; }
        public Node B { get; }
        public double Distance { get; }

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
    }
}
