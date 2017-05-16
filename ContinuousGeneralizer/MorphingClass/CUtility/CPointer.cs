using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;

namespace MorphingClass.CUtility
{
    public class CPointer<T>:CBasicBase
        where T: new()
    {
        private static int _intStaticGID;
        public T Item { set; get; }

        public CPointer()
        {
            this.Item = new T();
            this.GID = _intStaticGID++;
        }
    }
}
