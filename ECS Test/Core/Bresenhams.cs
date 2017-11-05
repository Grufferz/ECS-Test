using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    public static class Bresenhams
    {
        public static List<RogueSharp.Point> WalkGrid(RogueSharp.Point p0, RogueSharp.Point p1)
        {
            List<RogueSharp.Point> retList = new List<RogueSharp.Point>();

            int dx = p1.X - p0.X;
            int dy = p1.Y - p0.Y;
            int nx = Math.Abs(dx);
            int ny = Math.Abs(dy);
            int sign_x = dx > 0 ? 1 : -1;
            int sign_y = dy > 0 ? 1 : -1;

            retList.Add(new RogueSharp.Point( p0.X, p1.X ));
            RogueSharp.Point p = new RogueSharp.Point(p0.X, p0.Y);

            for (int ix = 0, iy = 0; ix < nx || iy < ny; )
            {
                if ((0.5 + ix) / nx < (0.5 + iy) / ny)
                {
                    p.X += sign_x;
                    ix++;
                }
                else
                {
                    p.Y += sign_y;
                    iy++;
                }
                retList.Add(new RogueSharp.Point(p.X, p.Y));
            }
            return retList;
        }

        public static List<RogueSharp.Point> SuperCoverLine(RogueSharp.Point p0, RogueSharp.Point p1)
        {
            List<RogueSharp.Point> retList = new List<RogueSharp.Point>();

            int dx = p1.X - p0.X;
            int dy = p1.Y - p0.Y;
            int nx = Math.Abs(dx);
            int ny = Math.Abs(dy);
            int sign_x = dx > 0 ? 1 : -1;
            int sign_y = dy > 0 ? 1 : -1;

            retList.Add(new RogueSharp.Point(p0.X, p1.X));
            RogueSharp.Point p = new RogueSharp.Point(p0.X, p0.Y);

            for (int ix = 0, iy = 0; ix < nx || iy < ny;)
            {
                if ((0.5 + ix) / nx == (0.5 + iy) / ny)
                {
                    p.X += sign_x;
                    p.Y += sign_y;
                    ix++;
                    iy++;
                }
                else if ((0.5+ix) / nx < (0.5+iy) / ny)
                {
                    p.X += sign_x;
                    ix++;
                }
                else
                {
                    p.Y += sign_y;
                    iy++;
                }
                retList.Add(new RogueSharp.Point(p.X, p.Y));
            }
            return retList;
        }
    }
}