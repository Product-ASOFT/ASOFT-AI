namespace SharpPacker.Strategy.BoxPacker.Models
{
    internal class WorkingVolume : Box
    {
        private readonly float depth;
        private readonly float length;
        private readonly float width;
        private readonly float maxWeight;

        public WorkingVolume(float _width,
                                float _length,
                                float _depth,
                                float _maxWeight
                                )
        {
            width = _width;
            length = _length;
            depth = _depth;
            maxWeight = _maxWeight;
        }

        public override float EmptyWeight => 0;
        public override float InnerDepth => depth;
        public override float InnerLength => length;
        public override float InnerWidth => width;
        public override float MaxWeight => maxWeight;
        public override float OuterDepth => depth;
        public override float OuterLength => length;
        public override float OuterWidth => width;
        public override string Reference => "Working Volume";
    }
}
