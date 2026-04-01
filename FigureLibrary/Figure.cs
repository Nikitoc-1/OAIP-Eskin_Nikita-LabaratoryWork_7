using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Controls;
using System.Linq.Expressions;

namespace Laba_7_Eskin
{
    public abstract class Figure
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }

        public abstract void Draw(DrawingContext drawingContext);
        public abstract void MoveTo(int deltaX, int deltaY);
    }

    public static class Init
    {
        public static Pen Pen = new Pen(Brushes.Black, 2);
        public static Image DrawingImage { get; set; }
    }

    public class Rectangle : Figure
    {
        public Rectangle(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            Name = "Прямоугольник";
        }

        public override void Draw(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(null, Init.Pen, new Rect(X, Y, Width, Height));
        }

        public override void MoveTo(int deltaX, int deltaY)
        {
            if (X + deltaX >= 0 && Y + deltaY >= 0 &&
                X + deltaX + Width <= Init.DrawingImage.ActualWidth &&
                Y + deltaY + Height <= Init.DrawingImage.ActualHeight)
            {
                X += deltaX;
                Y += deltaY;
            }
        }
    }

    public class Square : Rectangle
    {
        public Square(int x, int y, int side) : base(x, y, side, side)
        {
            X = x; Y = y;
            Width = side;
            Height = side;
            Name = "Квадрат";
        }
    }

    public class Circle : Ellipse
    {
        public int Radius { get; set; }

        public Circle(int x, int y, int radius) : base(x, y, radius, radius)
        {
            X = x; Y = y;
            Radius = radius;
            Width = radius * 2;
            Height = radius * 2;
            Name = "Окружность";
        }
    }

    public class Polygon : Figure
    {
        private int _side;
        private int count;
        public Polygon(int x, int y, int col, int side)
        {
            Name = "Многоугольник";
            X = x;
            Y = y;
            _side = side;
            count = col;
            double radius = _side / (2 * Math.Sin(Math.PI / count));
            Width = (int)(radius * 2);
            Height = (int)(radius * 2);

        }
        public override void Draw(DrawingContext drawingContext)
        {
            double radius = _side / (2 * Math.Sin(Math.PI / count));
            Point center = new Point(X + radius, Y + radius);
            Point[] arr = new Point[count];

            if (count < 3 || count > 15)
            {
                MessageBox.Show("Ошибка");
                return;
            }
            for (int i = 0; i < count; i++)
            {
                double angle = 2 * Math.PI * i / count - Math.PI / 2;

                double x = center.X + radius * Math.Cos(angle);
                double y = center.Y + radius * Math.Sin(angle);

                arr[i] = new Point(x, y);
            }

            for (int i = 0; i < count; i++)
            {
                int next = (i + 1) % count;
                drawingContext.DrawLine(Init.Pen, arr[i], arr[next]);
            }
        }


        public override void MoveTo(int deltaX, int deltaY)
        {
            int newX = X + deltaX;
            int newY = Y + deltaY;

            if (newX >= 0 && newY >= 0 &&
                newX + Width <= Init.DrawingImage.ActualWidth &&
                newY + Height <= Init.DrawingImage.ActualHeight)
            {
                X = newX;
                Y = newY;
            }
        }
    }
    public class Triangle : Polygon
    {
        private int _side;

        public Triangle(int x, int y, int side) : base(x, y, 3, side)
        {
            Name = "Треугольник";
            X = x;
            Y = y;
            _side = side;
            Width = side;
            Height = (int)(side * Math.Sqrt(3) / 2);
        }
    }

    public class Ellipse : Figure
    {
        public Ellipse(int x, int y, int w, int h)
        {
            X = x; Y = y; Width = w; Height = h;
            Name = "Эллипс";
        }

        public override void Draw(DrawingContext drawingContext)
        {
            Point center = new Point(X + Width / 2, Y + Height / 2);
            drawingContext.DrawEllipse(null, Init.Pen, center, Width / 2, Height / 2);
        }

        public override void MoveTo(int deltaX, int deltaY)
        {
            if (X + deltaX >= 0 && Y + deltaY >= 0 &&
                X + deltaX + Width <= Init.DrawingImage.ActualWidth &&
                Y + deltaY + Height <= Init.DrawingImage.ActualHeight)
            {
                X += deltaX;
                Y += deltaY;
            }
        }
    }

    public class Rocket : Figure
    {
        public Rectangle Body { get; set; }
        public Triangle Nose { get; set; }
        public Circle Window1 { get; set; }
        public Circle Window2 { get; set; }
        public Triangle Engine1 { get; set; }
        public Triangle Engine2 { get; set; }

        private int _flyOffset = 0;
        private int _flyDirection = 1;
        private int _flySpeed = 3;
        private int _flyRange = 120;
        public static bool CanFly = true;

        public Rocket(int x, int y)
        {
            X = x; Y = y;
            Width = 100; Height = 200;
            Name = "Ракета";

            Body = new Rectangle(x + 20, y + 30, 60, 140);
            Nose = new Triangle(x + 20, y - 22, 60);
            Engine1 = new Triangle(x + 20, y + 150, 30);
            Engine2 = new Triangle(x + 50, y + 150, 30);
            Window1 = new Circle(x + 35, y + 50, 15);
            Window2 = new Circle(x + 35, y + 90, 15);
        }

        public bool UpdateAnimation()
        {
            if (CanFly)
            {

                _flyOffset += _flyDirection * _flySpeed;

                if (Math.Abs(_flyOffset) > _flyRange)
                {
                    _flyDirection *= -1;
                }

                int newY = Y + (_flyDirection * _flySpeed);


                if (newY >= 20 && newY + Height <= Init.DrawingImage.ActualHeight)
                {

                    Y = newY;

                    Body.Y += _flyDirection * _flySpeed;
                    Nose.Y += _flyDirection * _flySpeed;
                    Engine1.Y += _flyDirection * _flySpeed;
                    Engine2.Y += _flyDirection * _flySpeed;
                    Window1.Y += _flyDirection * _flySpeed;
                    Window2.Y += _flyDirection * _flySpeed;

                    return true;
                }
            }
            return false;
        }

        public override void Draw(DrawingContext drawingContext)
        {
            Body.Draw(drawingContext);
            Nose.Draw(drawingContext);
            Engine1.Draw(drawingContext);
            Engine2.Draw(drawingContext);
            Window1.Draw(drawingContext);
            Window2.Draw(drawingContext);
        }

        public override void MoveTo(int deltaX, int deltaY)
        {
            if (X + deltaX >= 0 &&
                Y + deltaY >= 22 &&
                X + deltaX + Width <= Init.DrawingImage.ActualWidth &&
                Y + deltaY + Height <= Init.DrawingImage.ActualHeight)
            {
                X += deltaX;
                Y += deltaY;

                Body.MoveTo(deltaX, deltaY);
                Nose.MoveTo(deltaX, deltaY);
                Engine1.MoveTo(deltaX, deltaY);
                Engine2.MoveTo(deltaX, deltaY);
                Window1.MoveTo(deltaX, deltaY);
                Window2.MoveTo(deltaX, deltaY);
            }
        }

    }

    public class ShapeContainer
    {
        public static List<Figure> FigureList { get; set; } = new List<Figure>();

        public static void AddFigure(Figure figure) 
            => FigureList.Add(figure);
        public static void RemoveFigure(Figure figure) 
            => FigureList.Remove(figure);
        public static void ClearAll()   
            => FigureList.Clear();
    }
}