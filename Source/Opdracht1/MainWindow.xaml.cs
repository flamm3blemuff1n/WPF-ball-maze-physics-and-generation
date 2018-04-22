﻿using System;
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

            for(int i = 0; i < wallCoords.GetLength(0); i++)
            {
                WallContainer.Children.Add(new Cube(wallCoords[i,0], wallCoords[i, 1], wallCoords[i, 2], wallCoords[i, 3], wallCoords[i, 4], wallCoords[i, 5]).Model);
            }

            ModelVisual3D sphere = Sphere(-46, 2, 46, 2, 20, 30);
            SphereContainer.Children.Add(sphere);

            SimpleBallAnimation();
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
            return new ModelVisual3D{ Content = sphere };
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

        private void SimpleBallAnimation()
        {
            Transform3DGroup myTransform3DGroup = new Transform3DGroup();

            //rotation
            AxisAngleRotation3D axis = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);
            RotateTransform3D rotate = new RotateTransform3D(axis);
            rotate.CenterX = -46;
            rotate.CenterY = 2;
            rotate.CenterZ = 46;

            myTransform3DGroup.Children.Add(rotate);

            SphereContainer.Transform = myTransform3DGroup;

            NameScope scope = new NameScope();
            FrameworkContentElement element = new FrameworkContentElement();
            NameScope.SetNameScope(element, scope);

            element.RegisterName("rotation", axis);

            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = -1548,
                Duration = TimeSpan.FromSeconds(4),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard myStoryboard = new Storyboard();

            Storyboard.SetTargetProperty(animation, new PropertyPath("Angle"));

            Storyboard.SetTargetName(animation, "rotation");
            myStoryboard.Children.Add(animation);
            myStoryboard.Duration = TimeSpan.FromSeconds(4);
            
            //translation
            TranslateTransform3D tran = new TranslateTransform3D(0, 0, 0);

            myTransform3DGroup.Children.Add(tran);
            SphereContainer.Transform = myTransform3DGroup;

            element.RegisterName("translation", tran);

            DoubleAnimation animation2 = new DoubleAnimation
            {
                From = 0,
                To = 54.036,
                Duration = TimeSpan.FromSeconds(4),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTargetProperty(animation2, new PropertyPath("OffsetX"));

            Storyboard.SetTargetName(animation2, "translation");
            myStoryboard.Children.Add(animation2);
            myStoryboard.Duration = TimeSpan.FromSeconds(4);
            myStoryboard.RepeatBehavior = RepeatBehavior.Forever;
            myStoryboard.AutoReverse = true;

            this.Resources.Add("id1111", myStoryboard);
            myStoryboard.Begin(element, HandoffBehavior.Compose);
        }

        /*
        public Point3DCollection FloorPoints3D {
            get {
                double x = 50.0; // floor width / 2
                double z = 50.0; // floor length / 2
                double floorDepth = -2; // give the floor some depth so it's not a 2 dimensional plane 

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
        */
    }
}
