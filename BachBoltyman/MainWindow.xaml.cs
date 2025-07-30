using BachBoltzman;
using System;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace BachBoltyman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WriteableBitmap bitmap;
        public MainWindow()
        {
            //WriteableBitmap bitmap = new WriteableBitmap((int)SizeX,(int)SizeY,96,96,PixelFormats.Bgr32,null);
            //InputIm.Source = bitmap.WritePixels();

            //
            //InputIm.MouseMove += new MouseEventHandler(i_MouseMove);
            //InputIm.MouseLeftButtonDown +=
            // new MouseButtonEventHandler(i_MouseLeftButtonDown);
            //InputIm.MouseRightButtonDown +=
            //    new MouseButtonEventHandler(i_MouseRightButtonDown);

            InitializeComponent();

        }
        private int sizeX = 200;
        public int SizeX 
        { 
            get =>sizeX; 
            set => sizeX = value; 
        }
        private int sizeY = 50;  
        public int SizeY
        {
            get => sizeY; 
            set => sizeY = value;
          }
        public double Vizkozity { get; set; }
        private void Testing(object sender, RoutedEventArgs e)
        {
            int timeCykle = 500; //this says how many times whole lattice will be simulated
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

        private void InputIm_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void InputIm_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void InputIm_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        static void Draw(System.Windows.Input.MouseEventArgs e) 
        {
            //int column = (int)e.GetPosition(InputIm).X;
            //int row = (int)e.GetPosition(InputIm).Y;

            //bitmap.Lock();
        }
        //
        //static void DrawPixel(MouseEventArgs e)
        //{
        //    int column = (int)e.GetPosition(InputIm).X;
        //    int row = (int)e.GetPosition(InputIm).Y;

        //    try
        //    {
        //        // Reserve the back buffer for updates.
        //        bitmap.Lock();

        //        unsafe
        //        {
        //            // Get a pointer to the back buffer.
        //            IntPtr pBackBuffer = bitmap.BackBuffer;

        //            // Find the address of the pixel to draw.
        //            pBackBuffer += row * bitmap.BackBufferStride;
        //            pBackBuffer += column * 4;

        //            // Compute the pixel's color. //pink
        //            int color_data = 255 << 16; // R
        //            color_data |= 128 << 8;   // G 
        //            color_data |= 255 << 0;   // B

        //            // Assign the color data to the pixel.
        //            *((int*)pBackBuffer) = color_data;
        //        }

        //        // Specify the area of the bitmap that changed.
        //        bitmap.AddDirtyRect(new Int32Rect(column, row, 1, 1));
        //    }
        //    finally
        //    {
        //        // Release the back buffer and make it available for display.
        //        bitmap.Unlock();
        //    }
        //}

        //static void ErasePixel(MouseEventArgs e)
        //{
        //    byte[] ColorData = { 0, 0, 0, 0 }; // B G R

        //    Int32Rect rect = new Int32Rect(
        //            (int)(e.GetPosition(InputIm).X),
        //            (int)(e.GetPosition(InputIm).Y),
        //            1,
        //            1);

        //    bitmap.WritePixels(rect, ColorData, 4, 0);
        //}

        //static void i_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    ErasePixel(e);
        //}

        //static void i_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    DrawPixel(e);
        //}

        //static void MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        DrawPixel(e);
        //    }
        //    else if (e.RightButton == MouseButtonState.Pressed)
        //    {
        //        ErasePixel(e);
        //    }
        //}
    }
}