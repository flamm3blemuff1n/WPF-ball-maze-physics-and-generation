using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Opdracht1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            viewport.Children.Add(Cube(-50, 0, -50, 100, 2, 1)); //left wall
            viewport.Children.Add(Cube(49, 0, -50, 100, 2, 1)); //right wall
            viewport.Children.Add(Cube(-49, 0, 49, 1, 2, 98)); //front wall
            viewport.Children.Add(Cube(-49, 0, -50, 1, 2, 98)); //back wall

            viewport.Children.Add(Cube(-40, 0, -49, 30, 2, 1));
            viewport.Children.Add(Cube(-39, 0, -20, 1, 2, 40));
            viewport.Children.Add(Cube(15, 0, -40, 40, 2, 1));
            viewport.Children.Add(Cube(-25, 0, -40, 1, 2, 40));
            viewport.Children.Add(Cube(15, 0, 0, 1, 2, 25));
            viewport.Children.Add(Cube(40, 0, -49, 30, 2, 1));
            viewport.Children.Add(Cube(28, 0, -40, 30, 2, 1));
            viewport.Children.Add(Cube(-25, 0, -39, 10, 2, 1));
            viewport.Children.Add(Cube(0, 0, -30, 10, 2, 1));
            viewport.Children.Add(Cube(-40, 0, -10, 1, 2, 41));
            viewport.Children.Add(Cube(0, 0, 40, 1, 2, 40));
            viewport.Children.Add(Cube(0, 0, 20, 20, 2, 1));
            viewport.Children.Add(Cube(40, 0, 11, 30, 2, 1));
            viewport.Children.Add(Cube(0, 0, 10, 1, 2, 30));
            viewport.Children.Add(Cube(20, 0, 10, 20, 2, 1));
            viewport.Children.Add(Cube(-20, 0, 10, 20, 2, 1));
            viewport.Children.Add(Cube(-20, 0, 20, 1, 2, 10));
            viewport.Children.Add(Cube(-40, 0, 40, 1, 2, 30));
            viewport.Children.Add(Cube(20, 0, 40, 10, 2, 1));
            viewport.Children.Add(Cube(-49, 0, 29, 1, 2, 20));
            viewport.Children.Add(Cube(-40, 0, 10, 1, 2, 20));
            viewport.Children.Add(Cube(0, 0, 0, 1, 2, 20));
        }

        public Model3DGroup Triangle(Point3D p0, Point3D p1, Point3D p2)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);

            Vector3D normal = CalcNormal(p0, p1, p2);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);

            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Red));
            GeometryModel3D model = new GeometryModel3D(mesh, material);
            Model3DGroup group = new Model3DGroup();
            group.Children.Add(model);
            return group;
        }

        public static Vector3D CalcNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            Vector3D v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }

        public ModelVisual3D Cube(int x, int y, int z, double length, double height, double width)
        {
            Model3DGroup cube = new Model3DGroup();

            double l = length;
            double h = height;
            double w = width;

            Point3D p0 = new Point3D(x + 0, y + 0, z + 0);
            Point3D p1 = new Point3D(x + w, y + 0, z + 0);
            Point3D p2 = new Point3D(x + 0, y + h, z + 0);
            Point3D p3 = new Point3D(x + w, y + h, z + 0);
            Point3D p4 = new Point3D(x + 0, y + 0, z + l);
            Point3D p5 = new Point3D(x + w, y + 0, z + l);
            Point3D p6 = new Point3D(x + 0, y + h, z + l);
            Point3D p7 = new Point3D(x + w, y + h, z + l);

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

            return new ModelVisual3D{ Content = cube };
        }

        public Point3DCollection FloorPoints3D {
            get {
                double x = 50.0; // floor width / 2
                double z = 50.0; // floor length / 2
                double floorDepth = -2.5; // give the floor some depth so it's not a 2 dimensional plane 

                Point3DCollection points = new Point3DCollection(20);
                Point3D point;
                //top of the floor
                point = new Point3D(-x, 0, z);// Floor Index - 0
                points.Add(point);
                point = new Point3D(x, 0, z);// Floor Index - 1
                points.Add(point);
                point = new Point3D(x, 0, -z);// Floor Index - 2
                points.Add(point);
                point = new Point3D(-x, 0, -z);// Floor Index - 3
                points.Add(point);
                //front side
                point = new Point3D(-x, 0, z);// Floor Index - 4
                points.Add(point);
                point = new Point3D(-x, floorDepth, z);// Floor Index - 5
                points.Add(point);
                point = new Point3D(x, floorDepth, z);// Floor Index - 6
                points.Add(point);
                point = new Point3D(x, 0, z);// Floor Index - 7
                points.Add(point);
                //right side
                point = new Point3D(x, 0, z);// Floor Index - 8
                points.Add(point);
                point = new Point3D(x, floorDepth, z);// Floor Index - 9
                points.Add(point);
                point = new Point3D(x, floorDepth, -z);// Floor Index - 10
                points.Add(point);
                point = new Point3D(x, 0, -z);// Floor Index - 11
                points.Add(point);
                //back side
                point = new Point3D(x, 0, -z);// Floor Index - 12
                points.Add(point);
                point = new Point3D(x, floorDepth, -z);// Floor Index - 13
                points.Add(point);
                point = new Point3D(-x, floorDepth, -z);// Floor Index - 14
                points.Add(point);
                point = new Point3D(-x, 0, -z);// Floor Index - 15
                points.Add(point);
                //left side
                point = new Point3D(-x, 0, -z);// Floor Index - 16
                points.Add(point);
                point = new Point3D(-x, floorDepth, -z);// Floor Index - 17
                points.Add(point);
                point = new Point3D(-x, floorDepth, z);// Floor Index - 18
                points.Add(point);
                point = new Point3D(-x, 0, z);// Floor Index - 19
                points.Add(point);
                return points;
            }
        }

        public Int32Collection FloorPointsIndices {
            get {
                int[] indices = new int[] { 0, 1, 2, 0, 2, 3, 4, 5, 7, 5, 6, 7, 8, 9, 11, 9, 10, 11, 12, 13, 15, 13, 14, 15, 16, 17, 19, 17, 18, 19 };
                return new Int32Collection(indices);
            }
        }
    }
}
