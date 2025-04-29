using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using BachBoltzman;
using System.ComponentModel;
using System.Xaml;
using System.Linq;

namespace BachBoltyman
{
    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class Results : Window, INotifyPropertyChanged
    {
        public Results(double[,,] speedMap_, double[,,] densityMap_, int[] alltime_)
        {
            SpeedMap = speedMap_;
            DensityMap = densityMap_;
            AllTime = alltime_;
            DataContext = this;
            InitializeComponent();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DrawOutput(DensityMap, TimeSelect.SelectedIndex);
        }
        public double[,,] SpeedMap { get; set; }
        public double[,,] DensityMap { get; set; }
        public int[] AllTime
        {
            get; set;
        }
        public void DrawOutput(double[,,] map, int index)
        {
            Heatmap.Width = SpeedMap.GetLength(0);
            Heatmap.Height = SpeedMap.GetLength(1);
            int width = Convert.ToInt32(Heatmap.Width);
            int height = Convert.ToInt32(Heatmap.Height);
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null); //zdroj https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap?view=windowsdesktop-9.0&redirectedfrom=MSDN
            uint[] pixels = new uint[width * height];
            double[,] mapInTime = new double[map.GetLength(0), map.GetLength(1)];
            //

            for (int ix = 0; ix < width; ix++)
            {
                for (int iy = 0; iy < height; iy++)
                {
                    mapInTime[ix, iy] = map[ix, iy, index];
                }
            }
            //
            double maxValueMap = (from double d in mapInTime select d).Max();
            double minValueMap = (from double d in mapInTime select d).Min();
            //Console.WriteLine("Max hodnota" + "{0: F20}", maxValueMap);
            //Console.WriteLine("Min hodnota" + "{0: F20}", minValueMap);

            //pallete // <<24 is transparency 
            uint zc = (uint)((Convert.ToUInt32(255) << 24) + (Convert.ToUInt32(0) << 16) + (Convert.ToUInt32(0) << 8) + Convert.ToUInt32(0));
            //uint fc = (uint) ((Convert.ToUInt32(255) << 24) + (Convert.ToUInt32(251) << 16) + (Convert.ToUInt32(99) << 8) + Convert.ToUInt32(11));
            //uint sc = (uint) ((Convert.ToUInt32(255) << 24) + (Convert.ToUInt32(250) << 16) + (Convert.ToUInt32(70) << 8) + Convert.ToUInt32(48)); 
            //uint tc = (uint) ((Convert.ToUInt32(255) << 24) + (Convert.ToUInt32(233) << 16) + (Convert.ToUInt32(50) << 8) + Convert.ToUInt32(73)); 
            //uint foc =(uint) ((Convert.ToUInt32(255) << 24) + (Convert.ToUInt32(196) << 16) + (Convert.ToUInt32(54) << 8) + Convert.ToUInt32(87)); 
            //uint fic =(uint) ((Convert.ToUInt32(255) << 24) + (Convert.ToUInt32(171) << 16) + (Convert.ToUInt32(55) << 8) + Convert.ToUInt32(87));
            //uint bac = (uint)((Convert.ToUInt32(255) << 24) + (Convert.ToUInt32(0) << 16) + (Convert.ToUInt32(255) << 8) + Convert.ToUInt32(0));
            for (int ix = 0; ix < width; ix++)
            {
                for (int iy = 0; iy < height; iy++) 
                {
                    int i = width * iy + ix;
                    uint grad = Convert.ToUInt32(255 - ((maxValueMap - mapInTime[ix, iy]) / maxValueMap - minValueMap) * 255); //příliš malé rozdíly
                    switch (mapInTime[ix, iy])//colours cs^2 = 1/3 -> 0:0.577 
                    {
                        // spatne barvy << je byte posuv | RGB
                        case -1:
                            //pixels[i] = (uint)((Convert.ToUInt32(255) << 24) + (0 << 16) + (0 << 8));
                            pixels[i] = zc; //wall colour (pure black) //works for Density
                            break;
                        //case > 0.577:
                        //    pixels[i] = (uint)((0 << 24) + (0 << 16) + ((Convert.ToUInt32(255) << 8))); //error colour (pure blue) // for speed
                        //   break;

                        //case double n when (n > 0 && n < maxValueMap / 5):
                        //    pixels[i] = fc;
                        //    break;
                        //case double n when (n > maxValueMap / 5 && n < 2 * maxValueMap / 5):
                        //    pixels[i] = sc;
                        //    break;
                        //case double n when (n > 2 * maxValueMap / 5 && n < 3 * maxValueMap / 5):
                        //    pixels[i] = tc;
                        //    break;
                        //case double n when (n > 3 * maxValueMap / 5 && n < 4 * maxValueMap / 5):
                        //    pixels[i] = foc;
                        //    break;
                        //case double n when (n > 4 * maxValueMap && n < maxValueMap):
                        //    pixels[i] = fic;
                        //    break;
                        default:
                            pixels[i] = (uint)((Convert.ToUInt32(255) << 24) + (Convert.ToUInt32(255) << 16) + (Convert.ToUInt32(grad) << 8) + Convert.ToUInt32(grad)); ;
                            break;
                    }
                        bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
                        Heatmap.Source = bitmap;                    
                }
            }
        }
    }

    //    try 
    //    {
    //        bitmap.Lock();
    //        unsafe 
    //        {

    //        }
    //        bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
    //    }
    //    finally 
    //    {
    //        bitmap.Unlock(); 
    //    }
    //               Heatmap.Source = bitmap;
    //}

    //public class Output() : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler PropertyChanged;
    //    int selectedTime = 0;
    //    public int SelectedTime
    //    { get => selectedTime; set => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedTime")); }
    //    private double[,,] speedMap;
    //    public double[,,] SpeedMap { get => speedMap; set => speedMap = value; }
    //    public Lattice[,,] LatticesOutput { get; set; }

    //        public WriteableBitmap Heatmap
    //        {
    //            get 
    //            {
    //                int width = 500;
    //                int height = 500;
    //                WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
    //                uint[] pixels = new uint[width * height];
    //                 for (int ix = 0; ix < width; ix++)
    //                 {
    //                     for (int iy = 0; iy < height; iy++)
    //                    {
    //                        int i = width * iy + ix;
    //                        switch (speedMap[ix, iy, selectedTime]) //colours cs^2 = 1/3 -> 0:0.577 //ehm plotuju density
    //                        {
    //                        case -1:
    //                            pixels[i] = (uint)((255) + (0) + (0)); //wall colour (pure blue)
    //                            break;
    //                        case > 0.577:
    //                            pixels[i] = (uint)((0) + (0) + (255)); //error colour (pure green)
    //                            break;
    //                        case double n when (n > 0 && n < 0.115):
    //                            pixels[i] = (uint)((251) + (99) + (11));
    //                            break;
    //                        case double n when (n > 0.115 && n < 0.231):
    //                            pixels[i] = (uint)((250) + (70) + (48));
    //                            break;
    //                        case double n when (n > 0.231 && n < 0.346):
    //                            pixels[i] = (uint)((233) + (50) + (73));
    //                            break;
    //                        case double n when (n > 0.346 && n < 0.462):
    //                            pixels[i] = (uint)((196) + (54) + (87));
    //                            break;
    //                        case double n when (n > 0.462 && n < 0.577):
    //                            pixels[i] = (uint)((171) + (55) + (87));
    //                            break;
    //                         }                                          
    //                     }
    //                 }
    //            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
    //            return bitmap;
    //        }
    //        }
    //}
}
