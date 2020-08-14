using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EngineeringCourseFinder.Classes
{
    [Serializable]
    public struct CEABAU : INotifyPropertyChanged
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
        public float _Math;
        public float _NS;
        public float _CS;
        public float _ES;
        public float _ED;
        // Percentages
        public float MathP { get { float total = Math + NS + CS + ES + ED; return Math / total; } }
        public float NSP { get { float total = Math + NS + CS + ES + ED; return NS / total; } }
        public float CSP { get { float total = Math + NS + CS + ES + ED; return CS / total; } }
        public float ESP { get { float total = Math + NS + CS + ES + ED; return ES / total; } }
        public float EDP { get { float total = Math + NS + CS + ES + ED; return ED / total; } }

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

        #region Data Binding
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public float Math
        {
            get { return this._Math; }
            set
            {
                if (value != this._Math)
                {
                    _Math = value;
                    NotifyPropertyChanged("Math");
                    NotifyPropertyChanged("MathP");
                }
            }
        }
        public float NS
        {
            get { return this._NS; }
            set
            {
                if (value != this._NS)
                {
                    _NS = value;
                    NotifyPropertyChanged("NS");
                    NotifyPropertyChanged("NSP");
                }
            }
        }
        public float CS
        {
            get { return this._CS; }
            set
            {
                if (value != this._CS)
                {
                    _CS = value;
                    NotifyPropertyChanged("CS");
                    NotifyPropertyChanged("CSP");
                }
            }
        }
        public float ES
        {
            get { return this._ES; }
            set
            {
                if (value != this._ES)
                {
                    _ES = value;
                    NotifyPropertyChanged("ES");
                    NotifyPropertyChanged("ESP");
                }
            }
        }
        public float ED
        {
            get { return this._ED; }
            set
            {
                if (value != this._ED)
                {
                    _ED = value;
                    NotifyPropertyChanged("ED");
                    NotifyPropertyChanged("EDP");
                }
            }
        }
        #endregion
    }
}
