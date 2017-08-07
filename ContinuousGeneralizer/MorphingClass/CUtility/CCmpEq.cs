using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MorphingClass.CCorrepondObjects;
using MorphingClass.CGeometry;
using MorphingClass.CGeneralizationMethods;

namespace MorphingClass.CUtility
{

    public class CCmpEqCpgPairIncr : EqualityComparer<CValPairIncr<CPolygon>>
    {
        //public static CCmpCoordDbl_VerySmall sComparer = new CCmpCoordDbl_VerySmall();

        public override bool Equals(CValPairIncr<CPolygon> x, CValPairIncr<CPolygon> y)
        {
            if (x.val1.GID == y.val1.GID && x.val2.GID == y.val2.GID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode(CValPairIncr<CPolygon> x)
        {
            return Tuple.Create(x.val1.GID, x.val2.GID).GetHashCode();
        }
    }

    public class CCmpEqCptXY : EqualityComparer<CPoint>
    {
        //public static CCmpCoordDbl_VerySmall sComparer = new CCmpCoordDbl_VerySmall();

        public override bool Equals(CPoint x, CPoint y)
        {
            if (CCmpMethods.CmpCptYX(x, y) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode(CPoint cpt)
        {
            var x = Convert.ToInt64(cpt.X * CConstants.dblRationVerySmallFclipper);
            var y = Convert.ToInt64(cpt.Y * CConstants.dblRationVerySmallFclipper);
            return Tuple.Create(x, y).GetHashCode();
        }
    }

    public class CCmpEqCEdgeCoord : EqualityComparer<CEdge>
    {
        //public static CCmpCoordDbl_VerySmall sComparer = new CCmpCoordDbl_VerySmall();

        public override bool Equals(CEdge x, CEdge y)
        {
            if (CCmpMethods.CmpCEdgeCoord(x, y, true) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode(CEdge cedge)
        {
            CPoint frcpt;
            CPoint tocpt;
            CCmpMethods.GetCpts(cedge, true, out frcpt, out tocpt);

            var x1 = Convert.ToInt64(frcpt.X * CConstants.dblRationVerySmallFclipper);
            var y1 = Convert.ToInt64(frcpt.Y * CConstants.dblRationVerySmallFclipper);
            var x2 = Convert.ToInt64(tocpt.X * CConstants.dblRationVerySmallFclipper);
            var y2 = Convert.ToInt64(tocpt.Y * CConstants.dblRationVerySmallFclipper);
            return Tuple.Create(x1, y1, x2, y2).GetHashCode();
        }
    }
}
