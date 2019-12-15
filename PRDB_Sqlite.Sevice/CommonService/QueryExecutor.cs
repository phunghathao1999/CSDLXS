using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Infractructure.Common;
using PRDB_Sqlite.Infractructure.Constant;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PRDB_Sqlite.Sevice.CommonService
{
    public class QueryExecutor
    {

        // thuc thi cau query
        public string queryString { get; set; }
        public IList<PRelation> selectedRelations { get; set; }
        public PRelation relationResult { get; set; }
        public IList<PAttribute> selectedAttributes;
        public PDatabase PDatabase { get; set; }
        public string conditionString { get; set; }
        public bool flagNaturalJoin { get; set; }
        private string OperationNaturalJoin = string.Empty;

        public string MessageError { get; set; }
        public PRelation DescartesAndNaturalJoin { get; set; }
        public IList<String> querys { get; set; }

        public QueryExecutor(string queryString, PDatabase PDatabase)
        {
            this.selectedRelations = new List<PRelation>();
            this.selectedAttributes = new List<PAttribute>();
            this.relationResult = new PRelation();
            this.selectedAttributes = new List<PAttribute>();
            this.PDatabase = PDatabase;
            this.queryString = StandardizeQuery(queryString);
            this.flagNaturalJoin = false;
        }

        //chuân hóa query
        private string StandardizeQuery(string queryString)
        {
            try
            {
                string result = "";
                string S = queryString.Trim();
                //handle case double space
                for (int i = 0; i < S.Length; i++)
                    if (S[i] == ' ')
                    {
                        if (S[i - 1] != ' ')
                            result += S[i];
                    }
                    else
                        result += S[i];
                if (result.Contains("\r") || result.Contains("\n"))
                {
                    result = result.Replace("\r", "");
                    result = result.Replace("\n", ";");
                }
                //result = result.Replace("\n", " ");
                return result.ToLower();

            }
            catch (Exception)
            {
                return null;
            }
        }
        private bool CheckStringQuery(string stringQuery)
        {

            //////////////////////// Check Syntax ////////////////////////
            int indexSelect = stringQuery.IndexOf("select");
            int indexFrom = stringQuery.IndexOf("from");
            int indexWhere = stringQuery.IndexOf("where");
            int indexStart = stringQuery.IndexOf("*");
            int indexLastSelect = stringQuery.LastIndexOf("select");
            int indexLastFrom = stringQuery.IndexOf("from");
            int indexLastWhere = stringQuery.IndexOf("where");
            int indexLastStart = stringQuery.IndexOf("*");


            if (indexSelect == -1)
            {
                MessageError = "Syntax Error! Not Found keyword 'select' ";
                return false;
            }

            if (indexFrom == -1)
            {
                MessageError = "Syntax Error! Not Found FROM statement";
                return false;
            }

            if (indexWhere != -1)
            {

                if (indexSelect < indexFrom && indexFrom < indexWhere)
                {

                }
                else
                {
                    MessageError = "Syntax Error! The keyword must theo order 'Select From Where' ";
                    return false;
                }

            }
            else
            {
                if (indexSelect < indexFrom && indexWhere == -1)
                {

                }
                else
                {
                    MessageError = "Syntax Error! The keyword must theo order 'Select From ' ";
                    return false;
                }
            }

            if (indexSelect != indexLastSelect)
            {
                MessageError = "Syntax Error! In query statement only have a keyword 'select' ";
                return false;
            }

            if (indexFrom != indexLastFrom)
            {
                MessageError = "Syntax Error! In query statement only have a keyword 'from' ";
                return false;
            }

            if (indexLastWhere != indexWhere)
            {
                MessageError = "Syntax Error! In query statement only have a keyword 'where' ";
                return false;
            }


            #region check kí tự đặc biệt

            char[] SpecialCharacter = new char[] { '~', '!', '@', '#', '$', '%', '^', '&', '[', ']', '(', ')', '+', '`', ';', '<', '>', '?', '/', ':', '\"', '\'', '=', '{', '}', '\\', '|' };
            string specialcharacter = string.Empty;
            for (int i = 0; i < SpecialCharacter.Length; i++)
                specialcharacter += SpecialCharacter[i];

            string subString = string.Empty;
            if (stringQuery.Contains("where"))
            {
                int pOne = stringQuery.IndexOf("select") + 6;
                int pTwo = stringQuery.IndexOf("where") - 1;
                subString = stringQuery.Substring(pOne, pTwo);
                for (int i = 0; i < subString.Length; i++)
                {
                    if (specialcharacter.Contains(subString[i]))
                    {

                        MessageError = String.Format("Error: Do not input the special character '{0}' in query statement", subString[i]);
                        return false;

                    }
                }
            }
            else
            {
                for (int i = 0; i < stringQuery.Length; i++)
                {
                    if (specialcharacter.Contains(stringQuery[i]))
                    {
                        MessageError = String.Format("Error: Do not input the special character '{0}' in query statement", stringQuery[i]);
                        return false;

                    }
                }

            }
            #endregion
            return true;
        }
        private bool QueryAnalyze()
        {
            try
            {
                string S = this.queryString;
                //Kiểm tra câu truy vấn có hợp lệ
                if (!this.CheckStringQuery(S))
                {
                    return false;
                }


                //Get All Relation
                this.selectedRelations = GetAllRelation(S);


                if (this.selectedRelations == null)
                {
                    return false;
                }

                //Get All Attribute
                this.selectedAttributes = GetAttribute(S);
                if (this.selectedAttributes == null)
                    return false;

                this.conditionString = GetCondition(S);

                return true;


            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string GetCondition(string valueString)
        {

            string conditionString = string.Empty;
            int posOne;

            ///////////////////// Get Select Condition /////////////////
            if (valueString.Contains("where "))
            {
                posOne = valueString.IndexOf("where") + 5;
                conditionString = valueString.Substring(posOne);   // Get Select Condition in the Query Text
            }
            else
                conditionString = string.Empty;

            return conditionString;
        }

        private IList<PAttribute> GetAttribute(string valueString)
        {
            var listPAttribute = new List<PAttribute>();
            //////////////////////// Get Attributes //////////////////////
            int posOne, posTwo, posSelect = valueString.ToLower().IndexOf("select") + 6;


            // * là chọn tất cả các thuộc tính
            if (valueString.Contains("*"))
            {
                posOne = valueString.IndexOf("*");                                                   // start postion of attributes
                posTwo = valueString.IndexOf("from ") - 1;

                if (posOne > posTwo)
                {
                    MessageError = "Incorrect syntax near 'from'.";
                    return null;
                }
                if (!valueString.Substring(posSelect, posTwo - posSelect).Trim().Equals("*"))
                {
                    MessageError = "Incorrect syntax near 'select'.";
                    return null;
                }

                if (posOne < valueString.IndexOf("select"))
                {
                    MessageError = "Incorrect syntax near 'select'.";
                    return null;
                }

                if (valueString.Contains("where") && posOne > valueString.IndexOf("where"))
                {
                    MessageError = "Incorrect syntax near 'where'.";
                    return null;
                }

                if (posOne != valueString.LastIndexOf("*"))
                {
                    MessageError = "Incorrect syntax near 'select'.";
                    return null;
                }

                // end postion of attributes
                string attributes = valueString.Substring(posOne, posTwo - posOne + 1);

                // Nếu như phia sau dấu * có bất kì kí tự nào thì sẽ thông báo lỗi
                if (attributes.Trim().Length > 1)
                {
                    MessageError = "Incorrect syntax near 'select'.";
                    return null;
                }

                // thực hiện sao chép toàn bộ thuộc tính của các quan hệ vào danh sách thuộc tính chọn
                foreach (var item in this.selectedRelations.ToList())
                {
                    listPAttribute.AddRange(item.schema.Attributes);
                }
                return listPAttribute;

            }
            else // ngược lại là xuất theo thuộc tính chỉ định
            {

                posOne = valueString.IndexOf("select") + 6;                                                   // start postion of attributes
                posTwo = valueString.IndexOf("from ") - 1;                                                    // end postion of attributes


                string attributes = valueString.Substring(posOne, posTwo - posOne + 1);

                //kiểm tra cú pháp của chuổi thuộc tính
                if (!CheckStringAttribute(attributes))
                {
                    MessageError = "Incorrect syntax near 'select'.";
                    return null;

                }
                else
                {
                    string[] seperator = { "," };
                    string[] attribute = attributes.Split(seperator, StringSplitOptions.RemoveEmptyEntries); // split thành mảng các thuộc tính                    

                    foreach (string str in attribute)
                    {
                        if (!str.Contains("."))
                        {
                            string attributeName = str.Trim();
                            int countOne = 0;
                            int countSameAttribute = 0;
                            foreach (var relation in this.selectedRelations)
                            {
                                List<string> listOfAttributeName = relation.schema.Attributes.Select(p =>
                                {
                                    if (p.AttributeName.IndexOf('.') == -1)
                                    {
                                        return String.Format("{0}.{1}", relation.relationName, p.AttributeName).ToLower();
                                    }
                                    return p.AttributeName.ToLower();
                                }).ToList();
                                if (listOfAttributeName.Contains(String.Format("{0}.{1}", relation.relationName, attributeName.ToLower())))
                                {
                                    var attr =
                                        new PAttribute(relation.schema.Attributes[listOfAttributeName.IndexOf(String.Format("{0}.{1}", relation.relationName, attributeName.ToLower()))]);

                                    if (attr.AttributeName.IndexOf('.') == -1) attr.AttributeName = String.Format("{0}.{1}", relation.relationName, attr.AttributeName);
                                    listPAttribute.Add(attr);
                                    countSameAttribute++;
                                }
                                else
                                {
                                    countOne++;
                                }

                            }

                            if (countOne == this.selectedRelations.Count)
                            {
                                MessageError = String.Format(" Invalid attribute name '{0}'.", attributeName);
                                return null;

                            }

                            if (countSameAttribute == this.selectedRelations.Count && this.selectedRelations.Count >= 2)
                            {
                                MessageError = String.Format(" Ambiguous attribute name '{0}'.", attributeName);
                                return null;

                            }
                        }
                        else
                        {
                            string[] array = str.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                            if (array.Length != 2)
                            {
                                MessageError = "Incorrect syntax near the keyword 'select'.";
                                return null;
                            }

                            var relation = this.selectedRelations.SingleOrDefault(c => c.relationName.Trim() == array.First().Trim());

                            if (relation == null)
                            {
                                MessageError = String.Format("The multi-part identifier '{0}' could not be bound.", str);
                                return null;
                            }

                            var attr = new PAttribute(relation.schema.Attributes.SingleOrDefault(c => getStandardAttrName(c.AttributeName.Trim().ToLower()) == array.Last().Trim()));

                            if (attr.AttributeName.IndexOf('.') == -1) attr.AttributeName = String.Format("{0}.{1}", relation.relationName, attr.AttributeName);

                            if (attr == null)
                            {
                                MessageError = "Invalid attribute name '" + array[1] + "'.";
                                return null;
                            }


                            listPAttribute.Add(attr);

                        }
                    }

                    return listPAttribute.Count == 0 ? null : listPAttribute;


                }
            }
        }
        private static bool CheckStringAttribute(string stringAttribute)
        {

            string subString = stringAttribute;

            if (subString.Trim().Length <= 0)
                return false;

            for (int i = 0; i < subString.Length - 1; i++)
            {
                if (subString.ElementAt(i) == subString.ElementAt(i + 1) && subString.ElementAt(i) == ',')
                    return false;
            }

            if (subString.LastIndexOf(",") == subString.Length - 1)
            {
                return false;
            }


            return true;
        }
        private IList<PRelation> GetAllRelation(string pquery)
        {
            int posOne;
            int posTwo;
            string relationsString = string.Empty;
            //string[] seperator = { "," };
            //string[] relations;
            var relationls = new List<PRelation>();


            //////////////////////// Get Relations ///////////////////////
            posOne = pquery.IndexOf("from") + 4;

            if (!pquery.Contains("where"))
                posTwo = pquery.Length - 1;
            else
                posTwo = pquery.IndexOf("where") - 1;

            relationsString = pquery.Substring(posOne, posTwo - posOne + 1).Trim();   // Get Relation in the Query Text     




            if (relationsString.Trim().Length <= 0)
            {
                MessageError = "No relation exists in the query !";
                return null;
            }
            if (relationsString.Contains(" natural join in") || relationsString.Contains(" natural join ig") || relationsString.Contains(" natural join me"))
            {
                var relations = new string[2];

                if (relationsString.Contains(" natural join in"))
                {
                    relations[0] = relationsString.Substring(0, relationsString.IndexOf("natural join in")).Trim();
                    relations[1] = relationsString.Substring(relationsString.IndexOf("natural join in") + 15).Trim();
                    OperationNaturalJoin = "in";
                }
                else
                if (relationsString.Contains(" natural join ig"))
                {
                    relations[0] = relationsString.Substring(0, relationsString.IndexOf("natural join ig")).Trim();
                    relations[1] = relationsString.Substring(relationsString.IndexOf("natural join ig") + 15).Trim();
                    OperationNaturalJoin = "ig";
                    OperationNaturalJoin = "ig";
                }
                else
                {
                    relations[0] = relationsString.Substring(0, relationsString.IndexOf("natural join me")).Trim();
                    relations[1] = relationsString.Substring(relationsString.IndexOf("natural join me") + 15).Trim();
                    OperationNaturalJoin = "me";
                }
                foreach (string item in relations)
                {
                    if (item == "")
                    {
                        MessageError = "Incorrect syntax near 'from'.";
                        return null;
                    }
                    string[] listTmp = item.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (listTmp.Length != 1)
                    {
                        MessageError = "Incorrect syntax near 'from'.";
                        return null;
                    }
                    try
                    {
                        var relIndb = this.PDatabase.Relations.ToList().Where(p => p.relationName.ToLower().Equals(item.Trim().ToLower())).First();
                        relationls.Add(relIndb);
                    }
                    catch
                    {
                        MessageError = String.Format("Relation {0} does not exists in the query !", item);
                        return null;
                    }

                }
                flagNaturalJoin = true;
            }
            else
            if (relationsString.Contains(","))
            {
                var lsRel = relationsString.Split(',');
                foreach (var relName in lsRel)
                {
                    try
                    {
                        var relIndb = this.PDatabase.Relations.ToList().Where(p => p.relationName.ToLower().Equals(relName.Trim().ToLower())).First();
                        relationls.Add(relIndb);
                    }
                    catch
                    {
                        MessageError = String.Format("Relation {0} does not exists in the query !", relName);
                        return null;
                    }
                }
            }
            else
            {
                try
                {
                    var rel = this.PDatabase.Relations.ToList().Where(p => p.relationName.ToLower().Contains(relationsString.ToLower())).First();
                    relationls.Add(rel);
                }
                catch
                {
                    MessageError = String.Format("Relation {0} does not exists in the query !", relationsString); return null;
                }

            }
            return relationls;
        }

        public bool ExecuteQuery()
        {

            // multiQueryAnalyze();
            try
            {
                if (!QueryAnalyze()) return false;
                this.relationResult = new PRelation();

                if (this.selectedRelations.Count == 2)
                {
                    if (flagNaturalJoin != true)
                        this.selectedRelations[0] = Descartes();
                    else
                        this.selectedRelations[0] = NaturalJoin();
                    //luu y sau khi natural join => selectedAttr co the bi thay doi luong chuyen thanh format <relation>.<attributename>
                }
                else
                {
                    //only one rel
                    foreach (var attr in this.selectedRelations[0].schema.Attributes)
                    {
                        if (!attr.AttributeName.Contains("."))
                            attr.AttributeName = String.Format("{0}.{1}", this.selectedRelations[0].relationName, attr.AttributeName);
                    }
                }

                if (!this.queryString.Contains("where"))
                {
                    this.relationResult = getRelationBySelectAttribute(this.selectedRelations[0], this.selectedAttributes);
                    return true;
                }
                else
                {
                    //co where
                    SelectCondition Condition = new SelectCondition(this.selectedRelations[0], this.conditionString);
                    if (!Condition.CheckConditionString())
                    {
                        this.MessageError = Condition.MessageError;
                        return false;
                    }

                    foreach (var tuple in this.selectedRelations[0].tupes)
                        if (Condition.Satisfied(tuple))
                            this.relationResult.tupes.Add(tuple);

                    if (Condition.MessageError != string.Empty)
                    {
                        this.MessageError = Condition.MessageError;
                        return false;
                    }

                    if (Condition.conditionString == string.Empty) //mat condition string
                    {
                        this.MessageError = Condition.MessageError;
                        return false;
                    }

                    this.relationResult.schema = this.selectedRelations[0].schema;
                    this.relationResult = getRelationBySelectAttribute(this.relationResult, this.selectedAttributes);
                }
            }
            catch (Exception ex)
            {

                return false;
            }
            return true;
        }



        public PRelation getRelationBySelectAttribute(PRelation pRelation, IList<PAttribute> selectedAttributes)
        {
            var relation = new PRelation();
            relation.relationName = pRelation.relationName;

            foreach (var etuple in pRelation.tupes)
            {
                var tuple = new PTuple();
                foreach (var attr in selectedAttributes) //ds att dc chieu
                {
                    if (!ContantCls.emlementProb.Equals(getStandardAttrName(attr.AttributeName)))
                        tuple.valueSet.Add(attr.AttributeName, etuple.valueSet[attr.AttributeName]); //get trong danh sach att dc chieu
                }
                tuple.Ps = etuple.Ps;
                relation.tupes.Add(tuple);
            }

            #region projection
            e_Val_Equivalent(ref relation, selectedAttributes);
            #endregion projection

            relation.schema.Attributes = selectedAttributes;
            return relation;
        }

        private PRelation e_Val_Equivalent_err(ref PRelation pRelation, IList<PAttribute> pAttributes)
        {
            var Tuples = new List<PTuple>();
            var rootTuples = pRelation.tupes.ToList();
            foreach (var tup1 in rootTuples)
            {
                foreach (var tup2 in rootTuples)
                {
                    if (tup1 == tup2) continue;
                    var tuple = new PTuple();
                    if (check_e_Val_eql(tup1, tup2, pAttributes))
                    {
                        var collapsedTuple = getTupleCollapse(tup1, tup2, pAttributes);
                        /*
                         * remove tup1 and tup2
                         * replace collapse tuple in tuple 1
                         */
                        var tupidx_1 = pRelation.tupes.IndexOf(tup1);
                        var tupidx_2 = pRelation.tupes.IndexOf(tup2);
                        PTuple t1;
                        if (tupidx_1 != -1)
                            t1 = pRelation.tupes.ElementAt(tupidx_1);

                        if (tupidx_2 != -1)
                            pRelation.tupes.Remove(pRelation.tupes.ElementAt(tupidx_2));

                        t1 = collapsedTuple;

                    }
                }
            }

            return pRelation;
        }
        private void e_Val_Equivalent(ref PRelation pRelation, IList<PAttribute> pAttributes)
        {
            var length = pRelation.tupes.Count;
            for (int i = 0; i < pRelation.tupes.Count; i++)
            {
                for (int j = 0; j < pRelation.tupes.Count; j++)
                {
                    if (i != j && check_e_Val_eql(pRelation.tupes[i], pRelation.tupes[j], pAttributes))
                    {
                        pRelation.tupes[i] = getTupleCollapse(pRelation.tupes[i], pRelation.tupes[j], pAttributes);
                        pRelation.tupes.RemoveAt(j);
                        i = -1;
                        break;
                    }
                }
            }
        }
        private PTuple getTupleCollapse(PTuple pTuple_1, PTuple pTuple_2, IList<PAttribute> pAttributes)
        {
            var reTuple = new PTuple();
            //get val
            foreach (var att in pAttributes)
            {

                    #region 
                    float p = 1f;
                if (!getStandardAttrName(att.AttributeName).Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase))
                    reTuple.valueSet.Add(att.AttributeName, getInterset(ref p, pTuple_1.valueSet[att.AttributeName], pTuple_2.valueSet[att.AttributeName], att.Type.TypeName));
                #endregion
            }
            //get Prob

            var prop1 = new ElemProb(pTuple_1.Ps);
            var prop2 = new ElemProb(pTuple_2.Ps);
            var strategy = Parameter.curStrategy;
            switch (strategy.Substring(strategy.IndexOf("_") + 1))
            {

                case "ig": reTuple.Ps = new ElemProb(Math.Max(prop1.lowerBound, prop2.lowerBound), Math.Min(1, prop1.upperBound + prop2.upperBound)); break;
                case "in": reTuple.Ps = new ElemProb(prop1.lowerBound + prop2.lowerBound - (prop1.lowerBound * prop2.lowerBound), prop1.upperBound + prop2.upperBound - (prop1.upperBound * prop2.upperBound)); break;
                case "me": reTuple.Ps = new ElemProb(Math.Min(1, prop1.lowerBound + prop2.lowerBound), Math.Min(1, prop1.upperBound + prop2.upperBound)); break;
                default:
                    MessageError = "Invalid Current Strategy: " + Parameter.curStrategy;
                    break;
            }

            return reTuple;
        }
        private static PTuple getTupleCollapse_static(PTuple pTuple_1, PTuple pTuple_2, IList<PAttribute> pAttributes)
        {
            var reTuple = new PTuple();
            //get val
            foreach (var att in pAttributes)
            {

                #region 
                float p = 1f;
                if (!getStandardAttrName_static(att.AttributeName).Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase))
                    reTuple.valueSet.Add(att.AttributeName, getInterset(ref p, pTuple_1.valueSet[att.AttributeName], pTuple_2.valueSet[att.AttributeName], att.Type.TypeName));
                #endregion
            }
            //get Prob

            var prop1 = new ElemProb(pTuple_1.Ps);
            var prop2 = new ElemProb(pTuple_2.Ps);
            var strategy = Parameter.curStrategy;
            switch (strategy.Substring(strategy.IndexOf("_") + 1))
            {

                case "ig": reTuple.Ps = new ElemProb(Math.Max(prop1.lowerBound, prop2.lowerBound), Math.Min(1, prop1.upperBound + prop2.upperBound)); break;
                case "in": reTuple.Ps = new ElemProb(prop1.lowerBound + prop2.lowerBound - (prop1.lowerBound * prop2.lowerBound), prop1.upperBound + prop2.upperBound - (prop1.upperBound * prop2.upperBound)); break;
                case "me": reTuple.Ps = new ElemProb(Math.Min(1, prop1.lowerBound + prop2.lowerBound), Math.Min(1, prop1.upperBound + prop2.upperBound)); break;
                default:
                    break;
            }

            return reTuple;
        }
        private bool check_e_Val_eql(PTuple pTuple_1, PTuple pTuple_2, IList<PAttribute> pAttributes)
        {
            var probs = new List<float>();
            var eulerElem = new ElemProb(0, 0);
            foreach (var att in pAttributes)
            {
                if (!getStandardAttrName(att.AttributeName).Equals( ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase))
                {
                    #region 
                    //check equal
                    float p = 1f;
                    var innerSet = getInterset(ref p, pTuple_1.valueSet[att.AttributeName], pTuple_2.valueSet[att.AttributeName], att.Type.TypeName);

                    if (!(innerSet is null) && innerSet.Count != 0)
                        if (probs.Count > 0)
                        {
                            probs.Add(p);
                            eulerElem = calProp_e(ref probs);
                        }
                        else probs.Add(p);
                    else
                        return false;

                    #endregion
                }

            }

            return Parameter.eulerThreshold <= eulerElem.lowerBound && eulerElem.upperBound >= Parameter.eulerThreshold;
        }

        private ElemProb calProp_e(ref List<float> probs)
        {
            var first = probs.First();
            probs.Remove(first);
            var second = probs.First();
            probs.Remove(second);

            var prop1 = new ElemProb(first, first);
            var prop2 = new ElemProb(second, second);
            ElemProb ps = new ElemProb(0, 0);
            switch (Parameter.curStrategy)
            {
                case "⊗_ig": ps = new ElemProb(Math.Max(0, prop1.lowerBound + prop2.lowerBound - 1), Math.Min(prop1.upperBound, prop2.upperBound)); break;
                case "⊗_in": ps = new ElemProb(prop1.lowerBound * prop2.lowerBound, prop1.upperBound * prop2.upperBound); break;
                case "⊗_me": ps = new ElemProb(0, 0); break;
                case "⊕_ig": ps = new ElemProb(Math.Max(prop1.lowerBound, prop2.lowerBound), Math.Min(1, prop1.upperBound + prop2.upperBound)); break;
                case "⊕_in": ps = new ElemProb(prop1.lowerBound + prop2.lowerBound - (prop1.lowerBound * prop2.lowerBound), prop1.upperBound + prop2.upperBound - (prop1.upperBound * prop2.upperBound)); break;
                case "⊕_me": ps = new ElemProb(Math.Min(1, prop1.lowerBound + prop2.lowerBound), Math.Min(1, prop1.upperBound + prop2.upperBound)); break;
                default:
                    MessageError = "Invalid Current Strategy: " + Parameter.curStrategy;
                    break;
            }
            return ps;
        }
        private static ElemProb calProp_e_static(ref List<float> probs)
        {
            var first = probs.First();
            probs.Remove(first);
            var second = probs.First();
            probs.Remove(second);

            var prop1 = new ElemProb(first, first);
            var prop2 = new ElemProb(second, second);
            ElemProb ps = new ElemProb(0, 0);
            switch (Parameter.curStrategy)
            {
                case "⊗_ig": ps = new ElemProb(Math.Max(0, prop1.lowerBound + prop2.lowerBound - 1), Math.Min(prop1.upperBound, prop2.upperBound)); break;
                case "⊗_in": ps = new ElemProb(prop1.lowerBound * prop2.lowerBound, prop1.upperBound * prop2.upperBound); break;
                case "⊗_me": ps = new ElemProb(0, 0); break;
                case "⊕_ig": ps = new ElemProb(Math.Max(prop1.lowerBound, prop2.lowerBound), Math.Min(1, prop1.upperBound + prop2.upperBound)); break;
                case "⊕_in": ps = new ElemProb(prop1.lowerBound + prop2.lowerBound - (prop1.lowerBound * prop2.lowerBound), prop1.upperBound + prop2.upperBound - (prop1.upperBound * prop2.upperBound)); break;
                case "⊕_me": ps = new ElemProb(Math.Min(1, prop1.lowerBound + prop2.lowerBound), Math.Min(1, prop1.upperBound + prop2.upperBound)); break;
                default:
                    break;
            }
            return ps;
        }

        private PRelation Descartes()
        {
            var relation = new PRelation();

            //relation.ListRenameRelation.Add(this.selectedRelations[0].RelationName);
            foreach (var attr in this.selectedRelations[0].schema.Attributes)
            {
                if (attr.AttributeName.IndexOf(".") == -1)
                    attr.AttributeName = this.selectedRelations[0].relationName + "." + attr.AttributeName;
                relation.schema.Attributes.Add(attr);
            }

            //relation.ListRenameRelation.Add(this.selectedRelations[1].RelationName);
            foreach (var attr in this.selectedRelations[1].schema.Attributes)
            {
                if (attr.AttributeName.IndexOf(".") == -1)
                    attr.AttributeName = this.selectedRelations[1].relationName + "." + attr.AttributeName;
                relation.schema.Attributes.Add(attr);
            }

            foreach (var tupleOne in this.selectedRelations[0].tupes)
            {
                foreach (var tupleTwo in this.selectedRelations[1].tupes)
                {
                    var tuple = mergeRecord(tupleOne, tupleTwo);
                    relation.tupes.Add(tuple);
                }
            }
            return relation;
        }
        private PTuple mergeRecord(PTuple fstRecord, PTuple sndRecord)
        {
            var reVal = new PTuple();
            this.selectedRelations[0].schema.Attributes.ToList().ForEach(p =>
            {
                if (fstRecord.valueSet.ContainsKey((p.AttributeName)))
                    reVal.valueSet.Add(p.AttributeName, fstRecord.valueSet[(p.AttributeName)]);
            } //pattern [relationName].[attributeName]
            );
            this.selectedRelations[1].schema.Attributes.ToList().ForEach(p =>
            {
                if (sndRecord.valueSet.ContainsKey((p.AttributeName)))
                    reVal.valueSet.Add(p.AttributeName, sndRecord.valueSet[(p.AttributeName)]);
            });
            fstRecord.Ps.schemaName = this.selectedRelations[0].schema.SchemaName;
            sndRecord.Ps.schemaName = this.selectedRelations[1].schema.SchemaName;
            reVal.PsList = new List<ElemProb> { fstRecord.Ps, sndRecord.Ps };
            return reVal;
        }

        private PRelation NaturalJoin()
        {
            var relation = Descartes();

            /* Kiem tra join 
             * -cung ten
             * -cung kieu
             */


            //giai phap cap nhat schema lien tuc thay cho foreach
            for (int i = 0; i < relation.schema.Attributes.Count; i++)
            {
                for (int j = 0; j < relation.schema.Attributes.Count; j++)
                {
                    var attr1 = relation.schema.Attributes[i];
                    var attr2 = relation.schema.Attributes[j];
                    if (attr1 == attr2) continue;
                    if (!equalSchemaName(attr1.AttributeName, attr2.AttributeName)
                        && !getStandardAttrName(attr1.AttributeName).Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase)
                        && !getStandardAttrName(attr2.AttributeName).Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase))
                    {
                        //kiem tra cung ten
                        if (getStandardAttrName(attr1.AttributeName).Equals(getStandardAttrName(attr2.AttributeName), StringComparison.CurrentCultureIgnoreCase))
                        {
                            //kiem tra cung kieu
                            if (attr1.Type.TypeName.Equals(attr2.Type.TypeName, StringComparison.CurrentCultureIgnoreCase))
                            {
                                joinTupleOnAttr(ref relation, attr1, attr2, this.OperationNaturalJoin);
                            }
                        }
                    }
                }
            }

            //remove duplicate attr ps
            var psLs = new List<PAttribute>();
            relation.schema.Attributes.ToList().ForEach(p =>
            {
                if (getStandardAttrName(p.AttributeName).Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase))
                    psLs.Add(p);
            });

            var lst = psLs.Last();
            //remove
            psLs.Remove(lst);
            foreach (var item in psLs)
            {
                relation.schema.Attributes.Remove(item);
                var revAtt = this.selectedAttributes.Where(p => p.id == item.id).FirstOrDefault();
                if (revAtt != null)
                    this.selectedAttributes.Remove(revAtt);
            }

            lst.AttributeName = getStandardAttrName(lst.AttributeName);

            //this.selectedAttributes = relation.schema.Attributes;

            OperationNaturalJoin = string.Empty;
            flagNaturalJoin = false;

            return relation;
        }

        private void joinTupleOnAttr(ref PRelation relation, PAttribute attr1, PAttribute attr2, string OperationNaturalJoin)
        {
            foreach (var tuple in relation.tupes.ToList())
            {
                //primary with prop [1,1]
                if (attr1.primaryKey || attr2.primaryKey)
                {
                    //la khoa nen luon duy nhat
                    var getval1 = tuple.valueSet[attr1.AttributeName].FirstOrDefault();
                    var getval2 = tuple.valueSet[attr2.AttributeName].FirstOrDefault();
                    if (!SelectCondition.EQUAL(getval1, getval2, attr1.Type.TypeName))
                    {
                        //equal => get; otherwise => discard
                        relation.tupes.Remove(tuple);
                    }
                    else
                    {
                        //cal prop
                        tuple.Ps = calProp(tuple.PsList, Opr: this.OperationNaturalJoin);
                        //discard a duplicate attr
                        tuple.valueSet.Remove(attr2.AttributeName);
                        //dis card in selected Attr
                    }
                }
                else //normal attr
                {
                    var getval1 = tuple.valueSet[attr1.AttributeName];
                    var getval2 = tuple.valueSet[attr2.AttributeName];
                    float pSet = 1f;
                    var interSet = getInterset(ref pSet, getval1, getval2, attr1.Type.TypeName);
                    if (interSet is null)
                    {
                        //equal => get; otherwise => discard
                        relation.tupes.Remove(tuple);
                    }
                    else
                    {
                        tuple.valueSet[attr1.AttributeName] = interSet;
                        //cal prop 
                        tuple.Ps = calProp(tuple.PsList, propSet: 1, Opr: this.OperationNaturalJoin);
                        //discard a duplicate attr
                        tuple.valueSet.Remove(attr2.AttributeName);
                    }
                }
            }

            //discard a duplicate attr
            relation.schema.Attributes.Remove(attr2);
            //discard in selected Attr
            this.selectedAttributes.Remove(attr2);

        }
        private static IList<string> getInterset(ref float p, IList<string> set1, IList<string> set2, string typeName)
        {
            var interSet = new List<String>();
            var omega = getOmerga(set1, set2, typeName).Count;

            foreach (var item_set1 in set1)
            {
                foreach (var item_set2 in set2)
                {
                    //string is err 
                    if (SelectCondition.EQUAL(item_set1.Trim(), item_set2.Trim(), typeName))
                    {
                        interSet.Add(item_set1);
                    }
                }
            }
            //cal prop in set
            p = (float)interSet.Count / omega;

            return interSet.Count < 0 ? null : interSet;
        }

        private static IList<String> getOmerga(IList<string> set1, IList<string> set2, String typeName)
        {
            var omergaList = new List<String>();
            omergaList.AddRange(set1);
            foreach (var item_set2 in set2.ToList())
                foreach (var item_omg in omergaList.ToList())
                    //string is err 
                    if (!SelectCondition.EQUAL( item_set2.Trim(), item_omg.Trim(), typeName))
                    {
                        omergaList.Add(item_set2);
                    }
            return omergaList;
        }

        private ElemProb calProp(IList<ElemProb> probs, float propSet = 1f, string Opr = "in") //SE satified elements
        {
            ElemProb reVal = new ElemProb(0, 1); //ME

            #region cal Prop of Ps
            var fstProb = probs.First();
            foreach (var item in probs)
            {
                if (item == fstProb) continue;
                switch (Opr)
                {
                    case "in":
                        reVal.lowerBound = fstProb.lowerBound * item.lowerBound;
                        reVal.upperBound = fstProb.upperBound * item.upperBound;
                        fstProb = reVal;
                        break;
                    case "ig":
                        reVal.lowerBound = Math.Max(0, fstProb.lowerBound + item.upperBound - 1);
                        reVal.upperBound = Math.Min(fstProb.lowerBound, item.upperBound);
                        fstProb = reVal;
                        break;
                    case "me":
                        reVal.lowerBound = 0;                        // ^_^ //
                        reVal.upperBound = 0;
                        break;
                    default: break;
                }
            }
            #endregion
            return new ElemProb(propSet * reVal.lowerBound, propSet * reVal.upperBound);
        }

        private string getStandardAttrName(string attributeName)
        {
            if (attributeName.IndexOf('.') != -1)
            {
                var reVal = attributeName.Trim();
                return reVal.Substring(reVal.IndexOf('.') + 1).ToString();  //discard a dot
            }
            return attributeName;
        }
        private static string getStandardAttrName_static(string attributeName)
        {
            if (attributeName.IndexOf('.') != -1)
            {
                var reVal = attributeName.Trim();
                return reVal.Substring(reVal.IndexOf('.') + 1).ToString();  //discard a dot
            }
            return attributeName;
        }
        private bool equalSchemaName(string qryAttr1, string qryAttr2)
        {
            var schemaName1 = getRelationNamebyStrAttr(qryAttr1);
            var schemaName2 = getRelationNamebyStrAttr(qryAttr2);
            if (schemaName1.Equals(schemaName2, StringComparison.CurrentCultureIgnoreCase)) return true;
            return false;
        }
        private string getRelationNamebyStrAttr(string attr)
        {
            return attr.Split('.').FirstOrDefault().Trim().ToString();
        }
        public static PRelation unionOperator(PRelation pRelation1, PRelation pRelation2)
        {
            //clone
            var reVal = new PRelation()
            {
                id = pRelation1.id,
                schema = pRelation1.schema,
                relationName = pRelation1.relationName,
                tupes = pRelation1.tupes
            };
            foreach (var item in pRelation2.tupes)
            {
                reVal.tupes.Add(item);
            }
            return reVal;
        }
        public static PRelation intersertOperator(PRelation pRelation1, PRelation pRelation2,IList<PAttribute> pAttributes)
        {
            //clone
            var reVal = new PRelation();
           

                    for (int i = 0; i < pRelation1.tupes.Count; i++)
                    {
                        for (int j = 0; j < pRelation2.tupes.Count; j++)
                        {
                            if (check_e_Val_eq(pRelation1.tupes[i], pRelation2.tupes[j], pAttributes))
                            {
                                reVal.tupes.Add(getTupleCollapse_static(pRelation1.tupes[i], pRelation2.tupes[j], pAttributes));
                                //pRelation.tupes.RemoveAt(j);
                            }
                        }
                    }
            return reVal;
        }
        public static PRelation exceptOperator(PRelation pRelation1, PRelation pRelation2, IList<PAttribute> pAttributes)
        {
            var reVal = new PRelation()
            {
                id = pRelation1.id,
                schema = pRelation1.schema,
                relationName = pRelation1.relationName,
                tupes = pRelation1.tupes
            };
           // pRelation2.tupes.ToList().ForEach(p => reVal.tupes.Add(p));

            var interset = intersertOperator(pRelation1, pRelation2, pAttributes);
            for (int i = 0; i < reVal.tupes.Count; i++)
            {
                for (int j = 0; j < interset.tupes.Count; j++)
                {
                    if (check_e_Val_eq(reVal.tupes[i], interset.tupes[j], pAttributes))
                    {
                        try
                        {
                            reVal.tupes.RemoveAt(i);
                        }
                        catch { continue; }
                    }

                }
            }
            return reVal;
        }
        private static bool check_e_Val_eq(PTuple pTuple_1, PTuple pTuple_2, IList<PAttribute> pAttributes)
        {
            var probs = new List<float>();
            var eulerElem = new ElemProb(0, 0);
            foreach (var att in pAttributes)
            {
                if (!getStandardAttrName_static(att.AttributeName).Equals(ContantCls.emlementProb, StringComparison.CurrentCultureIgnoreCase))
                {
                    #region 
                    //check equal
                    float p = 1f;
                    var innerSet = getInterset(ref p, pTuple_1.valueSet[att.AttributeName], pTuple_2.valueSet[att.AttributeName], att.Type.TypeName);

                    if (!(innerSet is null) && innerSet.Count != 0)
                    {
                        if (pAttributes.Count == 1) { eulerElem.upperBound = 1;eulerElem.lowerBound = 1; }
                        else
                        {
                            if (probs.Count > 0)
                            {
                                probs.Add(p);
                                eulerElem = calProp_e_static(ref probs);
                            }
                            else probs.Add(p);
                        }
                       
                    }
                        
                    else
                        return false;

                    #endregion
                }

            }

            return Parameter.eulerThreshold <= eulerElem.lowerBound && eulerElem.upperBound >= Parameter.eulerThreshold;
        }
    }
}
