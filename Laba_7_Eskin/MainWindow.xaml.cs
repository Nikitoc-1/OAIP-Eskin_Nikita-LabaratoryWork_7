using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Laba_7_Eskin;

namespace Laba_7_Eskin
{
    public partial class MainWindow : Window
    {
        private RenderTargetBitmap _renderTarget;
        private Figure _selectedFigure;
        private Point _dragOffset;
        private bool _isDragging;
        private Random _random = new Random();
        public int count = 0;


        public MainWindow()
        {
            InitializeComponent();
            Init.DrawingImage = DrawingImage;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            foreach (var fig in ShapeContainer.FigureList)
            {
                if (fig is Rocket rocket)
                {
                    if (rocket.UpdateAnimation())
                    {
                        Redraw();
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                int width = (int)DrawingImage.ActualWidth;
                int height = (int)DrawingImage.ActualHeight;

                if (width <= 20) width = 900;
                if (height <= 20) height = 500;

                _renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                DrawingImage.Source = _renderTarget;

            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void Redraw()
        {
            if (_renderTarget == null) return;

            var drawingVisual = new DrawingVisual();
            using (var dc = drawingVisual.RenderOpen())
            {
                foreach (var figure in ShapeContainer.FigureList)
                    figure.Draw(dc);
            }

            _renderTarget.Clear();
            _renderTarget.Render(drawingVisual);
            DrawingImage.Source = _renderTarget;
        }

        private void AddFigure(Figure figure)
        {
            ShapeContainer.AddFigure(figure);
            Redraw();
        }

        private Point GetRandomPosition(int width, int height)
        {
            int maxX = Math.Max(20, (int)(DrawingImage.ActualWidth - width - 20));
            int maxY = Math.Max(20, (int)(DrawingImage.ActualHeight - height - 20));

            return new Point(
                _random.Next(20, maxX),
                _random.Next(20, maxY)
            );
        }

        private void BtnRectangle_Click(object sender, RoutedEventArgs e)
        {
            var pos = GetRandomPosition(120, 60);
            AddFigure(new Rectangle((int)pos.X, (int)pos.Y, 120, 60));
            count += 1;
            if (count > 5)
            {
                BtnRectangle.IsEnabled = false;
            }
        }

        private void BtnSquare_Click(object sender, RoutedEventArgs e)
        {
            var pos = GetRandomPosition(80, 80);
            AddFigure(new Square((int)pos.X, (int)pos.Y, 80));
            count += 1;
            if (count > 5)
            {
                BtnSquare.IsEnabled = false;
            }
        }

        private void BtnEllipse_Click(object sender, RoutedEventArgs e)
        {
            var pos = GetRandomPosition(100, 60);
            AddFigure(new Ellipse((int)pos.X, (int)pos.Y, 100, 60));
            count += 1;
            if (count > 5)
            {
                BtnEllipse.IsEnabled = false;
            }
        }

        private void BtnCircle_Click(object sender, RoutedEventArgs e)
        {
            var pos = GetRandomPosition(70, 70);
            AddFigure(new Circle((int)pos.X, (int)pos.Y, 35));
            count += 1;
            if (count > 5)
            {
                BtnCircle.IsEnabled = false;
            }
        }

        private void BtnTriangle_Click(object sender, RoutedEventArgs e)
        {
            var pos = GetRandomPosition(80, 70);
            AddFigure(new Triangle((int)pos.X, (int)pos.Y, 80));
            count += 1;
            if (count > 5)
            {
                BtnTriangle.IsEnabled = false;
            }
        }

        private void BtnRocket_Click(object sender, RoutedEventArgs e)
        {
            var pos = GetRandomPosition(100, 200);
            AddFigure(new Rocket((int)pos.X, (int)pos.Y));
            count += 1;
            if (count > 5)
            {
                BtnRocket.IsEnabled = false;
            }

        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ShapeContainer.ClearAll();
            Redraw();
            count = 0;
            BtnRectangle.IsEnabled = true;
            BtnSquare.IsEnabled = true;
            BtnCircle.IsEnabled = true;
            BtnEllipse.IsEnabled = true;
            BtnTriangle.IsEnabled = true;
            BtnRocket.IsEnabled = true;
        }

        private void DrawingImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(DrawingImage);

            for (int i = ShapeContainer.FigureList.Count - 1; i >= 0; i--)
            {
                var fig = ShapeContainer.FigureList[i];

                if (mousePos.X >= fig.X && mousePos.X <= fig.X + fig.Width &&
                    mousePos.Y >= fig.Y && mousePos.Y <= fig.Y + fig.Height)
                {
                    _selectedFigure = fig;
                    _dragOffset = new Point(mousePos.X - fig.X, mousePos.Y - fig.Y);
                    _isDragging = true;
                    DrawingImage.CaptureMouse();
                    e.Handled = true;
                    Rocket.CanFly = false;
                    break;
                }
            }
        }

        private void DrawingImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _selectedFigure != null)
            {
                var mousePos = e.GetPosition(DrawingImage);

                int newX = (int)(mousePos.X - _dragOffset.X);
                int newY = (int)(mousePos.Y - _dragOffset.Y);

                int deltaX = newX - _selectedFigure.X;
                int deltaY = newY - _selectedFigure.Y;

                if (deltaX != 0 || deltaY != 0)
                {
                    _selectedFigure.MoveTo(deltaX, deltaY);
                    Redraw();
                }
            }
        }

        private void DrawingImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                DrawingImage.ReleaseMouseCapture();
                DrawingImage.Cursor = null;
                _selectedFigure = null;
                Rocket.CanFly = true;
            }
        }

        private void DrawingImage_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(DrawingImage);

            for (int i = ShapeContainer.FigureList.Count - 1; i >= 0; i--)
            {
                var fig = ShapeContainer.FigureList[i];
                if (mousePos.X >= fig.X && mousePos.X <= fig.X + fig.Width &&
                    mousePos.Y >= fig.Y && mousePos.Y <= fig.Y + fig.Height)
                {
                    ShapeContainer.RemoveFigure(fig);
                    Redraw();
                    e.Handled = true;
                    count -= 1;
                    if (count < 6)
                    {
                        BtnRectangle.IsEnabled = true;
                        BtnSquare.IsEnabled = true;
                        BtnCircle.IsEnabled = true;
                        BtnEllipse.IsEnabled = true;
                        BtnTriangle.IsEnabled = true;
                        BtnRocket.IsEnabled = true;
                    }
                    break;
                }
            }
        }

        private void BtnApplyRec_Click(object sender, RoutedEventArgs e)
        {
            string rheight = RHeight.Text;
            string rwidth = RWidth.Text;

            if (int.TryParse(rheight, out int height) && (int.TryParse(rwidth, out int width)))
            {
                if (height > 20 && height < 500 && width > 20 && width < 500)
                {
                    foreach (var fig in ShapeContainer.FigureList)
                    {
                        if (fig is Rectangle rect)
                        {
                            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

                            rect.Width = width;
                            rect.Height = height;


                            rect.Y = (int)(center.Y - height / 2);
                        }
                    }
                }
                else{ MessageBox.Show("Ошибка в размерах"); }
            }
            else
            {
                Console.WriteLine("Ошибка.Введите корректное числор!");
            }
        }

        private void BtnApplyCir_Click(object sender, RoutedEventArgs e)
        {
            string rradius = RRadius.Text;
            if (int.TryParse(rradius, out int radius))
            {
                if (radius > 20 && radius < 100)
                {
                    foreach (var fig in ShapeContainer.FigureList)
                    {
                        if (fig is Circle circle)
                        {
                            Point center = new Point(circle.X + circle.Width / 2, circle.Y + circle.Height / 2);

                            circle.Radius = radius;
                            circle.Y = (int)(center.Y - radius / 2);
                        }
                    }
                }
                else { MessageBox.Show("Ошибка в размерах"); }
            }
        }

        private void BtnPolygon_Click(object sender, RoutedEventArgs e)
        {
            string countangle = CountAngle.Text;
            if (int.TryParse(countangle, out int countang))
            {
                var pos = GetRandomPosition(80, 70);
                AddFigure(new Polygon((int)pos.X, (int)pos.Y,countang, 80));
                count += 1;
                if (count > 5)
                {
                    BtnPolygon.IsEnabled = false;
                }
            }
        }
    }
}