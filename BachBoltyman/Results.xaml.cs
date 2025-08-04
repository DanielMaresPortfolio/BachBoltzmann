using BachBoltzman;
using Microsoft.VisualBasic.ApplicationServices;
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
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class Results : Window, INotifyPropertyChanged
    {
        private Lattice lattice;
        public Results(int[] alltime_, ref Lattice lattice_)
        {
            lattice = lattice_;
            AllTime = alltime_;
            DataContext = this;
            InitializeComponent();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DrawOutput(TimeSelect.SelectedIndex);
        }
        public int[] AllTime
        {
            get; set;
        }
        private int inputX = 0;
        public int InputX
        {
            get => inputX;
            set => inputX = value;
        }
        private int inputY = 0;
        public int InputY
        {
            get => inputY;
            set => inputY = value;
        }

        public void DrawOutput(int index)
        {
            double v = Lattice.Lattices[1, 1].Viskozity; //abych se koukl do Lattices
            double[,,] map = new double[Lattice.Lattices.GetLength(0), Lattice.Lattices.GetLength(1), AllTime.GetLength(0)];
            string jmenoSouboru = null;

            if (Convert.ToBoolean(SpeedXMap.IsChecked))
            {
                map = Lattice.OutputSpeedsX;
                jmenoSouboru = "rychlostiX" + Convert.ToString(index) +".PNG";
            }
            if (Convert.ToBoolean(SpeedYMap.IsChecked))
            {
                map = Lattice.OutputSpeedsY;
                jmenoSouboru = "rychlostiY" + Convert.ToString(index) + ".PNG";
            }
            if (Convert.ToBoolean(SpeedsMap.IsChecked))
            {
                map = Lattice.OutputSpeeds;
                jmenoSouboru = "rychlostiCelkove" + Convert.ToString(index) + ".PNG";
            }
            if (Convert.ToBoolean(DensityMap.IsChecked)) 
            {
                map = Lattice.OutputDensity;
                jmenoSouboru = "hustota" + Convert.ToString(index) + ".PNG";
            }

            Heatmap.Width = Lattice.OutputDensity.GetLength(0);
            Heatmap.Height = Lattice.OutputDensity.GetLength(1);
            int width = Convert.ToInt32(Heatmap.Width);
            int height = Convert.ToInt32(Heatmap.Height);
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null); //zdroj https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap?view=windowsdesktop-9.0&redirectedfrom=MSDN
            uint[] pixels = new uint[width * height];
            double[,] mapInTime = new double[map.GetLength(0), map.GetLength(1)];

            double maxValueMap = 0;
            double minValueMap = 2;
            //

            for (int ix = 0; ix < width; ix++)
            {
                for (int iy = 0; iy < height; iy++)
                {
                    mapInTime[ix, iy] = map[ix, iy, index];
                    if (mapInTime[ix, iy] < minValueMap && mapInTime[ix, iy] >= 0)
                    {
                        minValueMap = mapInTime[ix, iy];
                    }
                    if (mapInTime[ix, iy] > maxValueMap && mapInTime[ix, iy] >= 0)
                    {
                        maxValueMap = mapInTime[ix, iy];
                    }
                }
            }
            MinMaxMap.Content = "max value =" + Convert.ToString(maxValueMap) + Environment.NewLine + ",min value =" + Convert.ToString(minValueMap);
            for (int ix = 0; ix < width; ix++)
            {
                for (int iy = 0; iy < height; iy++)
                {
                    int i = width * iy + ix;
                    uint grad = 0;
                    if (Math.Round((decimal)(maxValueMap - minValueMap), 5) > 0)
                    {
                        grad = Convert.ToUInt32((Math.Round((decimal)maxValueMap, 5) - Math.Round((decimal)mapInTime[ix, iy], 5)) / (Math.Round((decimal)maxValueMap, 5) - (Math.Round((decimal)minValueMap, 5))) * 255);
                    }

                    if (Lattice.Lattices[ix, iy].Density == -1)
                    {
                        pixels[i] = (uint)((Convert.ToUInt32(255) << 24) + (Convert.ToUInt32(0) << 16) + (Convert.ToUInt32(0) << 8) + Convert.ToUInt32(0)); ; //wall colour (pure black)
                    }
                    else
                    {
                        if (mapInTime[ix, iy] > 1.5) //density err, I should expand for speed (0.3 absolute max)
                        {
                            pixels[i] = (uint)((Convert.ToUInt32(255) << 24) + (Convert.ToUInt32(0) << 16) + (Convert.ToUInt32(255) << 8) + Convert.ToUInt32(0));
                        }
                        else
                        {
                            pixels[i] = (uint)((Convert.ToUInt32(255) << 24) + (Convert.ToUInt32(grad * 2 / 3) << 16) + (Convert.ToUInt32(120) << 8) + Convert.ToUInt32(grad * 1 / 3));
                        }
                    }
                    //switch (mapInTime[ix, iy])
                    //{
                    //    case -1:
                    //        pixels[i] = zc; //wall colour (pure black) //works for Density
                    //        break;
                    //    case > 1.5: //Err
                    //        pixels[i] = (uint)((Convert.ToUInt32(255) << 24) + (Convert.ToUInt32(0) << 16) + (Convert.ToUInt32(255) << 8) + Convert.ToUInt32(0)); //pure green
                    //        break;
                    //    default:
                    //       pixels[i] = (uint)((Convert.ToUInt32(255) << 24) + (Convert.ToUInt32(175) << 16) + (Convert.ToUInt32(grad*2/3) << 8) + Convert.ToUInt32(grad*1/3));
                    //        break;
                    //}
                    bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
                    Heatmap.Source = bitmap;

                    //ukladani obrazku
             
                    SavePNG(bitmap, jmenoSouboru);
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //F0.Content = Lattice.Lattices[inputX,inputY].DF[0]; //neukladam vsechny Lattice, moc velky
            //F1.Content = Lattice.Lattices[inputX, inputY].DF[1];
            //F2.Content = Lattice.Lattices[inputX, inputY].DF[2];
            //F3.Content = Lattice.Lattices[inputX, inputY].DF[3];
            //F4.Content = Lattice.Lattices[inputX, inputY].DF[4];
            //F5.Content = Lattice.Lattices[inputX, inputY].DF[5];
            //F6.Content = Lattice.Lattices[inputX, inputY].DF[6];
            //F7.Content = Lattice.Lattices[inputX, inputY].DF[7];
            //F8.Content = Lattice.Lattices[inputX, inputY].DF[8];

            Density.Content = "density =" + Convert.ToString(Lattice.OutputDensity[inputX, inputY, TimeSelect.SelectedIndex]);
            SpeedInX.Content = "speed in X =" + Lattice.OutputSpeedsX[inputX, inputY, TimeSelect.SelectedIndex];
            SpeedInY.Content = "speed in Y =" + Lattice.OutputSpeedsY[inputX, inputY, TimeSelect.SelectedIndex];
        }

        // Save the WriteableBitmap into a PNG file.
        public static void SavePNG(WriteableBitmap wbitmap,
            string filename)
        {
            // Save the bitmap into a file.
            using (FileStream stream =
                new FileStream(filename, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbitmap));
                encoder.Save(stream);
            }
        }
    }
}
