using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MorphingClass.CUtility
{
    public class CQuadraticEquation
    {
        public double dbla{ get; set; }
        public double dblb{ get; set; }
        public double dblc{ get; set; }
        public double dblDelta{ get; set; }

        public CQuadraticEquation(double pdbla, double pdblb, double pdblc)
        {
            this.dbla = pdbla;
            this.dblb = pdblb;
            this.dblc = pdblc;

            this.dblDelta = pdblb * pdblb - 4 * pdbla * pdblc;
        }

        public CQuadraticEquation(double pdbla, double pdblb, double pdblc, double pdblfourA)
        {
            this.dbla = pdbla;
            this.dblb = pdblb;
            this.dblc = pdblc;

            this.dblDelta = pdblb * pdblb - pdblfourA * pdblc;  //b^2 - 4ac
        }


        public double QuadraticEquation(double dblX)
        {
            return (this.dbla * dblX * dblX + this.dblb * dblX + this.dblc);
        }

    }
}
