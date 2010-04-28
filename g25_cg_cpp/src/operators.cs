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

namespace G25.CG.CPP
{
    /// <summary>
    /// Main code generation class for the header file.
    /// </summary>
    class Operators
    {

        public static void WriteOperatorDefinitions(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            bool declOnly = false;
            WriteOperators(SB, S, cgd, declOnly);
        }

        public static void WriteOperatorDeclarations(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            bool declOnly = true;
            WriteOperators(SB, S, cgd, declOnly);
        }

        private static Dictionary<string, List<G25.Operator>> GetOperatorMap(Specification S)
        {
            Dictionary<string, List<G25.Operator>> operatorMap = new Dictionary<string, List<Operator>>();
            foreach (G25.Operator op in S.m_operators)
            {
                if (!operatorMap.ContainsKey(op.FunctionName))
                {
                    operatorMap[op.FunctionName] = new List<Operator>();
                }
                operatorMap[op.FunctionName].Add(op);
            }
            return operatorMap;
        }

        private static string GetFuncDecl(Specification S, bool declOnly, G25.fgs FGS, G25.Operator op, G25.FloatType FT, bool assign, bool constVal, bool returnByReference) 
        {
            StringBuilder SB = new StringBuilder();

            string inlineStr = G25.CG.Shared.Util.GetInlineString(S, (!declOnly) && S.m_inlineOperators, " ");
            string returnTypeName = (FGS.m_returnTypeName.Length > 0) ? FGS.m_returnTypeName : FT.type;
            if (!S.IsFloatType(returnTypeName)) returnTypeName = FT.GetMangledName(S, returnTypeName);
            string arg1typeName = FT.GetMangledName(S, FGS.ArgumentTypeNames[0]);
            string arg2typeName = (FGS.NbArguments > 1) ? FT.GetMangledName(S, FGS.ArgumentTypeNames[1]) : "";

            SB.Append(inlineStr);
            SB.Append(returnTypeName);
            SB.Append(" ");
            if (returnByReference) SB.Append("&");
            SB.Append("operator");
            SB.Append(op.Symbol);
            if (assign) SB.Append("=");
            SB.Append("(");
            if (constVal) SB.Append("const ");
            SB.Append(arg1typeName);
            SB.Append(" &");
            SB.Append(FGS.ArgumentVariableNames[0]);

            if (op.IsBinary())
            {
                SB.Append(", const ");
                SB.Append(arg2typeName);
                SB.Append(" &");
                SB.Append(FGS.ArgumentVariableNames[1]);
            }
            else if ((S.m_outputLanguage == OUTPUT_LANGUAGE.CPP) && op.IsPostfixUnary())
            { // add a dummy int argument so C++ knows this is a unary postfix op
                SB.Append(", int");
            }

            SB.Append(")");

            return SB.ToString();
        }

        private static string GetComment(Specification S, bool declOnly, G25.fgs FGS, G25.Operator op, G25.FloatType FT, bool assign)
        {
            StringBuilder SB = new StringBuilder();

            if ((S.m_outputLanguage == OUTPUT_LANGUAGE.CPP) && op.IsUnaryInPlace())
            {
                if (op.IsPrefix)
                {
                    SB.Append("returns (" + FGS.ArgumentVariableNames[0] + " = " + FGS.OutputName + "(" + FGS.ArgumentVariableNames[0] + "))");
                }
                else
                {
                    SB.Append("returns input value of " + FGS.ArgumentVariableNames[0] + ", but sets " + FGS.ArgumentVariableNames[0] + " to " + FGS.OutputName + "(" + FGS.ArgumentVariableNames[0] + ")");
                }
            }
            else if (assign)
            {
                SB.Append("returns (" + FGS.ArgumentVariableNames[0] + " = " + FGS.OutputName + "(" + FGS.ArgumentVariableNames[0]);
                SB.Append(", " + FGS.ArgumentVariableNames[1]);
                SB.Append("))");
            }
            else {
                SB.Append("returns " + FGS.OutputName + "(" + FGS.ArgumentVariableNames[0]);
                if (op.IsBinary())
                    SB.Append(", " + FGS.ArgumentVariableNames[1]);
                SB.Append(")");
            }

            return SB.ToString();
        }

        private static void WriteOperatorBody(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, G25.fgs FGS, G25.Operator op, string funcName)
        {
            bool returnTypeEqualsFirstArgument = FGS.ReturnTypeName == FGS.ArgumentTypeNames[0];
            if ((S.m_outputLanguage == OUTPUT_LANGUAGE.CPP) && op.IsUnaryInPlace() && returnTypeEqualsFirstArgument) // special unary case for C++
            {
                if (op.IsPrefix)
                {
                    SB.AppendLine("\t" + FGS.ArgumentVariableNames[0] + " = " + funcName + "(" + FGS.ArgumentVariableNames[0] + ");");
                    SB.AppendLine("\treturn " + FGS.ArgumentVariableNames[0] + ";");
                }
                else
                {
                    SB.AppendLine("\t" + FGS.ReturnTypeName + " retVal(" + FGS.ArgumentVariableNames[0] + ");");
                    SB.AppendLine("\t" + FGS.ArgumentVariableNames[0] + " = " + funcName + "(" + FGS.ArgumentVariableNames[0] + ");");
                    SB.AppendLine("\treturn retVal;");
                }
            }
            else // regular operator function
            {
                SB.Append("\treturn " + funcName + "(" + FGS.ArgumentVariableNames[0]);
                if (op.IsBinary()) SB.Append(", " + FGS.ArgumentVariableNames[1]);
                SB.AppendLine(");");
            }
        }

        private static void WriteOperator(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, bool declOnly, G25.fgs FGS, G25.Operator op)
        {
            bool comment = declOnly || S.m_inlineOperators;
            bool returnTypeEqualsFirstArgument = FGS.ReturnTypeName == FGS.ArgumentTypeNames[0];

            foreach (string floatName in FGS.FloatNames)
            {
                G25.FloatType FT = S.GetFloatType(floatName);

                string funcName = FGS.OutputName;

                { // regular operator
                    bool assign = false;
                    bool returnByReference = returnTypeEqualsFirstArgument && op.IsPrefixUnary() && op.IsUnaryInPlace(); // for unary prefix ++ and --, needs to return by reference (&)
                    bool constVal = !(op.IsUnaryInPlace() && returnTypeEqualsFirstArgument);
                    string funcDecl = GetFuncDecl(S, declOnly, FGS, op, FT, assign, constVal, returnByReference);


                    if (comment) // comment?
                        SB.AppendLine("/// " + GetComment(S, declOnly, FGS, op, FT, assign));
                    SB.Append(funcDecl);
                    if (declOnly) SB.AppendLine(";");
                    else
                    {
                        SB.AppendLine(" {");
                        WriteOperatorBody(SB, S, cgd, FGS, op, funcName);
                        SB.AppendLine("}");
                    }
                }

                // add an extra operator with assignment (like +=) ?
                if (op.IsBinary() && returnTypeEqualsFirstArgument) 
                {
                    bool assign = true;
                    bool constVal = false;
                    bool returnByReference = true; // todo: for unary prefix ++ needs to return by reference (&)
                    string funcDecl = GetFuncDecl(S, declOnly, FGS, op, FT, assign, constVal, returnByReference);

                    if (comment) // comment?
                        SB.AppendLine("/// " + GetComment(S, declOnly, FGS, op, FT, assign));
                    SB.Append(funcDecl);
                    if (declOnly) SB.AppendLine(";");
                    else
                    {
                        SB.AppendLine(" {");
                        SB.Append("\treturn (" + FGS.ArgumentVariableNames[0] + " = " + funcName + "(" + FGS.ArgumentVariableNames[0]);
                        SB.AppendLine(", " + FGS.ArgumentVariableNames[1] + "));");
                        SB.AppendLine("}");
                    }
                }

            }

        }

        private static void WriteOperators(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, bool declOnly)
        {
            Dictionary<string, List<G25.Operator>> operatorMap = GetOperatorMap(S);
            Dictionary<string, bool> boundOperators = new Dictionary<string,bool>();

            // for all functions, find the matching op, write function
            foreach (G25.fgs FGS in S.m_functions)
            {
                if (!operatorMap.ContainsKey(FGS.OutputName)) continue;
                //if (FGS.MetricName != S.m_metric[0].m_name) continue; // only bind for default metric

                // check if all operators are built-in
                bool allArgTypesAreBuiltin = true;
                for (int i = 0; i < FGS.m_argumentTypeNames.Length; i++)
                    if (!S.IsFloatType(FGS.m_argumentTypeNames[i]))
                        allArgTypesAreBuiltin = false;

                if (allArgTypesAreBuiltin) continue; // cannot override for builtin types

                List<G25.Operator> opList = operatorMap[FGS.OutputName];

                foreach (G25.Operator op in opList)
                {
                    if (op.NbArguments == FGS.NbArguments)
                    {
                        // check if this operator already bound to function with the same arguments
                        string uniqueOpArgId = op.Symbol;
                        for (int a = 0; a < FGS.NbArguments; a++)
                            uniqueOpArgId += "~_~" + FGS.m_argumentTypeNames[a];
                        if (boundOperators.ContainsKey(uniqueOpArgId)) continue;
                        else boundOperators[uniqueOpArgId] = true;

                        // write this operator for all float types
                        WriteOperator(SB, S, cgd, declOnly, FGS, op);

                    }
                }
            }
        }

    } // end of class Operators
} // end of namespace G25.CG.CPP
