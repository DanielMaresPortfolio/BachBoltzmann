using System.Configuration;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Shapes;
using System.Xaml;
using Path = System.IO.Path;

namespace BachBoltzman
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
    }
    public class InicLayout
    {
        private int sx = 500; //500
        private int sy = 200; //200
        public int SizeX { get => sx; set => sx = value; }
        public int SizeY { get => sy; set => sy = value; }
        public bool[,] MyLayout { get; set; }
        public bool[,] TestingLayout
        {
            get {
                //int px = sx/2;
                //int py = sy/2;
                //int r = 10;
                //bool[,] testingLayout = new bool[sx, sy];
                //for (int x = 0; x < sx; x++)
                //{
                //    for (int y = 0; y < sy; y++)
                //    {
                //        testingLayout[x, y] = false;
                //        if ((((x - px) * (x - px)) + ((y - py) * (y - py))) < r * r || y == 0 || y == (sy - 1))
                //        {
                //            testingLayout[x, y] = true;
                //        }
                //    }
                //}
                bool[,] testingLayout = new bool[sx, sy];
                for (int x = 0; x < sx; x++)
                {
                    for (int y = 0; y < sy; y++)
                    {
                        testingLayout[x, y] = false;
                        if (y == 0 || y == (sy - 1))
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
        public static D2Q9 Instance //Singleton (Maybe shoul be remade to be abstrakt class, then added constants to that)
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
        double[] wi = { 4.0 / 9, 1.0 / 9, 1.0 / 9, 1.0 / 9, 1.0 / 9, 1.0 / 36, 1.0 / 36, 1.0 / 36, 1.0 / 36 };
        int[] cx = { 0, 1, 0, -1, 0, 1, -1, -1, 1 };
        int[] cy = { 0, 0, 1, 0, -1, 1, 1, -1, -1 };
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
    public class Lattice
    {
        private static D2Q9 d2Q9 = D2Q9.Instance;
        private static Lattice[,] Lattices;
        public static double[,,] OutputDensity;
        public static double[,,] OutputSpeeds;
        public Lattice(int sizeX, int sizeY, double viscosity) //added this because i had problems with inicializing Lattices to max lenght
        {
            double vis = viscosity;

            Lattices = new Lattice[sizeX, sizeY];
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
        public double[] Equilibrium(double inSpeedInX, double inSpeedInY) //overloading construktor for first inic, where you have to use inicial velocities, in top, you use allready present speeds
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
                    Lattices[px, py].f[5] = Lattices[px, py].f_post[7];
                    Lattices[px, py].f[1] = Lattices[px, py].f_post[3];
                }
                if (Lattices[px, py + 1].IsWall) //up
                {
                    Lattices[px, py].f[7] = Lattices[px, py].f_post[5];
                    Lattices[px, py].f[4] = Lattices[px, py].f_post[2];
                    Lattices[px, py].f[8] = Lattices[px, py].f_post[6];
                }
                if (Lattices[px + 1, py].IsWall) //right
                {
                    Lattices[px, py].f[6] = Lattices[px, py].f_post[8];
                    Lattices[px, py].f[3] = Lattices[px, py].f_post[1];
                    Lattices[px, py].f[7] = Lattices[px, py].f_post[5];
                }
                if (Lattices[px, py - 1].IsWall) //down
                {
                    Lattices[px, py].f[6] = Lattices[px, py].f_post[8];
                    Lattices[px, py].f[2] = Lattices[px, py].f_post[4];
                    Lattices[px, py].f[5] = Lattices[px, py].f_post[7];
                }
        }
        static void InflowAndOuflow(int px, int py, double densityWall,double uWallX) 
        {
            //double densityWall = 1;
            //double uWallX = 0.1;
            if (Lattices[px, py].IsMovingWall) //left
            {
                Lattices[px, py].f[8] = Lattices[px, py].f_post[6] - 2 * d2Q9.WeightsOfEDFs[3] * densityWall * d2Q9.InicialSpeedX[6] * uWallX / d2Q9.SoundSpeedPowerTwo;
                Lattices[px, py].f[5] = Lattices[px, py].f_post[7] - 2 * d2Q9.WeightsOfEDFs[3] * densityWall * d2Q9.InicialSpeedX[7] * uWallX / d2Q9.SoundSpeedPowerTwo;
                Lattices[px, py].f[1] = Lattices[px, py].f_post[3] - 2 * d2Q9.WeightsOfEDFs[3] * densityWall * d2Q9.InicialSpeedX[3] * uWallX / d2Q9.SoundSpeedPowerTwo;
                // for first tests right wall is enough 
            }
            //pressure outlow Zao 2.79 -2.82
            if (Lattices[px, py].IsPressureOutFlow)
            {
                //double uin = Math.Sqrt(d2Q9.SoundSpeedPowerTwo) - Math.Sqrt(d2Q9.SoundSpeedPowerTwo) / densityWall * (2*Lattices[px, py].f[3]+2*Lattices[px, py].f[6]+2*Lattices[px, py].f[7]+ Lattices[px, py].f[0]+ Lattices[px, py].f[2]+ Lattices[px, py].f[4]);
                //Lattices[px, py].f[1] = Lattices[px, py].Equilibrium()[1] + Lattices[px, py].f[3] - Lattices[px, py].Equilibrium()[3];
                //Lattices[px, py].f[8] = (Lattices[px,py].f[2] - Lattices[px, py].f[4] + 2 * Lattices[px, py].f[6] + Lattices[px, py].f[3] - Lattices[px, py].f[1]+uin *densityWall/Math.Sqrt(d2Q9.SoundSpeedPowerTwo))/2;
                //Lattices[px, py].f[5] = densityWall - (Lattices[px, py].f[0]+ Lattices[px, py].f[2] + Lattices[px, py].f[3] + Lattices[px, py].f[4] + Lattices[px, py].f[6] + Lattices[px, py].f[7] + Lattices[px, py].f[8]);

                double uin = Math.Sqrt(d2Q9.SoundSpeedPowerTwo) - Math.Sqrt(d2Q9.SoundSpeedPowerTwo) / densityWall * (2 * Lattices[px, py].f_post[3] + 2 * Lattices[px, py].f[6] + 2 * Lattices[px, py].f_post[7] + Lattices[px, py].f_post[0] + Lattices[px, py].f_post[2] + Lattices[px, py].f_post[4]);
                Lattices[px, py].f[3] = Lattices[px, py].Equilibrium()[3] + Lattices[px, py].f[1] - Lattices[px, py].Equilibrium()[1];
                Lattices[px, py].f[7] = (Lattices[px, py].f[2] - Lattices[px, py].f[4] + 2 * Lattices[px, py].f_post[8] + Lattices[px, py].f[1] - Lattices[px, py].f[3] + uin * densityWall / Math.Sqrt(d2Q9.SoundSpeedPowerTwo)) / 2;
                Lattices[px, py].f[6] = densityWall - (Lattices[px, py].f[0] + Lattices[px, py].f[2] + Lattices[px, py].f[1] + Lattices[px, py].f[4] + Lattices[px, py].f[5] + Lattices[px, py].f[8] + Lattices[px, py].f[7]);
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
                    if (ix == 0)
                    {
                        Lattices[ix, iy].IsMovingWall = true;
                    }
                    if (ix == Lattices.GetLength(0))
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
                if (lattice.IsWall == false) 
                {
                    lattice.Viskozity = inicialViskozity;
                    lattice.f = lattice.Equilibrium(inicialSpeedX, inicialSpeedY);
                }
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
                            if (jd >= 0 && jd < x && id >= 0 && id < y)
                            {
                                if (Lattices[jd,id].IsWall == false)
                                {
                                    Lattices[j, i].f[k] = Lattices[jd, id].f_post[k];
                                }
                                else 
                                {
                                    if (Lattices[j,i].IsWall == false && j !=0 && j !=x-1) 
                                    {
                                        BounceBack(j, i);
                                    }
                                }
                            }
                        }
                    if (j == 0 || j == x)
                    {
                        InflowAndOuflow(j, i, 1,0.1);
                    }
                }
            }
        }
        static void CollideBGK()
        {
            int x = Lattices.GetLength(0);
            int y = Lattices.GetLength(1);

            for (int ix = 0; ix < x; ix++)
            {
                for (int iy = 0; iy < y; iy++)
                {
                    for (int k = 0; k < d2Q9.NumberOfSpeeds; k++)
                    {
                        if (Lattices[ix,iy].IsWall == false) 
                        {
                            double omega = (Lattices[ix, iy].f[k] - Lattices[ix, iy].Equilibrium()[k]) / Lattices[ix, iy].RelaxTime;
                            Lattices[ix, iy].f_post[k] = Lattices[ix, iy].f[k] - omega;
                        }
                    }
                }
            }
        }
        public void Run(int timeCykle, int timeSnap, double inicialSpeedInX, double inicialSpeedInY, double inicialViskozkozity, bool[,] layout)
        {
            int x = Lattices.GetLength(0);
            int y = Lattices.GetLength(1);

            OutputSpeeds = new double[x,y,Convert.ToInt32((timeCykle / timeSnap) + 1)];
            OutputDensity = new double[x, y, Convert.ToInt32((timeCykle / timeSnap) +  1)];
            InitializeLayout(layout);
            InitializeEquilibrium(inicialSpeedInX, inicialSpeedInY,inicialViskozkozity);
            int i = 0;
            //string densityLine = null;
            //List<string> DensityText = new List<string>();
            for (int t = 0; t <= timeCykle; t++)
            {
                if (t % timeSnap == 0)
                {
                    for (int px = 0; px < x; px++)
                    {
                        for (int py = 0; py < y; py++)
                        {
                            OutputSpeeds[px, py, i] = Math.Sqrt(Math.Pow(Lattices[px, py].SpeedInX, 2) + Math.Pow(Lattices[px, py].SpeedInY, 2));
                            OutputDensity[px, py, i] = Lattices[px, py].Density;
                            //densityLine +=(" {0: F10}", Lattices[px, py].Density);
                        }
                       // DensityText.Add("---------------------");
                        //DensityText.Add(densityLine);
                    }
                i++;
                    //using (StreamWriter outputFile = new StreamWriter("C:\\Users\\mares\\source\\repos\\BachBoltyman\\BachBoltyman\\Density.txt")) //nefungovalo .\Density
                    //{
                    //    outputFile.WriteLine(Convert.ToString(i*timeSnap));
                    //    foreach (string line in DensityText) 
                    //    {
                    //        outputFile.WriteLine(line);
                    //    }
                    //}
                }
                CollideBGK();
                Stream();
            }
        }
    }
}