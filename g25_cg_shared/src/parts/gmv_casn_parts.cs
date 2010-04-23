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
    namespace CG
    {
        namespace Shared
        {
            public class GmvCASNparts
            {

                public GmvCASNparts(Specification S, G25.CG.Shared.CGdata cgd)
                {
                    m_specification = S;
                    m_cgd = cgd;
                }

                protected Specification m_specification;
                protected G25.CG.Shared.CGdata m_cgd;

                /// <summary>
                /// Writes CASN parts to m_cgd.m_declSB and m_cgd.m_defSB.
                /// </summary>
                public void WriteGmvCASNparts()
                {
                    G25.CG.Shared.CANSparts.WriteCANSparts(m_specification, m_cgd);
                }


                /// <summary>
                /// Writes any addition or subtraction function for general multivectors,
                /// based on CASN parts code.
                /// </summary>
                /// <param name="S"></param>
                /// <param name="cgd"></param>
                /// <param name="FT"></param>
                /// <param name="FAI"></param>
                /// <param name="F"></param>
                /// <param name="comment"></param>
                /// <param name="funcType">ADD, SUB or HP</param>
                /// <returns>Full name of generated function.</returns>
                public static string WriteAddSubHpFunction(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT,
                    G25.CG.Shared.FuncArgInfo[] FAI, G25.fgs F,
                    string comment, G25.CG.Shared.CANSparts.ADD_SUB_HP_TYPE funcType)
                {
                    // setup instructions
                    System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
                    int nbTabs = 1;

                    // write this function:
                    string code = G25.CG.Shared.CANSparts.GetAddSubtractHpCode(S, cgd, FT, funcType, FAI, fgs.RETURN_ARG_NAME);

                    // add one instruction (verbatim code)
                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, code));

                    // because of lack of overloading, function names include names of argument types
                    G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(S, FT, F, FAI);

                    // setup return type and argument:
                    string returnTypeName = FT.GetMangledName(S, S.m_GMV.Name);
                    G25.CG.Shared.FuncArgInfo returnArgument = null;
                    if (S.m_outputLanguage == OUTPUT_LANGUAGE.C) 
                        returnArgument = new G25.CG.Shared.FuncArgInfo(S, CF, -1, FT, S.m_GMV.Name, false); // false = compute value

                    string funcName = CF.OutputName;
                    //if (S.m_outputLanguage == OUTPUT_LANGUAGE.C)
                      //  funcName = FT.GetMangledName(S, funcName);

                    // write function
                    bool inline = false; // never inline GMV functions
                    G25.CG.Shared.Functions.WriteFunction(S, cgd, F, inline, returnTypeName, funcName, returnArgument, FAI, I, comment);

                    return funcName;
                }


                /// <summary>
                /// Writes a zero or equality test function for general multivectors,
                /// based on CASN parts code.
                /// </summary>
                /// <param name="S"></param>
                /// <param name="cgd"></param>
                /// <param name="FT"></param>
                /// <param name="FAI"></param>
                /// <param name="F"></param>
                /// <param name="comment"></param>
                /// <param name="writeZero">When true, a function to test for zero is written.</param>
                /// <returns>full (mangled) name of generated function</returns>
                public static string WriteEqualsOrZeroOrGradeBitmapFunction(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT,
                    G25.CG.Shared.FuncArgInfo[] FAI, G25.fgs F,
                    String comment, G25.CG.Shared.CANSparts.EQUALS_ZERO_GRADEBITMAP_TYPE funcType)
                {
                    // setup instructions
                    System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
                    int nbTabs = 1;

                    // write this function:
                    string code = "";
                    if (funcType == G25.CG.Shared.CANSparts.EQUALS_ZERO_GRADEBITMAP_TYPE.EQUALS)
                        code = G25.CG.Shared.CANSparts.GetEqualsCode(S, cgd, FT, FAI);
                    else if (funcType == G25.CG.Shared.CANSparts.EQUALS_ZERO_GRADEBITMAP_TYPE.ZERO)
                        code = G25.CG.Shared.CANSparts.GetZeroCode(S, cgd, FT, FAI);
                    else if (funcType == G25.CG.Shared.CANSparts.EQUALS_ZERO_GRADEBITMAP_TYPE.GRADE_BITMAP)
                        code = G25.CG.Shared.CANSparts.GetGradeBitmapCode(S, cgd, FT, FAI);

                    // add one instruction (verbatim code)
                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, code));

                    // because of lack of overloading, function names include names of argument types
                    G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(S, FT, F, FAI);

                    // setup return type and argument:
                    string returnTypeName = null;
                    if (S.m_outputLanguage == OUTPUT_LANGUAGE.C) 
                    {
                        returnTypeName = "int";
                    }
                    else {
                        if (funcType == G25.CG.Shared.CANSparts.EQUALS_ZERO_GRADEBITMAP_TYPE.GRADE_BITMAP)
                            returnTypeName = "int";
                        else returnTypeName = "bool";
                    }
                    
                    G25.CG.Shared.FuncArgInfo returnArgument = null;

                    // write function
                    bool inline = false; // never inline GMV functions
                    G25.CG.Shared.Functions.WriteFunction(S, cgd, F, inline, returnTypeName, CF.OutputName, returnArgument, FAI, I, comment);

                    return CF.OutputName;
                }


                /// <summary>
                /// Writes any negation, reversion, conjugation or involution function for general multivectors,
                /// based on CASN parts code.
                /// </summary>
                /// <param name="S"></param>
                /// <param name="cgd"></param>
                /// <param name="FT"></param>
                /// <param name="FAI"></param>
                /// <param name="F"></param>
                /// <param name="comment"></param>
                /// <param name="T">Type of function (negation, reversion, conjugation or involution).</param>
                /// <returns>name of generated function</returns>
                public static string WriteUnarySignFunction(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT,
                    G25.CG.Shared.FuncArgInfo[] FAI, G25.fgs F,
                    String comment, G25.CG.Shared.CANSparts.UnaryToggleSignType T)
                {
                    // setup instructions
                    System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
                    int nbTabs = 1;

                    // write this function:
                    String code = G25.CG.Shared.CANSparts.GetUnaryToggleSignCode(S, cgd, FT, T, FAI, fgs.RETURN_ARG_NAME);

                    // add one instruction (verbatim code)
                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, code));

                    // because of lack of overloading, function names include names of argument types
                    G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(S, FT, F, FAI);

                    // setup return type and argument:
                    String returnTypeName = FT.GetMangledName(S, S.m_GMV.Name);


                    G25.CG.Shared.FuncArgInfo returnArgument = null;
                    if (S.m_outputLanguage == OUTPUT_LANGUAGE.C) 
                        returnArgument = new G25.CG.Shared.FuncArgInfo(S, CF, -1, FT, S.m_GMV.Name, false); // false = compute value

                    string funcName = CF.OutputName;
                    //if (S.m_outputLanguage == OUTPUT_LANGUAGE.C)
                      //  funcName = FT.GetMangledName(S, funcName);

                    // write function
                    bool inline = false; // never inline GMV functions
                    G25.CG.Shared.Functions.WriteFunction(S, cgd, F, inline, returnTypeName, funcName, returnArgument, FAI, I, comment);

                    return funcName;
                }

                protected static StringBuilder AddGradeArg(Specification S, String str, int gradeIdx, String GROUP_BITMAP_NAME)
                {
                    StringBuilder SB = new StringBuilder();
                    int idx = str.IndexOf(')');
                    if (idx < 0)
                    {
                        SB.Append(str);
                    }
                    else
                    {
                        SB.Append(str.Substring(0, idx));
                        SB.Append(", int " + GROUP_BITMAP_NAME);
                        SB.Append(str.Substring(idx));
                    }
                    return SB;
                }

                /// <summary>
                /// Writes any negation, reversion, conjugation or involution function for general multivectors,
                /// based on CASN parts code.
                /// </summary>
                /// <param name="S"></param>
                /// <param name="cgd"></param>
                /// <param name="FT"></param>
                /// <param name="FAI"></param>
                /// <param name="F"></param>
                /// <param name="comment"></param>
                /// <param name="gradeIdx">Grade to be selected (use -1 for user-specified).</param>
                public static string WriteGradeFunction(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT,
                    G25.CG.Shared.FuncArgInfo[] FAI, G25.fgs F,
                    String comment, int gradeIdx)
                {
                    // setup instructions
                    System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
                    int nbTabs = 1;

                    // write this function:
                    const String GROUP_BITMAP_NAME = "groupBitmap";
                    String code = G25.CG.Shared.CANSparts.GetGradeCode(S, cgd, FT, gradeIdx, FAI, fgs.RETURN_ARG_NAME, GROUP_BITMAP_NAME);

                    // add one instruction (verbatim code)
                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, code));

                    // because of lack of overloading, function names include names of argument types
                    G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(S, FT, F, FAI);

                    string funcName = CF.OutputName;
                    //if (S.m_outputLanguage == OUTPUT_LANGUAGE.C)
                      //  funcName = FT.GetMangledName(S, funcName);

                    // setup return type and argument:
                    string returnTypeName = FT.GetMangledName(S, S.m_GMV.Name);

                    G25.CG.Shared.FuncArgInfo returnArgument = null;
                    if (S.m_outputLanguage == OUTPUT_LANGUAGE.C)
                        returnArgument = new G25.CG.Shared.FuncArgInfo(S, CF, -1, FT, S.m_GMV.Name, false); // false = compute value

                    //StringBuilder tmpSB = new StringBuilder(); // G25.CG.Shared.Functions.WriteFunction() writes to this SB first so we can add the grade index if required

                    // write function
                    G25.CG.Shared.CGdata tmpCgd = new G25.CG.Shared.CGdata(cgd);
                    bool inline = false; // never inline GMV functions
                    G25.CG.Shared.Functions.WriteFunction(S, tmpCgd, F, inline, returnTypeName, funcName, returnArgument, FAI, I, comment);

                    if (gradeIdx < 0) // hack: if grade is func arg, then add it:
                    { // add extra argument (int) to select the grade
                        tmpCgd.m_declSB = AddGradeArg(S, tmpCgd.m_declSB.ToString(), gradeIdx, GROUP_BITMAP_NAME);
                        tmpCgd.m_defSB = AddGradeArg(S, tmpCgd.m_defSB.ToString(), gradeIdx, GROUP_BITMAP_NAME);
                        tmpCgd.m_inlineDefSB = AddGradeArg(S, tmpCgd.m_inlineDefSB.ToString(), gradeIdx, GROUP_BITMAP_NAME);
                    }

                    cgd.m_declSB.Append(tmpCgd.m_declSB);
                    cgd.m_defSB.Append(tmpCgd.m_defSB);
                    cgd.m_inlineDefSB.Append(tmpCgd.m_inlineDefSB);

                    return funcName;
                } // end of WriteGradeFunction()


                /// <summary>
                /// Writes any unit or versor inverse function for general multivectors,
                /// based on norm2 and CASN parts code.
                /// </summary>
                /// <param name="S"></param>
                /// <param name="cgd"></param>
                /// <param name="FT"></param>
                /// <param name="M"></param>
                /// <param name="FAI"></param>
                /// <param name="F"></param>
                /// <param name="comment"></param>
                /// <param name="funcType"> UNIT, VERSOR_INVERSE or DIV</param>
                public static string WriteDivFunction(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.Metric M,
                    G25.CG.Shared.FuncArgInfo[] FAI, G25.fgs F, string comment, G25.CG.Shared.CANSparts.DIVCODETYPE funcType)
                {
                    // setup instructions
                    System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
                    int nbTabs = 1;

                    // write this function:
                    string code = G25.CG.Shared.CANSparts.GetDivCode(S, cgd, FT, M, FAI, fgs.RETURN_ARG_NAME, funcType);

                    // add one instruction (verbatim code)
                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, code));

                    // because of lack of overloading, function names include names of argument types
                    G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(S, FT, F, FAI);

                    // setup return type and argument:
                    string returnTypeName = FT.GetMangledName(S, S.m_GMV.Name);

                    G25.CG.Shared.FuncArgInfo returnArgument = null;
                    if (S.m_outputLanguage == OUTPUT_LANGUAGE.C)
                        returnArgument = new G25.CG.Shared.FuncArgInfo(S, CF, -1, FT, S.m_GMV.Name, false); // false = compute value

                    string funcName = CF.OutputName;
                    //if (S.m_outputLanguage == OUTPUT_LANGUAGE.C)
                      //  funcName = FT.GetMangledName(S, funcName);

                    // write function
                    bool inline = false; // never inline GMV functions
                    G25.CG.Shared.Functions.WriteFunction(S, cgd, F, inline, returnTypeName, funcName, returnArgument, FAI, I, comment);

                    return funcName;
                }

                /// <summary>
                /// Writes a scale-and-add-scalar function (scalar * gmv + scalar)
                /// based on CASN parts code.
                /// </summary>
                /// <param name="S"></param>
                /// <param name="cgd"></param>
                /// <param name="FT"></param>
                /// <param name="FAI"></param>
                /// <param name="F"></param>
                /// <param name="comment"></param>
                public static void WriteSASfunction(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT,
                    G25.CG.Shared.FuncArgInfo[] FAI, G25.fgs F, string comment)
                {
                    // setup instructions
                    System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
                    int nbTabs = 1;

                    // write this function:
                    string code = G25.CG.Shared.CANSparts.GetSAScode(S, cgd, FT, FAI, fgs.RETURN_ARG_NAME);

                    // add one instruction (verbatim code)
                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, code));

                    // because of lack of overloading, function names include names of argument types
                    G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(S, FT, F, FAI);

                    // setup return type and argument:
                    string returnTypeName = FT.GetMangledName(S, S.m_GMV.Name);
                    G25.CG.Shared.FuncArgInfo returnArgument = null;
                    if (S.m_outputLanguage == OUTPUT_LANGUAGE.C)
                        returnArgument = new G25.CG.Shared.FuncArgInfo(S, CF, -1, FT, S.m_GMV.Name, false); // false = compute value

                    // write function
                    bool inline = false; // never inline GMV functions
                    G25.CG.Shared.Functions.WriteFunction(S, cgd, F, inline, returnTypeName, CF.OutputName, returnArgument, FAI, I, comment);
                }

                /// <summary>
                /// Writes a increment or decrement function.
                /// (not based on CASN parts code, but put here anyway).
                /// </summary>
                /// <param name="S"></param>
                /// <param name="cgd"></param>
                /// <param name="FT"></param>
                /// <param name="FAI"></param>
                /// <param name="F"></param>
                /// <param name="comment"></param>
                /// <param name="increment"></param>
                public static string WriteIncrementFunction(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT,
                    G25.CG.Shared.FuncArgInfo[] FAI, G25.fgs F, string comment, bool increment)
                {
                    // setup instructions
                    System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
                    int nbTabs = 1;

                    // write this function:
                    string code = G25.CG.Shared.CANSparts.GetIncrementCode(S, cgd, FT, FAI, fgs.RETURN_ARG_NAME, increment);

                    // add one instruction (verbatim code)
                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, code));

                    // because of lack of overloading, function names include names of argument types
                    G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(S, FT, F, FAI);

                    // setup return type and argument:
                    String returnTypeName = FT.GetMangledName(S, S.m_GMV.Name);
                    G25.CG.Shared.FuncArgInfo returnArgument = null;
                    if (S.m_outputLanguage == OUTPUT_LANGUAGE.C)
                        returnArgument = new G25.CG.Shared.FuncArgInfo(S, CF, -1, FT, S.m_GMV.Name, false); // false = compute value

                    // write function
                    bool inline = false; // never inline GMV functions
                    G25.CG.Shared.Functions.WriteFunction(S, cgd, F, inline, returnTypeName, CF.OutputName, returnArgument, FAI, I, comment);

                    return CF.OutputName;
                }

            } // end of class GmvCASNparts
        } // end of namespace 'C'
    } // end of namespace CG
} // end of namespace G25
