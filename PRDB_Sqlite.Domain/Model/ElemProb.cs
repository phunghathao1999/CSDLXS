using PRDB_Sqlite.Infractructure.Common;
using System;

namespace PRDB_Sqlite.Domain.Model
{
    public class ElemProb
    {
        public string schemaName { get; set; }
        private float _upperBound;
        private float _lowerBound;
        public float upperBound
        {
            get => _upperBound;
            set
            {
                if (_upperBound > 1 || _upperBound < 0) throw new Exception("Invalid Prod");
                var val = value.ToString("F5");
                _upperBound = float.Parse(val);
            }
        }
        public float lowerBound
        {
            get => _lowerBound;
            set
            {
                if (_lowerBound > 1 || _lowerBound < 0) throw new Exception("Invalid Prod");
                var val = value.ToString("F5");
                _lowerBound = float.Parse(val);
            }
        }
        public ElemProb(string strValue, string schemaName = "")
        {
            var temp = strValue.Trim();
            float lwB, upB;
            //format string handle
            try
            {
                temp = temp.Replace("[", "");
                temp = temp.Replace("]", "");
                var lst = temp.Split(',');

                float.TryParse(lst[0].ToString().Trim(), out lwB);
                float.TryParse(lst[1].ToString().Trim(), out upB);

                this.lowerBound = lwB;
                this.upperBound = upB;
                this.schemaName = schemaName;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                this.upperBound = -1;
                this.lowerBound = -1;
            }
        }

        public ElemProb(float lowerBound, float upperBound, string schemaName = "")
        {
            this.schemaName = schemaName;
            if (Math.Abs(upperBound - lowerBound) < Parameter.eulerThreshold) throw new Exception("Lower Bound must be smaller than Upper Bound");
            this._lowerBound = lowerBound;
            this._upperBound = upperBound;
        }
        public ElemProb(ElemProb ps)
        {
            if (Math.Abs(upperBound - lowerBound) < Parameter.eulerThreshold) throw new Exception("Lower Bound must be smaller than Upper Bound");
            this._lowerBound = ps.lowerBound;
            this._upperBound = ps.upperBound;
        }

        public override string ToString()
        {
            return String.Format("[ {0} , {1} ]", this.lowerBound, this.upperBound);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


    }
}