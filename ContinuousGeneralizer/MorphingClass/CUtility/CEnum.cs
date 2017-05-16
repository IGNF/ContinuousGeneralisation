using System;
using MorphingClass.CGeometry;
using System.Collections.Generic;
using SCG = System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace MorphingClass.CUtility
{
    public enum CEnumColor
    {
        white,
        gray,
        black
    }

    public enum CEnumScale
    {
        Larger,
        Smaller,        
        Single
    }

    public enum CEnumIntersectionType
    {
        NoNo,
        FrFr,
        FrIn,
        FrTo,
        InFr,
        InIn,
        InTo,
        ToFr,
        ToIn,
        ToTo,
        Overlap,   //In many cases, we can just ignore overlap, because if there is overlap, then there are also other cases
    }

    public enum CEnumDisMode
    {
        FrFr,
        FrIn,
        FrTo,
        InFr,
        InIn,
        InTo,
        ToFr,
        ToIn,
        ToTo,
        Overlap,
    }








}
