﻿using Opdracht1;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Opdracht2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Cube> walls;

        //Calc
        private const int CALCPERSEC = 80;
        private double frameInterval;

        //Board
        private double boardAngleX = 0;
        private double boardAngleZ = 0;

        //Ball
        private const double BALLRADIUS = 2;
        private const double BALLMASS = 1;
        private TranslateTransform3D sphereTranslation;
        private double ballSpeedX = 0;
        private double ballSpeedZ = 0;

        //Physics
        private const double GRAVITY = 981; //9.81 * 100 for m to cm, since 1 coordinate space is cm not m.
        private const double STATIC_FRICTION_COEFFICIENT = 0.05; //metal on wood friction
        private const double KINETIC_FRICTION_COEFFICIENT = 0.025;
        private const double COR = 0.45; // Aluminium 0.45; Iron 0.3; Titanium 0.85;

        public MainWindow()
        {
            InitializeComponent();

            walls = new List<Cube>();

            int[,] wallCoords = new int[,] {
                { -51, 0, -51, 102, 2, 1 }, { 50, 0, -51, 102, 2, 1 }, { -50, 0, -51, 1, 2, 100 }, { -50, 0, 50, 1, 2, 100 }, //4 sides
                { -40, 0, -50, 30, 2, 1 }, { -39, 0, -20, 1, 2, 40 }, { 15, 0, -40, 40, 2, 1 }, { -25, 0, -40, 1, 2, 40 },
                { 15, 0, 0, 1, 2, 25 }, { 40, 0, -50, 30, 2, 1 }, { 28, 0, -40, 30, 2, 1 }, { -25, 0, -39, 10, 2, 1 },
                { 0, 0, -30, 10, 2, 1 }, { -40, 0, -10, 1, 2, 41}, {0, 0, 40, 1, 2, 40 }, {0, 0, 20, 20, 2, 1 },
                {40, 0, 11, 30, 2, 1 }, { 0, 0, 10, 1, 2, 30}, { 20, 0, 10, 20, 2, 1}, {-20, 0, 10, 20, 2, 1 },
                {-20, 0, 20, 1, 2, 10 }, { -40, 0, 40, 1, 2, 30}, {20, 0, 40, 10, 2, 1 }, { -49, 0, 29, 1, 2, 20},
                { -40, 0, 10, 1, 2, 20}, {0, 0, 0, 1, 2, 20 }
            };

            for (int i = 0; i < wallCoords.GetLength(0); i++)
            {
                Cube wall = new Cube(wallCoords[i, 0], wallCoords[i, 1], wallCoords[i, 2], wallCoords[i, 3], wallCoords[i, 4], wallCoords[i, 5]);
                walls.Add(wall);
                WallContainer.Children.Add(wall.Model);
            }

            GeometryModel3D sphere = new Sphere(0, 0, 0, BALLRADIUS, 20, 30).Model;
            SphereContainer.Children.Add(new ModelVisual3D { Content = sphere });

            Transform3DGroup sphereTransformations = new Transform3DGroup();
            sphereTranslation = new TranslateTransform3D(45, 2, 45);
            sphereTransformations.Children.Add(sphereTranslation);
            sphere.Transform = sphereTransformations;

            StartGameTimer();
        }

        private void StartGameTimer()
        {
            frameInterval = 1 / (double)CALCPERSEC;
            int frameIntervalMs = (int)(this.frameInterval * 1000);
            var timer = new System.Timers.Timer
            {
                Interval = frameInterval
            };
            timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
            timer.Start();
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            Dispatcher.Invoke((Action)delegate ()
            {
                Frame();
            });
        }

        private void Frame()
        {
            double x = 0, z = 0;

            x = MoveX();
            z = MoveZ();
            
            double[] colissions = Collision(x, z);
            x = colissions[0];
            z = colissions[1];

            sphereTranslation.OffsetX += x;
            sphereTranslation.OffsetZ += z;
        }

        private double[] Collision(double x, double z)
        {
            double distanceX = x;
            double distanceZ = z;
            double ballPosX = sphereTranslation.OffsetX + x;
            double ballPosZ = sphereTranslation.OffsetZ + z;
            
            foreach (Cube cube in walls)
            {
                double wallToBallDistanceX = Math.Abs(ballPosX - cube.X - (cube.LX/2));
                double wallToBallDistanceZ = Math.Abs(ballPosZ - cube.Z - (cube.LZ/2));

                if ((wallToBallDistanceX <= cube.LX/2 + BALLRADIUS) && (ballPosZ + BALLRADIUS-0.25) > cube.Z && (ballPosZ - BALLRADIUS+0.25) < (cube.Z + cube.LZ))
                {
                    distanceX = 0;
                    //v = -e * v0
                    ballSpeedX = -(COR * ballSpeedX);
                }
                if ((wallToBallDistanceZ <= cube.LZ / 2 + BALLRADIUS) && (ballPosX + BALLRADIUS-0.25) > cube.X && (ballPosX - BALLRADIUS+0.25) < (cube.X + cube.LX))
                {
                    distanceZ = 0;
                    ballSpeedZ = -(COR * ballSpeedZ);
                }
            }

            return new double[] { distanceX, distanceZ };
        }

        private double MoveX()
        {
            double a = -GetBallAcceleration(boardAngleZ);
            int frictionDirection = ballSpeedX == 0 ? Math.Sign(a) : Math.Sign(ballSpeedX);
            double frictionForce = GetBallFrictionForce(BALLMASS, boardAngleZ) * frictionDirection;
            double friction = frictionForce / BALLMASS;
            double speed = GetBallSpeed(ballSpeedX, a, friction);
            ballSpeedX = speed;
            return GetDistance(friction, a, speed);
        }

        private double MoveZ()
        {
            double a = GetBallAcceleration(boardAngleX);
            int frictionDirection = ballSpeedZ == 0 ? Math.Sign(a) : Math.Sign(ballSpeedZ);
            double frictionForce = GetBallFrictionForce(BALLMASS, boardAngleX) * frictionDirection;
            double friction = frictionForce / BALLMASS;
            double speed = GetBallSpeed(ballSpeedZ, a, friction);
            ballSpeedZ = speed;
            return GetDistance(friction, a, speed);
        }

        /*
         * Gravity acceleration splits into 2 components cos and sin because of angle
         * a = a * sin(angle)
         */
        private double GetBallAcceleration(double angle)
        {
            return GRAVITY * Math.Sin(angle * Math.PI / 180); //Direction and value for gravity acceleration
        }

        /*
         * Choose friction coefficient based on ball movement, gravity is split in 2 components sin and cos because of angle
         * Fw = Fn * Coefficient
         * Fn = m * a = m * 9.81
         * Fw = m * 9.81 * Coefficient
         */
        private double GetBallFrictionForce(double mass, double angle)
        {
            double frictionCoefficient = KINETIC_FRICTION_COEFFICIENT; //MOVING
            if (ballSpeedX == 0 && ballSpeedZ == 0) frictionCoefficient = STATIC_FRICTION_COEFFICIENT; //NOT MOVING
            return mass * (GRAVITY * Math.Cos(angle * Math.PI / 180)) * frictionCoefficient; // Direction and friction according to gravity
        }

        /*
         * v = v0 + a * t
         */
        private double GetBallSpeed(double speed, double acceleration, double friction)
        {
            return speed + (acceleration - friction) * frameInterval;
        }

        /*
         * s = (v0 * t) + (1/2 * a * t^2)
         */
        private double GetDistance(double friction, double a, double speed)
        {
            return (speed * frameInterval) + ((1 / 2) * a * Math.Pow(frameInterval, 2));
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
            Transform3DGroup myTransform3DGroup = new Transform3DGroup();

            RotateTransform3D rotateTransform3D = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), angleZ));
            myTransform3DGroup.Children.Add(rotateTransform3D);

            rotateTransform3D = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), angleX));
            myTransform3DGroup.Children.Add(rotateTransform3D);

            Board.Transform = myTransform3DGroup;
            WallContainer.Transform = myTransform3DGroup;
            SphereContainer.Transform = myTransform3DGroup;
        }
    }
}
