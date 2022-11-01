using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Data.SqlTypes;
using static System.Windows.Forms.AxHost;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;

namespace kg_laba2
{
	public class Line
	{
		private Point a;
		private Point b;

		private double angle;
		private double len;

		public Line(Point x, Point y)
		{
			a = x;
			b = y;

			angle = Math.Atan((double)Math.Tan((double)(a.X - b.X) / (a.Y - b.Y)));
			len = Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
		}

		public Line(int Ax, int Ay, int Bx, int By)
		{
			a = new Point(Ax, Ay);
			b = new Point(Bx, By);

			angle = Math.Atan((double)Math.Tan((double)(a.X - b.X) / (a.Y - b.Y)));
			len = Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
		}

		public Point GetMediane()
		{
			int x = (int)Math.Round((double)(a.X + b.X) / 2);
			int y = (int)Math.Round((double)(a.Y + b.Y) / 2);

			return new Point(x, y);
		}

		public double GetAngle()
		{
			return angle;
		}

		public double GetLen()
		{
			return len;
		}

		public Point GetStart()
		{
			return a;
		}

		public Point GetEnd()
		{
			return b;
		}

		public void SetStart(Point x)
		{
			a = x;
		}

		public void SetEnd(Point x)
		{
			b = x;
		}

	}

	public partial class Form1 : Form
	{
		Bitmap bitmap;
		Stopwatch stopwatch;

		//Color backColor;//цвет фона

		public Form1()
		{
			InitializeComponent();
			InitDrawing();
			ImageHandler();
		}

		private void InitDrawing()
		{
			bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
		}

		private void ImageHandler()
		{
			int[] heighSize = { 0, 50, 75 };
			Color colorLine = Color.Black;
			int width = 40;
			int gap = 5;
			int N = 3;

			for (int i = 0; i < N; i++)
			{
				Line footing1 = new Line(0 + (width + gap) * i, N * width + (N - 1) * gap, width + (width + gap) * i, N * width + (N - 1) * gap);
				Line height1 = new Line(footing1.GetMediane(), new Point(footing1.GetMediane().X, heighSize[i]));

				Line A = new Line(height1.GetEnd(), footing1.GetStart()); //левая
				Line B = new Line(height1.GetEnd(), footing1.GetEnd()); //правая

				//основание
				DrawLineA(footing1.GetStart(), footing1.GetEnd(), colorLine);

				//боковые стороны
				DrawLineA(A.GetEnd(), A.GetStart(), colorLine);
				DrawLineA(B.GetEnd(), B.GetStart(), colorLine);

				//медианы
				DrawLineA(A.GetMediane(), footing1.GetEnd(), colorLine);
				DrawLineA(B.GetMediane(), footing1.GetStart(), colorLine);
				DrawLineA(height1.GetEnd(), footing1.GetMediane(), colorLine);

				Line med = new Line(A.GetMediane(), footing1.GetEnd());
				var angle = med.GetAngle();
				var len = med.GetLen();

				stopwatch = Stopwatch.StartNew();
				DrawLineA(med.GetStart(), med.GetEnd(), colorLine);
				stopwatch.Stop();
				Console.WriteLine("Angle: " + angle);
				Console.WriteLine("Len: " + len);
				Console.WriteLine("Time: " + stopwatch.ElapsedTicks);

				//основание
				DrawLineB(footing1.GetStart(), footing1.GetEnd(), colorLine);

				//боковые стороны
				DrawLineB(A.GetEnd(), A.GetStart(), colorLine);
				DrawLineB(B.GetEnd(), B.GetStart(), colorLine);

				//медианы
				DrawLineB(B.GetMediane(), footing1.GetStart(), colorLine);
				DrawLineB(height1.GetEnd(), footing1.GetMediane(), colorLine);

				stopwatch = Stopwatch.StartNew();
				DrawLineB(A.GetMediane(), footing1.GetEnd(), colorLine);
				stopwatch.Stop();
				Console.WriteLine("Time (B): " + stopwatch.ElapsedTicks);


				//Координаты для закраски каждого треугольника
				Point point6 = new Point(height1.GetStart().X - 1, height1.GetStart().Y - 1);
				Point point5 = new Point(height1.GetStart().X + 1, height1.GetStart().Y - 1);
				Point point4 = new Point(A.GetMediane().X + 2, A.GetMediane().Y - 1);
				Point point3 = new Point(A.GetMediane().X, A.GetMediane().Y + 5);
				Point point2 = new Point(B.GetMediane().X - 2, B.GetMediane().Y - 1);
				Point point1 = new Point(B.GetMediane().X, B.GetMediane().Y + 5);

				stopwatch = Stopwatch.StartNew();
				DrawFillA(colorLine, Color.Coral, point1);
				DrawFillA(colorLine, Color.Chartreuse, point2);
				DrawFillA(colorLine, Color.DarkSalmon, point3);
				DrawFillA(colorLine, Color.Khaki, point4);
				DrawFillA(colorLine, Color.DarkGreen, point5);
				DrawFillA(colorLine, Color.Indigo, point6);
				stopwatch.Stop();
				Console.WriteLine("Fill Time (A): " + stopwatch.ElapsedTicks);

				stopwatch = Stopwatch.StartNew();
				DrawFillB(colorLine, Color.Coral, point1);
				DrawFillB(colorLine, Color.Chartreuse, point2);
				DrawFillB(colorLine, Color.DarkSalmon, point3);
				DrawFillB(colorLine, Color.Khaki, point4);
				DrawFillB(colorLine, Color.DarkGreen, point5);
				DrawFillB(colorLine, Color.Indigo, point6);
				stopwatch.Stop();
				Console.WriteLine("Fill Time (B): " + stopwatch.ElapsedTicks);

				//Из каждой вершины треугольников,
				//образованных медианами исходного треугольника, также проведены медианы на
				//собственные стороны

				//Вершины треугольников от основных медиан
				Point vmed1 = footing1.GetStart();
				Point vmed2 = B.GetStart();
				Point vmed3 = B.GetEnd();
				Point vmed4 = A.GetMediane();
				Point vmed5 = B.GetMediane();
				Point vmed6 = footing1.GetMediane();
				Point vmed7 = new Point(footing1.GetMediane().X, footing1.GetMediane().Y - (int)Math.Round((double)(height1.GetLen()) / 3));


				//мини медианы
				DrawTriangleMediane(new Line(vmed1, vmed6), new Line(vmed1, vmed7), new Line(vmed6, vmed7), colorLine);
				DrawTriangleMediane(new Line(vmed6, vmed3), new Line(vmed3, vmed7), new Line(vmed6, vmed7), colorLine);
				DrawTriangleMediane(new Line(vmed7, vmed5), new Line(vmed7, vmed3), new Line(vmed5, vmed3), colorLine);
				DrawTriangleMediane(new Line(vmed1, vmed4), new Line(vmed1, vmed7), new Line(vmed4, vmed7), colorLine);
				DrawTriangleMediane(new Line(vmed4, vmed2), new Line(vmed4, vmed7), new Line(vmed2, vmed7), colorLine);
				DrawTriangleMediane(new Line(vmed2, vmed5), new Line(vmed2, vmed7), new Line(vmed5, vmed7), colorLine);

				//соединение точек пересечения мини медиан
				Line medx11 = new Line((new Line(vmed1, vmed6)).GetMediane(), vmed7);
				Line medx12 = new Line((new Line(vmed7, vmed6)).GetMediane(), vmed1);

				Line medx21 = new Line((new Line(vmed3, vmed6)).GetMediane(), vmed7);
				Line medx22 = new Line((new Line(vmed7, vmed3)).GetMediane(), vmed6);

				Line medx31 = new Line((new Line(vmed3, vmed7)).GetMediane(), vmed5);
				Line medx32 = new Line((new Line(vmed5, vmed3)).GetMediane(), vmed7);

				Line medx41 = new Line((new Line(vmed4, vmed7)).GetMediane(), vmed1);
				Line medx42 = new Line((new Line(vmed7, vmed1)).GetMediane(), vmed4);

				Line medx51 = new Line((new Line(vmed4, vmed2)).GetMediane(), vmed7);
				Line medx52 = new Line((new Line(vmed7, vmed2)).GetMediane(), vmed4);

				Line medx61 = new Line((new Line(vmed2, vmed5)).GetMediane(), vmed7);
				Line medx62 = new Line((new Line(vmed7, vmed2)).GetMediane(), vmed5);

				Point xmed1 = Cross(medx11, medx12);
				Point xmed2 = Cross(medx21, medx22);
				Point xmed3 = Cross(medx31, medx32);

				Point xmed4 = Cross(medx41, medx42);
				Point xmed5 = Cross(medx51, medx52);
				Point xmed6 = Cross(medx61, medx62);


				Color colorline2 = Color.Red;
				DrawLineA(xmed1, xmed2, colorline2);
				DrawLineA(xmed2, xmed4, colorline2);
				DrawLineA(xmed6, xmed4, colorline2);
				DrawLineA(xmed5, xmed6, colorline2);
				DrawLineA(xmed5, xmed3, colorline2);
				DrawLineA(xmed1, xmed3, colorline2);
			}


			



			LoopPic(new Point(0, 0), N * width + (N - 1) * gap + 1, N * width + (N - 1) * gap + 1, 3);
			pictureBox1.Image = bitmap;
		}

		private void DrawTriangleMediane(Line A, Line B, Line C, Color colorLine)
		{
			//медианы
			DrawLineA(A.GetMediane(), C.GetEnd(), colorLine);
			DrawLineA(B.GetMediane(), C.GetStart(), colorLine);
			DrawLineA(B.GetStart(), C.GetMediane(), colorLine);

		}

		private void DrawLineA(Point start, Point end, Color color)
		{
			//Если нам нужно двигать по x, то end.X > start.X и модуль тангенса <= 0,577 (45*)
			//Если нам нужно двигать по у, то end.Y > start.Y и модуль тангенса > 0,577
			//тангенс = y1 - y0 / x - x0
			double tg = Math.Abs((double)(end.Y - start.Y) / (end.X - start.X));

			if (start.X > end.X && tg < 1 || end.Y < start.Y && tg > 1)
			{
				int temp = start.X;
				start.X = end.X;
				end.X = temp;

				temp = end.Y;
				end.Y = start.Y;
				start.Y = temp;
			}

			//Уравнение прямой
			// ax + by + C = 0
			// (x - x1)/(x2 - x1)=(y - y1)/(y2 - y1) - уравнение прямой по двум точкам 
			//a
			int A = end.Y - start.Y;
			//b
			int B = -end.X + start.X;
			//int C = -start.X * (end.Y - start.Y) + end.Y * (end.X - start.X);

			double kx = (double)A / (-B);
			double ky = (double)B / (-A);

			if (tg < 1 || kx == 0)
			{
				double y = start.Y;

				for (double x = start.X; x <= end.X; x++)
				{
					bitmap.SetPixel((int)Math.Round(x), (int)Math.Round(y), color);
					y += kx;
				}
			}
			else
			{
				double x = start.X;

				for (double y = start.Y; y <= end.Y; y++)
				{
					bitmap.SetPixel((int)Math.Round(x), (int)Math.Round(y), color);
					x += ky;
				}
			}
		}

		private void DrawFillA(Color borderColor, Color fillColor, Point start)
		{
			if (!areColorsEqual(bitmap.GetPixel(start.X, start.Y), borderColor) && !areColorsEqual(bitmap.GetPixel(start.X, start.Y), fillColor))
			{
				bitmap.SetPixel(start.X, start.Y, fillColor);

				DrawFillA(borderColor, fillColor, new Point(start.X + 1, start.Y));
				DrawFillA(borderColor, fillColor, new Point(start.X, start.Y + 1));
				DrawFillA(borderColor, fillColor, new Point(start.X - 1, start.Y));
				DrawFillA(borderColor, fillColor, new Point(start.X, start.Y - 1));

			}
		}

		private bool areColorsEqual(Color col1, Color col2)
		{
			if (col1.A == col2.A && col1.R == col2.R && col1.G == col2.G && col1.B == col2.B) return true;
			return false;
		}

		//увеличивает область с точки старт в некоторое rang раз
		private void LoopPic(Point start, int width, int height, int rang)
		{
			Bitmap bitmap2 = new Bitmap(bitmap);

			int startX = start.X - (width / rang) > 0 ? start.X - (int)Math.Round((double)width / rang) : 0;
			int startY = start.Y - (height / rang) > 0 ? start.Y - (int)Math.Round((double)height / rang) : 0;

			for (int i = 0; i < width * rang; i++)
			{
				for (int j = 0; j < height * rang; j++)
				{
					bitmap2.SetPixel(startX + i, startY + j, bitmap.GetPixel(start.X + (int)(i / rang), start.Y + (int)(j / rang)));
				}
			}

			bitmap = bitmap2;
		}

		private void DrawLineB(Point start, Point end, Color color)
		{
			int xStartPixel = (int)Math.Round((double)start.X);
			int yStartPixel = (int)Math.Round((double)start.Y);
			int xEndPixel = (int)Math.Round((double)end.X);
			int yEndPixel = (int)Math.Round((double)end.Y);

			int width = Math.Abs(xStartPixel - xEndPixel);
			int height = Math.Abs(yStartPixel - yEndPixel);

			int length = Math.Max(width, height);

			if (length == 0)
			{
				bitmap.SetPixel(xStartPixel, yStartPixel, color);
			}

			length += 1;

			double dX = (end.X - start.X) / (double)length;
			double dY = (end.Y - start.Y) / (double)length;

			double x = start.X;
			double y = start.Y;

			while (length > 0)
			{
				x += dX;
				y += dY;

				var pixel = new Point((int)Math.Round(x), (int)Math.Round(y));

				if (pixel.X >= bitmap.Width || pixel.Y >= bitmap.Height)
					break;

				//if(areColorsEqual(bitmap.GetPixel(pixel.X, pixel.Y),color))
				//{
				//	bitmap.SetPixel(pixel.X, pixel.Y, Color.Black);
				//}
				//else
				//{
				//	bitmap.SetPixel(pixel.X, pixel.Y, Color.Green);
				//}

				bitmap.SetPixel(pixel.X, pixel.Y, Color.Black);

				length--;
			}
		}

		private void DrawFillB(Color borderColor, Color fillColor, Point start)
		{
			var stack = new Stack<Point>();
			stack.Push(start);

			while (stack.Count > 0)
			{
				var stackPixel = stack.Pop();
				if (!areColorsEqual(bitmap.GetPixel(stackPixel.X, stackPixel.Y), fillColor))
				{
					bitmap.SetPixel(stackPixel.X, stackPixel.Y, Color.Red);
				}

				var rPixel = RightPixel(stackPixel);
				var pixelColor = bitmap.GetPixel(rPixel.X, rPixel.Y);
				while (!areColorsEqual(pixelColor, borderColor))
				{
					if (!areColorsEqual(bitmap.GetPixel(rPixel.X, rPixel.Y), fillColor))
					{
						bitmap.SetPixel(rPixel.X, rPixel.Y, fillColor);
					}

					rPixel = RightPixel(rPixel);

					if (rPixel.X >= bitmap.Width)
						break;

					pixelColor = bitmap.GetPixel(rPixel.X, rPixel.Y);
				}

				var lPixel = LeftPixel(stackPixel);
				pixelColor = bitmap.GetPixel(lPixel.X, lPixel.Y);
				while (!areColorsEqual(pixelColor, borderColor))
				{
					if (!areColorsEqual(bitmap.GetPixel(lPixel.X, lPixel.Y), fillColor))
					{
						bitmap.SetPixel(lPixel.X, lPixel.Y, Color.Red);
					}


					if (lPixel.X == 0)
						break;

					lPixel = LeftPixel(lPixel);
					pixelColor = bitmap.GetPixel(lPixel.X, lPixel.Y);
				}

				if (stackPixel.Y < bitmap.Height)
					Scan(bitmap, stackPixel.Y + 1, lPixel.X + 1, rPixel.X - 1, stack, fillColor, borderColor);

				if (stackPixel.Y > 0)
					Scan(bitmap, stackPixel.Y - 1, lPixel.X + 1, rPixel.X - 1, stack, fillColor, borderColor);
			}
		}

		private void Scan(Bitmap canvas, int y, int lx, int rx, Stack<Point> stack, Color fillColor, Color borderColor)
		{
			for (int x = lx; x <= rx; x++)
			{
				var color = canvas.GetPixel(x, y);
				if (areColorsEqual(color, fillColor))
					return;

				if (areColorsEqual(color, borderColor))
				{
					continue;
				}

				stack.Push(new Point(x, y));
				return;
			}
		}

		Point RightPixel(Point pixel)
		{
			return new Point(pixel.X + 1, pixel.Y);
		}

		Point LeftPixel(Point pixel)
		{
			return new Point(pixel.X - 1, pixel.Y);
		}

		private Point Cross(Line line1, Line line2)
		{
			float x1 = line1.GetStart().X;
			float y1 = line1.GetStart().Y;
			float x2 = line1.GetEnd().X;
			float y2 = line1.GetEnd().Y;
			float x3 = line2.GetStart().X;
			float y3 = line2.GetStart().Y;
			float x4 = line2.GetEnd().X;
			float y4 = line2.GetEnd().Y;
			float n;
			if (y2 - y1 != 0)
			{
				float q = (x2 - x1) / (y1 - y2);
				float sn = (x3 - x4) + (y3 - y4) * q;
				float fn = (x3 - x1) + (y3 - y1) * q;
				n = fn / sn;
			}
			else
			{
				n = (y3 - y1) / (y3 - y4); // c(y)/b(y)
			}
			return new Point((int)(x3 + (x4 - x3) * n), (int)(y3 + (y4 - y3) * n));
		}
	}

}


