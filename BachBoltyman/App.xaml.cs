using System.Configuration;
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
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public bool[,] MyLayout { get; set; }
        public bool[,] TestingLayout 
        {
            get {
                int sx = 500;
                int sy = 100;
                int px = 200;
                int py = 50;
                int r = 2;
                bool[,] testingLayout = new bool[sx, sy];
                for (int x =0; x<sx;x++) 
                {
                    for (int y = 0; y < sy; y++)
                    {
                        if ((sx - x) * (sx - x) + (sy - y) * (sy - y) < r * r)
                        {
                            testingLayout[x, y] = true;
                        }
                        else 
                        {
                            testingLayout[x, y] = false;
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
          double cs = 1/3;
          double[] wi = [4/9,1/9,1/36];
          int[] cx = [0, 1, 0, -1, 0, 1, -1, -1, 1];
          int[] cy = [ 0, 0, 1, 0, -1, 1, 1, -1, -1 ];
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
        private static int x; 
        private static int y;
        private static double vis;

        public Lattice(int sizeX, int sizeY, double viscosity)
        {
           int x = sizeX;
           int y = sizeY;
            double vis = viscosity;
        }
         public static Lattice[,] Lattices = new Lattice[x,y];
         public static Lattice[,] Latices_post = new Lattice[x,y];
        //
        private double[] f = new double[d2Q9.NumberOfSpeeds] ;
        private double[] f_post = new double[d2Q9.NumberOfSpeeds];
        public double RelaxTime 
        {
            get 
            {
                return vis/(d2Q9.SoundSpeedPowerTwo)+0.5;
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
        private bool movingWall=false;
        public bool IsMovingWall
        {
            get => movingWall;
            set => movingWall = value;
        }
        public double Density
        {
            get
            {
                double density = 0;
                for (int i =0; i< d2Q9.NumberOfSpeeds; i++) 
                {
                    density += f[i];
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
        public double[] Equilibrium 
        {
            get
            {
                double[] v = new double[d2Q9.NumberOfSpeeds];
                double[] cu = new double[d2Q9.NumberOfSpeeds] ;
                for(int i = 0; i < d2Q9.InicialSpeedX.Length; i++ )
                {
                    cu[i] = d2Q9.InicialSpeedX[i] * SpeedInX + d2Q9.InicialSpeedY[i] * SpeedInY;
                    v[i] = d2Q9.WeightsOfEDFs[i] * Density * (1 + 3 * cu[i] + 4.5 * cu[i] * cu[i] -1.5*(SpeedInX*SpeedInX+SpeedInY*SpeedInY));
                }
                return v;
            }
        }
        //
        static void BounceBack (int px, int py)
        {
            if (Lattice.Lattices[px+1, py].IsWall) //left
            {
                Lattice.Lattices[px, py].f[8] = Lattice.Lattices[px, py].f_post[6];
                Lattice.Lattices[px, py].f[5] = Lattice.Lattices[px, py].f_post[7];
                Lattice.Lattices[px, py].f[1] = Lattice.Lattices[px, py].f_post[3];
            }
            if (Lattice.Lattices[px,py+1].IsWall) //up
            {
                Lattice.Lattices[px, py].f[8] = Lattice.Lattices[px, py].f_post[6];
                Lattice.Lattices[px, py].f[5] = Lattice.Lattices[px, py].f_post[7];
                Lattice.Lattices[px, py].f[1] = Lattice.Lattices[px, py].f_post[3];
            }
            if (Lattice.Lattices[px - 1, py].IsWall) //right
            {
                Lattice.Lattices[px, py].f[8] = Lattice.Lattices[px, py].f_post[6];
                Lattice.Lattices[px, py].f[5] = Lattice.Lattices[px, py].f_post[7];
                Lattice.Lattices[px, py].f[1] = Lattice.Lattices[px, py].f_post[3];
            }
            if (Lattice.Lattices[px, py-1].IsWall) //down
            {
                Lattice.Lattices[px, py].f[8] = Lattice.Lattices[px, py].f_post[6];
                Lattice.Lattices[px, py].f[5] = Lattice.Lattices[px, py].f_post[7];
                Lattice.Lattices[px, py].f[1] = Lattice.Lattices[px, py].f_post[3];
            }

            //checking moving wall
            if (Lattice.Lattices[px + 1, py].IsMovingWall) //left
            {
                Lattice.Lattices[px, py].f[8] = Lattice.Lattices[px, py].f_post[6];
                Lattice.Lattices[px, py].f[5] = Lattice.Lattices[px, py].f_post[7];
                Lattice.Lattices[px, py].f[1] = Lattice.Lattices[px, py].f_post[3];
            }
            if (Lattice.Lattices[px, py + 1].IsMovingWall) //up
            {
                Lattice.Lattices[px, py].f[8] = Lattice.Lattices[px, py].f_post[6];
                Lattice.Lattices[px, py].f[5] = Lattice.Lattices[px, py].f_post[7];
                Lattice.Lattices[px, py].f[1] = Lattice.Lattices[px, py].f_post[3];
            }
            if (Lattice.Lattices[px - 1, py].IsMovingWall) //right
            {
                Lattice.Lattices[px, py].f[8] = Lattice.Lattices[px, py].f_post[6];
                Lattice.Lattices[px, py].f[5] = Lattice.Lattices[px, py].f_post[7];
                Lattice.Lattices[px, py].f[1] = Lattice.Lattices[px, py].f_post[3];
            }
            if (Lattice.Lattices[px, py - 1].IsMovingWall) //down
            {
                Lattice.Lattices[px, py].f[8] = Lattice.Lattices[px, py].f_post[6];
                Lattice.Lattices[px, py].f[5] = Lattice.Lattices[px, py].f_post[7];
                Lattice.Lattices[px, py].f[1] = Lattice.Lattices[px, py].f_post[3];
            }
        }
        static void InitializeLayout(bool[,] layout) 
        {
            for (int ix = 0; ix < x; ix++)
            {
                for (int iy = 0; iy < y; iy++)
                {
                    Lattices[ix,iy].IsWall = layout[ix,iy];
                }
            }
        }
        static void InitializeEquilibrium(int RelaxTime)
        {
            foreach (Lattice lattice in Lattices) 
            {
                lattice.f = lattice.Equilibrium;
            }
        }
        static void Stream() 
        {
            int j;
            int i;
            int jd;
            int id;
            int k;
            for (j=0;j<=y;j++) 
            {
                for (i=0;i<=x;i++) 
                {
                    for (k=0;k<d2Q9.NumberOfSpeeds;k++) 
                    {
                        //Reseno Upwindem
                        jd = j - d2Q9.InicialSpeedY[k];
                        id = i - d2Q9.InicialSpeedX[k];
                        if (jd >= 0 && jd < x && id >= 0 && id <= y) 
                        {
                            Lattices[j,i].f[k]  = Lattices[j,i].f_post[k];
                        }
                    }
                }
            }
        }
        static void CollideBGK() //pokud budou jine tau tak predelat na res. v jednom uzlu
        {
            for(int ix = 0; ix < x; ix++)
            {
                for (int iy = 0; iy < y; iy++)
                {
                    for (int k = 0; k < d2Q9.NumberOfSpeeds; k++) 
                    {
                        double omega = (Lattices[ix, iy].f[k] - Lattices[ix, iy].Equilibrium[k])/ Lattices[ix,iy].RelaxTime;
                        Lattices[ix, iy].f_post[k] = Lattices[ix, iy].f[k] - omega;
                    }
                }
            }
        }
        static void Run(bool[,] layout, int timeCykle,int timeSnap) 
        {
            //ukoly ImgOutput https://nerdparadise.com/programming/csharpimageediting
            //Img input https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap?view=windowsdesktop-9.0&redirectedfrom=MSDN , https://learn.microsoft.com/en-us/dotnet/api/system.windows.uielement.mouserightbuttondown?view=windowsdesktop-9.0#system-windows-uielement-mouserightbuttondown
            double[,,] outputDensity = new double[x,y,Convert.ToInt32(timeCykle/timeSnap)];
 
            Lattice.InitializeLayout(layout);
            Lattice.InitializeEquilibrium(timeCykle);

            int i = 0;
            for (int t = 0; t < timeCykle; t++)
            {
                Lattice.CollideBGK();
                Lattice.Stream();
                if (t%timeSnap ==0) 
                {
                    for (int px = 0; px<x; px++) 
                    {
                        for (int py = 0; py<y; py++) 
                        {
                            outputDensity[px,py,i] =Lattice.Lattices[px,py].Density;
                            i++;
                        }
                    }
                }
            }
        }
    }
}