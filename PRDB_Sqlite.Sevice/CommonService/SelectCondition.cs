﻿using PRDB_Sqlite.Domain.Model;
using PRDB_Sqlite.Domain.Unit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PRDB_Sqlite.Sevice.CommonService
{
    public class SelectCondition
    {
        // xu ly cau dieu kien where
        
        public PRelation relations { get; set; }
        public PTuple tuple { get; set; }
        public string conditionString { get; set; }
        static public string[] Operator = new string[18] { "_<", "_>", "<=", ">=", "_=", "!=", "⊗_in", "⊗_ig", "⊗_me", "⊕_in", "⊕_ig", "⊕_me", "⊖_me", "⊖_in", "⊖_ig", "equal_in", "equal_ig", "equal_me" };
        private readonly IList<PAttribute> Attributes = new List<PAttribute>();
        public string MessageError { get; set; }
        public SelectCondition(PRelation probRelation, string conditionString)
        {
            // TODO: Complete member initialization
            relations = probRelation;


            int i = 0;
            while (i < conditionString.Length - 1)
            {
                if (conditionString[i] == '<' && conditionString[i + 1] != '=')
                    conditionString = conditionString.Insert(i++, "_");
                if (conditionString[i] == '>' && conditionString[i + 1] != '=')
                    conditionString = conditionString.Insert(i++, "_");
                if (conditionString[i] == '=' && conditionString[i - 1] != '!' && conditionString[i - 1] != '<' && conditionString[i - 1] != '>')
                    conditionString = conditionString.Insert(i++, "_");
                i++;
            }
            this.conditionString = conditionString.Trim();
            this.Attributes = probRelation.schema.Attributes;
            this.MessageError = string.Empty;
        }

        public SelectCondition()
        {
            // TODO: Complete member initialization
        }
        #region check Expression
        public bool CheckConditionString()
        {
            //sample select * from patient where (patient.bloodtype={A})[0.5,0.7]

            string str = this.conditionString.ToLower();
            int indexI = 0;

            str = str.Trim();

            if (!str.Contains('(') || !str.Contains(')') || !str.Contains('[') || !str.Contains(']'))
            {
                MessageError = "Incorrect syntax near the keyword 'where'.";
                return false;
            }



            if (str[0] != '(')
            {
                MessageError = "Incorrect syntax near the keyword 'where'.";
                return false;
            }

            if (str[str.Length - 1] != ']')
            {
                MessageError = "Incorrect syntax near the keyword 'where'.";
                return false;
            }


            while (indexI < str.Length - 1)
            {
                if (str[indexI] == '(')
                {
                    int indexJ = indexI + 1;
                    while (indexJ < str.Length)
                    {
                        if (str[indexJ] == '(' || str[indexJ] == '[' || str[indexJ] == ']')
                        {
                            MessageError = "Incorrect syntax near the keyword 'where'.";
                            return false;
                        }
                        if (str[indexJ] == ')')
                            break;
                        indexJ++;
                    }
                    if (indexJ == str.Length)
                    {
                        MessageError = "Incorrect syntax near the keyword 'where'.";
                        return false;
                    }
                }


                if (str[indexI] == '[')
                {
                    int indexJ = indexI + 1;

                    while (indexJ < str.Length)
                    {
                        if (str[indexJ] == '(' || str[indexJ] == '[' || str[indexJ] == '}')
                        {
                            MessageError = "Incorrect syntax near the keyword 'where'.";
                            return false;
                        }
                        if (str[indexJ] == ']')
                            break;
                        indexJ++;
                    }

                    if (indexJ == str.Length)
                    {
                        MessageError = "Incorrect syntax near the keyword 'where'.";
                        return false;
                    }
                }

                if (str[indexI] == '(' && str[indexI + 1] == '(')
                {
                    MessageError = "Incorrect syntax near the keyword 'where'.";
                    return false;
                }
                if (str[indexI] == '(' && str[indexI + 1] == ')')
                {
                    MessageError = "Incorrect syntax near the keyword 'where'.";
                    return false;
                }
                if (str[indexI] == '[' && str[indexI + 1] == '[')
                {
                    MessageError = "Incorrect syntax near the keyword 'where'.";
                    return false;
                }
                if (str[indexI] == ']' && str[indexI + 1] == ']')
                {
                    MessageError = "Incorrect syntax near the keyword 'where'.";
                    return false;
                }

                indexI++;
            }
            return true;
        }
        private static string CutSpareSpace(string S)
        {
            string result = "";
            for (int i = 0; i < S.Length; i++)
                if (S[i] == ' ')
                {
                    if (S[i - 1] != ' ') result += S[i];
                }
                else result += S[i];
            return result;

        }
        private static string converConditionStringToExpression(string conditionString)
        {

            try
            {
                return conditionString.Substring(conditionString.IndexOf("("), conditionString.IndexOf(")") - conditionString.IndexOf("(") + 1);

            }
            catch (Exception)
            {

                return string.Empty;
            }

        }
        private static List<double> convertConditionStringToProbInterVal(string S) //(patient.bloodtype_= 'b')[1,1]
        {
            List<double> interval = new List<double>();
            try
            {
                // Get Probabilistic Interval
                int j1 = S.IndexOf('[') + 1, j2 = S.IndexOf(']') - 1;
                string StrInterval = S.Substring(j1, j2 - j1 + 1);
                S = S.Replace(String.Format("[{0}]", StrInterval), "");
                StrInterval = StrInterval.Replace(" ", "");
                string[] StrProb = StrInterval.Split(',');
                double _MinProb = Convert.ToDouble(StrProb[0]);
                double _MaxProb = Convert.ToDouble(StrProb[1]);
                interval.Add(Convert.ToDouble(_MinProb));
                interval.Add(Convert.ToDouble(_MaxProb));
                return interval;

            }
            catch (Exception)
            {

                return null;
            }

        }
        private ElemProb convertConditionStrToProbInterVal(string S) //(patient.bloodtype_= 'b')[1,1]
        {
            try
            {
                // Get Probabilistic Interval
                int j1 = S.IndexOf('[') + 1, j2 = S.IndexOf(']') - 1;
                string StrInterval = S.Substring(j1, j2 - j1 + 1);
                S = S.Replace(String.Format("[{0}]", StrInterval), "");
                StrInterval = StrInterval.Replace(" ", "");
                return new ElemProb(StrInterval);
            }
            catch (Exception ex)
            {
                MessageError = ex.Message;
                return null;
            }

        }
        private bool IsSelectionExpression(string conditionString)
        {

            if (conditionString.Contains(" and ") || conditionString.Contains(" or ") || conditionString.Contains(" not "))
            {
                MessageError = "\nAnd/or doesn\'t handle the probabilistic Interval";
                return false;

            }


            string str = conditionString.Substring(conditionString.IndexOf("(") + 1, conditionString.IndexOf(")") - 1); //lay phan value
            if (str == string.Empty || str.Contains("(") || str.Contains(")"))
            { //check valid conStr
                MessageError = "\nCondition String is Invalid";
                return false;
            }

            if (convertConditionStrToProbInterVal(conditionString) == null)
                return false;


            List<string> arrayStr = new List<string>();

            if (str.Contains("'"))
            {


                int index = -1;
                for (int i = 0; i < str.Length - 1; i++)
                {
                    if (str[i] == ' ' && str[i + 1] == '\'')
                    {
                        index = i + 1;
                        break;
                    }
                }

                if (index != -1)
                {
                    string tmp = str.Substring(index, str.Length - index);
                    str = str.Substring(0, index - 1);
                    arrayStr.AddRange(str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    arrayStr.Add(tmp);
                }
                else
                {
                    arrayStr.AddRange(str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    int k = 0;
                    for (int i = 0; i < arrayStr.Count; i++)
                    {
                        if (isCompareOperator(arrayStr[i]))
                        {
                            k = i + 1;
                            break;
                        }
                    }

                    string valueError = "";
                    for (int i = k; i < arrayStr.Count; i++)
                    {
                        valueError += arrayStr[i] + " ";
                    }

                    MessageError = "Unclosed quotation mark before the character string " + valueError;
                    return false;
                }
            }
            else
            {
                arrayStr.AddRange(str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }

            for (int i = 1; i < arrayStr.Count - 1; i++)
            {
                if (Operator.Contains(arrayStr[i]) && (Operator.Contains(arrayStr[i - 1]) || Operator.Contains(arrayStr[i + 1])))
                {
                    MessageError = "Incorrect syntax near the keyword 'where'."; //ko co toan tu de thuc hien
                    return false;
                }

                if (!Operator.Contains(arrayStr[i]) && (!Operator.Contains(arrayStr[i - 1]) || !Operator.Contains(arrayStr[i + 1])))
                {
                    MessageError = "Incorrect syntax near the keyword 'where'.";
                    return false;
                }
            }

            return true;
        }
        #endregion


        #region kiểm tra bộ có thỏa mãn điều kiện chọn
        public bool Satisfied(PTuple tuple)
        {

            this.tuple = tuple;
            string conditionStr = this.conditionString;

            #region
            int i = 0;
            int j = 0;
            while (i < conditionStr.Length - 1)
            {
                if (conditionStr[i] == '(')
                {
                    j = i + 1;
                    while (j < conditionStr.Length)
                    {
                        if (conditionStr[j] == ']')
                        {

                            if (IsSelectionExpression(conditionStr.Substring(i, j - i + 1)))
                            {
                                string expValue = ExpressionValue(conditionStr.Substring(i, j - i + 1)) ? "1" : "0";
                                conditionStr = conditionStr.Insert(i, expValue); //tao sao ko get ra roi xai replace
                                conditionStr = conditionStr.Remove(i + 1, j - i + 1);
                                break;
                            }

                            if (MessageError != string.Empty)
                            {
                                return false;
                            }
                        }
                        j++;
                    }
                }
                i++;
            }

            #endregion          

            conditionStr = conditionStr.Replace(" ", "").ToLower();
            conditionStr = conditionStr.Replace("and", "&");
            conditionStr = conditionStr.Replace("or", "|");
            conditionStr = conditionStr.Replace("not", "!");
            List<string> rpn = SC_PostfixNotation(conditionStr);            // reverse to postfix notation 
            return CalculateCondition(rpn);
        }

        private bool ExpressionValue(string Str) //str is a single expression ex:"(patient.bloodtype_= 'b')"
        {
            // Get Probabilistic Interval
            ElemProb probInterval = convertConditionStrToProbInterVal(Str);
            // Get Selection Expression
            Str = converConditionStringToExpression(Str);
            List<string> array = new List<string>();
            // reverse to postfix notation 
            array = SE_PostfixNotation(Str);

            return CalculateExpression(array, probInterval.lowerBound, probInterval.upperBound);

        }
        private static string AddSeperateCharacter(string Str)
        {
            // Thêm vào các kí tự phân cách
            Str = Str.Replace("(", "|(|");
            Str = Str.Replace(")", "|)|");
            Str = Str.Replace("_<", "|_<|");
            Str = Str.Replace("_>", "|_>|");
            Str = Str.Replace("<=", "|<=|");
            Str = Str.Replace(">=", "|>=|");
            Str = Str.Replace("_=", "|_=|");
            Str = Str.Replace("!=", "|!=|");
            Str = Str.Replace("⊗_in", "|⊗_in|");
            Str = Str.Replace("⊗_ig", "|⊗_ig|");
            Str = Str.Replace("⊗_me", "|⊗_me|");
            Str = Str.Replace("⊕_in", "|⊕_in|");
            Str = Str.Replace("⊕_ig", "|⊕_ig|");
            Str = Str.Replace("⊕_me", "|⊕_me|");
            Str = Str.Replace("⊖_me", "|⊖_me|");
            Str = Str.Replace("⊖_me", "|⊖_in|");
            Str = Str.Replace("⊖_me", "|⊖_ig|");
            Str = Str.Replace("equal_in", "|equal_in|");
            Str = Str.Replace("equal_ig", "|equal_ig|");
            Str = Str.Replace("equal_me", "|equal_me|");

            while (Str[0] == '|') Str = Str.Remove(0, 1);

            int i;
            for (i = Str.Length; i >= 0; i--)
                if (Str[i - 1] != '|')
                    break;


            if (i < Str.Length)
                Str = Str.Remove(i);

            i = 0;
            while (i < Str.Length - 1)
            {
                if (Str[i] == ' ' && (Str[i + 1] == '|' || Str[i - 1] == '|'))
                    Str = Str.Remove(i, 1);
                i++;
            }
            return Str;
        }
        private static List<string> SE_PostfixNotation(string Str)
        {
            List<string> O = new List<string>();  // Stack
            List<string> V = new List<string>();
            string[] array = AddSeperateCharacter(Str).Split('|');


            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].CompareTo("(") == 0)
                    O.Add(array[i]);
                else
                    if (array[i].CompareTo(")") == 0)
                {
                    while (O.Count > 0 && O[O.Count - 1].CompareTo("(") != 0)
                    {
                        V.Add(O[O.Count - 1]);     // V.Add(" " + O[O.Count - 1] + " ");
                        O.RemoveAt(O.Count - 1);
                    }
                    O.RemoveAt(O.Count - 1);      // remove '('
                }
                else
                        if (Operator.Contains(array[i]))        // nếu là operator
                {
                    while (O.Count > 0 && Priority(O[O.Count - 1]) >= Priority(array[i]))
                    {
                        V.Add(O[O.Count - 1]);        // V.Add(" " + O[O.Count - 1] + " ");
                        O.RemoveAt(O.Count - 1);
                    }
                    O.Add(array[i]);
                }
                else
                    V.Add(array[i]);           // nếu là value
            }

            while (O.Count > 0)      // Sau khi kết thúc, lấy toàn bộ giá trị trong stack đưa vào V
            {
                V.Add(O[O.Count - 1]);
                O.RemoveAt(O.Count - 1);
            }
            return V;
        }

        private static int Priority(string Str)
        {
            switch (Str)
            {
                case "_<":
                case "_>":
                case "<=":
                case ">=":
                case "_=":
                case "!=":
                case "equal_ig":
                case "equal_in":
                case "equal_me": return 3;
                case "⊖_ig":
                case "⊖_in":
                case "⊖_me":
                case "⊗_ig":
                case "⊗_in":
                case "⊗_me":
                case "⊕_in":
                case "⊕_ig":
                case "⊕_me": return 2;
                case "|":
                case "&": return 2;
                case "!": return 3;
                default: return 0;
            }
        }


        private bool CalculateExpression(List<string> array, double L, double U)
        {
            List<string> stack = new List<string>();

            for (int i = 0; i < array.Count; i++)
                if (!Operator.Contains(array[i]))
                {
                    stack.Add(array[i]);
                }
                else         // Lấy hai giá trị ra khỏi stack và tính toán
                {
                    string valueTwo = stack[stack.Count - 1].Trim(); // get value
                    string valueOne = stack[stack.Count - 2].Trim();
                    stack.RemoveAt(stack.Count - 1); // xóa valueTwo
                    stack.RemoveAt(stack.Count - 1); // xóa valueOne

                    string value = GetProbInterval(valueOne, valueTwo, array[i].Trim());
                    if (value == string.Empty)
                    {
                        //this.conditionString = string.Empty; //kiem tra lai
                        return false;
                    }
                    stack.Add(value);
                }

            // Lấy khoảng xác suất của biểu thức chọn sau khi tính toán
            var tupleProp = new ElemProb(stack[0].Trim());

            return (L <= tupleProp.lowerBound && tupleProp.upperBound <= U); // trả về true nếu khoảng xác biểu thức chọn thỏa mãn xác xuất câu truy vấn
        }
        private string GetProbInterval(string valueOne, string valueTwo, string operaterStr)
        {
            ElemProb prop = null;

            int indexOne, indexTwo, countValue;
            PTuple tuple = this.tuple;
            //string typenameOne;
            //string typenameTwo;


            try
            {
                // Biểu thức so sánh bằng giữa hai thuộc tính trên cùng một bộ
                if (operaterStr.Contains("equal_ig") || operaterStr.Contains("equal_in") || operaterStr.Contains("equal_me"))
                {
                    indexOne = IndexOf(valueOne);
                    indexTwo = IndexOf(valueTwo);
                    if (indexOne == -1 || indexTwo == -1)
                        return string.Empty;

                    if (!Attributes[indexOne].Type.DataType.Equals(Attributes[indexTwo].Type.DataType))
                    {
                        //Attribute value does not match the data type !
                        MessageError = String.Format("Error :{0} and {1} must  the same data type", valueOne, valueTwo);
                        return string.Empty;
                    }

                    var low = 0f;
                    var up = 0f;
                    foreach (var valInAttr1 in tuple.valueSet[(valueOne)])
                    {
                        foreach (var valInAttr2 in tuple.valueSet[(valueTwo)])
                        {
                            if (EQUAL(valInAttr1.Trim(), valInAttr2.Trim(), Attributes[indexOne].Type.DataType))
                            {
                                switch (operaterStr)
                                {
                                    case "equal_in":
                                        low += tuple.Ps.lowerBound * tuple.Ps.upperBound;
                                        up = Math.Min(1, up + tuple.Ps.upperBound * tuple.Ps.upperBound);
                                        break;
                                    case "equal_ig":
                                        low += Math.Min(0, tuple.Ps.lowerBound + tuple.Ps.lowerBound - 1);
                                        up = Math.Min(1, up + Math.Min(tuple.Ps.upperBound, tuple.Ps.upperBound));
                                        break;
                                    case "equal_me":
                                        low = 0;
                                        up = Math.Min(1, up + 0); break;
                                    default: break;

                                }
                            }
                        }
                    }
                }
                else
                if (SelectCondition.isCompareOperator(operaterStr))     // Biểu thức so sánh giữa một thuộc tính với một giá trị
                {
                    indexOne = this.IndexOf(valueOne); // vị trí của thuộc tính trong ds các thuộc tính //what is THIS !?
                    if (indexOne == -1)
                        return string.Empty;

                    if (valueTwo.Contains("'"))
                    {

                        int count = valueTwo.Split(new char[] { '\'' }).Length - 1;


                        if (valueTwo.Substring(0, 1) != "'")
                        {
                            MessageError = "Unclosed quotation mark before the character string " + valueTwo;
                            return string.Empty;
                        }

                        if (valueTwo.Substring(valueTwo.Length - 1, 1) != "'")
                        {
                            MessageError = "Unclosed quotation mark after the character string " + valueTwo;
                            return string.Empty;
                        }


                        if (count != 2)
                        {
                            MessageError = "Unclosed quotation mark at the character string " + valueTwo;
                            return string.Empty;
                        }

                        valueTwo = valueTwo.Remove(0, 1);
                        valueTwo = valueTwo.Remove(valueTwo.Length - 1, 1);

                    }

                    PDataType dataType = new PDataType();
                    dataType.TypeName = Attributes[indexOne].Type.TypeName;
                    dataType.DataType = Attributes[indexOne].Type.DataType;
                    dataType.Domain = Attributes[indexOne].Type.Domain;
                    dataType.DomainString = Attributes[indexOne].Type.DomainString;

                    if (!dataType.CheckDataTypeOfVariables(valueTwo))
                    {
                        MessageError = String.Format("Conversion failed when converting the varchar value {0} to data type {1}.", valueTwo, Attributes[indexOne].Type.DataType.ToString());
                        return string.Empty;
                    }
                    countValue = tuple.valueSet[(valueOne)].Count;

                    prop = new ElemProb(0, 0);

                    foreach (var value in tuple.valueSet[(valueOne)])
                    {
                        if (this.Compare(value.ToString().Trim(), valueTwo.ToString(), operaterStr, Attributes[indexOne].Type.DataType.ToString())) //duyet tung value
                        {
                            try
                            {
                                prop.lowerBound += (1 / (float)countValue) ;
                                prop.upperBound += (1 / (float)countValue) ;
                            }
                            catch (Exception ex)
                            {
                                MessageError = ex.Message;
                            }
                            //break;
                        }
                    }
                    prop.lowerBound *= tuple.Ps.lowerBound;
                    prop.upperBound *= tuple.Ps.upperBound;

                }
                else                     // Biểu thức kết hợp giữa hai khoảng xác suất
                {
                    var prop1 = new ElemProb(valueOne);
                    var prop2 = new ElemProb(valueTwo);
                    switch (operaterStr)
                    {
                        case "⊗_ig": prop = new ElemProb(Math.Max(0, prop1.lowerBound + prop2.lowerBound - 1), Math.Min(prop1.upperBound, prop2.upperBound)); break;
                        case "⊗_in": prop = new ElemProb(prop1.lowerBound * prop2.lowerBound, prop1.upperBound * prop2.upperBound); break;
                        case "⊗_me": prop = new ElemProb(0, 0); break;
                        case "⊕_ig": prop = new ElemProb(Math.Max(prop1.lowerBound, prop2.lowerBound), Math.Min(1, prop1.upperBound + prop2.upperBound)); break;
                        case "⊕_in": prop = new ElemProb(prop1.lowerBound + prop2.lowerBound - (prop1.lowerBound * prop2.lowerBound), prop1.upperBound + prop2.upperBound - (prop1.upperBound * prop2.upperBound)); break;
                        case "⊕_me": prop = new ElemProb(Math.Min(1, prop1.lowerBound + prop2.lowerBound), Math.Min(1, prop1.upperBound + prop2.upperBound)); break;
                        case "⊖_ig": prop = new ElemProb(Math.Max(0, prop1.lowerBound - prop2.upperBound), Math.Min(prop1.upperBound,1-prop2.lowerBound));break;
                        case "⊖_in": prop = new ElemProb(prop1.lowerBound * (1 - prop2.upperBound), prop1.upperBound * (1 - prop2.lowerBound));break;
                        case "⊖_me": prop = new ElemProb(prop1.lowerBound, 1 - prop2.lowerBound);break;
                        default:
                            MessageError = "Incorrect syntax near 'where'.";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageError = "Incorrect syntax near 'where'.";
                return string.Empty;
            }

            //maxProb = 1 > maxProb ? maxProb : 1; // check maxProb
            if (prop is null)
            {
                return String.Empty;
            }
            return (String.Format("[{0},{1}]", prop.lowerBound, prop.upperBound));

        }
        public int IndexOf(string S)
        {
            string value = S.Trim().ToLower();
            int indexAttributeS = -1;
            if (value.IndexOf(".") != -1)
            {
                string[] arr = value.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                int count = 0;

                if (count == 2)
                {
                    MessageError = String.Format("The multi-part identifier {0} could not be bound.", value);
                    return -1;
                }


                for (int i = 0; i < Attributes.Count; i++)
                {
                    if (value == Attributes[i].AttributeName.ToLower())
                    {
                        return i;
                    }
                }

                MessageError = String.Format("Invalid attribute name '{0}'.", arr[1]);
                return -1;

            }
            else
            {
                int count = 0;
                for (int i = 0; i < Attributes.Count; i++)
                {
                    string[] arr = Attributes[i].AttributeName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (value == arr[1].ToLower().Trim())
                    {
                        count++;
                        indexAttributeS = i;
                    }
                }

                if (count >= 2)
                {
                    MessageError = String.Format("Ambiguous attribute name '{0}'.", value);
                    return -1;
                }
                if (count == 0)
                {
                    MessageError = String.Format("Invalid attribute name '{0}'.", value);
                    return -1;
                }

                return indexAttributeS;
            }
        }
        #endregion



        #region những hàm compare
        public static bool isCompareOperator(string S)
        {
            for (int i = 0; i < 6; i++)
                if (Operator[i].CompareTo(S) == 0)
                    return true;
            return false;
        }
        public static bool EQUAL(object a, object b, string type)
        {
            switch (type)
            {
                
                case "Int16":
                case "Int64":
                case "Int32":
                case "Byte":
                case "Currency": return int.Parse(a.ToString()) == int.Parse(b.ToString());
                case "DateTime":
                    //return SelectCondition.DtCompare(a.ToString(), b.ToString(), "_=");
                case "String":
                case "UserDefined":
                case "Binary": return (a.ToString().CompareTo(b.ToString()) == 0);
                case "Decimal":
                case "Single":
                case "Double": return (Math.Abs((double)a - (double)b) < 0.001);
                case "Boolean": return Boolean.Parse(a.ToString()) == Boolean.Parse(b.ToString());
                default: return (a.ToString().CompareTo(b.ToString()) == 0);break;
            }
        }
        public static bool IntCompare(Int16 valueOne, Int16 valueTwo, string OpratorStr)
        {
            switch (OpratorStr)
            {
                case "_<": return (valueOne < valueTwo);
                case "_>": return (valueOne > valueTwo);
                case "<=": return (valueOne <= valueTwo);
                case ">=": return (valueOne >= valueTwo);
                case "_=": return (valueOne == valueTwo);
                case "!=": return (valueOne != valueTwo);
                default: return false;
            }
        }
        public static bool StrCompare(string valueOne, string valueTwo, string OpratorStr)
        {

            switch (OpratorStr)
            {
                case "_=": return (valueOne.ToLower().CompareTo(valueTwo) == 0);
                case "!=": return (valueOne.ToLower().CompareTo(valueTwo) != 0);
                case "_<": return (valueOne.ToLower().ToArray()[0] < valueTwo.ToLower().ToArray()[0]);
                case "_>": return (valueOne.ToLower().ToArray()[0] > valueTwo.ToLower().ToArray()[0]);
                case "<=": return (valueOne.ToLower().ToArray()[0] <= valueTwo.ToLower().ToArray()[0]);
                case ">=": return (valueOne.ToLower().ToArray()[0] >= valueTwo.ToLower().ToArray()[0]);
                default: return false;
            }

        }
        public static bool DoubleCompare(double valueOne, double valueTwo, string OpratorStr)
        {
            switch (OpratorStr)
            {
                case "_<": return (valueOne < valueTwo);
                case "_>": return (valueOne > valueTwo);
                case "<=": return (valueOne <= valueTwo);
                case ">=": return (valueOne >= valueTwo);
                case "_=": return (Math.Abs(valueOne - valueTwo) < 0.001);
                case "!=": return (Math.Abs(valueOne - valueTwo) > 0.001);
                default: return false;
            }

        }
        public static bool BoolCompare(bool valueOne, bool valueTwo, string OpratorStr)
        {
            switch (OpratorStr)
            {
                case "_=": return (valueOne == valueTwo);
                case "!=": return (valueOne != valueTwo);
                default: return false;
            }

        }
        private static bool DtCompare(string valueOne, string valueTwo, string opratorStr)
        {

            try
            {
                var date1 = DateTime.ParseExact(valueOne, "MM/dd/yyyy", null);
                var date2 = DateTime.ParseExact(valueTwo, "MM/dd/yyyy", null);
                switch (opratorStr)
                {
                    case "_<": return (date1.CompareTo(date2) < 0);
                    case "_>": return (date1.CompareTo(date2) > 0);
                    case "<=": return (date1.CompareTo(date2) <= 0);
                    case ">=": return (date1.CompareTo(date2) >= 0);
                    case "_=": return (date1.CompareTo(date2) == 0);
                    case "!=": return (date1.CompareTo(date2) != 0);
                    default: return false;
                }
            }
            catch
            {
                throw new Exception("Cannot Convert data to DateTime!");
            }
           
        }
        public bool Compare(object valueOne, string valueTwo, string opratorStr, string typename)
        {
            switch (typename)
            {
                case "Int16":
                case "Int64":
                case "Int32":
                case "Byte":
                case "Currency":
                    return IntCompare(Convert.ToInt16(valueOne), Convert.ToInt16(valueTwo), opratorStr);
                case "DateTime": //chu y ngay
                    return DtCompare(valueOne.ToString(), valueTwo, opratorStr);
                case "String":
                case "UserDefined":
                case "Binary":
                    return StrCompare(valueOne.ToString(), valueTwo, opratorStr);
                case "Decimal":
                case "Single":
                case "Double":
                    return DoubleCompare((double)valueOne, Convert.ToDouble(valueTwo), opratorStr);
                case "Boolean":
                    return BoolCompare((bool)valueOne, Convert.ToBoolean(valueTwo), opratorStr);
                default: return false;
            }

        }

       
        #endregion




        private static bool CalculateCondition(List<string> Str)
        {
            List<string> stack = new List<string>();
            string valueOne, valueTwo;

            for (int i = 0; i < Str.Count; i++)
                if (Str[i].CompareTo("!") == 0)
                {
                    valueOne = stack[stack.Count - 1].Trim();
                    stack.RemoveAt(stack.Count - 1);
                    stack.Add((valueOne.CompareTo("1") == 0) ? "0" : "1");
                }
                else
                    if (Str[i].CompareTo("&") == 0 || Str[i].CompareTo("|") == 0)         // Lấy hai giá trị ra khỏi stack và tính toán
                {
                    valueTwo = stack[stack.Count - 1].Trim();
                    valueOne = stack[stack.Count - 2].Trim();
                    stack.RemoveAt(stack.Count - 1);
                    stack.RemoveAt(stack.Count - 1);
                    bool v1 = (valueOne.CompareTo("1") == 0) ? true : false;
                    bool v2 = (valueTwo.CompareTo("1") == 0) ? true : false;
                    switch (Str[i])
                    {
                        case "&": stack.Add(v1 && v2 ? "1" : "0"); break;
                        case "|": stack.Add(v1 || v2 ? "1" : "0"); break;
                        default: break;
                    }
                }
                else
                    stack.Add(Str[i]);          // Nếu là giá trị thì add vào stack

            return (stack[0].CompareTo("1") == 0);
        }
        private List<string> SC_PostfixNotation(string S)
        {
            List<string> O = new List<string>();  // Stack
            List<string> V = new List<string>();

            S = S.Replace("(", " ( ");
            S = S.Replace(")", " ) ");
            S = S.Replace("&", " & ");
            S = S.Replace("|", " | ");
            S = S.Replace("!", " ! ");
            S = CutSpareSpace(S.Trim());

            string[] A = S.Split(' ');

            for (int i = 0; i < A.Length; i++)
            {
                if (A[i].CompareTo("(") == 0)
                    O.Add(A[i]);
                else
                    if (A[i].CompareTo(")") == 0)
                {
                    while (O.Count > 0 && O[O.Count - 1].CompareTo("(") != 0)
                    {
                        V.Add(O[O.Count - 1]);     // V.Add(" " + O[O.Count - 1] + " ");
                        O.RemoveAt(O.Count - 1);
                    }
                    O.RemoveAt(O.Count - 1);      // remove '('
                }
                else
                        if (A[i].CompareTo("&") == 0 || A[i].CompareTo("|") == 0 || A[i].CompareTo("!") == 0)        // operator
                {
                    while (O.Count > 0 && Priority(O[O.Count - 1]) >= Priority(A[i]))
                    {
                        V.Add(O[O.Count - 1]);        // V.Add(" " + O[O.Count - 1] + " ");
                        O.RemoveAt(O.Count - 1);
                    }
                    O.Add(A[i]);
                }
                else
                    V.Add(A[i]);           // value
            }


            while (O.Count > 0)      // Sau khi kết thúc, lấy toàn bộ giá trị trong stack đưa vào V
            {
                V.Add(O[O.Count - 1]);
                O.RemoveAt(O.Count - 1);
            }
            return V;
        }
        private string getStandardAttrName(string attributeName)
        {
            var reVal = attributeName.Trim();
            return reVal.Substring(reVal.IndexOf('.') + 1).ToString();  //discard a dot
        }


    }

}
