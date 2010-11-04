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
    /// Contains various utility functions for reporting the usage of specialized multivectors 
    /// in functions over general multivectors.
    /// </summary>
    public class ReportUsage
    {
        private const string PLACE_HOLDER = "__place~holder__";
        public const string INVALID = "INVALID";

        public static string GetSpecializedConstantName(Specification S, string typename)
        {
            return (S.m_namespace + "_" + typename).ToUpper();
        }


        // add someextra instructions for report usage?
        // add extra verbatim code here if rep usage?
        // what arguments are required??
        public static Instruction GetReportInstruction(Specification S, G25.fgs F, FuncArgInfo[] FAI)
        {
            if ((S.OutputC()) || 
                (!S.m_reportUsage)  ||
                (FAI.Length == 0)) return new NOPinstruction();

            // check if all arguments are GMVs
            for (int i = 0; i < FAI.Length; i++)
            {
                if (!FAI[i].IsGMV()) return new NOPinstruction();
            }

            // get XML spec
            string XMLstr = GetXMLstring(S, F, FAI);

            StringBuilder SB = new StringBuilder();

            if (S.OutputCSharpOrJava())
            {
                for (int i = 0; i < FAI.Length; i++)
                {
                    SB.AppendLine("SmvType type_" + FAI[i].Name + " = " + FAI[i].Name + ".to_" + FAI[i].MangledTypeName +"().m_t;");
                }
            }

            {
                string MV_CONSTANT = GetSpecializedConstantName(S, S.m_GMV.Name);
                string INVALID_CONSTANT = GetSpecializedConstantName(S, INVALID);
                // output the test for all specialized MVs
                SB.Append("if (");
                for (int i = 0; i < FAI.Length; i++)
                {
                    if (i > 0)
                    {
                        SB.AppendLine(" && ");
                        SB.Append("\t");
                    }
                    if (S.OutputCpp())
                    {
                        SB.Append("(" + FAI[i].Name + ".m_t > " + MV_CONSTANT + ") && (" + FAI[i].Name + ".m_t < " + INVALID_CONSTANT + ")");
                    }
                    else if (S.OutputCSharp())
                    {
                        SB.Append("(type_" + FAI[i].Name + " > SmvType." + MV_CONSTANT + ") && (type_" + FAI[i].Name + " < SmvType." + INVALID_CONSTANT + ")");
                    }
                    else if (S.OutputJava())
                    {
                        SB.Append("(type_" + FAI[i].Name + ".compareTo(SmvType." + MV_CONSTANT + ") > 0) && (type_" + FAI[i].Name + ".compareTo(SmvType." + INVALID_CONSTANT + ") < 0)");
                    }
                }
                SB.AppendLine(") {");


                if (S.OutputCpp())
                {
                    SB.Append("\t\tstd::string reportUsageString = std::string(\"\") + ");
                }
                else if (S.OutputCSharp())
                {
                    SB.Append("\t\tstring reportUsageString = ");
                }
                else if (S.OutputJava())
                {
                    SB.Append("\t\tString reportUsageString = ");
                }
                // output XMLstr, replace placeholders with code
                int XMLstrIdx = 0;
                int argIdx = 0;
                while (XMLstrIdx < XMLstr.Length)
                {
                    string placeHolder = GetPlaceHolderString(argIdx);

                    int nextIdx = XMLstr.IndexOf(placeHolder, XMLstrIdx);
                    if (nextIdx < 0) nextIdx = XMLstr.Length;

                    SB.Append(Util.StringToCode(XMLstr.Substring(XMLstrIdx, nextIdx - XMLstrIdx)));
                    if (argIdx < FAI.Length)
                    {
                        if (S.OutputCpp())
                        {
                            SB.Append("+ g_" + S.m_namespace + "Typenames[" + FAI[argIdx].Name + ".m_t] + ");
                        }
                        else if (S.OutputCSharp())
                        {
                            SB.Append("+ typenames[(int)type_" + FAI[argIdx].Name + "] + ");
                        }
                        else if (S.OutputJava())
                        {
                            SB.Append("+ typenames[type_" + FAI[argIdx].Name + ".getId()] + ");
                        }
                    }

                    argIdx++;
                    XMLstrIdx = nextIdx + placeHolder.Length;
                }
                SB.AppendLine(";");
                if (S.OutputCpp())
                {
                    SB.AppendLine("\t\tReportUsage::mergeReport(new ReportUsage(reportUsageString));");
                }
                else if (S.OutputCSharp())
                {
                    SB.AppendLine("\t\tReportUsage.MergeReport(new ReportUsage(reportUsageString));");
                }
                else if (S.OutputJava())
                {
                    SB.AppendLine("\t\tReportUsage.mergeReport(new ReportUsage(reportUsageString));");
                }

                SB.AppendLine("}");
            }

            int nbTabs = 1;
            return new VerbatimCodeInstruction(nbTabs, SB.ToString());
        }

        private static string GetPlaceHolderString(int idx)
        {
            return PLACE_HOLDER + idx;
        }

        private static string GetXMLstring(Specification S, G25.fgs F, FuncArgInfo[] FAI)
        {
            // no return forced type in XML
            string returnTypeName = null;

            // get placeholder arguments for XML
            string[] argumentTypeNames = new string[FAI.Length];
            for (int i = 0; i < FAI.Length; i++)
            {
                argumentTypeNames[i] = GetPlaceHolderString(i);
            }

            // use a single float type only in XML
            string[] floatNames = new string[1] { FAI[0].FloatType.type };

            // get a copy of F, but put insert placeholders for the typenames
            G25.fgs tmpF = new G25.fgs(F.Name, F.OutputName, returnTypeName, argumentTypeNames,
                F.ArgumentVariableNames, floatNames, F.MetricName, F.Comment, F.Options);

            string XMLstr = XML.FunctionToXmlString(S, tmpF);

            return XMLstr;
        }


    } // end of class ReportUsage
} // end of namepace G25.CG.Shared
