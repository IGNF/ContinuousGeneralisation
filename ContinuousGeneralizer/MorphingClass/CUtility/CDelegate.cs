using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MorphingClass.CGeometry;

namespace MorphingClass.CUtility
{
    public class CDelegate
    {

        Func<CPoint, double> CPoint_X = delegate(CPoint cpt)
        { return cpt.X; };

        Func<CPoint, double> CPoint_Y = delegate(CPoint cpt)
        { return cpt.Y; }; 


    }

    //delegate double CPoint_X(CPoint cpt)
}
