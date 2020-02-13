using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApplication1
{
    class Polygon
    {
        private PointF[] poly;
        private Color color;

        public Polygon(PointF[] poly, Color color)
        {
            this.poly = poly;
            this.color = color;
        }
        public void DrawInChart(Graphics g, Axis ax, Axis ay)
        {            
            if (poly != null)
            {
                List<DataPoint> dp = new List<DataPoint>();
                for (int i = 0; i < poly.Length; i++)
                    dp.Add(new DataPoint(poly[i].X, poly[i].Y));

                List<PointF> points = dp.Select(x => new PointF(
                    (float)ax.ValueToPixelPosition(x.XValue),
                    (float)ay.ValueToPixelPosition(x.YValues[0])
                    )).ToList();

                using (SolidBrush brush = new SolidBrush(Color.FromArgb(32, color)))
                {
                    g.FillPolygon(brush, points.ToArray());
                }
            }
        }

        public bool IsInsidePoly(double x, double y)
        {

            if (poly == null)
                return false;

            double minX = poly[0].X;
            double maxX = poly[0].X;
            double minY = poly[0].Y;
            double maxY = poly[0].Y;
            for (int i = 1; i < poly.Length; i++)
            {
                PointF q = poly[i];
                minX = Math.Min(q.X, minX);
                maxX = Math.Max(q.X, maxX);
                minY = Math.Min(q.Y, minY);
                maxY = Math.Max(q.Y, maxY);
            }

            if (x < minX || x > maxX || y < minY || y > maxY)
            {
                return false;
            }

            // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            bool inside = false;
            for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
            {
                if ((poly[i].Y > y) != (poly[j].Y > y) &&
                     x < (poly[j].X - poly[i].X) * (y - poly[i].Y) / (poly[j].Y - poly[i].Y) + poly[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}
