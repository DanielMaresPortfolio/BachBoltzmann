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

namespace BachBoltyman
{
    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class Results : Window, INotifyPropertyChanged
    {
        public Results(double[,,] speedMap_,double[,,] densityMap_ ,int[] alltime_)
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

            DrawOutput(SpeedMap,TimeSelect.SelectedIndex);
        }
        public double[,,] SpeedMap { get; set; }
        public double[,,] DensityMap { get; set; }
        public int[] AllTime
        {
            get;set;
        }
        public void DrawOutput(double[,,] map, int index) //vymyslet dynamickej resize / nebo prostě to po výsledcích dát z lattice nastavit  
        {
                Heatmap.Width = SpeedMap.GetLength(0);
                Heatmap.Height = SpeedMap.GetLength(1);
                int width = Convert.ToInt32(Heatmap.Width);
                int height = Convert.ToInt32(Heatmap.Height);
                WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
                            uint[] pixels = new uint[width * height];
                           for (int ix = 0; ix < width; ix++)
                            {
                                for (int iy = 0; iy < height; iy++) //nastavit velikost podle vstupu!
                                {
                                   int i = width * iy + ix;
                                    switch (map[ix, iy, index])//colours cs^2 = 1/3 -> 0:0.577 
                                    {
                                             // spatne barvy << je byte posuv
                                             // proc furt nic neukazuje? Fak v koncich
                                             case -1:
                                                pixels[i] = (uint)((255 >> 24) + (0>>16) + (0>>8)); //wall colour (pure blue)
                                            break;
                                             case > 0.577:
                                                pixels[i] = (uint)((0 >> 24) + (0 >> 16) + (255 >> 8)); //error colour (pure green)
                                            break;
                                             case double n when (n > 0 && n < 0.115):
                                                pixels[i] = (uint)((251 >> 24) + (99 >> 16) + (11 >> 8));
                                            break;
                                            case double n when (n > 0.115 && n < 0.231):
                                                pixels[i] = (uint)((250 >> 24) + (70 >> 16) + (48 >> 8));
                                            break;
                                            case double n when (n > 0.231 && n < 0.346):
                                                pixels[i] = (uint)((233 >> 24) + (50 >> 16) + (73 >> 8));
                                            break;
                                            case double n when (n > 0.346 && n < 0.462):
                                                pixels[i] = (uint)((196 >> 24) + (54 >> 16) + (87 >> 8));
                                            break;
                                            case double n when (n > 0.462 && n < 0.577):
                                                pixels[i] = (uint)((171 >> 24) + (55 >> 16) + (87 >> 8));
                                            break;
                                            default:
                                                pixels[i] = (uint)((0 >> 24) + (0 >>16) + (255 >> 8)); //error colour (pure green)
                                            break;
                                    }                                          
                                }
                            }
                       bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
                       Heatmap.Source = bitmap;
        }

    }
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
