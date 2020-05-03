using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;     // für PathGradientBrush und ColorBlend
using System.Windows.Forms;

namespace HeatMapTest3 {
	public partial class Form1 : Form {
		List<Wert> wertListe = new List<Wert>();                                                                    // Liste mit wert,x,y,r,phi
		int alphaTransparenz = 0;                                                                                   // Transparenz der HeatMap
		int mindestAbstand = 20;                                                                                    // (( ungenutzt ))) Mindestabstand von Mauskoordinate
		bool firstPointIsCenterPoint = true;                                                                        // legt fest ob der erste wert der liste der CenterPoint ist oder nicht.

		public Form1() {
			InitializeComponent();
			alphaTransparenz = trackBar_transparenz.Value;
		}

		private void pictureBox1_MouseClick(object sender, MouseEventArgs e) {
			wertListe.Add(new Wert(80, e.X, e.Y));
			// Beispielwerte
			// -------------
			//wertListe.Add(new Wert(80, 80, 40));
			//wertListe.Add(new Wert(80, 110, 70));
			//wertListe.Add(new Wert(80, 70, 90));
			//wertListe.Add(new Wert(80, 70, 90));
			//wertListe.Add(new Wert(80, 130, 40));
			//wertListe.Add(new Wert(80, 130, 90));
			//wertListe.Add(new Wert(80, 110, 110));
			//wertListe.Add(new Wert(80, 70, 110));
			//wertListe.Add(new Wert(80, 50, 90));
			//wertListe.Add(new Wert(80, 50, 40));
			//wertListe.Add(new Wert(80, 70, 20));

			Point cp = findCenterPoint(wertListe);
			fillWertListRandPhi(wertListe, cp);
			wertListe.Sort(); // sortiert per Default nach Phi.
			pictureBox1.Image = drawDataPoint(wertListe, cp);
		}                                     // hinzufügen und verarbeiten eines neuen Datanpunkt.

		public Point findCenterPoint(List<Wert> dp) {
			if(firstPointIsCenterPoint == true) {
				// wenn der erste wert der centerpoint ist.
				return new Point(dp[0].X, dp[0].Y);
			}
			else {
				// wenn der erste wert nicht der centerpoint ist.
				int maxX = int.MinValue, maxY = int.MinValue, minX = int.MaxValue, minY = int.MaxValue;
				foreach(Wert DataPoint in dp) {
					if(DataPoint.X >= maxX) {
						maxX = (int)DataPoint.X;
					}
					if(DataPoint.Y >= maxY) {
						maxY = (int)DataPoint.Y;
					}
					if(DataPoint.X <= minX) {
						minX = (int)DataPoint.X;
					}
					if(DataPoint.Y <= minY) {
						minY = (int)DataPoint.Y;
					}
				}
				return new Point((maxX - minX) / 2, (maxY - minY) / 2);
			}
		}                                                             // ermittelt den gemeinsamen Mittelpunkt aller Datenpunkte.

		public int DistanceToCenterPoint(Point dp, Point cp) {
			return (int)Math.Sqrt
						 (
							 Math.Pow(dp.X - cp.X, 2) +
							 Math.Pow(dp.Y - cp.Y, 2)
						 );
		}                                                     // ((( ungenutzt ))) bestimmt die Distanz vom Datenpunt zum gemeinsamen Mittelpunkt aller Datenpunkte.

		private double ConvertDegreesToRadians(double degrees) {
			double radians = (Math.PI / 180) * degrees;
			return (radians);
		}                                                   // ((( ungenutzt ))) rechnet Grad in Bogenmaß um

		public void fillWertListRandPhi(List<Wert> dp, Point cp) {
			for(int i = 0; i < dp.Count; i++) {
				dp[i].R = dp[i].AddRToWert(dp[i], cp);
				dp[i].Phi = dp[i].AddPhiToWert(dp[i], cp);
				// optional ausgabe in Label
				label1.Text = "x:" + dp[i].X + " y:" + dp[i].Y + "\nr:" + dp[i].R + "\nphi:" + dp[i].Phi + "\ncpoffsetx:" + (dp[i].X - cp.X) + " cpoffsety:" + (dp[i].Y - cp.Y);
			}
		}                                                 // fügt allen Punkte aus List<Wert> .R und .Phi hinzu.

		public Bitmap drawDataPoint(List<Wert> dp, Point cp) {
			Bitmap bitMap = new Bitmap(pictureBox1.Width, pictureBox1.Height);                                          // erstellt leere Bitmap
			Graphics DrawMask = Graphics.FromImage(bitMap);                                                             // erzeugt eine neue Maske
			DrawMask.Clear(Color.Transparent);                                                                          // füllt die neue Maske mit einer Farbe
			DrawMask.SmoothingMode = SmoothingMode.AntiAlias;                                                           // aktiviert Kantenglättung

			Pen stift = new Pen(Color.FromArgb(alphaTransparenz, 0, 255, 0), 2.0f);                                     // erstellem eines zeichenstift in der angegebenen farbe.
			stift.StartCap = LineCap.Round;                                                                             // anfang der linie abrunden.
			stift.EndCap = LineCap.Round;                                                                               // ende der linie abrunden.
			stift.LineJoin = LineJoin.Round;                                                                            // aneinandergefügte linien abrunden (Miter für spitzen).

			for(int i = 0; i < dp.Count; i++)																																						// geht die liste mit werten durch
			{
				stift.Color = Color.FromArgb(alphaTransparenz, 0, 150, 150);                                              // ### farbe wird später an Stärke angepasst ###
				DrawMask.DrawRectangle(stift, dp[i].X, dp[i].Y, 2, 2);                                                    // markiert den angeklickten Punkt
				// Detils zeichnen
				// ---------------
				DrawMask.DrawLine(stift, cp.X, cp.Y, dp[i].X, dp[i].Y);                                                   // zeichnet eine linie zwischen mitte und dem angeklickten Punkt
				//stift.Color = Color.Blue;
				//DrawMask.DrawLine(stift, cp.X, cp.Y, dp[i].X, cp.Y);																										// zeichnet eine linie zwischen mitte und x des angeklickten Punkt
				//DrawMask.DrawLine(stift, cp.X, cp.Y, cp.X, dp[i].Y);																										// zeichnet eine linie zwischen mitte und y des angeklickten Punkt
				//stift.Color = Color.Purple;
				//DrawMask.DrawLine(stift, dp[i].X, dp[i].Y, dp[i].X, cp.Y);																							// zeichnet eine linie zwischen y der mitte und dem angeklickten Punkt
				//DrawMask.DrawLine(stift, dp[i].X, dp[i].Y, cp.X, dp[i].Y);                                              // zeichnet eine linie zwischen x der mitte und dem angeklickten Punkt
			}

			List<Point> PointsList = new List<Point>();                                                                 // liste mit umrandung
			foreach(Wert DataPoint in dp)																																								// geht die liste mit werten durch (evtl nach Wert.wert filterbar?)
			{
				PointsList.Add(new Point(DataPoint.X, DataPoint.Y));
			}

			if(firstPointIsCenterPoint == true) {
				// wenn erster wert der centerpoint ist.
				PointsList.RemoveAt(0);                                                                                   // entfernt den ersten wert (den centerpoint aus der liste
			}

			if(PointsList.Count > 2) {
				Point[] PointsArray = PointsList.ToArray();                                                               // erstelle array aus unterpunken.
				PathGradientBrush pinsel2 = new PathGradientBrush(PointsArray);                                           // ob PointsArray oder was anders ???????    k.a.
				Color[] randfarbe = { Color.FromArgb(alphaTransparenz, 255, 255, 0) };                                    // argb farbe als array. ### farbe wird später an Stärke angepasst ###
				pinsel2.SurroundColors = randfarbe;                                                                       // festlegen der außenfarbe.
				pinsel2.CenterColor = Color.FromArgb(alphaTransparenz, 0, 255, 0);                                        // argb farbe für die mitte. ### farbe wird später an Stärke angepasst ###
				GraphicsPath path = new GraphicsPath();
				path.AddLines(PointsArray);                                                                               // erzeugt aus dem Array einen Pfad.
				DrawMask.DrawPath(stift, path);                                                                           // zeichnet mit dem stift einen Pfad anhand der Werte im path
				DrawMask.FillPath(pinsel2, path);                                                                         // füllt mit pinsel2 die Fläche welche von den Werte im path umrandet wurde
				DrawMask.DrawPolygon(stift, PointsArray);                                                                 // zeichnet mit dem stift ein Polygon angand der Werte im PointArray
				DrawMask.FillPolygon(pinsel2, PointsArray);                                                               // füllt mit pinsel2 ein Polygon anhand der Werte im PointArray
			}
			return bitMap;                                                                                              // Maske ohne farbe und transparenz
		}                                                     // zeichnet alle Punkte aus List<Wert>.

		private void DrawWerte(Graphics Canvas, Wert Wert) {
			List<Point> CircumferencePointsList = new List<Point>();
			Point CircumferencePoint;
			Point[] CircumferencePointsArray;
			float fRatio = 1F / Byte.MaxValue;
			byte bHalf = Byte.MaxValue / 2;
			int iIntensity = (byte)(Wert.Stärke - ((Wert.Stärke - bHalf) * 2));
			float fIntensity = iIntensity * fRatio;
			for(double i = 0; i <= 360; i += 10) {
				CircumferencePoint = new Point();
				CircumferencePoint.X = Convert.ToInt32(Wert.X + Wert.Stärke * Math.Cos(ConvertDegreesToRadians(i))); // Wert.Stärke = früher Radius
				CircumferencePoint.Y = Convert.ToInt32(Wert.Y + Wert.Stärke * Math.Sin(ConvertDegreesToRadians(i))); // Wert.Stärke = früher Radius
				CircumferencePointsList.Add(CircumferencePoint);
			}
			CircumferencePointsArray = CircumferencePointsList.ToArray();
			PathGradientBrush GradientShaper = new PathGradientBrush(CircumferencePointsArray);
			ColorBlend GradientSpecifications = new ColorBlend(4);
			GradientSpecifications.Positions = new float[4] { 0, fIntensity, fIntensity, 1 };
			if((Wert.Stärke / 50) < 0) {
				if((Wert.Stärke / 25) < 0) {
					GradientSpecifications.Colors = new Color[4] {
												Color.FromArgb(0, Color.White),
												Color.FromArgb(Wert.Stärke/4, Color.LightGray),
												Color.FromArgb(Wert.Stärke/2, Color.DarkGray),
												Color.FromArgb(Wert.Stärke*2, Color.Black)
										};
				}
				else {
					GradientSpecifications.Colors = new Color[4] {
												Color.FromArgb(0, Color.White),
												Color.FromArgb(Wert.Stärke/25, Color.LightGray),
												Color.FromArgb(Wert.Stärke/9, Color.DarkGray),
												Color.FromArgb(Wert.Stärke*2, Color.Black)
										};
				}
			}
			else {
				GradientSpecifications.Colors = new Color[4] {
										Color.FromArgb(0, Color.White),
										Color.FromArgb(Wert.Stärke/50, Color.LightGray),
										Color.FromArgb(Wert.Stärke/25, Color.DarkGray),
										Color.FromArgb(Wert.Stärke*2, Color.Black)
								};
			}
			GradientShaper.InterpolationColors = GradientSpecifications;
			Canvas.FillPolygon(GradientShaper, CircumferencePointsArray);
		}                                                       // ((( ungenutzt ))) fügt der Maske(NoiceMap) einen Wert hinzu

		private void trackBar_transparenz_ValueChanged(object sender, EventArgs e) {
			alphaTransparenz = trackBar_transparenz.Value;
			Point cp = findCenterPoint(wertListe);
			pictureBox1.Image = drawDataPoint(wertListe, cp);
		}                               // zeichnet bei event die grafik neu.
	}
}

public class Wert : IComparable<Wert> {
	public int Stärke { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
	public double R { get; set; }
	public double Phi { get; set; }

	public struct PolarPoint {
		public double R { get; set; }
		public double Phi { get; set; }
		public PolarPoint(double r, double phi) {
			R = r;
			Phi = phi;
		}
	}

	public Wert(int s, int px, int py) {
		this.Stärke = s;
		this.X = px;
		this.Y = py;
	}

	// weitere Formeln:
	// ----------------
	// Radiant pro Grad = PI / 180 =  0,0175
	// Grad pro Radiant = 180 / PI = 57,2958
	// Bogenmaß = Radiant pro Grad * Winkel
	// Kreisbogenlänge = (Winkel/180) * PI * Radius
	// Winkel = Grad pro Radiant * Bogenmaß
	// Radius = Kreisbogenlänge / Bogenmaß

	public PolarPoint PointToPolarPoint(Point p, Point cp) {
		int centerX = cp.X, centerY = cp.Y;
		int ppx = p.X - centerX, ppy = p.Y - centerY;
		double r, phi, u, b, kt, a;
		// umrechnen von point zu polar
		r = Math.Sqrt((ppx * ppx) + (ppy * ppy));
		if(ppx > 0 && ppy > 0) {
			phi = Math.Acos(ppx / r) * (180 / Math.PI);
		}
		else {
			if(ppx < 0 && ppy > 0) {
				phi = Math.Acos(ppx / r) * (180 / Math.PI);
			}
			else {
				if(ppx < 0 && ppy < 0) {
					phi = Math.Acos(ppx / -r) * (180 / Math.PI) + 180;
				}
				else {
					if(ppx > 0 && ppy < 0) {
						phi = Math.Acos(ppx / -r) * (180 / Math.PI) + 180;
					}
					else {
						// Winkel über 360° sind Fehler.
						phi = 0;
					}
				}
			}
		}
		return new PolarPoint(r, phi);
	}																// rechnet kartesische Koordinaten in Polarkoordinaten (r,phi) um

	public Point PolarPointToPoint(PolarPoint pp, Point cp) {
		double r = pp.R, phi = pp.Phi;
		int centerX = cp.X, centerY = cp.Y;
		int x, y;
		// umrechnen von polar zu point
		if(phi >= 0 && phi <= 90) {
			x = centerX + Convert.ToInt32(Math.Cos(phi) * r);                 // x ist die Ankatete eines rechtwinkligen Dreieck und r ist dessen Hypotenuse
			y = centerY + Convert.ToInt32(Math.Sin(phi) * r);                 // y ist die Gegenkatete eines rechtwinklingen Dreicke und r ist dessen Hypotenuse
		}
		else {
			if(phi <= 180) {
				x = centerX - Convert.ToInt32(Math.Cos(phi) * r);
				y = centerY + Convert.ToInt32(Math.Sin(phi) * r);
			}
			else {
				if(phi <= 270) {
					x = centerX - Convert.ToInt32(Math.Cos(phi) * r);
					y = centerY - Convert.ToInt32(Math.Sin(phi) * r);
				}
				else {
					if(phi <= 360) {
						x = centerX + Convert.ToInt32(Math.Cos(phi) * r);
						y = centerY - Convert.ToInt32(Math.Sin(phi) * r);
					}
					else {
						// Winkel über 360° sind Fehler.
						x = 0;
						y = 0;
					}
				}
			}
		}
		return new Point(x, y);
	}																// rechnet Polarkoordinaten (r,phi) in kartesische Koordinaten (x,y) um

	public double AddRToWert(Wert w, Point cp) {
		PolarPoint p = PointToPolarPoint(new Point(w.X, w.Y), cp);
		return p.R;
	}
	public double AddPhiToWert(Wert w, Point cp) {
		PolarPoint p = PointToPolarPoint(new Point(w.X, w.Y), cp);
		return p.Phi;
	}

	public int CompareTo(Wert compare) {
		// A null value means that this object is greater.
		if(compare == null) {
			return 1;
		}
		else {
			return this.Phi.CompareTo(compare.Phi);
		}
	}																										// Default vergleichs Methode für .Sort(IComparable)
}                                                                                           // Klasse für Messwerte.