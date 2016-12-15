using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using MorphingClass.CGeometry;
using MorphingClass.CCorrepondObjects;


namespace MorphingClass.CUtility
{
    //public class CMoveInformation
    //{
    //    List<CIntersectionLine> _PreIntersectionLineLt = new List<CIntersectionLine>();   //线段与位于该线段之前线段的相交记录
    //    List<CIntersectionLine> _FolIntersectionLineLt = new List<CIntersectionLine>();   //线段与位于该线段之后线段的相交记录
    //    List<double> _dblRatiopassedLt = new List<double>();               //本线段各节点处的Ration值
    //    List<double> _dblTpassedLt = new List<double>();              //本线段各节点处的t值
    //    List<double> _dblRatiointervalLt = new List<double>();          //各节点之间Ratio区间的大小
    //    List<double> _dblTintervalLt = new List<double>();                //完成该Ratio区间所给定的时间


    //    public CMoveInformation()
    //    {



    //    }





    //    /// <summary>
    //    /// 属性：线段与位于该线段之前线段的相交记录
    //    /// </summary>
    //    public List<CIntersectionLine> PreIntersectionLineLt
    //    {
    //        get { return _PreIntersectionLineLt; }
    //        set { _PreIntersectionLineLt = value; }
    //    }

    //    /// <summary>
    //    /// 属性：线段与位于该线段之后线段的相交记录
    //    /// </summary>
    //    public List<CIntersectionLine> FolIntersectionLineLt
    //    {
    //        get { return _FolIntersectionLineLt; }
    //        set { _FolIntersectionLineLt = value; }
    //    }

    //    /// <summary>
    //    /// 属性：本线段各节点处的Ration值
    //    /// </summary>
    //    public List<double> dblRatiopassedLt
    //    {
    //        get { return _dblRatiopassedLt; }
    //        set { _dblRatiopassedLt = value; }
    //    }

    //    /// <summary>
    //    /// 属性：本线段各节点处的t值
    //    /// </summary>
    //    public List<double> dblTpassedLt
    //    {
    //        get { return _dblTpassedLt; }
    //        set { _dblTpassedLt = value; }
    //    }

    //    /// <summary>
    //    /// 属性：各节点之间Ratio区间的大小
    //    /// </summary>
    //    public List<double> dblRatiointervalLt
    //    {
    //        get { return _dblRatiointervalLt; }
    //        set { _dblRatiointervalLt = value; }
    //    }

    //    /// <summary>
    //    /// 属性：完成该Ratio区间所给定的时间
    //    /// </summary>
    //    public List<double> dblTintervalLt
    //    {
    //        get { return _dblTintervalLt; }
    //        set { _dblTintervalLt = value; }
    //    }













    //}


    //public class CIntersectionLine
    //{
    //    private CEdge _ceLine;
    //    private double _dblSelfRatio;
    //    private double _dblTOtherRatio;
    //    private double _dblTimeToIntersection;

    //    public CIntersectionLine(double dblSelfRatio, double dblTOtherRatio, CEdge ceLine)
    //    {
    //        _dblSelfRatio = dblSelfRatio;
    //        _dblTOtherRatio = dblTOtherRatio;
    //        _ceLine = ceLine;

    //    }

    //    /// <summary>属性：相交点在源线段上的相对位置</summary>
    //    public double dblSelfRatio
    //    {
    //        get { return _dblSelfRatio; }
    //        set { _dblSelfRatio = value; }
    //    }

    //    /// <summary>属性：相交点在目标线段上的相对位置</summary>
    //    public double dblTOtherRatio
    //    {
    //        get { return _dblTOtherRatio; }
    //        set { _dblTOtherRatio = value; }
    //    }

    //    /// <summary>属性：相交线段</summary>
    //    public CEdge pLine
    //    {
    //        get { return _ceLine; }
    //        set { _ceLine = value; }
    //    }

    //    /// <summary>属性：到相交点的时刻</summary>
    //    public double dblTimeToIntersection
    //    {
    //        get { return _dblTimeToIntersection; }
    //        set { _dblTimeToIntersection = value; }
    //    }
    //}

}
