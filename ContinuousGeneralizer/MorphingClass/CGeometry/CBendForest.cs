using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
//System.Runtime.InteropServices

namespace MorphingClass.CGeometry
{
    //[ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class CBendForest : SortedDictionary<int,  CBend>
    {
        private double _dblDepthAverage;
        private double _dblDepthMax;
        private List<CBend> _pBendLt;             //所有弯曲的列表（包括重合弯曲）
        private List<double> _dblDeepBendDepthLt;
        private int _intPathCount;
        

        /// <summary>该曲线的平均深度</summary>
        public double dblDepthAverage
        {
            get { return _dblDepthAverage; }
            set { _dblDepthAverage = value; }
        }

        /// <summary>该曲线的最大深度</summary>
        public double dblDepthMax
        {
            get { return _dblDepthMax; }
            set { _dblDepthMax = value; }
        }

        /// <summary>底层弯曲深度记录</summary>
        public List<double> dblDeepBendDepthLt
        {
            get { return _dblDeepBendDepthLt; }
            set { _dblDeepBendDepthLt = value; }
        }


        /// <summary>分支数量（低层次弯曲数量）</summary>
        public int intPathCount
        {
            get { return _intPathCount; }
            set { _intPathCount = value; }
        }

        /// <summary>所有弯曲的列表（包括重合弯曲）</summary>
        public List<CBend> pBendLt
        {
            get { return _pBendLt; }
            set { _pBendLt = value; }
        }

        //警告	1	类型库导出程序在处理“MorphingClass.CGeometry.CBendForest, MorphingClass”时发出警告。警告: 类型库导出程序遇到从泛型类派生并且未标记为 [ClassInterface(ClassInterfaceType.None)] 的类型。无法公开这种类型的类接口。请考虑用 [ClassInterface(ClassInterfaceType.None)] 标记该类型，并使用 ComDefaultInterface 属性向 COM 公开某个显式接口作为默认接口。	MorphingClass








    }
}
