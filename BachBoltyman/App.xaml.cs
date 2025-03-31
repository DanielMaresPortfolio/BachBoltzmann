﻿using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;
using System.Xaml;

namespace BachBoltzman
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

    }
    public class InicLayout
    {
        private int sx = 500;
        private int sy = 100;
        public int SizeX { get => sx; set => sx = value; }
        public int SizeY { get => sy; set => sy = value; }
        public bool[,] MyLayout { get; set; }
        public bool[,] TestingLayout
        {
            get {
                int px = 200;
                int py = 50;
                int r = 20;
                bool[,] testingLayout = new bool[sx, sy];
                for (int x = 0; x < sx; x++)
                {
                    for (int y = 0; y < sy; y++)
                    {
                        testingLayout[x, y] = false;
                        if ((x - px) * (x - px) + (y - py) * (y - py) < r * r || y == 0 || y == py)
                        {
                            testingLayout[x, y] = true;
                        }
                    }
                }
                return testingLayout;
            }
        }
    }
    public class D2Q9
    {
        private static D2Q9 instance;
        public static D2Q9 Instance //Singleton (nejspspíše předělám na dědičnost z abstrakt třídy)
        {
            get
            {
                if (instance == null)
                    instance = new D2Q9();

                return instance;
            }
        }
        int d = 2;
        int q = 9;
        double cs = 1.0 / 3;
        double[] wi = [4.0 / 9, 1.0 / 9, 1.0 / 9, 1.0 / 9, 1.0 / 36, 1.0 / 36, 1.0 / 36, 1.0 / 36, 1.0 / 36];
        int[] cx = [0, 1, 0, -1, 0, 1, -1, -1, 1];
        int[] cy = [0, 0, 1, 0, -1, 1, 1, -1, -1];
        public double SoundSpeedPowerTwo
        {
            get => cs;
            set { }
        }
        public double[] WeightsOfEDFs
        {
            get => wi;
            set { }
        }
        public int Dimensions
        {
            get => d;
            set { }
        }
        public int NumberOfSpeeds
        {
            get => q;
            set { }
        }
        public int[] InicialSpeedX
        {
            get => cx;
            set { }
        }
        public int[] InicialSpeedY
        {
            get => cy;
            set { }
        }
    }
    class Lattice
    {
        private static D2Q9 d2Q9 = D2Q9.Instance;
        private static Lattice[,] Lattices;
        private static Lattice[,] Latices_post;
        public static double[,,] OutputDensity;
        public static double[,,] OutputSpeeds;
        public Lattice(int sizeX, int sizeY, double viscosity)
        {
            double vis = viscosity;

            Lattices = new Lattice[sizeX, sizeY];
            Latices_post = new Lattice[sizeX, sizeY];
        }
        public Lattice()
        {
        }
        //
        private double[] f = new double[d2Q9.NumberOfSpeeds];
        private double[] f_post = new double[d2Q9.NumberOfSpeeds];
        private double vis;
        public double Viskozity { get => vis; set => vis = value; }
        public double RelaxTime
        {
            get
            {
                return Viskozity / (d2Q9.SoundSpeedPowerTwo) + 0.5;
            }
        }
        public double[] DF
        {
            get => f;
            set => f = value;
        }
        private bool wall = false;
        public bool IsWall
        {
            get => wall;
            set => wall = value;
        }
        private bool movingWall = false;
        public bool IsMovingWall
        {
            get => movingWall;
            set => movingWall = value;
        }
        private bool pressureOutFlow;
        public bool IsPressureOutFlow
        {
            get => pressureOutFlow;
            set => pressureOutFlow = value;
        }
        double density;
        public double Density
        {
            get
            {
                if (wall == false)
                {
                    double tempSumF = 0;
                    for (int i = 0; i < d2Q9.NumberOfSpeeds; i++)
                    {
                        tempSumF += f[i];
                    }
                    density = tempSumF;
                }
                else
                {
                    density = -1;
                }

                return density;
            }
        }
        public double SpeedInX
        {
            get
            {
                double ux = 0;
                switch (d2Q9.NumberOfSpeeds)
                {
                    case 9:
                        ux = (f[1] + f[5] + f[8] - f[3] - f[6] - f[2]) / Density;
                        break;
                }

                return ux;
            }
        }
        public double SpeedInY
        {
            get
            {
                double uy = 0;
                switch (d2Q9.NumberOfSpeeds)
                {
                    case 9:
                        uy = (f[5] + f[6] + f[2] - f[7] - f[8] - f[4]) / Density;
                        break;
                }

                return uy;
            }
        }
        //
        public double[] Equilibrium()
        {
            double[] v = new double[d2Q9.NumberOfSpeeds];
            double[] cu = new double[d2Q9.NumberOfSpeeds];
            for (int i = 0; i < d2Q9.InicialSpeedX.Length; i++)
            {
                cu[i] = d2Q9.InicialSpeedX[i] * this.SpeedInX + d2Q9.InicialSpeedY[i] * this.SpeedInY;
                v[i] = d2Q9.WeightsOfEDFs[i] * this.Density * (1 + 3 * cu[i] + 4.5 * cu[i] * cu[i] - 1.5 * (this.SpeedInX * this.SpeedInX + this.SpeedInY * this.SpeedInY));
            }
            return v;
        }
        public double[] Equilibrium(double inSpeedInX, double inSpeedInY)
        {
            double[] v = new double[d2Q9.NumberOfSpeeds];
            double[] cu = new double[d2Q9.NumberOfSpeeds];
            double inicDensity = 1;
            for (int i = 0; i < d2Q9.InicialSpeedX.Length; i++)
            {
                cu[i] = d2Q9.InicialSpeedX[i] * inSpeedInX + d2Q9.InicialSpeedY[i] * inSpeedInY;
                v[i] = d2Q9.WeightsOfEDFs[i] * inicDensity * (1 + 3 * cu[i] + 4.5 * cu[i] * cu[i] - 1.5 * (inSpeedInX * inSpeedInX + inSpeedInY * inSpeedInY));
            }
            return v;
        }

        static void BounceBack(int px, int py)
        {
                if (Lattices[px - 1, py].IsWall) //left
                {
                    Lattices[px, py].f[8] = Lattices[px, py].f_post[6];
                    Lattice.Lattices[px, py].f[5] = Lattice.Lattices[px, py].f_post[7];
                    Lattice.Lattices[px, py].f[1] = Lattice.Lattices[px, py].f_post[3];
                }
                if (Lattice.Lattices[px, py + 1].IsWall) //up
                {
                    Lattice.Lattices[px, py].f[8] = Lattice.Lattices[px, py].f_post[6];
                    Lattice.Lattices[px, py].f[5] = Lattice.Lattices[px, py].f_post[7];
                    Lattice.Lattices[px, py].f[1] = Lattice.Lattices[px, py].f_post[3];
                }
                if (Lattice.Lattices[px + 1, py].IsWall) //right
                {
                    Lattice.Lattices[px, py].f[8] = Lattice.Lattices[px, py].f_post[6];
                    Lattice.Lattices[px, py].f[5] = Lattice.Lattices[px, py].f_post[7];
                    Lattice.Lattices[px, py].f[1] = Lattice.Lattices[px, py].f_post[3];
                }
                if (Lattice.Lattices[px, py - 1].IsWall) //down
                {
                    Lattice.Lattices[px, py].f[8] = Lattice.Lattices[px, py].f_post[6];
                    Lattice.Lattices[px, py].f[5] = Lattice.Lattices[px, py].f_post[7];
                    Lattice.Lattices[px, py].f[1] = Lattice.Lattices[px, py].f_post[3];
                }

                //checking moving wall, I should make it changeble
                double densityWall = 1;
                double uWallX = 0.1;
                if (Lattice.Lattices[px - 1, py].IsMovingWall) //right
                {
                    Lattice.Lattices[px, py].f[8] = Lattice.Lattices[px, py].f_post[6] - 2 * d2Q9.WeightsOfEDFs[3] * densityWall * d2Q9.InicialSpeedX[6] * uWallX / d2Q9.SoundSpeedPowerTwo;
                    Lattice.Lattices[px, py].f[5] = Lattice.Lattices[px, py].f_post[7] - 2 * d2Q9.WeightsOfEDFs[3] * densityWall * d2Q9.InicialSpeedX[7] * uWallX / d2Q9.SoundSpeedPowerTwo;
                    Lattice.Lattices[px, py].f[1] = Lattice.Lattices[px, py].f_post[3] - 2 * d2Q9.WeightsOfEDFs[3] * densityWall * d2Q9.InicialSpeedX[3] * uWallX / d2Q9.SoundSpeedPowerTwo;
                    // for first tests right wall is enough 
                }
                //pressure outlow
                if (Lattice.Lattices[px + 1, py].IsPressureOutFlow)
                {
                    Lattice.Lattices[px, py].f[1] = Lattice.Lattices[px, py - d2Q9.InicialSpeedY[3]].f_post[1];
                    Lattice.Lattices[px, py].f[5] = Lattice.Lattices[px, py - d2Q9.InicialSpeedY[7]].f_post[8];
                    Lattice.Lattices[px, py].f[8] = Lattice.Lattices[px, py - d2Q9.InicialSpeedY[6]].f_post[5];
                }
        }
        static void InitializeLayout(bool[,] layout)
        {
            for (int ix = 0; ix < Lattices.GetLength(0); ix++)
            {
                for (int iy = 0; iy < Lattices.GetLength(1); iy++)
                {
                    Lattices[ix, iy] = new Lattice();
                    Lattices[ix, iy].IsWall = layout[ix, iy];
                    if (ix == Lattices.GetLength(0))
                    {
                        Lattices[ix, iy].IsMovingWall = true;
                    }
                    if (ix == 0)
                    {
                        Lattices[ix, iy].IsPressureOutFlow = true;
                    }
                }
            }
        }
        static void InitializeEquilibrium(double inicialSpeedX, double inicialSpeedY, double inicialViskozity)
        {
            foreach (Lattice lattice in Lattices)
            {
                lattice.Viskozity = inicialViskozity;
                lattice.f = lattice.Equilibrium(inicialSpeedX, inicialSpeedY);
            }
        }
        static void Stream()
        {
            int x = Lattices.GetLength(0);
            int y = Lattices.GetLength(1);

            int j;
            int i;
            int jd;
            int id;
            int k;
            for (j = 0; j < x; j++)
            {
                for (i = 0; i < y; i++)
                {
                    for (k = 0; k < d2Q9.NumberOfSpeeds; k++)
                    {
                        //Reseno Upwindem
                        jd = j - d2Q9.InicialSpeedY[k];
                        id = i - d2Q9.InicialSpeedX[k];
                        if (jd >= 0 && jd < x && id >= 0 && id <= y)
                        {
                            Lattices[j, i].f[k] = Lattices[j, i].f_post[k];
                            if (j!=0 && i !=0 && j > x && i > y) 
                            {
                                BounceBack(j, i);
                            }
                        }
                    }
                }
            }
        }
        static void CollideBGK() //pokud budou jine tau tak predelat na res. v jednom uzlu
        {
            int x = Lattices.GetLength(0);
            int y = Lattices.GetLength(1);

            for (int ix = 0; ix < x; ix++)
            {
                for (int iy = 0; iy < y; iy++)
                {
                    for (int k = 0; k < d2Q9.NumberOfSpeeds; k++)
                    {
                        double omega = (Lattices[ix, iy].f[k] - Lattices[ix, iy].Equilibrium()[k]) / Lattices[ix, iy].RelaxTime;
                        Lattices[ix, iy].f_post[k] = Lattices[ix, iy].f[k] - omega;
                    }
                }
            }
        }
        public void Run(int timeCykle, int timeSnap, double inicialSpeedInX, double inicialSpeedInY, double inicialViskozkozity, bool[,] layout)
        {
            int x = Lattices.GetLength(0);
            int y = Lattices.GetLength(1);

            OutputSpeeds = new double[x,y,Convert.ToInt32(timeCykle/timeSnap)];
            OutputDensity = new double[x, y, Convert.ToInt32(timeCykle / timeSnap)];
            InitializeLayout(layout);
            InitializeEquilibrium(inicialSpeedInX, inicialSpeedInY,inicialViskozkozity);
            int i = 0;
            for (int t = 0; t < timeCykle-1; t++)
            {
                if (t % timeSnap == 0)
                {
                    for (int px = 0; px < x; px++)
                    {
                        for (int py = 0; py < y; py++)
                        {
                            OutputSpeeds[px, py, i] = Math.Sqrt(Math.Pow(Lattices[px, py].SpeedInX, 2) + Math.Pow(Lattices[px, py].SpeedInY, 2));
                            OutputDensity[px, py, i] = Lattices[px, py].Density;
                        }
                    }
                i++;
                }
                CollideBGK();
                Stream();
            }
        }
        //private static double[,,] outputSpeed;
        //public static double[,,] OutputSpeed {get => outputSpeed; set => outputSpeed = value; }
        //private static double[,,] outputDensity;
        //public static double[,,] OutputDensity { get => outputDensity; set => outputDensity = value; }
    }
}