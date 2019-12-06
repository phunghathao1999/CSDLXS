using System;
using System.Collections.Generic;
using System.Linq;

namespace PRDB_Sqlite.Domain.Model
{
    public class PTuple
    {
        public IDictionary<String, IList<String>> valueSet { get; set; }
        private IList<ElemProb> _ps;
        public ElemProb Ps
        {
            get => _ps.Count > 0 ? _ps.FirstOrDefault() : null;
            set
            {
                _ps = new List<ElemProb>() { value };
            }
        }
        public IList<ElemProb> PsList
        {
            get => _ps.Count > 0 ? _ps : null;
            set
            {
                _ps = value;
            }
        }
        public PTuple(IDictionary<string, IList<String>> valueSet, ElemProb ps)
        {
            this.valueSet = valueSet;
            this.Ps = ps;
        }
        //decart use this
        public PTuple(IDictionary<string, IList<String>> valueSet, IList<ElemProb> psLst)
        {
            this.valueSet = valueSet;
            this.PsList = psLst;
        }
        public PTuple()
        {
            this.Ps = new ElemProb(0f, 1f);
            this.valueSet = new Dictionary<String, IList<String>>();
        }
        public PTuple(PRelation pRelation)
        {
            this.Ps = new ElemProb(0f, 1f);
                var valList = new Dictionary<String, IList<String>>();
            this.valueSet = new Dictionary<String, IList<String>>();
            foreach (var attr in pRelation.schema.Attributes)
                this.valueSet.Add(String.Format("{0}.{1}", pRelation.relationName.ToLower(), attr.AttributeName.ToLower()), new List<String>());
        }
        public PTuple(string strTuple)
        {

        }

        public override string ToString()
        {
            return this.valueSet.Count + "rows";
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
