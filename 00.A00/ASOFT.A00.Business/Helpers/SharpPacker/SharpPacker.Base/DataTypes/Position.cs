using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPacker.Base.DataTypes
{
    public struct Position
    {
        public Position(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public float X;
        public float Y;
        public float Z;

        public override string ToString()
        {
            return $"{{\"X\": {X}, \"Y\": {Y}, \"Z\": {Z}}}";
        }
    }
}
