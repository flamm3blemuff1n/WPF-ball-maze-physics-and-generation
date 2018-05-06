using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Opdracht1
{
    public class Sphere
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        public double Radius { get; }

        private int VerticesVertical;
        private int VerticesHorizontal;

        public GeometryModel3D Model { get; private set; }

        public Sphere(int x, int y, int z, double radius, int verticesVertical, int verticesHorizontal)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Radius = radius;
            this.VerticesVertical = verticesVertical;
            this.VerticesHorizontal = verticesHorizontal;

            CreateSphere();
        }

        /*
         * http://csharphelper.com/blog/2017/05/make-3d-globe-wpf-c/
         */
        private void CreateSphere()
        {
            MeshGeometry3D sphere_mesh = new MeshGeometry3D();
            GeometryModel3D model = new GeometryModel3D(sphere_mesh, new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri("images/ball.jpg", UriKind.Relative)))));

            double dphi = Math.PI / VerticesVertical;
            double dtheta = 2 * Math.PI / VerticesHorizontal;

            int pt0 = sphere_mesh.Positions.Count;

            // Points
            double phi1 = Math.PI / 2;
            for (int p = 0; p <= VerticesVertical; p++)
            {
                double r1 = Radius * Math.Cos(phi1);
                double y1 = Radius * Math.Sin(phi1);

                double theta = 0;
                for (int t = 0; t <= VerticesHorizontal; t++)
                {
                    sphere_mesh.Positions.Add(new Point3D(
                        X + r1 * Math.Cos(theta),
                        Y + y1,
                        Z + -r1 * Math.Sin(theta)));
                    sphere_mesh.TextureCoordinates.Add(new Point(
                        (double)t / VerticesHorizontal, (double)p / VerticesVertical));
                    theta += dtheta;
                }
                phi1 -= dphi;
            }

            // Triangles.
            int i1, i2, i3, i4;
            for (int p = 0; p <= VerticesVertical - 1; p++)
            {
                i1 = p * (VerticesHorizontal + 1);
                i2 = i1 + (VerticesHorizontal + 1);
                for (int t = 0; t <= VerticesHorizontal - 1; t++)
                {
                    i3 = i1 + 1;
                    i4 = i2 + 1;
                    sphere_mesh.TriangleIndices.Add(pt0 + i1);
                    sphere_mesh.TriangleIndices.Add(pt0 + i2);
                    sphere_mesh.TriangleIndices.Add(pt0 + i4);

                    sphere_mesh.TriangleIndices.Add(pt0 + i1);
                    sphere_mesh.TriangleIndices.Add(pt0 + i4);
                    sphere_mesh.TriangleIndices.Add(pt0 + i3);
                    i1 += 1;
                    i2 += 1;
                }
            }
            this.Model = model;
        }
    }
}
