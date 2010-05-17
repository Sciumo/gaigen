// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

// Copyright 2008-2010, Daniel Fontijne, University of Amsterdam -- fontijne@science.uva.nl

using System;
using System.Collections.Generic;
using System.Text;

namespace G25
{

    /// <summary>
    /// 
    /// The G25.fgs class holds a Function Generation Specification. This means a request of the
    /// code generator back-end to generate a specific function over specific multivector/outermorphism 
    /// arguments. When a G25.fgs is constructed, it is not checked whether the content is valid. It is 
    /// up to the actual code generator to complain (during code generation).
    /// 
    /// The function generation specification holds:
    ///   - a function name (this name must be recognized by the code generator).
    ///   - an output function name (optional). If this name is null, it is assumed to be the same as the function name.
    ///     This allows the user to override the default function names of the code generator.
    ///   - An override for the return type (optional). By default, the code generator will determine the return type
    ///     of the functions it generates, but it is possible to override this default by setting it explicitly.
    /// 
    ///    Some code genertors may set the return type at some point.
    ///   - typenames for the arguments (optional). This can be any type of multivector, outermorphism or floating point type.
    ///     Not all (combinations of) types may be valid for all functions for all code generators. This depends on the
    ///     capabilities of the code generator.
    /// 
    ///     If no argument typenames are given, the code generator will assume the defaults. The code generator
    ///     may then fill in the blanks itself.
    ///   - argument variable names (optional). If not null, this allows the user to specify the names of the arguments
    ///     inside the function itself. Otherwise default names 'a', 'b', etc are used.
    ///   - the floating point type(s) to use for the multivector and outermorphism types. Multiple types may be specified,
    ///     and the code generator should generate an implementation for each type.
    ///     
    /// If a function generation specification does not have any argument specifications, this means
    /// that the appropriate general multivector/outermorphism type will be used for each argument. 
    /// This is up to the code generator to actually implement.
    /// I.e., the code generator should be prepared to handle G25.fgs without any arguments, and fill in
    /// the blanks itself.
    /// 
    /// </summary>
    public class fgs : IComparable  
    {

        public static String RETURN_ARG_NAME = "_dst";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the requested function (must be known by code generator).</param>
        /// <param name="outputName">The name of the generated function (may be different from functionName); if null, is set to functionName.</param>
        /// <param name="returnTypeName">Override of return type (may be null or "" for default).</param>
        /// <param name="argumentTypeNames">Typenames of arguments (may be multivector or outermorphism, or regular floating point type).</param>
        /// <param name="argumentVariableNames">The names of the arguments inside the function (if null, set to "a", "b", "c", etc).</param>
        /// <param name="floatNames">Name(s) of floating point type(s) for which the function should be generated.</param>
        /// <param name="metricName">The metric to be used (default = "default", used when metricName = null).</param>
        /// <param name="comment">Optional comments. Can be null.</param>
        /// <param name="options">Options as specified by <c>optionXXX=""</c> attributes in the original XML file (can be null).</param>
        public fgs(string name, string outputName, 
            string returnTypeName,
            string[] argumentTypeNames, string[] argumentVariableNames, 
            string[] floatNames,
            string metricName,
            string comment, 
            Dictionary<String, String> options)
        {
            // check if optional arguments have been specified:
            if (outputName == null) outputName = name;

            if (argumentTypeNames == null) argumentTypeNames = new string[0];

            if (argumentVariableNames == null) {                
                argumentVariableNames = new string[argumentTypeNames.Length];
                for (int i = 0; i < argumentTypeNames.Length; i++)
                    argumentVariableNames[i] = DefaultArgumentName(i);  // "a", "b", "c", "d", etc
            }
            else if (argumentTypeNames.Length != argumentVariableNames.Length)
                throw new Exception("G25.fgs.fgs(): invalid number of argument variable names (must match the number of argument type names)");
            if (returnTypeName == null) returnTypeName = "";
            if (comment == null) comment = "";

            // set values:
            m_name = name;
            m_outputName = outputName;
            m_returnTypeName = returnTypeName;
            m_argumentTypeNames = argumentTypeNames;
            m_argumentVariableNames = argumentVariableNames;
            m_argumentPtr = new bool[0];
            m_argumentArr = new bool[0];
            m_floatNames = floatNames;
            m_metricName = (metricName == null) ? "default" : metricName;
            m_comment = comment;
            m_options = (options == null) ? new Dictionary<String, String>() : options;

            m_hashCode = ComputeHashCode();
        }

        /// <summary>
        /// Copies 'F', but changes the function OutputName (m_outputName). 
        /// 
        /// This constructor is used mostly to change the name of a function when 
        /// the output language does not support overloading.
        /// </summary>
        /// <param name="F">fgs to be copied.</param>
        /// <param name="newName">New name for the copy of 'F'.</param>
        public fgs(fgs F, string newName)
        {
            m_name = F.m_name;
            m_outputName = newName;
            m_returnTypeName = F.m_returnTypeName;
            m_argumentTypeNames = F.m_argumentTypeNames;
            m_argumentVariableNames = F.m_argumentVariableNames;
            m_argumentPtr = F.m_argumentPtr;
            m_argumentArr = F.m_argumentArr;
            m_floatNames = F.m_floatNames;
            m_metricName = F.m_metricName;
            m_comment = F.m_comment;
            m_options = F.m_options;

            m_hashCode = ComputeHashCode();
        }


        
        /// <summary>
        /// Must be called by constructor to initialize m_hashCode (after all member variables have been set)
        /// 
        /// m_outputName does not contribute to hash code.
        /// </summary>
        protected int ComputeHashCode()
        {
            int hashCode =
                m_name.GetHashCode() ^
//                (m_outputName.GetHashCode() << 1) ^
                m_returnTypeName.GetHashCode() ^
                m_metricName.GetHashCode();

            {
                String[] options = OptionsToStringArray();
                for (int i = 0; i < options.Length; i++)
                    hashCode ^= options[i].GetHashCode() << i;
            }

            if (m_argumentTypeNames != null)
                for (int i = 0; i < m_argumentTypeNames.Length; i++)
                    hashCode ^= m_argumentTypeNames[i].GetHashCode() << i;

            if (m_argumentVariableNames != null)
                for (int i = 0; i < m_argumentVariableNames.Length; i++)
                    hashCode ^= m_argumentVariableNames[i].GetHashCode() << (1 + i);

            if (m_argumentPtr != null)
                for (int i = 0; i < m_argumentPtr.Length; i++)
                    hashCode ^= m_argumentPtr[i].GetHashCode() << (1 + i);

            if (m_argumentArr != null)
                for (int i = 0; i < m_argumentArr.Length; i++)
                    hashCode ^= m_argumentArr[i].GetHashCode() << (1 + i);

            // do not use comment for hashcode!

            if (m_floatNames != null)
                for (int i = 0; i < m_floatNames.Length; i++)
                    hashCode ^= m_floatNames[i].GetHashCode() << (2 + i);

            return hashCode;
        }

        /// <summary>
        /// Note: the hash code will be invalid if you write to m_argumentTypeNames, m_argumentVariableNames or m_returnTypeName
        /// for fill-in.
        /// 
        /// NOTE: cg_data.cs contains a dictionary Dictionary<G25.fgs, int> of m_fgs, which is searched.
        /// Still it doesn't matter because these are never altered???
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return m_hashCode;
        }

        /// <summary>
        /// Return true when this equals the value of 'obj'.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null) return false;

            // If parameter cannot be cast to fgs return false.
            fgs B = obj as fgs;
            if (B == null) return false;
            else return (CompareTo(obj) == 0);
        }


        /// <summary>
        /// IComparable.CompareTo implementation.
        /// 
        /// This function is not used to find function by FindFunctionEx()
        /// because only part of the FGS needs to be compared for that.
        /// 
        /// m_outputName is not compared.
        /// </summary>
        /// <param name="obj">The object to which 'this' is compared</param>
        public int CompareTo(object obj)
        {
            if (obj is fgs)
            {
                fgs B = (fgs)obj;

                int C; 
                if ( (C = m_name.CompareTo(B.m_name)) != 0) return C;
//                if ((C = m_outputName.CompareTo(B.m_outputName)) != 0) return C;
                if ((C = CompareArrays(m_argumentTypeNames, B.m_argumentTypeNames)) != 0) return C;
                if ( (C = m_returnTypeName.CompareTo(B.m_returnTypeName)) != 0) return C;
                if ( (C = m_metricName.CompareTo(B.m_metricName)) != 0) return C;
                // do not use compare the comment!
                if ((C = CompareArrays(m_floatNames, B.m_floatNames)) != 0) return C;

                if ((C = CompareArrays(m_argumentVariableNames, B.m_argumentVariableNames)) != 0) return C;

                if ((C = CompareBoolArrays(m_argumentPtr, B.m_argumentPtr)) != 0) return C;

                if ((C = CompareBoolArrays(m_argumentArr, B.m_argumentArr)) != 0) return C;

                if (m_options.Count < B.m_options.Count) return -1;
                else if (m_options.Count > B.m_options.Count) return 1;
                else {
                    // convert options to arrays, then compare:
                    String[] OA = OptionsToStringArray();
                    String[] OB = B.OptionsToStringArray();

                    if ((C = CompareArrays(OA, OB)) != 0) return C;
                }

                return 0; // this equals 'B'
            }
            throw new ArgumentException("object is not a G25.fgs");
        }



        /// <summary>
        /// Why is there not standard library function for this one?
        /// </summary>
        /// <param name="A1">May be null</param>
        /// <param name="A2">May be null</param>
        /// <returns>-1 is A1 < A2, 0 is A1 == A2, +1 if A1 > A2</returns>
        public static int CompareArrays(IComparable[] A1, IComparable[] A2)
        {
            if (A1 == null)
            {
                return (A2 == null) ? 0 : -1;
            }
            else if (A2 == null) return 1;
            else
            {
                if (A1.Length < A2.Length) return -1;
                else if (A1.Length > A2.Length) return 1;

                int C;
                for (int i = 0; i < A1.Length; i++)
                    if ((C = A1[i].CompareTo(A2[i])) != 0) return C;

                return 0;
            }
        }

        public static int CompareBoolArrays(bool[] A1, bool[] A2)
        {
            if (A1 == null)
            {
                return (A2 == null) ? 0 : -1;
            }
            else if (A2 == null) return 1;
            else
            {
                if (A1.Length < A2.Length) return -1;
                else if (A1.Length > A2.Length) return 1;

                //int C;
                for (int i = 0; i < A1.Length; i++)
                    if (A1[i] && (!A2[i])) return 1;
                    else if ((!A1[i]) && A2[i]) return -1;


                return 0;
            }
        }

        /// <summary>
        /// Converts options to array of strings. Used for comparing, hashcode, etc.
        /// </summary>
        /// <returns></returns>
        public String[] OptionsToStringArray() 
        {
            String[] str = new String[m_options.Count];
            int idx = 0;
            foreach (KeyValuePair<String, String> KVP in m_options) str[idx++] = "__" + KVP.Key + "__" + KVP.Value + "__";
            return str;
        }

        /// <summary>
        /// Merges the default comment (as determined by the function plugin) with the
        /// (optional) user comment.
        /// </summary>
        /// <param name="defaultComment">Default comment of function.</param>
        /// <returns>Default comment plus optional user comment.</returns>
        public String AddUserComment(String defaultComment)
        {
            if (Comment.Length == 0) return defaultComment;
            else return defaultComment + "\n" + Comment;
        }


        /// <summary>The name of the requested function (for example "gp").</summary>
        public String Name { get { return m_name; } }
        /// <summary>The name of the generated function (may be different from FunctionName, for example, "geometricProduct" instead of "gp"). 
        /// Always has a valid value, never null.
        /// Is not used in hash code computation and CompareTo.</summary>
        public String OutputName { get { return m_outputName; } }
        /// <summary>Override for the return type; to use the default (as determined by the code generator) set to "".</summary>
        public String ReturnTypeName { get { return m_returnTypeName; } set { m_returnTypeName = value; } }
        /// <summary>The number of arguments listed.</summary>
        public int NbArguments { get { return m_argumentTypeNames.Length; } }
        /// <summary>Typenames of arguments (may be multivector or outermorphism, or regular floating point type). Has the same length as ArgumentVariableNames.</summary>
        public String[] ArgumentTypeNames { get { return m_argumentTypeNames; } }
        /// <summary>The names of the arguments inside the function ("a", "b", "c", etc by default).. Has the same length as ArgumentTypeNames.</summary>
        public String[] ArgumentVariableNames { get { return m_argumentVariableNames; } }
        /// <summary>Whether arguments are pointers or not. After fill-in always has the same length as ArgumentTypeNames.</summary>
        public bool[] ArgumentPtr { get { return m_argumentPtr; } }
        /// <summary>Whether arguments are arrays or not. After fill-in always has the same length as ArgumentTypeNames.</summary>
        public bool[] ArgumentArr { get { return m_argumentArr; } }
        /// <summary>The number of floating point typenames listed.</summary>
        public int NbFloatNames { get { return this.m_floatNames.Length; } }
        /// <summary>Name(s) of floating point type(s) for which the function should be generated.</summary>
        public String[] FloatNames { get { return m_floatNames; } }
        /// <summary>Name of metric for metric products.</summary>
        public String MetricName { get { return m_metricName; } }
        /// <summary>Returns optional user comments. These should be appended to the the standard by the code generation plugin.
        /// </summary>
        public String Comment { get { return m_comment; } }

        /// <summary>
        /// Returns all options in the form of a dictionary.
        /// </summary>
        public Dictionary<String, String> Options { get { return m_options; } }

        /// <summary>
        /// Gets an options for a specific name.
        /// </summary>
        /// <param name="name">The options you want.</param>
        /// <returns>The value of the option, or null if none set.</returns>
        public String GetOption(String name)
        {
            if (m_options.ContainsKey(name.ToLower())) return m_options[name.ToLower()];
            else return null;
        }

        /// <summary>Used by code generators to check whether the number of arguments matches.</summary>
        /// <param name="nb">The required number of arguments.</param>
        /// <returns>true if the number of arguments is either 0 (which means: use default arguments) or equal to 'nb'</returns>
        public bool MatchNbArguments(int nb)
        {
            return ((NbArguments == 0) || (NbArguments == nb));
        }

        /// <summary>
        /// Used to get the typename of an argument, regardless of whether the user specified
        /// it or not. If the user did not specify a typename, the 'defaultTypeName' is returned.
        /// 
        /// When argIdx < 0, always returns 'defaultTypeName'. (used for artificial return argument).
        /// </summary>
        /// <param name="argIdx">Index of argument.</param>
        /// <param name="defaultTypeName">Default typename, used when user has not specified any argument.</param>
        /// <returns>typename of argument 'argIdx'.</returns>
        public String GetArgumentTypeName(int argIdx, String defaultTypeName) {
            if ((argIdx < 0) || (NbArguments == 0)) return defaultTypeName;
            else if (argIdx < NbArguments) return ArgumentTypeNames[argIdx];
            else throw new Exception("G25.fgs.GetArgumentTypeName(): index out of range");
        }

        /// <summary>
        /// Returns the name of argument 'argIdx'. If the user did not specify a name, the 
        /// default name is returned.
        /// </summary>
        /// <param name="argIdx">Index of argument.</param>
        /// <returns>name of argument 'argIdx'.</returns>
        public String GetArgumentName(int argIdx)
        {
            if (argIdx < 0) return RETURN_ARG_NAME;
            if (NbArguments == 0) return DefaultArgumentName(argIdx);
            else return ArgumentVariableNames[argIdx];
        }

        /// <summary>
        /// Returns the whether argument 'argIdx' is a pointer. 
        /// If not filled in, 'false' is returned.
        /// </summary>
        /// <param name="S">Specification.</param>
        /// <param name="argIdx">Index of argument.</param>
        /// <returns>name of argument 'argIdx'.</returns>
        public bool GetArgumentPtr(Specification S, int argIdx)
        {
            if (argIdx < 0) return (S.m_outputLanguage == OUTPUT_LANGUAGE.C) ? true : false;
            else if (NbArguments == 0) return false;
            else return ArgumentPtr[argIdx];
        }

        /// <summary>
        /// Returns the whether argument 'argIdx' is an array.
        /// If not filled in, 'false' is returned.
        /// </summary>
        /// <param name="S">Specification.</param>
        /// <param name="argIdx">Index of argument.</param>
        /// <returns>name of argument 'argIdx'.</returns>
        public bool GetArgumentArr(Specification S, int argIdx)
        {
            if (argIdx < 0) return false;
            else if (NbArguments == 0) return false;
            else return ArgumentArr[argIdx];
        }

        public override string ToString()
        {
            // return Name|OutputName[FloatNames](ArgumentTypeNames Ptr? ArgumentVariableNames);
            StringBuilder SB = new StringBuilder();
            SB.Append(Name);
            if (OutputName != Name) {
                SB.Append("|");
                SB.Append(OutputName);
            }
            SB.Append("[");
            for (int i = 0; i < FloatNames.Length; i++) {
                if (i > 0) SB.Append(", ");
                SB.Append(FloatNames[i]);
            }
            SB.Append("](");
            for (int i = 0; i < ArgumentTypeNames.Length; i++) {
                if (i > 0) SB.Append(", ");
                SB.Append(ArgumentTypeNames[i]);
                SB.Append(" ");
                if (ArgumentPtr[i]) SB.Append("*");
                SB.Append(ArgumentVariableNames[i]);
            }
            SB.Append(");");

            return SB.ToString();
        }

        /// <returns>the default name for argument 'idx'. (idx=0 -> "a", idx=1 -> "b", and so on)</returns>
        public static string DefaultArgumentName(int idx)
        {
            char name = (char)('a' + idx);
            return name.ToString();
        }

        /// <summary>Used to determine if a list of floating point type names matches the member list m_floatNames.</summary>
        /// <returns>true if all floating point typenames listed in 'floatNames' are also m_floatNames (and vice versa)</returns>
        public bool UsesAllFloatTypes(List<FloatType> floatTypes)
        {
            if (FloatNames.Length != floatTypes.Count) return false;
            for (int i = 0; i < floatTypes.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < FloatNames.Length; j++)
                {
                    if (floatTypes[i].type == FloatNames[j])
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether this fgs is a converter. To be a converted, the name
        /// of the function should start with and underscore (<c>_</c>) and the remaining
        /// part of the name should be a specialized multivector typename.
        /// </summary>
        /// <returns>true if this fgs is a converter (underscore constructor).</returns>
        public bool IsConverter(Specification S)
        {
            if ((Name.Length > 0) && (Name[0] == '_'))
                return (S.IsSpecializedMultivectorName(Name.Substring(1)));
            else return false;
        }


        /// <summary>
        /// Initializes <c>m_argumentPtr</c> automatically from the <c>m_argumentTypeNames</c>.
        /// The length of the <c>m_argumentPtr</c> array is adjusted to match <c>m_argumentTypeNames</c>.
        /// Existing values are not changed. The other values are set in a language-dependent way.
        /// (currently only for 'C' language).
        /// 
        /// Does nothing when <c>m_argumentTypeNames</c> is null.
        /// </summary>
        /// <param name="S">Used for <c>m_outputLanguage</c> and the types.</param>
        public void InitArgumentPtrFromTypeNames(Specification S)
        {
            if (m_argumentTypeNames == null) return;

            int i = (m_argumentPtr == null) ? 0 : m_argumentPtr.Length;
            if (i != m_argumentTypeNames.Length)
            {// resize the array
                System.Array.Resize<bool>(ref m_argumentPtr, m_argumentTypeNames.Length);
                System.Array.Resize<bool>(ref m_argumentArr, m_argumentTypeNames.Length);
            }

            if (S.m_outputLanguage == OUTPUT_LANGUAGE.C) {
                // if it is not a float or a double, then it is a pointer
                for (; i < m_argumentTypeNames.Length; i++)
                {
                    m_argumentPtr[i] = !S.IsFloatType(m_argumentTypeNames[i]);
                }
            }
        }

        /// <summary>
        /// Sets <c>m_argumentPtr</c>c> to <c>value</c> for argument <c>[argIdx]</c>.
        public void SetArgumentPtr(int argIdx, bool value) 
        {
            m_argumentPtr[argIdx] = value;
        }

        /// <summary>
        /// Sets <c>m_argumentArr</c>c> to <c>value</c> for argument <c>[argIdx]</c>.
        public void SetArgumentArr(int argIdx, bool value)
        {
            m_argumentArr[argIdx] = value;
        }

        /// <summary>
        /// The name of the requested function (this name must be 'recognized' by the code generator).
        /// </summary>
        protected readonly string m_name;
        /// <summary>
        /// The name of the generated function (may be different from m_name). Always has a valid value, never null.
        /// 
        /// This variable can be written to, such that code generators can fill it in (fill in the blanks).
        /// But never over write it when OutputName != Name because then the user want to
        /// override the default name.
        /// </summary>
        public string m_outputName;
        /// <summary>
        /// Override for the return type. The value is never null.
        /// To use the default (as determined by the code generator) set to "". 
        /// 
        /// This variable can be written to, such that code generators can fill it in (fill in the blanks).
        /// </summary>
        public string m_returnTypeName;
        /// <summary>
        /// Typenames of arguments (may be multivector or outermorphism, or regular floating point type).
        /// Must be of the same length as m_argumentVariableNames.
        /// 
        /// This variable can be written to, such that code generators can fill it in (fill in the blanks).
        /// </summary>
        public string[] m_argumentTypeNames;
        /// <summary>
        /// The names of the arguments inside the function ("a", "b", "c", etc by default).
        /// Must be of the same length as m_argumentTypeNames.
        /// 
        /// This variable can be written to, such that code generators can fill it in (fill in the blanks).
        /// </summary>
        public string[] m_argumentVariableNames;
        /// <summary>
        /// The whether arguments are pointers.
        /// Must be of the same length as m_argumentTypeNames.
        /// 
        /// This variable can be written to, such that code generators can fill it in (fill in the blanks).
        /// </summary>
        public bool[] m_argumentPtr;

        /// <summary>
        /// The whether arguments are arrays.
        /// Must be of the same length as m_argumentTypeNames.
        /// 
        /// This variable can be written to, such that code generators can fill it in (fill in the blanks).
        /// </summary>
        public bool[] m_argumentArr;

        /// <summary>
        /// Name(s) of floating point type(s) for which the function should be generated.
        /// </summary>
        protected readonly string[] m_floatNames;
        /// <summary>
        /// Name of metric used for metric products.
        /// </summary>
        protected readonly String m_metricName;
        /// <summary>
        /// Optional extra comments. Does not influence hashcode or comparison with other fgs.
        /// </summary>
        protected readonly String m_comment;

        /// <summary>
        /// Options as specified by <c>optionXXX=""</c> attributes.
        /// </summary>
        protected readonly Dictionary<String, String> m_options;

        protected readonly int m_hashCode;

    } // end of class fgs
} // end of namespace G25
