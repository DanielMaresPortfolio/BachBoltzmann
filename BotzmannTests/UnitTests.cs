using BachBoltzman;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BotzmannTests
{
    [TestClass]
    public partial class UnitTest1
    {
        [TestMethod]
        public void TestingSteaming()
        {
            //Arrenge
            int sizeX = 5;
            int sizeY = 2;
            double viscosity = 0.1;
            Lattice lattice = new Lattice(sizeX, sizeY, viscosity);

            double[,] inicialDensity = new double[sizeX, sizeY];
            double[,] expectedDensity = new double[sizeX, sizeY];
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    inicialDensity[x, y] = x * 10 + y;
                }
            }

            //Act
            Lattice.Stream();

            //Assert
            Assert.AreEqual(expectedDensity, inicialDensity);
        }
        [TestMethod]
        public void TestingCollision()
        {
            //Arrenge
            int sizeX = 5;
            int sizeY = 5;
            double viscosity = 0.1;
            Lattice lattice = new Lattice(sizeX, sizeY, viscosity);
            double[,,] EqDFs = new double[sizeX, sizeY, 9];
            double[] EqDF = new double[9];
            double biggestDiference = 0;

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    EqDF = Lattice.Lattices[x, y].Equilibrium();
                }
            }
            //Act
            for (int k = 0; k < 100; k++)
            {
                Lattice.CollideBGK();
            }

            for (int x = 0; x < sizeX; x++)  //counting difference
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        biggestDiference = Lattice.Lattices[x, y].DF[i] - EqDFs[x, y, i];
                    }
                }
            }
            //Assert
            Assert.IsTrue(biggestDiference < 0.0001);
        }
        //[TestMethod]
        //public void TestingBounceBack()
        //{
        //    //Assert;
        //}

        // Use the UITestMethod attribute for tests that need to run on the UI thread. //Theese can test UI of app, 
        //[UITestMethod]
        //public void TestMethod2()
        //{
        //}
    }
}
