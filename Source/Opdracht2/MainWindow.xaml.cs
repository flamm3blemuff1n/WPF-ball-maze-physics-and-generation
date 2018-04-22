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

            WallContainer.Children.Add(new Cube(-51, 0, -51, 102, 2, 1).Model);
            WallContainer.Children.Add(new Cube(50, 0, -51, 102, 2, 1).Model);
            WallContainer.Children.Add(new Cube(-50, 0, -51, 1, 2, 100).Model);
            WallContainer.Children.Add(new Cube(-50, 0, 50, 1, 2, 100).Model);

            int[,] wallCoords = new int[,] {
                { -40, 0, -49, 30, 2, 1 }, { -39, 0, -20, 1, 2, 40 }, { 15, 0, -40, 40, 2, 1 }, { -25, 0, -40, 1, 2, 40 },
                { 15, 0, 0, 1, 2, 25 }, { 40, 0, -49, 30, 2, 1 }, { 28, 0, -40, 30, 2, 1 }, { -25, 0, -39, 10, 2, 1 },
                { 0, 0, -30, 10, 2, 1 }, { -40, 0, -10, 1, 2, 41}, {0, 0, 40, 1, 2, 40 }, {0, 0, 20, 20, 2, 1 },
                {40, 0, 11, 30, 2, 1 }, { 0, 0, 10, 1, 2, 30}, { 20, 0, 10, 20, 2, 1}, {-20, 0, 10, 20, 2, 1 },
                {-20, 0, 20, 1, 2, 10 }, { -40, 0, 40, 1, 2, 30}, {20, 0, 40, 10, 2, 1 }, { -49, 0, 29, 1, 2, 20},
                { -40, 0, 10, 1, 2, 20}, {0, 0, 0, 1, 2, 20 }
            };

            for (int i = 0; i < wallCoords.GetLength(0); i++)
            {
                WallContainer.Children.Add(new Cube(wallCoords[i, 0], wallCoords[i, 1], wallCoords[i, 2], wallCoords[i, 3], wallCoords[i, 4], wallCoords[i, 5]).Model);
            }

            ModelVisual3D sphere = Sphere(-46, 2, 46, 2, 20, 30);
            SphereContainer.Children.Add(sphere);
        }

        /*
         *  http://csharphelper.com/blog/2017/05/make-3d-globe-wpf-c/
         */
        private ModelVisual3D Sphere(double x, double y, double z, double radius, int num_phi, int num_theta)
        {
            Model3DGroup sphere = new Model3DGroup();

            MeshGeometry3D sphere_mesh = new MeshGeometry3D();
            GeometryModel3D model = new GeometryModel3D(sphere_mesh, new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri("images/ball.jpg", UriKind.Relative)))));
            sphere.Children.Add(model);

            double dphi = Math.PI / num_phi;
            double dtheta = 2 * Math.PI / num_theta;

            int pt0 = sphere_mesh.Positions.Count;

            // Points
            double phi1 = Math.PI / 2;
            for (int p = 0; p <= num_phi; p++)
            {
                double r1 = radius * Math.Cos(phi1);
                double y1 = radius * Math.Sin(phi1);

                double theta = 0;
                for (int t = 0; t <= num_theta; t++)
                {
                    sphere_mesh.Positions.Add(new Point3D(
                        x + r1 * Math.Cos(theta),
                        y + y1,
                        z + -r1 * Math.Sin(theta)));
                    sphere_mesh.TextureCoordinates.Add(new Point(
                        (double)t / num_theta, (double)p / num_phi));
                    theta += dtheta;
                }
                phi1 -= dphi;
            }

            // Triangles.
            int i1, i2, i3, i4;
            for (int p = 0; p <= num_phi - 1; p++)
            {
                i1 = p * (num_theta + 1);
                i2 = i1 + (num_theta + 1);
                for (int t = 0; t <= num_theta - 1; t++)
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
            return new ModelVisual3D { Content = sphere };
        }

        private void Slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeBoardRotation(e.NewValue, Slider2.Value);
        }

        private void Slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeBoardRotation(Slider1.Value, e.NewValue);
        }

        private void ChangeBoardRotation(double angleX, double angleZ)
        {
            Transform3DGroup myTransform3DGroup = new Transform3DGroup();

            RotateTransform3D rotateTransform3D = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), angleZ));
            myTransform3DGroup.Children.Add(rotateTransform3D);

            rotateTransform3D = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), angleX));
            myTransform3DGroup.Children.Add(rotateTransform3D);

            Board.Transform = myTransform3DGroup;
            WallContainer.Transform = myTransform3DGroup;
        }
    }
}
