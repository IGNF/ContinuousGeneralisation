using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MorphingClass.CGeometry.CGeometryBase
{
    public class CLandCoverBase : CBasicBase
    {

        public int intType { get; set; }
        public int intTypeIndex { get; set; } //the index (0, 1, 2, ...) of a type. used for access type distance directly. this is only set for patches at step 1; at other steps the patch index is stored in a dictionary

    }
}
