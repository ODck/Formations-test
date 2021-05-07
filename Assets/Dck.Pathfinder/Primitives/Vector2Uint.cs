using System;
using System.Numerics;

namespace Dck.Pathfinder.Primitives
{
    public struct Vector2Uint
    {
        public uint X { get; set; }
        public uint Y { get; set; }

        public Vector2Uint(uint x, uint y)
        {
            X = x;
            Y = y;
        }

        public Vector2Uint(int x, int y)
        {
            if (x < 0 || y < 0) throw new Exception("Any index is negative");
            X = (uint) x;
            Y = (uint) y;
        }

        public void Set(uint x, uint y)
        {
            X = x;
            Y = y;
        }
        
        // Returns the length of this vector (RO).
        public float Magnitude => (float) Math.Sqrt((X * X + Y * Y));
        public static float Distance(Vector2Uint a, Vector2Uint b) { return (a - b).Magnitude; }

        // Returns a vector that is made from the smallest components of two vectors.
        public static Vector2Uint Min(Vector2Uint lhs, Vector2Uint rhs) { return new Vector2Uint(Math.Min(lhs.X, rhs.X), Math.Min(lhs.Y, rhs.Y)); }

        // Returns a vector that is made from the largest components of two vectors.
        public static Vector2Uint MaX(Vector2Uint lhs, Vector2Uint rhs) { return new Vector2Uint(Math.Max(lhs.X, rhs.X), Math.Max(lhs.Y, rhs.Y)); }

        // Multiplies two vectors component-wise.
        public static Vector2Uint Scale(Vector2Uint a, Vector2Uint b) { return new Vector2Uint(a.X * b.X, a.Y * b.Y); }

        // Multiplies every component of this vector by the same component of /scale/.
        public void Scale(Vector2Uint scale) { X *= scale.X; Y *= scale.Y; }
        
        
        public static Vector2Uint operator +(Vector2Uint a, Vector2Uint b)
        {
            return new Vector2Uint(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2Uint operator -(Vector2Uint a, Vector2Uint b)
        {
            return new Vector2Uint(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2Uint operator *(Vector2Uint a, Vector2Uint b)
        {
            return new Vector2Uint(a.X * b.X, a.Y * b.Y);
        }

        public static Vector2Uint operator *(Vector2Uint a, int b)
        {
            return new Vector2Uint((uint) (a.X * b), (uint) (a.Y * b));
        }

        public static bool operator ==(Vector2Uint lhs, Vector2Uint rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y;
        }

        public static bool operator !=(Vector2Uint lhs, Vector2Uint rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object other)
        {
            return other is Vector2Uint vector2Uint && Equals(vector2Uint);
        }

        public bool Equals(Vector2Uint other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ (Y.GetHashCode() << 2);
        }
    }

    public static class Vector2UintExtensions
    {
        public static Vector2 ToVector2(this Vector2Uint v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Vector2Uint ToVector2Uint(this Vector2 v)
        {
            return new Vector2Uint((uint) v.X, (uint) v.Y);
        }
    }
}