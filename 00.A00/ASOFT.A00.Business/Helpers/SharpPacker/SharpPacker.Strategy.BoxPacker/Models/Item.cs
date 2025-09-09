using System;

namespace SharpPacker.Strategy.BoxPacker.Models
{
    public class Item : IComparable<Item>
    {
        public float Depth { get; set; }
        public string Description { get; set; }

        public bool KeepFlat { get; set; }
        public float Length { get; set; }
        public float Volume => ((float)Width * Length * Depth);
        public float Weight { get; set; }
        public float Width { get; set; }

        public int CompareTo(Item other)
        {
            throw new NotImplementedException();
        }

        override public string ToString()
        {
            return $"Item {Description} [w{Width}, l{Length}, d{Depth}]";
        }
    }
}
