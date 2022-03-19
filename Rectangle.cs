using System;
using System.Collections.Generic;
namespace RectangleGame
{
    /// <summary>
    /// represents rectangle with given height and width at (LeftTopX, LeftTopY)
    /// </summary>
    class Rectangle : IEquatable<Rectangle>
    {
        private int height;
        private int width;
        public int LeftTopX;
        public int LeftTopY;
        public int Height
        {
            get => height;
            set
            {
                height = value;
                Square = value * Width;
            }
        }
        public int Width
        {
            get => width;
            set
            {
                width = value;
                Square = value * Height;
            }
        }

        public int Square;

        public Rectangle(int X, int Y, int Width, int Height)
        {
            LeftTopX = X;
            LeftTopY = Y;
            height = Height;
            width = Width;
            Square = Height * Width;
        }
        /// <summary>
        /// Divides given rectangle with <param name="rect"></param> rectangle up to 4 possible partial rectangles of max square
        /// for ex. if rect. 0 slice with rect. 1 result will be four rects.
        /// 000000000    LLL000RRR UUUUUUUUU    LLL   RRR UUUUUUUUU
        /// 000000000    LLL000RRR UUUUUUUUU    LLL   RRR UUUUUUUUU
        /// 000111000 => LLL111RRR 000111000 or LLL   RRR
        /// 000111000    LLL111RRR 000111000    LLL   RRR
        /// 000000000    LLL000RRR LLLLLLLLL    LLL   RRR LLLLLLLLL
        /// </summary>
        /// <returns> List of partial rectangles</returns>
        public List<Rectangle> SliceWith(Rectangle rect)
        {
            var listRectResult = new List<Rectangle>();

            var rectIntersection = Rectangle.Intersection(this, rect);

            if (rectIntersection.Square == 0)// nothing to do
            {
                return listRectResult;
            }

            if (rectIntersection.LeftTopX > LeftTopX)// slice from the left
            {
                listRectResult.Add(new Rectangle(LeftTopX, LeftTopY, rectIntersection.LeftTopX - LeftTopX, Height));
            }

            if (rectIntersection.LeftTopX + rectIntersection.Width < LeftTopX + Width)// slice from the right
            {
                listRectResult.Add(new Rectangle(rectIntersection.LeftTopX + rectIntersection.Width, LeftTopY, Width - rectIntersection.Width - 1, Height));
            }

            if (rectIntersection.LeftTopY > LeftTopY)// upper slice
            {
                listRectResult.Add(new Rectangle(LeftTopX, LeftTopY, Width, rectIntersection.LeftTopY - LeftTopY));
            }

            if (rectIntersection.LeftTopY + rectIntersection.Height < LeftTopY + Height)// lower slice
            {
                listRectResult.Add(new Rectangle(LeftTopX, rectIntersection.LeftTopY + rectIntersection.Height, Width, Height - rectIntersection.Height - 1));
            }

            return listRectResult;
        }
        /// <summary>
        /// returns rectangle that is an intersection of rectangles
        /// <param name="rect1"></param> and
        /// <param name="rect2"></param>
        /// if rectangles are not intersectable Square, Width and Height of result rectangle equal 0
        /// </summary>
        public static Rectangle Intersection(Rectangle rect1, Rectangle rect2)
        {
            int x1 = Math.Max(rect1.LeftTopX, rect2.LeftTopX);
            int y1 = Math.Max(rect1.LeftTopY, rect2.LeftTopY);
            int x2 = Math.Min(rect1.LeftTopX + rect1.width - 1, rect2.LeftTopX + rect2.width - 1);
            int y2 = Math.Min(rect1.LeftTopY + rect1.height - 1, rect2.LeftTopY + rect2.height - 1);

            if ((x2 >= x1) && (y2 >= y1))
            {
                return new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
            }
            return new Rectangle(x1, y1, 0, 0);
        }

        public override bool Equals(object obj) => obj is Rectangle other && this.Equals(other);

        public bool Equals(Rectangle p) => LeftTopX == p.LeftTopX && LeftTopY == p.LeftTopY && height == p.height && width == p.width;

        public override int GetHashCode() => (LeftTopX, LeftTopY, height, width).GetHashCode();

        public static bool operator ==(Rectangle lhs, Rectangle rhs) => lhs.Equals(rhs);

        public static bool operator !=(Rectangle lhs, Rectangle rhs) => !(lhs == rhs);
    }
}
