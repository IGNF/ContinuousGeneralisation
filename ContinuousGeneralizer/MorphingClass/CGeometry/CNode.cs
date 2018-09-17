using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MorphingClass.CEntity;
using MorphingClass.CUtility;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CGeometry.CGeometryBase;

namespace MorphingClass.CGeometry
{
    public class CNode: CBasicBase
    {
        public double dblCost { set; get; } = double.MaxValue;
        public List<CNode> NbrCNodeLt { set; get; }
        public CNode PrevCNode { set; get; }
        public CNode NextCNode { set; get; }
        public string strColor { set; get; } = "white";  //the color can be "white", "gray", "black"

        private static int _intStaticGID;

        public CNode()
        {
            this.GID = _intStaticGID++;
        }

        //public int CompareTo(CNode other)
        //{
        //    return this.dblCost.CompareTo(other.dblCost);
        //}
    }
}
