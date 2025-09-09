namespace SharpPacker.Strategy.BoxPacker.Models
{
    public class PackedItem
    {
        public Item Item;

        public PackedItem()
        {
        }

        public PackedItem(Item item,
                int x,
                int y,
                int z,
                int width,
                int length,
                int depth
            )
        {
            this.Item = item;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Width = width;
            this.Length = length;
            this.Depth = depth;
        }

        public float Depth { get; set; }
        public float Length { get; set; }
        public float Volume => ((float)Width * Length * Depth);
        public float Width { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Weight => Item.Weight;

        public static PackedItem FromOrientatedItem(OrientatedItem oi, float x, float y, float z)
        {
            return new PackedItem()
            {
                Item = oi.Item,
                Width = oi.Width,
                Length = oi.Length,
                Depth = oi.Depth,
                X = x,
                Y = y,
                Z = z,
            };
        }

        public OrientatedItem ToOrientatedItem()
        {
            var result = new OrientatedItem()
            {
                Item = this.Item,
                Width = this.Width,
                Length = this.Length,
                Depth = this.Depth,
            };

            return result;
        }

        public override string ToString()
        {
            return $"PackedItem4d {Item?.Description} [w{Width}, l{Length}, d{Depth}] (x{X}, y{Y}, z{Z})";
        }
    }
}
