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

namespace G25.CG.Java
{
    /// <summary>
    /// Handles code generation for general multivectors (classes, constructors, set functions, etc).
    /// </summary>
    public class GMV
    {
        /// <summary>
        /// Writes comments of a GMV class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'GMV'.</param>
        /// <param name="gmv">The general multivector for which the class should be written.</param>
        public static void WriteComment(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GMV gmv)
        {
            SB.AppendLine("/**");
            SB.AppendLine(" * This class can hold a general multivector.");
            SB.AppendLine(" * ");

            SB.AppendLine(" * The coordinates are stored in type " + FT.type + ".");
            SB.AppendLine(" * ");

            SB.AppendLine(" * There are " + gmv.NbGroups + " coordinate groups:");
            for (int g = 0; g < gmv.NbGroups; g++)
            {
                SB.Append(" * group " + g + ":");
                for (int i = 0; i < gmv.Group(g).Length; i++)
                {
                    if (i > 0) SB.Append(", ");
                    SB.Append(gmv.Group(g)[i].ToString(S.m_basisVectorNames));

                }
                if (gmv.Group(g).Length > 0)
                    SB.Append("  (grade " + gmv.Group(g)[0].Grade() + ")");

                SB.AppendLine(".");
            }
            SB.AppendLine(" * ");

            switch (S.m_GMV.MemoryAllocationMethod)
            {
                case G25.GMV.MEM_ALLOC_METHOD.PARITY_PURE:
                    SB.AppendLine(" * " + (gmv.NbCoordinates / 2) + " " + FT.type + "s are allocated inside the struct ('parity pure').");
                    SB.AppendLine(" * Hence creating a multivector which needs more than that number of coordinates ");
                    SB.AppendLine(" * will result in unpredictable behaviour (buffer overflow).");
                    break;
                case G25.GMV.MEM_ALLOC_METHOD.FULL:
                    SB.AppendLine(" * " + gmv.NbCoordinates + " " + FT.type + "s are allocated inside the struct.");
                    break;
            }

            SB.AppendLine(" */");
        }

        /// <summary>
        /// Writes member variables of a GMV class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="gmv">The general multivector for which the struct should be written.</param>
        public static void WriteMemberVariables(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GMV gmv)
        {
            cgd.m_cog.EmitTemplate(SB, "GMVmemberVariables", "S=", S, "FT=", FT);
        }

        /// <summary>
        /// Writes constructors of a GMV class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="gmv">The general multivector for which the struct should be written.</param>
        /// <param name="className">Mangled name of GMV class.</param>
        public static void WriteConstructors(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GMV gmv, string className)
        {
            cgd.m_cog.EmitTemplate(SB, "GMVconstructors", "S=", S, "FT=", FT, "className=", className);
        }

        /// <summary>
        /// Writes gu() and c() functions.
        /// </summary>
        /// <param name="SB"></param>
        /// <param name="S"></param>
        /// <param name="cgd"></param>
        /// <param name="FT"></param>
        public static void WriteGetGuC(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT)
        {
            cgd.m_cog.EmitTemplate(SB, "GMVgetGroupUsageCoords", "S=", S, "FT=", FT);
        }


        /// <summary>
        /// Generates a source file with the GMV class definition.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd"></param>
        /// <param name="FT"></param>
        /// <returns></returns>
        public static string GenerateCode(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT)
        {
            G25.GMV gmv = S.m_GMV;
            string className = FT.GetMangledName(S, gmv.Name);

            // get filename, list of generated filenames
            List<string> generatedFiles = new List<string>();
            string sourceFilename = MainGenerator.GetClassOutputPath(S, className);
            generatedFiles.Add(sourceFilename);

            // get StringBuilder where all generated code goes
            StringBuilder SB = new StringBuilder();

            // output license, copyright
            G25.CG.Shared.Util.WriteCopyright(SB, S);
            G25.CG.Shared.Util.WriteLicense(SB, S);

            // open namespace
            G25.CG.Shared.Util.WriteOpenNamespace(SB, S);

            // write class comment
            WriteComment(SB, S, cgd, FT, gmv);

            // open class
            G25.CG.Shared.Util.WriteOpenClass(SB, S, G25.CG.Shared.AccessModifier.AM_public, className, null, null);

            // write member vars
            WriteMemberVariables(SB, S, cgd, FT, gmv);

            // write constructors
            WriteConstructors(SB, S, cgd, FT, gmv, className);

            WriteGetGuC(SB, S, cgd, FT);

            // write 'set' functions
            G25.CG.CSJ.GMV.WriteSetZero(SB, S, cgd, FT);
            G25.CG.CSJ.GMV.WriteSetScalar(SB, S, cgd, FT);
            G25.CG.CSJ.GMV.WriteSetCompressedArray(SB, S, cgd, FT);
            G25.CG.CSJ.GMV.WriteSetExpandedArray(SB, S, cgd, FT);
            G25.CG.CSJ.GMV.WriteGMVtoGMVcopy(SB, S, cgd, FT);
            G25.CG.CSJ.GMV.WriteSMVtoGMVcopy(SB, S, cgd, FT);

            // write 'get coordinate' functions
            G25.CG.CSJ.GMV.WriteGetCoord(SB, S, cgd, FT);

            // write SetGroupUsage()
            G25.CG.CSJ.GMV.WriteSetGroupUsage(SB, S, cgd, FT);        

            // write 'reserve group' functions
            G25.CG.CSJ.GMV.WriteReserveGroup(SB, S, cgd, FT);

            // write 'set coordinate' functions
            G25.CG.CSJ.GMV.WriteSetCoord(SB, S, cgd, FT);

            // write 'largest coordinate' functions
            G25.CG.CSJ.GMV.WriteLargestCoordinates(SB, S, cgd, FT);

            // close class
            G25.CG.Shared.Util.WriteCloseClass(SB, S, className);

            // close namespace
            G25.CG.Shared.Util.WriteCloseNamespace(SB, S);

            // write all to file
            G25.CG.Shared.Util.WriteFile(sourceFilename, SB.ToString());

            return sourceFilename;
        }

#if RIEN
        
        public static string SET_GROUP_USAGE = "setGroupUsage";

        public GMV(Specification S, CG.Shared.CGdata cgd)
        {
            m_specification = S;
            m_cgd = cgd;
        }

        protected Specification m_specification;
        protected CG.Shared.CGdata m_cgd;



        /// <summary>
        /// Writes floating point type definition of a GMV class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="gmv">The general multivector for which the struct should be written.</param>
        /// <param name="className">Mangled name of GMV class.</param>
        public static void WriteFloatType(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GMV gmv, string className)
        {
            cgd.m_cog.EmitTemplate(SB, "classFloatType", "S=", S, "FT=", FT, "className=", className);
        }

        /// <summary>
        /// Writes constructors of a GMV class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="gmv">The general multivector for which the struct should be written.</param>
        /// <param name="className">Mangled name of GMV class.</param>
        public static void WriteConstructors(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GMV gmv, string className)
        {
            cgd.m_cog.EmitTemplate(SB, "GMVconstructors", "S=", S, "FT=", FT, "className=", className);
        }

         /// <summary>
        /// Writes assignment operators of a GMV class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="gmv">The general multivector for which the struct should be written.</param>
        /// <param name="className">Mangled name of GMV class.</param>
        public static void WriteAssignmentOperators(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GMV gmv, string className)
        {
            cgd.m_cog.EmitTemplate(SB, "GMVassignmentOps", "S=", S, "FT=", FT, "className=", className);
        }

         /// <summary>
        /// Writes declarations of set functions of a GMV class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="gmv">The general multivector for which the struct should be written.</param>
        /// <param name="className">Mangled name of GMV class.</param>
        public static void WriteSetDeclarations(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GMV gmv, string className)
        {
            cgd.m_cog.EmitTemplate(SB, "GMVsetDecl", "S=", S, "FT=", FT, "className=", className);
        }

        /// <summary>
        /// Writes declarations of set functions of a GMV class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="gmv">The general multivector for which the struct should be written.</param>
        /// <param name="className">Mangled name of GMV class.</param>
        public static void WriteCompressExpandDeclarations(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GMV gmv, string className)
        {
            cgd.m_cog.EmitTemplate(SB, "GMVcompressDecl", "S=", S, "FT=", FT, "className=", className, "gmv=", gmv);
        }

        /// <summary>
        /// Writes declarations of set functions of a GMV class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="gmv">The general multivector for which the struct should be written.</param>
        /// <param name="className">Mangled name of GMV class.</param>
        public static void WriteLargestCoordinateDeclarations(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GMV gmv, string className)
        {
            cgd.m_cog.EmitTemplate(SB, "GMVlargestCoordinateDecl", "FT=", FT);
        }


        /// <summary>
        /// Writes declarations of set functions of a GMV class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="gmv">The general multivector for which the struct should be written.</param>
        /// <param name="className">Mangled name of GMV class.</param>
        public static void WriteToString(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GMV gmv, string className)
        {
            cgd.m_cog.EmitTemplate(SB, "MVtoStringHeader", "NAMESPACE=", (S.m_namespace.Length > 0) ? "::" + S.m_namespace + "::" : "");
        }


        /// <summary>
        /// Writes the definition of an GMV class to 'SB' (including comments).
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="gmv">The general multivector for which the struct should be written.</param>
        public static void WriteGMVclass(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.GMV gmv)
        {
            SB.AppendLine("");

            WriteComment(SB, S, cgd, FT, gmv);

            string className = FT.GetMangledName(S, gmv.Name);

            SB.AppendLine("class " + className);
            SB.AppendLine("{");

            WriteMemberVariables(SB, S, cgd, FT, gmv);

            SB.AppendLine("public:");

            WriteFloatType(SB, S, cgd, FT, gmv, className);

            WriteConstructors(SB, S, cgd, FT, gmv, className);

            WriteAssignmentOperators(SB, S, cgd, FT, gmv, className);

            WriteSetDeclarations(SB, S, cgd, FT, gmv, className);

            WriteGetCoord(S, cgd, FT, SB);
            WriteSetCoord(S, cgd, FT, SB);

            WriteCompressExpandDeclarations(SB, S, cgd, FT, gmv, className);

            WriteLargestCoordinateDeclarations(SB, S, cgd, FT, gmv, className);

            // grade usage
            SB.AppendLine("\t/// returns grade/group.");
            SB.AppendLine("\tinline int gu() const {return m_gu;}");

            WriteToString(SB, S, cgd, FT, gmv, className);

            // function which returns pointer to some array of zeros
            SB.AppendLine("public:");
            SB.AppendLine("\tinline " + FT.type + " const *nullFloats() const {");
	        SB.AppendLine("\t\tstatic " + FT.type + " *nf = NULL;");
	        SB.AppendLine("\t\treturn (nf == NULL) ? (nf = new " + FT.type + "[" + S.m_GMV.NbCoordinates + "]) : nf;");
	        SB.AppendLine("\t}");

            // function for setting grade/group usage, reallocting memory
            cgd.m_cog.EmitTemplate(SB, "GMVsetGroupUsage", "S=", S, "FT=", FT, "className=", className, "gmv=", gmv);

            // function for allocating a group
            cgd.m_cog.EmitTemplate(SB, "GMVallocateGroups", "S=", S, "FT=", FT, "className=", className, "gmv=", gmv);


            SB.AppendLine("}; // end of class " + className);

        } // end of WriteGMVclass()

        /// <summary>
        /// Writes classes for GMV to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteGMVclasses(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            // for each float type
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                WriteGMVclass(SB, S, cgd, FT, S.m_GMV);
            }
        }

        /// <summary>
        /// Writes typenames of all GMV to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteGMVtypes(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            // for each float type
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string className = FT.GetMangledName(S, S.m_GMV.Name);
                // typedef
                SB.AppendLine("class " + className + ";");                        
            }
        }

        /// <summary>
        /// Writes functions to set the GMV types to zero.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSetZero(Specification S, G25.CG.Shared.CGdata cgd)
        {
            //StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string className = FT.GetMangledName(S, gmv.Name);
                string funcName = "set";

                // do we inline this func?
                string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

                string funcDecl = inlineStr + "void " + className + "::" + funcName + "()";

                //declSB.Append(funcDecl);
                //declSB.AppendLine(";");

                defSB.Append(funcDecl);
                defSB.AppendLine(" {");
                defSB.AppendLine("\t" + SET_GROUP_USAGE + "(0);");

                if (S.m_reportUsage)
                {
                    defSB.AppendLine("\tm_t = " + G25.CG.Shared.ReportUsage.GetSpecializedConstantName(S, gmv.Name) + ";");
                }

                defSB.AppendLine("}");
            }
        }

        /// <summary>
        /// Writes functions to set the GMV types to scalar value.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSetScalar(Specification S, G25.CG.Shared.CGdata cgd)
        {
            //StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string className = FT.GetMangledName(S, gmv.Name);
                string funcName = "set";

                // do we inline this func?
                string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

                string funcDecl = inlineStr + "void " + className + "::" + funcName + "(" + FT.type + " val)";

                defSB.Append(funcDecl);
                defSB.AppendLine(" {");
                defSB.AppendLine("\t" + SET_GROUP_USAGE + "(" + (1 << gmv.GetGroupIdx(RefGA.BasisBlade.ONE)) + ");");
                defSB.AppendLine("\tm_c[0] = val;");

                if (S.m_reportUsage)
                {
                    defSB.AppendLine("\tm_t = " + G25.CG.Shared.ReportUsage.GetSpecializedConstantName(S, FT.type) + ";");
                }

                defSB.AppendLine("}");
            }
        }

        /// <summary>
        /// Writes functions to set the GMV types by array value.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSetArray(Specification S, G25.CG.Shared.CGdata cgd)
        {
            //StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string className = FT.GetMangledName(S, gmv.Name);
                string funcName = "set";

                // do we inline this func?
                string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

                string funcDecl = inlineStr + "void " + className + "::" + funcName + "(int gu, const " + FT.type + " *arr)";

                defSB.Append(funcDecl);
                defSB.AppendLine(" {");
                defSB.AppendLine("\t" + SET_GROUP_USAGE + "(gu);");
                defSB.AppendLine("\t" + G25.CG.Shared.Util.GetCopyCode(S, FT, "arr", "m_c", S.m_namespace + "_mvSize[gu]"));

                if (S.m_reportUsage)
                {
                    defSB.AppendLine("\tm_t = " + G25.CG.Shared.ReportUsage.GetSpecializedConstantName(S, gmv.Name) + ";");
                }

                defSB.AppendLine("}");
            }
        }

        /// <summary>
        /// Writes functions to copy GMVs from one float type to another.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteGMVtoGMVcopy(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;
            foreach (G25.FloatType srcFT in S.m_floatTypes)
            {
                String srcClassName = srcFT.GetMangledName(S, gmv.Name);
                foreach (G25.FloatType dstFT in S.m_floatTypes)
                {
                    string dstClassName = dstFT.GetMangledName(S, gmv.Name);

                    string funcName = "set";

                    // do we inline this func?
                    string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

                    string funcDecl = inlineStr + "void " + dstClassName + "::" + funcName + "(const " + srcClassName + " &src)";

                    defSB.Append(funcDecl);
                    defSB.AppendLine(" {");
                    defSB.AppendLine("\t" + SET_GROUP_USAGE + "(src.gu());");
                    defSB.AppendLine("\tconst " + srcFT.type + "*srcC = src.getC();");
                    if (dstFT == srcFT)
                    {
                        defSB.AppendLine("\t" + G25.CG.Shared.Util.GetCopyCode(S, dstFT, "srcC", "m_c", S.m_namespace + "_mvSize[src.gu()]"));
                    }
                    else
                    {
                        defSB.AppendLine("\tfor (int i = 0; i < " + S.m_namespace + "_mvSize[src.gu()]; i++)");
                        defSB.AppendLine("\t\tm_c[i] = (" + dstFT.type + ")srcC[i];");
                    }

                    if (S.m_reportUsage)
                    {
                        defSB.AppendLine("\tm_t = src.m_t;");
                    }

                    defSB.AppendLine("}");
                }
            }
        }

        /// <summary>
        /// Writes functions to copy GMVs to SMVs
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteGMVtoSMVcopy(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string srcClassName = FT.GetMangledName(S, gmv.Name);
                for (int s = 0; s < S.m_SMV.Count; s++)
                {
                    G25.SMV smv = S.m_SMV[s];
                    string dstClassName = FT.GetMangledName(S, smv.Name);

                    bool dstPtr = true;
                    String[] smvAccessStr = G25.CG.Shared.CodeUtil.GetAccessStr(S, smv, G25.CG.Shared.SmvUtil.THIS, dstPtr);

                    string funcName = "set";

                    // do we inline this func?
                    string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

                    string funcDecl = inlineStr + "void " +dstClassName + "::" + funcName + "(const " + srcClassName + " &src)";

                    defSB.Append(funcDecl);
                    {
                        defSB.AppendLine(" {");
                        defSB.AppendLine("\tconst " + FT.type + " *ptr = src.getC();\n");

                        // get a dictionary which tells you for each basis blade of 'smv' where it is in 'gmv'
                        Dictionary<Tuple<int, int>, Tuple<int, int>> D = G25.MV.GetCoordMap(smv, gmv);

                        // what is the highest group of the 'gmv' that must be (partially) copied to the 'smv'
                        int highestGroup = -1;
                        foreach (KeyValuePair<Tuple<int, int>, Tuple<int, int>> KVP in D)
                            if (KVP.Value.Value1 > highestGroup) highestGroup = KVP.Value.Value1;

                        // generate code for each group
                        for (int g = 0; g <= highestGroup; g++)
                        {
                            // determine if group 'g' is to be copied to smv:
                            bool groupIsUsedBySMV = false;
                            foreach (KeyValuePair<Tuple<int, int>, Tuple<int, int>> KVP in D)
                            {
                                if (KVP.Value.Value1 == g)
                                {
                                    groupIsUsedBySMV = true;
                                    break;
                                }
                            }

                            // if group is present in GMV:
                            defSB.AppendLine("\tif (src.gu() & " + (1 << g) + ") {");
                            if (groupIsUsedBySMV)
                            {
                                bool mustCast = false;
                                bool srcPtr = true;
                                int nbTabs = 2;
                                RefGA.Multivector[] value = G25.CG.Shared.Symbolic.GMVtoSymbolicMultivector(S, gmv, "ptr", srcPtr, g);
                                bool writeZeros = false;
                                string str = G25.CG.Shared.CodeUtil.GenerateSMVassignmentCode(S, FT, mustCast, smv, G25.CG.Shared.SmvUtil.THIS, dstPtr, value[g], nbTabs, writeZeros);
                                defSB.Append(str);
                            }
                            if ((g+1) <= highestGroup)
                                defSB.AppendLine("\t\tptr += " + gmv.Group(g).Length + ";");
                            defSB.AppendLine("\t}");

                            // else, if group is not present in GMV:
                            if (groupIsUsedBySMV)
                            {
                                defSB.AppendLine("\telse {");
                                foreach (KeyValuePair<Tuple<int, int>, Tuple<int, int>> KVP in D)
                                    if ((KVP.Value.Value1 == g) && (!smv.IsCoordinateConstant(KVP.Key.Value2)))
                                    {
                                        // translate KVP.Key.Value2 to non-const idx, because the accessStrs are only about non-const blades blades!
                                        int bladeIdx = smv.BladeIdxToNonConstBladeIdx(KVP.Key.Value2);
                                        defSB.AppendLine("\t\t" + smvAccessStr[bladeIdx] + " = " + FT.DoubleToString(S, 0.0) + ";");
                                    }
                                defSB.AppendLine("\t}");
                            }
                        }
                        defSB.AppendLine("}");
                    }
                } // end of loop over all SMVs
            } // end of loop over all float types
        } // end of WriteGMVtoSMVcopy()


        /// <summary>
        /// Writes functions to copy SMVs to GMVs
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSMVtoGMVcopy(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;
            Boolean gmvParityPure = (S.m_GMV.MemoryAllocationMethod == G25.GMV.MEM_ALLOC_METHOD.PARITY_PURE);
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                String dstClassName = FT.GetMangledName(S, gmv.Name);
                for (int s = 0; s < S.m_SMV.Count; s++)
                {
                    G25.SMV smv = S.m_SMV[s];

                    // do not generate converter if the GMV cannot hold the type
                    if (gmvParityPure && (!smv.IsParityPure())) continue;

                    string srcClassName = FT.GetMangledName(S, smv.Name);

                    string funcName = "set";

                    // do we inline this func?
                    string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

                    string funcDecl = inlineStr + "void " + dstClassName + "::" + funcName + "(const " + srcClassName + " &src)";

                    defSB.Append(funcDecl);

                    {
                        defSB.AppendLine(" {");

                        // get a dictionary which tells you for each basis blade of 'gmv' where it is in 'smv'
                        Dictionary<Tuple<int, int>, Tuple<int, int>> D = G25.MV.GetCoordMap(smv, gmv);

                        // convert SMV to symbolic Multivector:
                        bool smvPtr = false;
                        RefGA.Multivector value = G25.CG.Shared.Symbolic.SMVtoSymbolicMultivector(S, smv, "src", smvPtr);

                        // find out which groups are present
                        int gu = 0;
                        foreach (KeyValuePair<Tuple<int, int>, Tuple<int, int>> KVP in D)
                            gu |= 1 << KVP.Value.Value1;

                        // generate the code to set group usage:
                        defSB.AppendLine("\t" + SET_GROUP_USAGE + "(" + gu + ");");

                        // a helper pointer which is incremented
                        string dstArrName = "ptr";
                        defSB.AppendLine("\t" + FT.type + " *" + dstArrName + " = m_c;");


                        // for each used group, generate the assignment code
                        for (int g = 0; (1 << g) <= gu; g++)
                        {
                            if (((1 << g) & gu) != 0)
                            {
                                bool mustCast = false;
                                int nbTabs = 1;
                                bool writeZeros = true;
                                String str = G25.CG.Shared.CodeUtil.GenerateGMVassignmentCode(S, FT, mustCast, gmv, dstArrName, g, value, nbTabs, writeZeros);
                                defSB.Append(str);

                                if ((1 << (g+1)) <= gu)
                                    defSB.AppendLine("\tptr += " + gmv.Group(g).Length + ";");
                            }

                        }

                        if (S.m_reportUsage)
                        {
                            defSB.AppendLine("\tm_t = " + G25.CG.Shared.ReportUsage.GetSpecializedConstantName(S, smv.Name) + ";");
                        }

                        defSB.AppendLine("}");
                    }
                } // end of loop over all SMVs
            } // end of loop over all float types
        } // end of WriteGMVtoSMVcopy()


        private static void WriteGetCoordFunction(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, StringBuilder SB,
            string gmvTypeName, int groupIdx, int elementIdx, RefGA.BasisBlade B)
        {
            String bladeName = B.ToLangString(S.m_basisVectorNames);

            // do we inline this func?
            string inlineStr = "inline ";

            string funcName = MainGenerator.GETTER_PREFIX + bladeName;

            string funcDecl = "\t" + inlineStr + FT.type + " " + funcName + "() const";

            SB.AppendLine("\t/// Returns the " + bladeName + " coordinate of this " + gmvTypeName + ".");
            SB.Append(funcDecl);
            SB.AppendLine(" {");
            SB.AppendLine("\t\treturn (m_gu & " + (1 << groupIdx) + ") ? " +
                "m_c[" + S.m_namespace + "_mvSize[m_gu & " + ((1 << groupIdx) - 1) + "] + " + elementIdx + "] : " + 
                FT.DoubleToString(S, 0.0) + ";");
            SB.AppendLine("\t}");
        }

        /// <summary>
        /// Writes functions to extract coordinates from the GMV
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT"></param>
        /// <param name="SB"></param>
        public static void WriteGetCoord(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, StringBuilder SB)
        {
            G25.GMV gmv = S.m_GMV;
            string typeName = FT.GetMangledName(S, gmv.Name);

            for (int groupIdx = 0; groupIdx < gmv.NbGroups; groupIdx++)
            {
                for (int elementIdx = 0; elementIdx < gmv.Group(groupIdx).Length; elementIdx++)
                {
                    WriteGetCoordFunction(S, cgd, FT, SB, typeName, groupIdx, elementIdx, gmv.Group(groupIdx)[elementIdx]);
                }
            }

            SB.AppendLine("\t/// Returns array of compressed coordinates.");
            SB.AppendLine("\tinline const " + FT.type + " *getC() const { return m_c;}");

        }

        private static void WriteSetCoordFunction(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, StringBuilder SB,
            string gmvTypeName, int groupIdx, int elementIdx, int groupSize, RefGA.BasisBlade B)
        {
            String bladeName = B.ToLangString(S.m_basisVectorNames);

            // do we inline this func?
            string inlineStr = "inline ";

            string funcName = MainGenerator.SETTER_PREFIX + bladeName;

            string coordName = "val";

            string funcDecl = "\t" + inlineStr + "void " + funcName + "(" + FT.type + " " + coordName + ") ";

            SB.AppendLine("\t/// Sets the " + bladeName + " coordinate of this " + gmvTypeName + ".");
            SB.Append(funcDecl);
            SB.AppendLine(" {");

            SB.AppendLine("\t\treserveGroup_" + groupIdx + "();");

            SB.AppendLine("\t\tm_c[" + S.m_namespace + "_mvSize[m_gu & " + ((1 << groupIdx) - 1) + "] + " + elementIdx + "] = " + coordName + ";");

            SB.AppendLine("\t}");
        }

        /// <summary>
        /// Writes functions to set coordinates of the GMV
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT"></param>
        /// <param name="SB"></param>
        public static void WriteSetCoord(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, StringBuilder SB)
        {
            G25.GMV gmv = S.m_GMV;
            string typeName = FT.GetMangledName(S, gmv.Name);

            for (int groupIdx = 0; groupIdx < gmv.NbGroups; groupIdx++)
            {
                for (int elementIdx = 0; elementIdx < gmv.Group(groupIdx).Length; elementIdx++)
                {
                    WriteSetCoordFunction(S, cgd, FT, SB, typeName, groupIdx, elementIdx, gmv.Group(groupIdx).Length, gmv.Group(groupIdx)[elementIdx]);
                }
            }

        }

        /// <summary>
        /// Writes code for abs largest coordinate
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd"></param>
        public static void WriteLargestCoordinateDefinitions(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder defSB = cgd.m_defSB;

            G25.GMV gmv = S.m_GMV;

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string fabsFunc = "fabs";
                if (FT.type == "float") fabsFunc = "fabsf";

                string gmvName = FT.GetMangledName(S, gmv.Name);

                cgd.m_cog.EmitTemplate(defSB, "GMVlargestCoordinateDef",
                    "S=", S,
                    "FT=", FT,
                    "gmvName=", gmvName,
                    "fabsFunc=", fabsFunc);
            }
        } // end of WriteLargestCoordinateFunctions()



        /// <summary>
        /// Writes code to extract the scalar part of an SMV via a non-member function.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteExtractScalarPart(Specification S, G25.CG.Shared.CGdata cgd)
        {
            G25.GMV gmv = S.m_GMV;

            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineFunctions) ? cgd.m_inlineDefSB : cgd.m_defSB;
            declSB.AppendLine("");
            defSB.AppendLine("");

            const string gmvName = "x";
            RefGA.BasisBlade scalarBlade = RefGA.BasisBlade.ONE;
            string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineFunctions, " ");

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string className = FT.GetMangledName(S, gmv.Name);
                string funcName = "_" + FT.type;
                string altFuncName = "_Float";
                string comment = "/// Returns scalar part of  " + className;

                declSB.AppendLine(comment);
                string funcDecl = FT.type + " " + funcName + "(const " + className + " &" + gmvName + ")";
                declSB.Append(funcDecl);
                declSB.AppendLine(";");

                declSB.AppendLine(comment);
                string altFuncDecl = FT.type + " " + altFuncName + "(const " + className + " &" + gmvName + ")";
                declSB.Append(G25.CG.Shared.Util.GetInlineString(S, true, " ") + altFuncDecl + " {return " + funcName + "(" + gmvName + "); }");
                declSB.AppendLine(";");

                defSB.Append(inlineStr + funcDecl);
                {
                    defSB.AppendLine(" {");

                    int elementIdx = gmv.GetElementIdx(scalarBlade);
                    double multiplier = 1.0 / gmv.BasisBlade(0, elementIdx).scale;

                    string multiplerString = (multiplier != 1.0) ? (FT.DoubleToString(S, multiplier) + " * ") : "";

                    // this line assumes that the scalar is the first element of the first group (which is a requirement).
                    defSB.AppendLine("\treturn ((x.gu() & 1) != 0) ? " + multiplerString + "x.getC()[0] : " + FT.DoubleToString(S, 0.0) + ";");

                    defSB.AppendLine("}");
                }
            } // end of loop over all float types
        } // end of WriteExtractScalarPart()

        public void WriteCompressExpand(Specification S, G25.CG.Shared.CGdata cgd)
        {
            G25.GMV gmv = S.m_GMV;
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = cgd.m_defSB;
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                string className = FT.GetMangledName(S, gmv.Name);
                string fabsFunc = (FT.type == "float") ? "fabsf" : "fabs";

                cgd.m_cog.EmitTemplate(declSB, "compressDecl", "S=", S, "FT=", FT, "className=", className, "gmv=", gmv);
                cgd.m_cog.EmitTemplate(defSB, "compressDef", "S=", S, "FT=", FT, "className=", className, "gmv=", gmv, "fabsFunc=", fabsFunc);
            }
        }


        /// <summary>
        /// Writes set, setZero, copy and copyCrossFloat, coord extract, largest coordinate functions for all general multivector types.
        /// </summary>
        public void WriteSetFunctions()
        {
            WriteSetZero(m_specification, m_cgd);
            WriteSetScalar(m_specification, m_cgd);
            WriteSetArray(m_specification, m_cgd);
            WriteGMVtoGMVcopy(m_specification, m_cgd);
            WriteGMVtoSMVcopy(m_specification, m_cgd);
            WriteSMVtoGMVcopy(m_specification, m_cgd);
            WriteLargestCoordinateDefinitions(m_specification, m_cgd);
            WriteExtractScalarPart(m_specification, m_cgd);
            WriteCompressExpand(m_specification, m_cgd);
        }
#endif
    } // end of class GMV

} // end of namespace G25.CG.Java

