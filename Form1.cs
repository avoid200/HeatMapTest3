using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;     // für PathGradientBrush und ColorBlend
using System.Drawing.Imaging;       // für PixelFormat
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeatMapTest3
{
	public partial class Form1 : Form
	{

		List<Werte> wertListe = new List<Werte>();																																	// erstellt leere Liste

		public Form1()
		{
			InitializeComponent();
		}

		private void addWert(object sender, MouseEventArgs e)
		{
      wertListe.Add(new Werte() { Wert = 80, X = e.X, Y = e.Y });
      updateBitmap();
		}                                                    // erzeugt eine Liste mit werten für eine NoiceMap.

		private void updateBitmap()
		{
			Bitmap bitMap = new Bitmap(pictureBox1.Width, pictureBox1.Height);																				// erstellt leere Bitmap
			bitMap = CreateIntensityMask(bitMap, wertListe);                                                          // füllt Bitmap mit Werten
      pictureBox1.Image = Colorize(bitMap, (byte)trackBar_transparenz.Value);                                   // färbt die Bitmap  incl. Transparenz
      //pictureBox1.Image = bitMap;                                                                             // Maske ohne farbe und transparenz
    }

		private Bitmap CreateIntensityMask(Bitmap bitMapMask, List<Werte> wertListeList)
		{
			Graphics DrawMask = Graphics.FromImage(bitMapMask);                                                       // erzeugt eine neue Maske
			DrawMask.Clear(Color.White);                                                                              // füllt die neue Maske mit einer Farbe
			foreach (Werte DataPoint in wertListeList)                                                                // geht die liste mit werten durch
			{
        DrawWerte(DrawMask, DataPoint);                                                             // fügt der maske die werte aus der liste hinzu
			}
			return bitMapMask;                                                                                        // gibt die ausgefüllte maske zurück
		}                         // erstellt eine Maske(NoiceMap) der Werte und gibt diese zurück

    private void DrawWerte(Graphics Canvas, Werte Wert)
    {
      List<Point> CircumferencePointsList = new List<Point>();
      Point CircumferencePoint;
      Point[] CircumferencePointsArray;
      float fRatio = 1F / Byte.MaxValue;
      byte bHalf = Byte.MaxValue / 2;
      int iIntensity = (byte)(Wert.Wert - ((Wert.Wert - bHalf) * 2));
      float fIntensity = iIntensity * fRatio;
      for (double i = 0; i <= 360; i += 10)
      {
        CircumferencePoint = new Point();
        CircumferencePoint.X = Convert.ToInt32(Wert.X + Wert.Wert * Math.Cos(ConvertDegreesToRadians(i))); // Wert.Wert = früher Radius
        CircumferencePoint.Y = Convert.ToInt32(Wert.Y + Wert.Wert * Math.Sin(ConvertDegreesToRadians(i))); // Wert.Wert = früher Radius
        CircumferencePointsList.Add(CircumferencePoint);
      }
      CircumferencePointsArray = CircumferencePointsList.ToArray();
      PathGradientBrush GradientShaper = new PathGradientBrush(CircumferencePointsArray);
      ColorBlend GradientSpecifications = new ColorBlend(4);
      GradientSpecifications.Positions = new float[4] { 0, fIntensity, fIntensity, 1 };
      if ((Wert.Wert / 50) < 0)
      {
        if ((Wert.Wert / 25) < 0)
        {
          GradientSpecifications.Colors = new Color[4] {
                        Color.FromArgb(0, Color.White),
                        Color.FromArgb(Wert.Wert/4, Color.LightGray),
                        Color.FromArgb(Wert.Wert/2, Color.DarkGray),
                        Color.FromArgb(Wert.Wert*2, Color.Black)
                    };
        }
        else
        {
          GradientSpecifications.Colors = new Color[4] {
                        Color.FromArgb(0, Color.White),
                        Color.FromArgb(Wert.Wert/25, Color.LightGray),
                        Color.FromArgb(Wert.Wert/9, Color.DarkGray),
                        Color.FromArgb(Wert.Wert*2, Color.Black)
                    };
        }
      }
      else
      {
        GradientSpecifications.Colors = new Color[4] {
                    Color.FromArgb(0, Color.White),
                    Color.FromArgb(Wert.Wert/50, Color.LightGray),
                    Color.FromArgb(Wert.Wert/25, Color.DarkGray),
                    Color.FromArgb(Wert.Wert*2, Color.Black)
                };
      }
      GradientShaper.InterpolationColors = GradientSpecifications;
      Canvas.FillPolygon(GradientShaper, CircumferencePointsArray);
    }                                                      // fügt der Maske(NoiceMap) einen Wert hinzu

    private double ConvertDegreesToRadians(double degrees)
    {
      double radians = (Math.PI / 180) * degrees;
      return (radians);
    }                                                   // rechnet Grad in Bogenmaß um

    public static Bitmap Colorize(Bitmap Mask, byte Alpha)
    {
      Bitmap Output = new Bitmap(Mask.Width, Mask.Height, PixelFormat.Format32bppArgb);
      Graphics Surface = Graphics.FromImage(Output);
      Surface.Clear(Color.Transparent);
      ColorMap[] Colors = CreatePaletteIndex(Alpha);
      ImageAttributes Remapper = new ImageAttributes();
      Remapper.SetRemapTable(Colors);
      Surface.DrawImage(Mask, new Rectangle(0, 0, Mask.Width, Mask.Height), 0, 0, Mask.Width, Mask.Height, GraphicsUnit.Pixel, Remapper);
      return Output;
    }

    private static ColorMap[] CreatePaletteIndex(byte Alpha)
    {
      ColorMap[] OutputMap = new ColorMap[256];
      Bitmap Palette = Properties.Resources.intensity_mask2;                                                    // intensity_mask2.bmp als Resources
      for (int X = 0; X <= 255; X++)
      {
        OutputMap[X] = new ColorMap();
        OutputMap[X].OldColor = Color.FromArgb(X, X, X);
        OutputMap[X].NewColor = Color.FromArgb(Alpha, Palette.GetPixel(X, 0));
      }
      return OutputMap;
    }

    private void trackBar_transparenz_ValueChanged(object sender, EventArgs e)
    {
      updateBitmap();
    }
  }
  public struct Werte
	{
		public int Wert { get; set; }
		public float X { get; set; }
		public float Y { get; set; }
	}																																											  // list Objekt für die Messwerte.
}
