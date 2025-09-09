using System;

namespace SharpPacker.Strategy.BoxPacker.Models
{
    public class Box : IComparable<Box>
    {
        public virtual string Reference { get; set; }

        public virtual float EmptyWeight { get; set; }
        public virtual float MaxWeight { get; set; }
        public float WeightCapacity {
            get {
                if (MaxWeight <= 0)
                {
                    return 0;
                }
                else
                {
                    return MaxWeight - EmptyWeight;
                }
            }
        }

        public virtual float InnerDepth { get; set; }
        public virtual float InnerLength { get; set; }
        public virtual float InnerWidth { get; set; }
        public float InnerVolume => ((float)InnerWidth * InnerLength * InnerDepth);

        public virtual float OuterDepth { get; set; }
        public virtual float OuterLength { get; set; }
        public virtual float OuterWidth { get; set; }
        public float OuterVolume => ((float)OuterWidth * OuterLength * OuterDepth);

        public int CompareTo(Box other)
        {
            throw new NotImplementedException();
        }

        override public string ToString()
        {
            return $"Box {Reference} [w{InnerWidth}, l{InnerLength}, d{InnerDepth}]";
        }
    }
}
