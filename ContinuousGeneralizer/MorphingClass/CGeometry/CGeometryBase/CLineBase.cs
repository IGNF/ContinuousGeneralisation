using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MorphingClass.CUtility;

namespace MorphingClass.CGeometry.CGeometryBase
{
    public class CLineBase<CGeo> : CGeometricBase<CGeo>
        where CGeo : class
    {
        protected CPoint _FrCpt;    //start
        protected CPoint _ToCpt;      //end

        public CEnvelope CEnv { set; get; }

        public virtual void SetEnvelope()
        {

        }


        /// <summary>
        /// Start of cedge
        /// </summary>
        public virtual CPoint FrCpt
        {
            get { return _FrCpt; }
            set { _FrCpt = value; }  //only when _FrCpt and value have the same coordinates may one be allowed to do this.
        }

        /// <summary>
        /// End of cedge
        /// </summary>
        public virtual CPoint ToCpt
        {
            get { return _ToCpt; }
            set { _ToCpt = value; }    //only when _ToCpt and value have the same coordinates may one be allowed to do this.
        }



    }
}
