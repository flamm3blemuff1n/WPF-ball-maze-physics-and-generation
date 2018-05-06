using Opdracht1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Library
{
    public class Physics
    {
        //Board
        private List<Cube> Walls;
        private double boardAngleX = 0;
        private double boardAngleZ = 0;

        //Ball
        private double BALLRADIUS;
        private double BALLMASS;
        private double ballSpeedX = 0;
        private double ballSpeedZ = 0;
        private TranslateTransform3D sphereTranslation;

        //Time
        private double frameInterval;

        //Physics
        private const double GRAVITY = 981; //9.81 * 100 for m to cm, since 1 coordinate space is cm not m.
        private const double STATIC_FRICTION_COEFFICIENT = 0.05; //metal on wood friction
        private const double KINETIC_FRICTION_COEFFICIENT = 0.025;
        private const double COR = 0.45; // Aluminium 0.45; Iron 0.3; Titanium 0.85;

        public Physics(TranslateTransform3D translation, double radius, double mass, double frameInterval)
        {
            this.sphereTranslation = translation;
            this.BALLMASS = mass;
            this.BALLRADIUS = radius;
            this.frameInterval = frameInterval;
        }

        public void UpdateWalls(List<Cube> walls)
        {
            this.Walls = walls;
        }

        public void UpdateTranslation(TranslateTransform3D translation)
        {
            this.sphereTranslation = translation;
        }

        public void UpdateAngle(double angleX, double angleZ)
        {
            this.boardAngleX = angleX;
            this.boardAngleZ = angleZ;
        }

        public void StopMovement()
        {
            this.ballSpeedX = 0;
            this.ballSpeedZ = 0;
        }

        public void Frame()
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

            foreach (Cube cube in Walls)
            {
                double wallToBallDistanceX = Math.Abs(ballPosX - cube.X - (cube.LX / 2));
                double wallToBallDistanceZ = Math.Abs(ballPosZ - cube.Z - (cube.LZ / 2));

                if ((wallToBallDistanceX <= cube.LX / 2 + BALLRADIUS) && (ballPosZ + BALLRADIUS - 0.5) > cube.Z && (ballPosZ - BALLRADIUS + 0.5) < (cube.Z + cube.LZ))
                {
                    distanceX = 0;
                    //v = -e * v0
                    ballSpeedX = -(COR * ballSpeedX);
                }
                if ((wallToBallDistanceZ <= cube.LZ / 2 + BALLRADIUS) && (ballPosX + BALLRADIUS - 0.5) > cube.X && (ballPosX - BALLRADIUS + 0.5) < (cube.X + cube.LX))
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
            double speed = GetBallSpeed(ballSpeedX, a, friction, frameInterval);
            ballSpeedX = speed;
            return GetDistance(friction, a, speed, frameInterval);
        }

        private double MoveZ()
        {
            double a = GetBallAcceleration(boardAngleX);
            int frictionDirection = ballSpeedZ == 0 ? Math.Sign(a) : Math.Sign(ballSpeedZ);
            double frictionForce = GetBallFrictionForce(BALLMASS, boardAngleX) * frictionDirection;
            double friction = frictionForce / BALLMASS;
            double speed = GetBallSpeed(ballSpeedZ, a, friction, frameInterval);
            ballSpeedZ = speed;
            return GetDistance(friction, a, speed, frameInterval);
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
        private double GetBallSpeed(double speed, double acceleration, double friction, double time)
        {
            return speed + (acceleration - friction) * time;
        }

        /*
         * s = (v0 * t) + (1/2 * a * t^2)
         */
        private double GetDistance(double friction, double a, double speed, double time)
        {
            return (speed * time) + ((1 / 2) * a * Math.Pow(time, 2));
        }
    }
}
