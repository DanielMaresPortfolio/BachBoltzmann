using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BachBoltzman;

namespace BachBoltyman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }
        private void Testing(object sender, RoutedEventArgs e)
        {
            int timeCykle = 100; //this says how many times whole lattice will be simulated
            int timeSnap = 1;  //this says whitch data will be saved(every n-th) 
            InicLayout layout = new InicLayout();
            Lattice lattice = new Lattice(layout.SizeX, layout.SizeY, 0.1);
            lattice.Run(timeCykle,timeSnap,0.1,0.1,0.1,layout.TestingLayout);
            int[] timeScale = new int[(timeCykle / timeSnap)+1];
            //
            for (int i = 0; i <= timeCykle / timeSnap; i++)
            {
                timeScale[i] = i * timeSnap;
            }
            Results results = new Results(Lattice.OutputSpeeds, Lattice.OutputDensity, timeScale);
            this.Visibility = Visibility.Collapsed;
            results.Show();
        }
    }
}