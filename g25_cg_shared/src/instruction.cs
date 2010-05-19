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

namespace G25.CG.Shared
{
    /// <summary>
    /// Interface for a high-level geometric algebra instruction, which 
    /// is used as an intermediate language for writing functions.
    /// 
    /// This class does do much by itself. Intended only as common superclass.
    /// It holds the number of tabs in front of the code, and possibly child instructions
    /// (used by if-else).
    /// 
    /// Examples of instructions are:
    ///   - G25.CG.Shared.AssignInstruction
    ///   - G25.CG.Shared.ReturnInstruction
    ///   - G25.CG.Shared.CommentInstruction
    ///   - G25.CG.Shared.VerbatimCodeInstruction
    ///   - G25.CG.Shared.IfElseInstruction
    /// 
    /// </summary>
    public class Instruction
    {
        /// <summary>
        /// Constructs an instruction containing no child instructions.
        /// </summary>
        /// <param name="nbTabs">How many tabs to put in front of generated code.</param>
        public Instruction(int nbTabs)
        {
            m_nbTabs = nbTabs;
        }

        /// <summary>
        /// Constructs an instruction withg child instructions.
        /// </summary>
        /// <param name="nbTabs">How many tabs to put in front of generated code.</param>
        /// <param name="childInstructions">Child instructions (may be null). Used IfElseInstruction instruction.</param>
        public Instruction(int nbTabs, List<List<Instruction> > childInstructions)
        {
            m_nbTabs = nbTabs;
            m_childInstructions = childInstructions;
        }

        /// <summary>
        /// Writes code for instruction to 'SB'.
        /// Subclass should override this function.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Specification of algebra.</param>
        /// <param name="cgd">Not used yet.</param>
        public virtual void Write(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            AppendTabs(SB);

            SB.AppendLine("/* to do: implement " + GetType().ToString() + " */");
        }

        /// <summary>
        /// Appends m_nbTabs tabs in front of the instruction.
        /// </summary>
        /// <param name="SB"></param>
        public void AppendTabs(StringBuilder SB)
        {
            SB.Append('\t', m_nbTabs);
        }

        /// <summary>
        /// Helper function to turn a single list of instruction into a list of lists of instructions.
        /// </summary>
        /// <param name="L1">The list of instructions</param>
        /// <returns>A list of list of instructions.</returns>
        public static List<List<Instruction>> MakeListOfLists(List<Instruction> L1)
        {
            List<List<Instruction>> LL = new List<List<Instruction>>();
            LL.Add(L1);
            return LL;
        }

        /// <summary>
        /// Helper function to turn a two lists of instruction into a list of lists of instructions.
        /// 
        /// Used by IfElseInstruction constructor.
        /// </summary>
        /// <param name="L1">The first list of instructions.</param>
        /// <param name="L2">The second list of instructions.</param>
        /// <returns>A list of list of instructions.</returns>
        public static List<List<Instruction>> MakeListOfLists(List<Instruction> L1, List<Instruction> L2)
        {
            List<List<Instruction>> LL = new List<List<Instruction>>();
            LL.Add(L1);
            LL.Add(L2);
            return LL;
        }


        /// <summary>
        /// Recurses through all instructions in 'I', searching for return instructions.
        /// 
        /// Collects their types and float point types in 'returnTypes' and 'returnTypeFT'.
        /// </summary>
        /// <param name="L">Instructions to be searched for return types.</param>
        /// <param name="returnTypes">Used as output.</param>
        /// <param name="returnTypeFT">Used as output.</param>
        public static void GetReturnType(List<Instruction> L, List<G25.VariableType> returnTypes, List<G25.FloatType> returnTypeFT)
        {
            foreach(Instruction I in L) 
            {
                if (I is ReturnInstruction)
                {
                    ReturnInstruction RI = I as ReturnInstruction;
                    returnTypes.Add(RI.m_type);
                    returnTypeFT.Add(RI.m_floatType);
                }

                if (I.m_childInstructions != null)
                    foreach (List<Instruction> childL in I.m_childInstructions)
                        GetReturnType(childL, returnTypes, returnTypeFT);
            }
        }




        /// <summary>
        /// How many tabs to put in front of code
        /// </summary>
        public readonly int m_nbTabs;

        /// <summary>
        /// Child Instructions of this Instruction.
        /// Used for example by if-else Instruction
        /// </summary>
        public readonly List<List<Instruction>> m_childInstructions;

    } // end of interface Instruction

    public class NOPinstruction : Instruction
    {
        /// <summary>
        /// Constructs an instruction containing no child instructions.
        /// </summary>
        public NOPinstruction()
            : base(0)
        {
        }

        /// <summary>
        /// Writes code for instruction to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Specification of algebra.</param>
        /// <param name="cgd">Not used yet.</param>
        public override void Write(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
        }
    }

    /// <summary>
    /// Superclass for both AssignInstruction and ReturnInstruction.
    /// (both these classes share a lot of data).
    /// </summary>
    public class AssignOrReturnInstruction : Instruction
    {
        /// <summary>
        /// Instruction for generating code for an assignment to or a return of a variable.
        /// </summary>
        /// <param name="nbTabs">How many tabs to put in front of code.</param>
        /// <param name="T">Type of assigned variable.</param>
        /// <param name="FT">Floating point type of coordinates of assigned variable.</param>
        /// <param name="mustCast">When assigning to variable, should the coordinates be cast to FT?</param>
        /// <param name="value">The assigned value.</param>
        public AssignOrReturnInstruction(int nbTabs, G25.VariableType T, G25.FloatType FT, bool mustCast, RefGA.Multivector value)
            : base(nbTabs)
        {
            m_type = T;
            m_floatType = FT;
            m_mustCast = mustCast;
            m_value = value;
        }

        /// <summary>
        /// Instruction for generating code for an assignment to or a return of a variable.
        /// 
        /// This version of the constructor allows for a 'post operation' to be applied to the 'value'.
        /// Because Gaigen 2.5 cannot (yet) find common subexpressions, it is hard to generate code like
        ///     <code>
        ///     A.c[0] = (expr) / n2;
        ///     A.c[1] = (expr) / n2;
        ///     etc
        ///     </code>
        /// because the division will distribute to all terms of expr. So instead you'd get:
        ///     <code>
        ///     A.c[0] = (expr1 / n2) + (expr2 / n2) + ... +  (exprn / n2);
        ///     A.c[1] = (expr1 / n2) + (expr2 / n2) + ... +  (exprn / n2);
        ///     etc
        ///     </code>
        /// So this hack allows one to specify a post
        /// operator (currently multiplier (<c>"*"</c>) or divisor (<c>"/"</c>) and a value to use for that purpose.
        /// </summary>
        /// <param name="nbTabs">How many tabs to put in front of code.</param>
        /// <param name="T">Type of assigned variable.</param>
        /// <param name="FT">Floating point type of coordinates of assigned variable.</param>
        /// <param name="mustCast">When assigning to variable, should the coordinates be cast to FT?</param>
        /// <param name="value">The assigned value.</param>
        /// <param name="postOp">'post operation'. Currently allows for global multiplier (<c>"*"</c>) or divisor (<c>"/"</c>).</param>
        /// <param name="postOpValue">Value to use with 'post operation' (currently must be scalar).</param>
        public AssignOrReturnInstruction(int nbTabs, G25.VariableType T, G25.FloatType FT, bool mustCast, RefGA.Multivector value, String postOp, RefGA.Multivector postOpValue)
            : base(nbTabs)
        {
            m_type = T;
            m_floatType = FT;
            m_mustCast = mustCast;
            m_value = value;
            m_postOp = postOp;
            m_postOpValue = postOpValue;
        }


        /// <summary>
        /// Applies 'm_postOp m_postOpValue' to 'valueStr'.
        /// </summary>
        /// <param name="S">Specification of algebra.</param>
        /// <param name="cgd">Not used yet.</param>
        /// <param name="valueStr">The array of value strings to which the postop should be applied</param>
        /// <param name="BL">Not used yet. May be used later on to known what basis blade each valueStr refers to.</param>
        public void ApplyPostOp(Specification S, G25.CG.Shared.CGdata cgd, RefGA.BasisBlade[] BL, String[] valueStr)
        {
            if (m_postOp == null) return;

            // TODO: currently only for *= and /=

            // get string of value:
            String postOpValueStr;
            RefGA.Multivector sc = m_postOpValue.ScalarPart();
            if (sc.IsZero()) postOpValueStr = CodeUtil.ScalarToLangString(S, m_floatType, RefGA.BasisBlade.ZERO);
            else postOpValueStr = CodeUtil.ScalarToLangString(S, m_floatType, sc.BasisBlades[0]);

            // apply "m_postOp postOpValueStr" to all valueStr
            for (int i = 0; i < valueStr.Length; i++)
            {
                valueStr[i] = "(" + valueStr[i] + ")" + m_postOp + ((m_mustCast) ? m_floatType.castStr : "") + "(" + postOpValueStr + ")";
            }
        }

        /// <summary>
        /// The returned or assigned type.
        /// </summary>
        public G25.VariableType m_type;

        /// <summary>
        /// Floating point type of m_type. If m_type is a floating point type, then is equal to m_type.
        /// </summary>
        public G25.FloatType m_floatType;

        /// <summary>
        /// When true, coordinates of m_value must be cast to m_floatType before assigning
        /// or returning it.
        /// </summary>
        public bool m_mustCast;

        /// <summary>
        /// The returned or assigned value.
        /// </summary>
        public RefGA.Multivector m_value;

        /// <summary>
        /// 'post operation'. Currently allows for global multiplier (<c>"*"</c>) or divisor (<c>"/"</c>).
        /// </summary>
        public String m_postOp;

        /// <summary>
        /// Value to use with 'post operation' (currently must be scalar).
        /// </summary>
        public RefGA.Multivector m_postOpValue;

    } // end of class AssignOrReturnInstruction


    /// <summary>
    /// Instruction for assigning a value to a variable.
    /// </summary>
    public class AssignInstruction : AssignOrReturnInstruction
    {
        /// <summary>
        /// Instruction for generating code for an assignment to a variable, with an optional declration of that same variable.
        /// </summary>
        /// <param name="nbTabs">How many tabs to put in front of code.</param>
        /// <param name="T">Type of assigned variable.</param>
        /// <param name="FT">Floating point type of coordinates of assigned variable. If T is a floating point type, then is equal to T.</param>
        /// <param name="mustCast">When assigning to variable, should the coordinates be cast to FT?</param>
        /// <param name="value">The assigned value.</param>
        /// <param name="name">Name of assigned variable.</param>
        /// <param name="ptr">Is the assigned variable a pointer?</param>
        /// <param name="declareVariable">If true, code for declraring the variable is also generated.</param>
        public AssignInstruction(int nbTabs, G25.VariableType T, G25.FloatType FT, bool mustCast, RefGA.Multivector value, String name, bool ptr, bool declareVariable)
            : 
            base(nbTabs, T, FT, mustCast, value)
        {
            m_name = name;
            m_ptr = ptr;
            m_declareVariable = declareVariable;
        }

        /// <summary>
        /// Instruction for generating code for an assignment to a variable, with an optional declration of that same variable.
        /// </summary>
        /// <param name="nbTabs">How many tabs to put in front of code.</param>
        /// <param name="T">Type of assigned variable.</param>
        /// <param name="FT">Floating point type of coordinates of assigned variable.</param>
        /// <param name="mustCast">When assigning to variable, should the coordinates be cast to FT?</param>
        /// <param name="value">The assigned value.</param>
        /// <param name="name">Name of assigned variable.</param>
        /// <param name="ptr">Is the assigned variable a pointer?</param>
        /// <param name="declareVariable">If true, code for declraring the variable is also generated.</param>
        /// <param name="postOp">'post operation'. Currently allows for global multiplier (<c>"*"</c>) or divisor (<c>"/"</c>).</param>
        /// <param name="postOpValue">Value to use with 'post operation' (currently must be scalar).</param>
        public AssignInstruction(int nbTabs, G25.VariableType T, G25.FloatType FT, bool mustCast, RefGA.Multivector value, String name, bool ptr, bool declareVariable, String postOp, RefGA.Multivector postOpValue)
            : this(nbTabs, T, FT, mustCast, value, name, ptr, declareVariable)
        {
            m_postOp = postOp;
            m_postOpValue = postOpValue;
        }


        /// <summary>
        /// Writes code for m_name = m_value.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Specification of algebra.</param>
        /// <param name="cgd">Not used yet.</param>
        public override void Write(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            if (m_type is G25.SMV)
            {
                G25.SMV dstSmv = m_type as G25.SMV;

                if (m_declareVariable)
                    SB.AppendLine("/* cannot yet assign and declare SMV type at the same time */");

                RefGA.BasisBlade[] BL = BasisBlade.GetNonConstBladeList(dstSmv);
                string[] accessStr = CodeUtil.GetAccessStr(S, dstSmv, m_name, m_ptr);
                bool writeZeros = true;
                string[] valueStr = CodeUtil.GetAssignmentStrings(S, m_floatType, m_mustCast, BL, m_value, writeZeros);

                // apply post operation (like "/ n2")
                ApplyPostOp(S, cgd, BL, valueStr);

                SB.AppendLine(CodeUtil.GenerateAssignmentCode(S, accessStr, valueStr, m_nbTabs, writeZeros));
            }
            else if (m_type is G25.FloatType)
            {
                // temp hack to override float type.
                G25.FloatType FT = this.m_floatType; // m_type as G25.FloatType;

                AppendTabs(SB);

                if (m_declareVariable)
                { // also declare the variable right here?
                    // output "/t type "
                    SB.Append(FT.type + " ");
                }

                // output name = ....;
                RefGA.BasisBlade[] BL = new RefGA.BasisBlade[1] { RefGA.BasisBlade.ONE };
                String[] accessStr = new String[1] { m_name };
                bool writeZeros = true;
                String[] valueStr = CodeUtil.GetAssignmentStrings(S, FT, m_mustCast, BL, m_value, writeZeros);

                // apply post operation (like "/ n2")
                ApplyPostOp(S, cgd, BL, valueStr);

                SB.AppendLine(CodeUtil.GenerateAssignmentCode(S, accessStr, valueStr, 0, writeZeros));
            }
            else
            {
                SB.AppendLine("/* to do: implement " + GetType().ToString() + " for type " + m_type.GetType().ToString() + " */");
            }
        }


        /// <summary>
        /// The variable name to assign to.
        /// </summary>
        public String m_name;

        /// <summary>
        /// Is variable m_name a pointer?
        /// </summary>
        public bool m_ptr;

        /// <summary>
        /// If true, code for declraring the variable is also generated.
        /// </summary>
        public bool m_declareVariable;

    } // end of class AssignInstruction

    /// <summary>
    /// Instruction for returning a value.
    /// </summary>
    public class ReturnInstruction : AssignOrReturnInstruction
    {
        /// <summary>
        /// Instruction for generating code for returning a  variable.
        /// </summary>
        /// <param name="nbTabs">How many tabs to put in front of code.</param>
        /// <param name="T">Type of assigned variable.</param>
        /// <param name="FT">Floating point type of coordinates of assigned variable.</param>
        /// <param name="mustCast">When assigning to variable, should the coordinates be cast to FT?</param>
        /// <param name="value">The assigned value.</param>
        public ReturnInstruction(int nbTabs, G25.VariableType T, G25.FloatType FT, bool mustCast, RefGA.Multivector value) 
            :
            base(nbTabs, T, FT, mustCast, value)
        {
        }

        /// <summary>
        /// Instruction for generating code for returning a  variable.
        /// </summary>
        /// <param name="nbTabs">How many tabs to put in front of code.</param>
        /// <param name="T">Type of assigned variable.</param>
        /// <param name="FT">Floating point type of coordinates of assigned variable.</param>
        /// <param name="mustCast">When assigning to variable, should the coordinates be cast to FT?</param>
        /// <param name="value">The assigned value.</param>
        /// <param name="postOp">'post operation'. Currently allows for global multiplier (<c>"*"</c>) or divisor (<c>"/"</c>).</param>
        /// <param name="postOpValue">Value to use with 'post operation' (currently must be scalar).</param>
        public ReturnInstruction(int nbTabs, G25.VariableType T, G25.FloatType FT, bool mustCast, RefGA.Multivector value, String postOp, RefGA.Multivector postOpValue)
            :
            base(nbTabs, T, FT, mustCast, value, postOp, postOpValue)
        {
        }

        /// <summary>
        /// Writes code for return m_name.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Specification of algebra.</param>
        /// <param name="cgd">Not used yet.</param>
        public override void Write(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            if (m_type is G25.FloatType)
            {
                AppendTabs(SB);

                // Temp hack to override the float type:
                G25.FloatType FT = m_floatType; //m_type as G25.FloatType;

                // Should postops still be applied?  ApplyPostOp(S, plugins, cog, BL, valueStr);
                // Not required so far?
                SB.AppendLine(CodeUtil.GenerateScalarReturnCode(S, FT, m_mustCast, m_value));
            }
            else if (m_type is G25.SMV)
            {
                if (S.OutputC())
                {
                    bool ptr = true;
                    bool declareVariable = false;
                    new AssignInstruction(m_nbTabs, m_type, m_floatType, m_mustCast, m_value, G25.fgs.RETURN_ARG_NAME, ptr, declareVariable, m_postOp, m_postOpValue).Write(SB, S, cgd);
                }
                else
                {
                    G25.SMV smv = m_type as G25.SMV;
                    RefGA.BasisBlade[] BL = BasisBlade.GetNonConstBladeList(smv);
                    bool writeZeros = true;
                    string[] valueStr = CodeUtil.GetAssignmentStrings(S, m_floatType, m_mustCast, BL, m_value, writeZeros);

                    // apply post operation (like "/ n2")
                    ApplyPostOp(S, cgd, BL, valueStr);

                    SB.AppendLine(CodeUtil.GenerateReturnCode(S, smv, m_floatType, valueStr, m_nbTabs, writeZeros));

                }
            }
        }

    } // end of class ReturnInstruction


    /// <summary>
    /// Base class for generating (multiple lines of) verbatim code or comments.
    /// Do not instantiate directly; use VerbatimCodeInstruction or CommentInstruction.
    /// 
    /// The code is cut into lines using Util.CutStringAtNewLine().
    /// </summary>
    public class VerbatimInstruction : Instruction
    {
        /// <summary>
        /// Instruction for generating (multiple lines of) verbatim code or comments.
        /// Do not instantiate directly; use VerbatimCodeInstruction or CommentInstruction.
        /// </summary>
        /// <param name="nbTabs">How many tabs to put in front of code.</param>
        /// <param name="text">The text to generate.</param>
        protected VerbatimInstruction(int nbTabs, String text)
            :
            base(nbTabs)
        {
            m_text = Util.CutStringAtNewLine(text);
        }


        /// <summary>
        /// List of text (split up at each end of line).
        /// </summary>
        public List<String> m_text;

    }

    /// <summary>
    /// Instruction for generating a (multi-line) comment.
    /// </summary>
    public class CommentInstruction : VerbatimInstruction
    {
        /// <summary>
        /// Instruction for generating a comment.
        /// </summary>
        /// <param name="nbTabs">How many tabs to put in front of code.</param>
        /// <param name="comment">The comment. Do not include the comment delimiters. May be multiple lines.</param>
        public CommentInstruction(int nbTabs, String comment)
            :
            base(nbTabs, comment)
        {
        }

        /// <summary>
        /// Writes the comments. 
        /// 
        /// The comment is nicely split across multiple lines
        /// as the user specified and the comment symbols are applied depending
        /// on the output language.
        /// 
        /// Tabs are applied in front of the comments.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Specification of algebra.</param>
        /// <param name="cgd">Not used yet.</param>
        public override void Write(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            switch (S.m_outputLanguage)
            {
                case OUTPUT_LANGUAGE.C:
                    AppendTabs(SB); SB.Append("/* ");
                    if (m_text.Count > 1) SB.AppendLine(""); // make it a single line comment when there is only one line, multiple line otherwise
                    foreach (string str in m_text)
                    {
                        if (m_text.Count > 1) AppendTabs(SB); 
                        SB.Append(str);
                        if (m_text.Count > 1) SB.AppendLine(""); // make it a single line comment when there is only one line, multiple line otherwise
                    }
                    if (m_text.Count > 1) AppendTabs(SB); 
                    SB.AppendLine(" */");
                    break;
                case OUTPUT_LANGUAGE.CPP:
                    foreach (string str in m_text)
                    {
                        AppendTabs(SB); SB.Append("// "); SB.AppendLine(str);
                    }
                    break;
            }
        }

    } // end of class CommentInstruction

    /// <summary>
    /// Instruction for generating a (multi-line) verbatim code..
    /// </summary>
    public class VerbatimCodeInstruction : VerbatimInstruction
    {
        /// <summary>
        /// Instruction for generating a verbatim code.
        /// </summary>
        /// <param name="nbTabs">How many tabs to put in front of code.</param>
        /// <param name="code">The code.May be multiple lines.</param>
        public VerbatimCodeInstruction(int nbTabs, String code)
            :
            base(nbTabs, code)
        {
        }

        /// <summary>
        /// Writes verbatim code.
        /// 
        /// Tabs are applied in front of the code.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Specification of algebra.</param>
        /// <param name="cgd">Not used yet.</param>
        public override void Write(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            switch (S.m_outputLanguage)
            {
                case OUTPUT_LANGUAGE.C:
                case OUTPUT_LANGUAGE.CPP:
                case OUTPUT_LANGUAGE.CSHARP:
                case OUTPUT_LANGUAGE.JAVA:
                    foreach (String str in m_text)
                    {
                        AppendTabs(SB); SB.AppendLine(str);
                    }
                    break;
            }
        }

    } // end of class VerbatimCodeInstruction

    /// <summary>
    /// Instruction for making an if-else construction.
    /// 
    /// The conditional if-else branches are instructions themselves, but
    /// the condition code is just a string.
    /// </summary>
    public class IfElseInstruction : Instruction
    {
        /// <summary>
        /// Instruction for generating if-else code.
        /// </summary>
        /// <param name="nbTabs">How many tabs in front of code.</param>
        /// <param name="condition">conditional code, copied verbatim into if (...) clause.</param>
        /// <param name="ifInstruction">Instruction(s) to execute in if-branch.</param>
        /// <param name="elseInstruction">Instruction(s) to execute in else-branch.</param>
        public IfElseInstruction(int nbTabs, String condition, List<Instruction> ifInstruction, List<Instruction> elseInstruction)
            :
            base(nbTabs, MakeListOfLists(ifInstruction, elseInstruction))
        {
            m_condition = condition;
        }


        /// <summary>
        /// Writes code for if else.
        /// 
        /// If the else part is null, it is not emitted.
        /// </summary>
        /// <param name="SB"></param>
        /// <param name="S"></param>
        /// <param name="cgd">Not used yet.</param>
        public override void Write(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            AppendTabs(SB); SB.AppendLine("if (" + m_condition + ") {");
            foreach (Instruction I in IfInstruction)
                I.Write(SB, S, cgd);
            AppendTabs(SB); SB.AppendLine("}");
            if ((m_childInstructions.Count > 1) && (ElseInstruction != null) && (ElseInstruction.Count > 0))
            {
                AppendTabs(SB); SB.AppendLine("else {");
                foreach (Instruction I in ElseInstruction)
                    I.Write(SB, S, cgd);
                AppendTabs(SB); SB.AppendLine("}");
            }
        }

        public List<Instruction> IfInstruction { get { return m_childInstructions[0]; } }
        public List<Instruction> ElseInstruction { get { return m_childInstructions[1]; } }

        public String m_condition;
    } // end of class IfElseInstruction



} // end of  namespace G25.CG.Shared
