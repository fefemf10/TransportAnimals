using System.Drawing;
using TransportAnimals.Models;
using TransportAnimals.ViewModels.Request;

namespace TransportAnimals.Helpers
{
    public struct AABB
    {
        public AABB(RequestLocationPoint[] p)
        {
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i].Latitude < xMin)
                    xMin = p[i].Latitude.Value;
                if (p[i].Latitude > xMax)
                    xMax = p[i].Latitude.Value;
                if (p[i].Longitude < yMin)
                    yMin = p[i].Longitude.Value;
                if (p[i].Longitude > yMax)
                    yMax = p[i].Longitude.Value;
            }
        }
        public bool Intersects(AABB box)
        {
            bool collisionX = X + SizeX > box.X && box.X + box.SizeX > X;
            bool collisionY = Y + SizeY > box.Y && box.Y + box.SizeY > Y;
            return collisionX && collisionY;
        }
        public bool Contains(RequestLocationPoint p)
        {
            bool containsX = p.Latitude > X && p.Latitude <= X + SizeX;
            bool containsY = p.Longitude > Y && p.Longitude <= Y + SizeY;
            return containsX && containsY;
        }
        private double xMin = double.MaxValue;
        private double xMax = double.MinValue;
        private double yMin = double.MaxValue;
        private double yMax = double.MinValue;
        private double SizeX { get { return xMax - xMin; } }
        private double SizeY { get { return yMax - yMin; } }
        private double X { get { return xMin; } }
        private double Y { get { return yMin; } }
    }
    public static class Geometry
    {
        const double EPS = 1E-18;
        public static bool InLine(RequestLocationPoint p1, RequestLocationPoint p2, RequestLocationPoint p)
        {
            double xDiff = p2.Latitude.Value - p1.Latitude.Value;
            double yDiff = p2.Longitude.Value - p1.Longitude.Value;
            double top = (p.Latitude.Value - p1.Latitude.Value) * yDiff - (p.Longitude.Value - p1.Longitude.Value) * xDiff;
            return top == 0;
        }
        //public static bool InLine(RequestLocationPoint p1, RequestLocationPoint p2, RequestLocationPoint p)
        //{
        //    return (p.Latitude - p1.Latitude) / (p2.Latitude - p1.Latitude) == (p.Longitude - p1.Longitude) / (p2.Longitude - p1.Longitude);
        //}
        public static bool InLineAllPoints(RequestLocationPoint[] points)
        {
            for (int i = 2; i < points.Length; i++)
            {
                if (!InLine(points[0], points[1], points[i]))
                    return false;
            }
            return true;
        }
        public static bool TestAABB(in RequestLocationPoint[] p1, in RequestLocationPoint[] p2)
        {
            AABB box1 = new(p1);
            AABB box2 = new(p2);
            return box1.Intersects(box2);
        }
        public static bool TestAABB(in RequestLocationPoint[] p1, in RequestLocationPoint p)
        {
            AABB box1 = new(p1);
            return box1.Contains(p);
        }
        public static bool InPolygon(bool hasSamePoints, RequestLocationPoint[] points, RequestLocationPoint p)
        {
            return InPolygon2(points, p);
        }
        //public static bool InPolygon(in RequestLocationPoint[] points, in RequestLocationPoint p)
        //{
        //    int i1, i2, n, N;
        //    bool flag = false;
        //    double S, S1, S2, S3;
        //    N = points.Length;
        //    for (n = 0; n < N; n++)
        //    {
        //        flag = false;
        //        i1 = n < N - 1 ? n + 1 : 0;
        //        while (!flag)
        //        {
        //            i2 = i1 + 1;
        //            if (i2 > N)
        //                i2 = 0;
        //            if (i2 == (n < N - 1 ? n + 1 : 0))
        //                break;
        //            S = Math.Abs((double)(points[i1].Latitude * (points[i2].Longitude - points[n].Longitude) +
        //                     points[i2].Latitude * (points[n].Longitude - points[i1].Longitude) +
        //                     points[n].Latitude * (points[i1].Longitude - points[i2].Longitude)));
        //            S1 = Math.Abs((double)(points[i1].Latitude * (points[i2].Longitude - p.Longitude) +
        //                      points[i2].Latitude * (p.Longitude - points[i1].Longitude) +
        //                      p.Latitude * (points[i1].Longitude - points[i2].Longitude)));
        //            S2 = Math.Abs((double)(points[n].Latitude * (points[i2].Longitude - p.Longitude) +
        //                      points[i2].Latitude * (p.Longitude - points[n].Longitude) +
        //                      p.Latitude * (points[n].Longitude - points[i2].Longitude)));
        //            S3 = Math.Abs((double)(points[i1].Latitude * (points[n].Longitude - p.Longitude) +
        //                      points[n].Latitude * (p.Longitude - points[i1].Longitude) +
        //                      p.Latitude * (points[i1].Longitude - points[n].Longitude)));
        //            if (S == S1 + S2 + S3)
        //            {
        //                flag = true;
        //                break;
        //            }
        //            i1 = i1 + 1;
        //            if (i1 > N)
        //                i1 = 0;
        //            break;
        //        }
        //        if (!flag)
        //            break;
        //    }
        //    return flag;
        //}
        public static bool InPolygon1(RequestLocationPoint[] points, RequestLocationPoint p)
        {
            bool result = false;
            int j = points.Length - 1;
            for (int i = 0; i < points.Length; i++)
            {
                if ((points[i].Longitude <= p.Longitude && points[j].Longitude >= p.Longitude || points[j].Longitude <= p.Longitude && points[i].Longitude >= p.Longitude) &&
                     (points[i].Latitude + (p.Longitude - points[i].Longitude) / (points[j].Longitude - points[i].Longitude) * (points[j].Latitude - points[i].Latitude) < p.Latitude))
                    result = !result;
                j = i;
            }
            return result;
        }
        public static bool InPolygon2(RequestLocationPoint[] points, RequestLocationPoint p)
        {
            if (points.Any(a => a.Latitude == p.Latitude && a.Longitude == p.Longitude))
                return false;
            bool isInside = false;

            for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
            {
                if (((points[i].Longitude > p.Longitude) != (points[j].Longitude > p.Longitude)) &&
                (p.Latitude < (points[j].Latitude - points[i].Latitude) * (p.Longitude - points[i].Longitude) / (points[j].Longitude - points[i].Longitude) + points[i].Latitude))
                {
                    isInside = !isInside;
                }
            }

            return isInside;
            //bool result = false;
            //int j = points.Length - 1;
            //for (int i = 0; i < points.Length; i++)
            //{
            //    if ((points[i].Longitude <= p.Longitude && points[j].Longitude >= p.Longitude || points[j].Longitude <= p.Longitude && points[i].Longitude >= p.Longitude) &&
            //         (points[i].Latitude + (p.Longitude - points[i].Longitude) / (points[j].Longitude - points[i].Longitude) * (points[j].Latitude - points[i].Latitude) < p.Latitude))
            //        result = !result;
            //    j = i;
            //}
            //return result;
        }
        private static double area(RequestLocationPoint a, RequestLocationPoint b, RequestLocationPoint c)
        {
            double s = (double)((b.Latitude - a.Latitude) * (c.Longitude - a.Longitude) - (b.Longitude - a.Longitude) * (c.Latitude - a.Latitude));
            return Math.Abs(s) < EPS ? 0 : s > 0 ? 1 : -1;
        }

        private static bool intersect_1(double a, double b, double c, double d)
        {
            if (a > b) (a, b) = (b, a);
            if (c > d) (c, d) = (d, c);
            return Math.Max(a, c) < Math.Min(b, d) + EPS;
        }
        public static bool Intersect(RequestLocationPoint a, RequestLocationPoint b, RequestLocationPoint c, RequestLocationPoint d)
        {
            return intersect_1(a.Latitude.Value, b.Latitude.Value, c.Latitude.Value, d.Latitude.Value)
                && intersect_1(a.Longitude.Value, b.Longitude.Value, c.Longitude.Value, d.Longitude.Value)
                && area(a, b, c) * area(a, b, d) <= 0
                && area(c, d, a) * area(c, d, b) <= 0;
        }
        public static bool IntersectPolygon(RequestLocationPoint[] points)
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                for (int j = i + 1; j < points.Length - 1; j++)
                {
                    if (i != j && Intersect(points[i], points[i + 1], points[j], points[j + 1]))
                      return true;
                }
            }
            return false;
        }

        public static bool IsSelfIntersecting(RequestLocationPoint[] points)
        {
            // Iterate through all edges in the polygon
            for (int i = 0; i < points.Length; i++)
            {
                RequestLocationPoint firstEndpoint = points[i];
                RequestLocationPoint secondEndpoint = points[(i + 1) % points.Length];

                // Iterate through all edges again
                for (int j = 0; j < points.Length; j++)
                {
                    RequestLocationPoint thirdEndpoint = points[j];
                    RequestLocationPoint fourthEndpoint = points[(j + 1) % points.Length];

                    if (i != j && AreLinesIntersecting(firstEndpoint, secondEndpoint, thirdEndpoint, fourthEndpoint))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool AreLinesIntersecting(RequestLocationPoint firstEndpoint, RequestLocationPoint secondEndpoint, RequestLocationPoint thirdEndpoint, RequestLocationPoint fourthEndpoint)
        {
            double d1 = (double)((secondEndpoint.Latitude - firstEndpoint.Latitude) * (thirdEndpoint.Longitude - firstEndpoint.Longitude) - (secondEndpoint.Longitude - firstEndpoint.Longitude) * (thirdEndpoint.Latitude - firstEndpoint.Latitude));
            double d2 = (double)((secondEndpoint.Latitude - firstEndpoint.Latitude) * (fourthEndpoint.Longitude - firstEndpoint.Longitude) - (secondEndpoint.Longitude - firstEndpoint.Longitude) * (fourthEndpoint.Latitude - firstEndpoint.Latitude));
            double d3 = (double)((fourthEndpoint.Latitude - thirdEndpoint.Latitude) * (firstEndpoint.Longitude - thirdEndpoint.Longitude) - (fourthEndpoint.Longitude - thirdEndpoint.Longitude) * (firstEndpoint.Latitude - thirdEndpoint.Latitude));
            double d4 = (double)((fourthEndpoint.Latitude - thirdEndpoint.Latitude) * (secondEndpoint.Longitude - thirdEndpoint.Longitude) - (fourthEndpoint.Longitude - thirdEndpoint.Longitude) * (secondEndpoint.Latitude - thirdEndpoint.Latitude));

            if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) && ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)))
            {
                return true;
            }

            return false;
        }

    }
}
