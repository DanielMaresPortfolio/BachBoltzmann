using BachBoltzman;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xaml;



namespace BachBoltyman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public bool[,] CustomLayout 
        {
            get;set;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public MainWindow()
        { 
            InitializeComponent();
        }
        private double viskozity = 0.1;
        public double Viskozity
        {
            get => viskozity;
            set => viskozity = value;
        }
        private void Testing(object sender, RoutedEventArgs e)
        {
            int timeCykle = 1000; //this says how many times whole lattice will be simulated
            int timeSnap = 50;  //this says whitch data will be saved(every n-th) 
            InicLayout layout = new InicLayout();
            Lattice lattice = new Lattice(layout.SizeX, layout.SizeY, 0.1);
            Lattice.Run(timeCykle,timeSnap,0.0,0.0,0.1,layout.TestingLayout);
            int[] timeScale = new int[(timeCykle / timeSnap)+1];
            //
            for (int i = 0; i <= timeCykle / timeSnap; i++)
            {
                timeScale[i] = i * timeSnap;
            }
            Results results = new Results(timeScale,ref lattice);
            this.Visibility = Visibility.Collapsed;
            results.Show();
        }
        private static System.Windows.Point start;
        private static Polyline line;

        private void InpIm_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
           
            if (e.LeftButton ==MouseButtonState.Pressed) 
            {

                System.Windows.Point currentpoint = e.GetPosition(relativeTo: InpIm);
                if (start != currentpoint) 
                {
                    line.Points.Add(currentpoint);
                    CustomLayout[(int)currentpoint.X,(int)currentpoint.Y] = true;
                }
            }
        }

        private void InpIm_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            start = e.GetPosition(InpIm);
            line = new Polyline();
            line.StrokeThickness = 3;
            line.Stroke = new SolidColorBrush(Colors.White);

            InpIm.Children.Add(line);
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            int timeCykle = 50; //this says how many times whole lattice will be simulated
            int timeSnap = 50;  //this says whitch data will be saved(every n-th) 
            int sx = (int)InpIm.Width;
            int sy = (int)InpIm.Height;
            InicLayout Layout = new InicLayout(sx,sy);
            //
            bool[,] layot = new bool[sx,sy];
            bool[,] layotWE = new bool[sx+2,sy+2]; //Přidání okrajových podmínek
            Int32Rect int32Rect = new Int32Rect(0,0,sx,sy);

            //Bitmap bitmap = new Bitmap(sx,sy);
            //bitmap  = BitmapFromWriteableBitmap(SaveAsWriteableBitmap(InpIm));
            uint[] pixels = new uint[sx*sy];
            SaveAsWriteableBitmap(InpIm).CopyPixels(int32Rect, pixels,sx*4,0);

            int temp = 0;
                for (int x =0; x<sx;x++) 
                {
                    for (int y = 0; y < sy; y++)
                    {
                        if (pixels[temp] != 4278190080)//bitmap.GetPixel(x,y) == Colors.White)
                            {
                                layot[x, y] = true;
                            }
                            else 
                            {
                                layot[x, y] = false;
                            }
                        }
                    temp++;
                    }
            
            //
            for (int x =0; x<sx+2;x++) 
            {
                for (int y =0; y<sy+2;y++) 
                {
                    if (x == 0 || x == sx + 1 || y==0 || y ==sy+1)
                    {
                        layotWE[x, y] = true;
                    }
                    else 
                    {
                        layotWE[x, y] =layot[x-1, y-1];
                    }
                }
            }
            //
            Layout.MyLayout = layotWE;
            Lattice lattice = new Lattice(sx+2, sy+2, this.Viskozity);
            Lattice.Run(timeCykle, timeSnap, 0.0, 0.0, 0.1, Layout.MyLayout);
            int[] timeScale = new int[(timeCykle / timeSnap) + 1];
            //
            for (int i = 0; i <= timeCykle / timeSnap; i++)
            {
                timeScale[i] = i * timeSnap;
            }
            Results results = new Results(timeScale, ref lattice);
            this.Visibility = Visibility.Collapsed;
            results.Show();
        }
        public WriteableBitmap SaveAsWriteableBitmap(Canvas surface) //Kod z Stack overflow
        {
            if (surface == null) return null;

            // Save current canvas transform
            Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            // Get the size of canvas
            System.Windows.Size size = new System.Windows.Size(surface.ActualWidth, surface.ActualHeight);
            // Measure and arrange the surface
            // VERY IMPORTANT
            surface.Measure(size);
            surface.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
              (int)size.Width,
              (int)size.Height,
              96d,
              96d,
              PixelFormats.Pbgra32);
            renderBitmap.Render(surface);


            //Restore previously saved layout
            surface.LayoutTransform = transform;

            //create and return a new WriteableBitmap using the RenderTargetBitmap
            return new WriteableBitmap(renderBitmap);

        }

        private void InpIm_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                CustomLayout = new bool[(int)InpIm.Width, (int)InpIm.Height];
                for (int y = 0; y < InpIm.Height; y++)
                {
                    for (int x = 0; x < InpIm.Width; x++)
                    {
                        CustomLayout[x, y] = false;
                    }
                }
            }
            catch 
            { 
            }
           
        }
        //private System.Drawing.Bitmap BitmapFromWriteableBitmap(WriteableBitmap writeBmp)
        //{
        //    System.Drawing.Bitmap bmp;
        //    using (MemoryStream outStream = new MemoryStream())
        //    {
        //        BitmapEncoder enc = new BmpBitmapEncoder();
        //        enc.Frames.Add(BitmapFrame.Create((BitmapSource)writeBmp));
        //        enc.Save(outStream);
        //        bmp = new System.Drawing.Bitmap(outStream);
        //    }
        //    return bmp;
        //}
    }
}