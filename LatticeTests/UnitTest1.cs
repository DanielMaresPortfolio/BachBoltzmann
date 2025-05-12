using BachBoltzman;

namespace LatticeTests
{
    [TestClass]
    public class Testing
    {
        [TestMethod]
        public void TestingSpeeds()
        {
            int sizeX = 10;
            int sizeY = 5;
            double[,] startingValues = new double[10, 5];

            Lattice lattice = new Lattice(sizeX,sizeY,0.1);
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    startingValues[i, j] = 10 * sizeX + 100 * sizeY;
                }
            }
        }
        [TestMethod]
        public void TestingCollisions()
        {

        }
    }
}