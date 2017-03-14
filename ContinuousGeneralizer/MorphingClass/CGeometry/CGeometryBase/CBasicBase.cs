using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MorphingClass.CGeometry.CGeometryBase
{
    public class CBasicBase : IComparable<CBasicBase>
    {
        protected int _intID = -1;
        public int GID { get; set; } //global id
        
        

        public int CompareTo(CBasicBase other)
        {
            return this.GID.CompareTo(other.GID);
        }
        
        

        public int ID
        {
            get { return _intID; }
            set { _intID = value; }
        }


    }
}
