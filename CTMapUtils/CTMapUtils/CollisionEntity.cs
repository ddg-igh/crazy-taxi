using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;

namespace CTMapUtils
{
    public class CollisionEntity
    {
        ////Ist eine Seite des Autos, die kontrolliert werden muss
        //private class Line
        //{
        //    private System.Windows.Vector DirectionAndLength;
        //    private Point StartLocation;

        //    /// <summary>
        //    /// Erstellt eine Linie aus einer Seite des Autos
        //    /// </summary>
        //    /// <param name="carPosition">Mittelpunkt des Autos</param>
        //    /// <param name="xOffset">Abstand der Linie vom Mittelpunkt des Autos in X-Richtung</param>
        //    /// <param name="yOffset">Abstand der Linie vom Mittelpunkt des Autos in Y-Richtung</param>
        //    /// <param name="side">Länge und Ausrichtung der Linie</param>
        //    /// <returns></returns>
        //    public static Line FromCarSide(Point startLocation, System.Windows.Vector side)
        //    {
        //        Line returnvalue = new Line();

        //        returnvalue.StartLocation = startLocation;
        //        returnvalue.DirectionAndLength = side;

        //        return returnvalue;
        //    }

        //    /// <summary>
        //    /// Gibt 8 gleichmäßig verteilte Punkte auf der Linie zurück
        //    /// </summary>
        //    public List<Point> AllPoints
        //    {
        //        get
        //        {
        //            var returnvalue = new List<Point>(8);

        //            double stepSize = (double)1 / (double)8;
        //            double step = 0;
        //            while (step < 1)
        //            {
        //                var x = DirectionAndLength.X * step;
        //                var y = DirectionAndLength.Y * step;
        //                x += StartLocation.X;
        //                y += StartLocation.Y;
        //                returnvalue.Add(new Point((int)x, (int)y));

        //                step += stepSize;
        //            }

        //            return returnvalue;
        //        }
        //    }
        //}

        private List<Vector> Front;
        private List<Vector> Back;
        private List<Vector> Left;
        private List<Vector> Right;

        public CollisionManager collision;

        public int PointsPerSide
        {
            get;
            private set;
        }

        internal CollisionEntity(CollisionManager manager, System.Drawing.Size size, int pointsPerSide)
        {
            PointsPerSide = pointsPerSide;

            var position = new Vector((size.Width/2), -(size.Height/2)); //Top Right
            var direction = new Vector(0, size.Height); //TopToBottom

            Right = getPoints(position, direction);

            position.X = -(size.Width / 2); //TopLeft

            Left = getPoints(position, direction);

            direction = new Vector(size.Width, 0); //LeftToRight

            Front = getPoints(position, direction);

            position.Y = size.Height / 2; //BottomLeft

            Back = getPoints(position, direction);



            //var bottomRightCorner = new Vector(size.Width/2, size.Height/2);
            //var leftToRightDirection = new Vector(-size.Width, size.Height);

            //Front = getPoints(topLeft, topRight);
            //Right = getPoints(topRight, bottomRight);
            //Back = getPoints(bottomRight, bottomLeft);
            //Left = getPoints(bottomLeft, topRight);




            collision = manager;
        }

        public CollisionResult Update(System.Drawing.Point position, int rotation, bool drivingForward)
        {
            CollisionResult result = new CollisionResult();
            
            List<Vector> frontBack;
            if (drivingForward)
            {
                frontBack = Front;
            }
            else
            {
                frontBack = Back;
            }

            var fb = new Vector[PointsPerSide];
            var left = new Vector[PointsPerSide];
            var right = new Vector[PointsPerSide];
            for (int i = 0; i < PointsPerSide; i++)
            {
                fb[i] = rotateVector(frontBack[i], rotation);
                fb[i].X += position.X;
                fb[i].Y += position.Y;

                left[i] = rotateVector(Left[i], rotation);
                left[i].X += position.X;
                left[i].Y += position.Y;

                right[i] = rotateVector(Right[i], rotation);
                right[i].X += position.X;
                right[i].Y += position.Y;
            }


            if (drivingForward)
                result.Front = collision.Collides(fb, (int)position.X, (int)position.Y/* ,true*/);
            else
                result.Back = collision.Collides(fb, (int)position.X, (int)position.Y/* ,true*/);

            result.Left = collision.Collides(left, (int)position.X, (int)position.Y/* ,true*/);
            result.Right = collision.Collides(right, (int)position.X, (int)position.Y/* ,true*/);
            
            return result;
        }

        /// <summary>
        /// Gibt einen neuen Vektor zurück, der den übergebenen Vektor um eine Gradzahl gedreht darstellt
        /// </summary>
        /// <param name="vector">Der zu drehende Vektor</param>
        /// <param name="rotation">Der Winkel</param>
        /// <returns></returns>
        private System.Windows.Vector rotateVector(System.Windows.Vector vector, double rotation)
        {
            var returnvalue = new Vector(vector.X, vector.Y);

            rotation = rotation * Math.PI / 180;
            returnvalue.X = (vector.X * Math.Cos(rotation)) + (vector.Y * -Math.Sin(rotation));
            returnvalue.Y = (vector.X * Math.Sin(rotation)) + (vector.Y * Math.Cos(rotation));

            return returnvalue;
        }

        private List<Vector> getPoints(Vector startLocation, Vector directionAndLength)
        {
            var returnvalue = new List<Vector>(PointsPerSide);

            double stepSize = (double)1 / (double)(PointsPerSide - 1);
            double step = 0;
            while (step <= 1)
            {
                var x = directionAndLength.X * step;
                var y = directionAndLength.Y * step;
                x += startLocation.X;
                y += startLocation.Y;
                returnvalue.Add(new Vector(x, y));

                step += stepSize;
            }

            return returnvalue;
        }
    }
}
