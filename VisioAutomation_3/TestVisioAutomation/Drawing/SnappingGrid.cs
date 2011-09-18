using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisioAutomation.Extensions;
using IVisio = Microsoft.Office.Interop.Visio;
using VA = VisioAutomation;

namespace TestVisioAutomation
{
    [TestClass]
    public class SnappingGraidTests : VisioAutomationTest
    {

        [TestMethod]
        public void Snap1()
        {
            double delta = 0.000000001;

            var g1 = new VA.Drawing.SnappingGrid(1.0, 1.0);
            
           AssertSnap(0.0, 0.0, g1, 0.0, 0.0, delta);
           AssertSnap(0.0, 0.0, g1, 0.3,0.3, delta);
           AssertSnap(0.0, 0.0, g1, 0.49999, 0.49999, delta);
           AssertSnap(1.0, 1.0, g1, 0.5, 0.5, delta);
           AssertSnap(1.0, 1.0, g1, 0.500001, 0.500001, delta);
           AssertSnap(1.0, 1.0, g1, 1.0, 1.0, delta);
           AssertSnap(1.0, 1.0, g1, 1.3,1.3, delta);
           AssertSnap(1.0, 1.0, g1, 1.49999, 1.49999, delta);
           AssertSnap(2.0, 2.0, g1, 1.5, 1.5, delta);
           AssertSnap(2.0, 2.0, g1, 1.500001, 1.500001, delta);

            var g2 = new VA.Drawing.SnappingGrid(1.0,0.3);

            AssertSnap(0.0, 0.0, g2, 0.0, 0.0, delta);
            AssertSnap(0.0, 0.0, g2, 0.3, 0.1, delta);
            AssertSnap(0.0, 0.0, g2, 0.49999, 0.149, delta);
            AssertSnap(1.0, 0.3, g2, 0.5, 0.3, delta);
            AssertSnap(1.0, 0.3, g2, 0.500001, 0.30001, delta);
        }

        private void AssertSnap(double ex, double ey, VA.Drawing.SnappingGrid g1, double ix, double iy, double delta)
        {
            AssertX.AreEqual(ex, ey, g1.Snap(ix, iy), delta);            
        }

    }
}