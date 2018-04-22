using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Opdracht1
{
    public class Cube
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        public double LZ { get; }
        public double LY { get; }
        public double LX { get; }

        public ModelVisual3D Model { get; private set; }

        public Cube(int x, int y, int z, double pZLength, double pYLength, double pXLength)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.LZ = pZLength;
            this.LY = pYLength;
            this.LX = pXLength;

            CreateCube();
        }

        private void CreateCube()
        {
            Point3D p0 = new Point3D(X + 0, Y + 0, Z + 0);
            Point3D p1 = new Point3D(X + LX, Y + 0, Z + 0);
            Point3D p2 = new Point3D(X + 0, Y + LY, Z + 0);
            Point3D p3 = new Point3D(X + LX, Y + LY, Z + 0);
            Point3D p4 = new Point3D(X + 0, Y + 0, Z + LZ);
            Point3D p5 = new Point3D(X + LX, Y + 0, Z + LZ);
            Point3D p6 = new Point3D(X + 0, Y + LY, Z + LZ);
            Point3D p7 = new Point3D(X + LX, Y + LY, Z + LZ);

            Model3DGroup cube = new Model3DGroup();

            //front
            cube.Children.Add(Triangle(p0, p2, p3));
            cube.Children.Add(Triangle(p3, p1, p0));
            //left
            cube.Children.Add(Triangle(p0, p4, p6));
            cube.Children.Add(Triangle(p6, p2, p0));
            //right
            cube.Children.Add(Triangle(p1, p3, p7));
            cube.Children.Add(Triangle(p7, p5, p1));
            //bottom
            cube.Children.Add(Triangle(p0, p4, p5));
            cube.Children.Add(Triangle(p5, p1, p0));
            //top
            cube.Children.Add(Triangle(p2, p6, p7));
            cube.Children.Add(Triangle(p7, p3, p2));
            //back
            cube.Children.Add(Triangle(p4, p5, p7));
            cube.Children.Add(Triangle(p7, p6, p4));

            this.Model = new ModelVisual3D { Content = cube };
        }

        private Model3DGroup Triangle(Point3D p0, Point3D p1, Point3D p2)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);

            Vector3D normals = GetNormals(p0, p1, p2);
            mesh.Normals.Add(normals);
            mesh.Normals.Add(normals);
            mesh.Normals.Add(normals);

            GeometryModel3D triangle = new GeometryModel3D(mesh, new DiffuseMaterial(new SolidColorBrush(Colors.Red)));
            Model3DGroup group = new Model3DGroup();
            group.Children.Add(triangle);
            return group;
        }

        /*
        * http://www.pererikstrandberg.se/blog/index.cgi?page=WpfCubeThreeDee
        */
        public static Vector3D GetNormals(Point3D p0, Point3D p1, Point3D p2)
        {
            Vector3D v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }
    }
}
