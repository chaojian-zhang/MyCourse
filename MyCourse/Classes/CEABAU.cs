using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Classes
{
    [Serializable]
    public class CEABAU : IEquatable<CEABAU>
    {
        public CEABAU(float math = 0, float ns = 0, float cs = 0, float es = 0, float ed = 0)
        {
            Math = math;
            NS = ns;
            CS = cs;
            ES = es;
            ED = ed;
        }

        // Values
        public float Math { get; set; }
        public float NS { get; set; }
        public float CS { get; set; }
        public float ES { get; set; }
        public float ED { get; set; }

        #region Comparing Interface
        public static bool operator ==(CEABAU c1, CEABAU c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(CEABAU c1, CEABAU c2)
        {
            return !c1.Equals(c2);
        }
        public bool Equals(CEABAU other)
        {
            return Equals(other, this);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            CEABAU objectToCompareWith = (CEABAU)obj;

            return objectToCompareWith.Math == Math
                && objectToCompareWith.NS == NS
                && objectToCompareWith.CS == CS
                && objectToCompareWith.ES == ES
                && objectToCompareWith.ED == ED;

        }

        public override int GetHashCode()
        {
            var calculation = Math + NS + CS + ES + ED;
            return calculation.GetHashCode();
        }
        #endregion

    }
}
