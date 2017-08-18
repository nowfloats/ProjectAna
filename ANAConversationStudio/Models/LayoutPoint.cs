namespace ANAConversationStudio.Models
{
    public class LayoutPoint
    {
        public LayoutPoint(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public LayoutPoint() { }
        public double X { get; set; }
        public double Y { get; set; }
    }
}