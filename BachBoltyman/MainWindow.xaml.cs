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
        private void Testing(object sender, RoutedEventArgs e)
        {
            int timeCykle = 10000; //this says how many times whole lattice will be simulated
            int timeSnap = 1000;  //this says whitch data will be saved(every n-th) 
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
                    for (int x =-1;x<2; x++) 
                    {
                        for (int y = -1; y < 2; y++)
                        {
                            CustomLayout[(int)currentpoint.X+x, (int)currentpoint.Y+y] = true;
                        }
                    }
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
            int timeCykle = 500; //this says how many times whole lattice will be simulated
            int timeSnap = 50;  //this says whitch data will be saved(every n-th) 
            int sx = (int)InpIm.Width;
            int sy = (int)InpIm.Height;
            //
            bool[,] layotWE = new bool[sx+2,sy+2]; //Přidání okrajových podmínek     
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
                        layotWE[x, y] =CustomLayout[x-1, y-1];
                    }
                }
            }
            //
            InicLayout Layout = new InicLayout(sx, sy);
            Layout.MyLayout = layotWE;
            Lattice lattice = new Lattice(sx+2, sy+2, 0.1);
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
    }
}