using Opdracht1;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Windows.Input;
using Library;
using System.Windows.Media;

namespace Opdracht2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Maze
        private Maze maze;
        private List<Cube> walls;

        //Board
        private double boardAngleX = 0;
        private double boardAngleZ = 0;

        //Physics
        private Physics physics;

        //Calc
        private Timer timer;
        private const int CALCPERSEC = 80;
        private double frameInterval;

        //Ball
        private const double BALLMASS = 1;
        private TranslateTransform3D sphereTranslation;
        private GeometryModel3D sphere;

        public MainWindow()
        {
            InitializeComponent();

            int cells = Int32.Parse(CellsTextBox.Text);
            SetMaze(cells);

            double radius = double.Parse(BallRadiusTextBox.Text);
            SetBall(radius);
            SetGameTimer();
            SetPhysics(radius, BALLMASS, frameInterval, sphereTranslation);
        }

        /*
         * https://www.codeproject.com/Questions/330780/Calling-method-every-hour
         */
        private void SetGameTimer()
        {
            frameInterval = 1 / (double)CALCPERSEC;
            int frameIntervalMs = (int)(this.frameInterval * 1000);
            timer = new Timer
            {
                Interval = frameInterval
            };
            timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            Dispatcher.Invoke((Action)delegate ()
            {
                this.physics.Frame();
            });
        }

        private void SetBall(double radius)
        {
            sphere = new Sphere(0, 0, 0, radius, 20, 30).Model;
            SphereContainer.Children.Add(new ModelVisual3D { Content = sphere });

            Transform3DGroup sphereTransformations = new Transform3DGroup();
            sphereTranslation = new TranslateTransform3D(50-radius*1.5, radius, 50-radius*1.5);
            sphereTransformations.Children.Add(sphereTranslation);
            sphere.Transform = sphereTransformations;
        }

        private void SetPhysics(double ballRadius, double ballMass, double frameInterval, TranslateTransform3D translateTransform)
        {
            this.physics = new Physics(translateTransform, ballRadius, ballMass, frameInterval);
            this.physics.UpdateWalls(walls);
        }

        private void SetMaze(int cells)
        {
            maze = new Maze(cells, WallContainer);
            this.walls = maze.Walls;
            maze.GenerateRecursiveBacktrack();
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
            this.boardAngleX = angleX;
            this.boardAngleZ = angleZ;
            this.physics.UpdateAngle(angleX, angleZ);

            Transform3DGroup myTransform3DGroup = new Transform3DGroup();

            RotateTransform3D rotateTransform3D = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), angleZ));
            myTransform3DGroup.Children.Add(rotateTransform3D);

            rotateTransform3D = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), angleX));
            myTransform3DGroup.Children.Add(rotateTransform3D);

            Board.Transform = myTransform3DGroup;
            WallContainer.Transform = myTransform3DGroup;
            SphereContainer.Transform = myTransform3DGroup;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void MazeButton_Click(object sender, RoutedEventArgs e)
        {
            CellsTextBox.BorderBrush = Brushes.Black;
            try
            {
                int cells = Int32.Parse(CellsTextBox.Text);
                if (cells > 0)
                {
                    SetMaze(cells);
                    this.physics.UpdateWalls(walls);
                }
                else
                {
                    CellsTextBox.BorderBrush = Brushes.Red;
                }
            }
            catch(FormatException)
            {
                CellsTextBox.BorderBrush = Brushes.Red;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            BallRadiusTextBox.BorderBrush = Brushes.Black;
            try
            {
                double radius = double.Parse(BallRadiusTextBox.Text);

                ChangeBoardRotation(0, 0);
                Slider1.Value = 0;
                Slider2.Value = 0;

                SphereContainer.Children.Clear();
                SetBall(radius);
                SetPhysics(radius, BALLMASS, frameInterval, sphereTranslation);
            }
            catch(FormatException)
            {
                BallRadiusTextBox.BorderBrush = Brushes.Red;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up && boardAngleX < 15) boardAngleX += 1;
            if (e.Key == Key.Down && boardAngleX > -15) boardAngleX -= 1;
            if (e.Key == Key.Left && boardAngleZ > -15) boardAngleZ -= 1;
            if (e.Key == Key.Right && boardAngleZ < 15) boardAngleZ += 1;

            ChangeBoardRotation(boardAngleX, boardAngleZ);
            Slider1.Value = boardAngleX;
            Slider2.Value = boardAngleZ;
        }
    }
}
