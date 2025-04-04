﻿using System.Text;
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
            int timeCykle = 1;
            int timeSnap = 1;
            int[] timeScale = new int[timeCykle/timeSnap];
            InicLayout layout = new InicLayout();
            Lattice lattice = new Lattice(layout.SizeX, layout.SizeY, 0.1);
            lattice.Run(1,1,0.1,0.1,0.1,layout.TestingLayout);
            //
            for (int i = 0; i < timeCykle / timeSnap; i++)
            {
                timeScale[i] = i * timeSnap;
            }
            Results results = new Results(Lattice.OutputSpeeds, Lattice.OutputDensity, timeScale);
            //results.DensityMap = Lattice.OutputDensity;
            this.Visibility = Visibility.Collapsed;
            results.Show();
        }
    }
}