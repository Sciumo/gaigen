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
    /// Handles code generation for specialized multivectors (classes, constructors, set functions, etc).
    /// </summary>
    public class SMV
    {

        /// <summary>
        /// Generates a source file with the SMV class definition.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd"></param>
        /// <param name="smv"></param>
        /// <param name="FT"></param>
        /// <returns></returns>
        public static string GenerateCode(Specification S, G25.CG.Shared.CGdata cgd, G25.SMV smv, FloatType FT)
        {
            string className = FT.GetMangledName(S, smv.Name);

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

            // todo: write class comment

            // open class
            G25.CG.Shared.Util.WriteOpenClass(SB, S, G25.CG.Shared.AccessModifier.AM_public, className, null, null);

            // ....

            // close class
            G25.CG.Shared.Util.WriteCloseClass(SB, S, className);

            // close namespace
            G25.CG.Shared.Util.WriteCloseNamespace(SB, S);

            // write all to file
            G25.CG.Shared.Util.WriteFile(sourceFilename, SB.ToString());

            return sourceFilename;
        }

#if RIEN
        public SMV(Specification S, CG.Shared.CGdata cgd)
        {
            m_specification = S;
            m_cgd = cgd;
        }

        protected Specification m_specification;
        protected CG.Shared.CGdata m_cgd;

        public static Dictionary<string, int> GetSpecializedTypeDictionary(Specification S) {
            Dictionary<string, int> D = new Dictionary<string, int>();

            int idx = 0;

            // gmv
            D[S.m_GMV.Name] = idx;
            idx++;

            // float types
            foreach (FloatType FT in S.m_floatTypes)
            {
                D[FT.GetName()] = idx;
                idx++;
            }

            // specialized types
            foreach (G25.SMV smv in S.m_SMV)
            {
                D[smv.GetName()] = idx;
                idx++;
            }

            return D;

        }


        public static void WriteSMVtypeConstants(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            SB.AppendLine("");

            SB.AppendLine("// These constants define a unique number for each specialized multivector type.");
            SB.AppendLine("// They are used to report usage of non-optimized functions.");

            /*{
                int idx = 0;
                // for each floating point type
                foreach (G25.FloatType FT in S.m_floatTypes)
                {
                    // for each smv
                    foreach (G25.SMV smv in S.m_SMV)
                    {
                        string name = (S.m_namespace + "_" + FT.GetMangledName(smv.Name)).ToUpper();
                        SB.AppendLine("#define " + name + " " + idx);
                        idx++;

                    }
                }
            }*/
            Dictionary<string, int> STD = GetSpecializedTypeDictionary(S);
            SB.AppendLine("typedef enum {");
            SB.AppendLine("\t" + G25.CG.Shared.ReportUsage.GetSpecializedConstantName(S, "NONE") + " = -1,");

            foreach (KeyValuePair<string, int> kvp in STD)
            {
                string name = G25.CG.Shared.ReportUsage.GetSpecializedConstantName(S, kvp.Key);
                SB.AppendLine("\t" + name + " = " + kvp.Value + ",");
            }

            SB.AppendLine("\t" + G25.CG.Shared.ReportUsage.GetSpecializedConstantName(S, G25.CG.Shared.ReportUsage.INVALID));
            SB.AppendLine("} SMV_TYPE;");

            SB.AppendLine("");
            SB.AppendLine("/// For each specialized multivector type, the mangled typename.");
            SB.AppendLine("/// This is used to report usage of non-optimized functions.");
            SB.AppendLine("extern const char *g_" + S.m_namespace + "Typenames[];");

        }


        public static void WriteSMVtypenames(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            Dictionary<string, int> STD = GetSpecializedTypeDictionary(S);

            SB.AppendLine("");
            SB.AppendLine("const char *g_" + S.m_namespace + "Typenames[] = ");
            SB.AppendLine("{");
            {
                /*// for each floating point type
                foreach (G25.FloatType FT in S.m_floatTypes)
                {
                    // for each smv
                    foreach (G25.SMV smv in S.m_SMV)
                    {
                        if (idx > 0) SB.AppendLine(",");
                        SB.Append("\t\"" + FT.GetMangledName(smv.Name) + "\"");
                        idx++;
                    }
                }*/
                bool first = true;
                foreach (KeyValuePair<string, int> kvp in STD)
                {
                    if (!first) SB.AppendLine(",");
                    SB.Append("\t\"" + kvp.Key + "\"");
                    first = false;
                }

                if (STD.Count == 0) SB.Append("\t\"There are no specialized types defined\"");
            }
            SB.AppendLine("");
            SB.AppendLine("};");
        }



        /// <summary>
        /// Returns a symbolic name for the coordinate 'idx' of 'smv' which can be used in the
        /// output code.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="smv">The specialized multivector for which the coordinate indices should be written.</param>
        /// <param name="idx">The specialized multivector for which the coordinate indices should be written.</param>
        /// <param name="FT"></param>
        /// <returns>The symbol for the define for coordinate index 'idx' of 'smv'.</returns>
        public static string GetCoordIndexDefine(Specification S, FloatType FT, G25.SMV smv, int idx)
        {
            return smv.GetCoordLangID(idx, S, COORD_STORAGE.VARIABLES).ToUpper();
//                    return FT.GetMangledName(smv.Name) + "::" + smv.GetCoordLangID(idx, S, COORD_STORAGE.VARIABLES).ToUpper();
        }

        /// <summary>
        /// Writes the <c>defines</c> for indices of the smv struct to 'SB'. For example,  <c>define VECTOR_E1 0</c>.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names.</param>
        /// <param name="FT"></param>
        /// <param name="smv">The specialized multivector for which the coordinate indices should be written.</param>
        public static void WriteSMVcoordIndices(StringBuilder SB, Specification S, FloatType FT, G25.SMV smv)
        {
            string className = FT.GetMangledName(S, smv.Name);
            SB.AppendLine("\t/// Array indices of " + className + " coordinates.");
            SB.AppendLine("\ttypedef enum {");
            for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
            {
                SB.AppendLine("\t\t/// index of coordinate for " + smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames) + " in " + className);
                SB.AppendLine("\t\t" + GetCoordIndexDefine(S, FT, smv, i) + " = " + i + ", ");
            }
            SB.AppendLine("\t} ArrayIndex;");
        }

        /// <summary>
        /// Writes the <c>defines</c> for indices of the smv struct to 'SB'. For example,  <c>define VECTOR_E1 0</c>.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names.</param>
        /// <param name="FT"></param>
        /// <param name="smv">The specialized multivector for which the coordinate indices should be written.</param>
        public static void WriteCoordinateOrder(StringBuilder SB, Specification S, FloatType FT, G25.SMV smv)
        {
            //string className = FT.GetMangledName(smv.Name);
            SB.AppendLine("\ttypedef enum {");
            SB.AppendLine("\t\t/// the order of coordinates (this is the type of the first argument of coordinate-handling functions)");
            SB.AppendLine("\t\t" + G25.CG.Shared.SmvUtil.GetCoordinateOrderConstant(S, smv));
            SB.AppendLine("\t} " + G25.CG.Shared.SmvUtil.COORDINATE_ORDER_ENUM + ";");
        }

        /// <summary>
        /// Writes the SMV class comment 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Not used yet.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="smv">The specialized multivector for which the struct should be written.</param>
        /// <param name="emitCoordIndices">Whether to emit constants for array indices to coordinates.</param>
        public static void WriteComment(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv, bool emitCoordIndices)
        {
            SB.AppendLine("/**");
            SB.AppendLine(" * This class can hold a specialized multivector of type " + FT.GetMangledName(S, smv.Name) + ".");
            SB.AppendLine(" * ");

            SB.AppendLine(" * The coordinates are stored in type  " + FT.type + ".");
            SB.AppendLine(" * ");

            if (smv.NbNonConstBasisBlade > 0)
            {
                SB.AppendLine(" * The variable non-zero coordinates are:");
                for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                {
                    SB.AppendLine(" *   - coordinate " + smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames) + "  (array index: " + GetCoordIndexDefine(S, FT, smv, i) + " = " + i + ")");
                }
            }
            else SB.AppendLine(" * The type is constant.");
            SB.AppendLine(" * ");

            if (smv.NbConstBasisBlade > 0)
            {
                SB.AppendLine(" * The constant non-zero coordinates are:");
                for (int i = 0; i < smv.NbConstBasisBlade; i++)
                    SB.AppendLine(" *   - " + smv.ConstBasisBlade(i).ToString(S.m_basisVectorNames) + " = " + smv.ConstBasisBladeValue(i).ToString());
            }
            else SB.AppendLine(" * The type has no constant coordinates.");
            SB.AppendLine(" * ");

            if ((smv.Comment != null) && (smv.Comment.Length > 0))
            {
                SB.AppendLine(" * ");
                SB.AppendLine(" * " + smv.Comment);
            }
            SB.AppendLine(" */");
        }

        /// <summary>
        /// Writes the SMV class to 'SB' (including comments).
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Not used yet.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="smv">The specialized multivector for which the struct should be written.</param>
        public static void WriteMemberVariables(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv)
        {
            SB.AppendLine("public:");
            // individual coordinates or one array?
            if (S.m_coordStorage == COORD_STORAGE.VARIABLES)
            {
                for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                {
                    SB.AppendLine("\t/// The " + smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames) + " coordinate.");
                    SB.AppendLine("\t" + FT.type + " m_" + smv.GetCoordLangID(i, S) + ";");
                }
            }
            else
            {
                if (smv.NbNonConstBasisBlade > 0) {
                    // emit: float c[3]; // e1, e2, e3
                    SB.AppendLine("\t/// The coordinates (stored in an array).");
                    SB.Append("\t" + FT.type + " m_c[" + smv.NbNonConstBasisBlade + "]; // ");
                    for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                    {
                        if (i > 0) SB.Append(", ");
                        SB.Append(smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames));
                    }
                    SB.AppendLine("");
                }
            }
        }


        /// <summary>
        /// Might need to put this function elsewhere . . .
        /// </summary>
        /// <param name="S"></param>
        /// <param name="smv"></param>
        /// <param name="coordIdx"></param>
        /// <returns></returns>
        public static string GetCoordAccessString(Specification S, G25.SMV smv, int coordIdx) {
            if (S.m_coordStorage == COORD_STORAGE.VARIABLES)
            {
                return "m_" + smv.GetCoordLangID(coordIdx, S);
            }
            else
            {
                return "m_c[" + coordIdx + "]";
            }
        }

        /// <summary>
        /// Writes getters and setters for the SMV coordinates..
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Not used yet.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="smv">The specialized multivector for which the struct should be written.</param>
        public static void WriteGetSetCoord(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv)
        {
            // write variable coordinates
            for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
            {
                string name = smv.NonConstBasisBlade(i).ToLangString(S.m_basisVectorNames);
                string accessName = GetCoordAccessString(S, smv, i);

                SB.AppendLine("\t/// Returns the " + smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames) + " coordinate.");
                SB.AppendLine("\tinline " + FT.type + " " + MainGenerator.GETTER_PREFIX + name + "() const { return " + accessName + ";}");
                SB.AppendLine("\t/// Sets the " + smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames) + " coordinate.");
                SB.AppendLine("\tinline void " + MainGenerator.SETTER_PREFIX + name + "(" + FT.type + " " + name + ") { " + accessName + " = " + name + ";}");
            }

            // write constant coordinates
            for (int i = 0; i < smv.NbConstBasisBlade; i++)
            {
                RefGA.BasisBlade B = smv.ConstBasisBlade(i);
                SB.AppendLine("\t/// Returns the " + B.ToString(S.m_basisVectorNames) + " coordinate.");
                SB.AppendLine("\tinline " + FT.type + " " + MainGenerator.GETTER_PREFIX + B.ToLangString(S.m_basisVectorNames) + "() const { return " + FT.DoubleToString(S, smv.ConstBasisBladeValue(i)) + ";}");
            }

            // write a getter for the scalar which returns 0 if no scalar coordinate is present
            if (smv.GetElementIdx(RefGA.BasisBlade.ONE) < 0) {
                RefGA.BasisBlade B = RefGA.BasisBlade.ONE; 
                SB.AppendLine("\t/// Returns the scalar coordinate (which is always 0).");
                SB.AppendLine("\tinline " + FT.type + " " + MainGenerator.GETTER_PREFIX + B.ToLangString(S.m_basisVectorNames) + "() const { return " + FT.DoubleToString(S, 0.0) + ";}");
            }

            // getter for the coordinates (stored in array)
            if ((S.m_coordStorage == COORD_STORAGE.ARRAY) && (smv.NbNonConstBasisBlade > 0))
            {
                SB.AppendLine("\t/// Returns array of coordinates.");
                SB.AppendLine("\tinline const " + FT.type + " *getC(" + G25.CG.Shared.SmvUtil.COORDINATE_ORDER_ENUM + ") const { return m_c;}");
            }
        }

        /// <summary>
        /// Writes constructors.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Not used yet.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="smv">The specialized multivector for which the struct should be written.</param>
        public static void WriteConstructors(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv)
        {
            cgd.m_cog.EmitTemplate(SB, "SMVconstructors",
                "S=", S,
                "smv=", smv,
                "className=", FT.GetMangledName(S, smv.Name),
                "gmvClassName=", FT.GetMangledName(S, S.m_GMV.Name),
                "FT=", FT);
        }

        /// <summary>
        /// Writes assignment operators.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Not used yet.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="smv">The specialized multivector for which the struct should be written.</param>
        public static void WriteAssignmentOperators(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv)
        {
            cgd.m_cog.EmitTemplate(SB, "SMVassignmentOps",
                "S=", S,
                "smv=", smv,
                "className=", FT.GetMangledName(S, smv.Name),
                "gmvClassName=", FT.GetMangledName(S, S.m_GMV.Name),
                "FT=", FT);
        }

        /// <summary>
        /// Writes set functions..
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Not used yet.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="smv">The specialized multivector for which the struct should be written.</param>
        public static void WriteSetDeclarations(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv)
        {
            cgd.m_cog.EmitTemplate(SB, "SMVsetDecl",
                "S=", S,
                "smv=", smv,
                "className=", FT.GetMangledName(S, smv.Name),
                "gmvClassName=", FT.GetMangledName(S, S.m_GMV.Name),
                "FT=", FT);
        }

        /// <summary>
        /// Writes set functions..
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Not used yet.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="smv">The specialized multivector for which the struct should be written.</param>
        public static void WriteLargestCoordinateDeclarations(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv)
        {
            cgd.m_cog.EmitTemplate(SB, "SMVlargestCoordinate", "FT=", FT);
        }


        /// <summary>
        /// Writes declarations of set functions of a GMV class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="smv">The specialized multivector for which the code should be written.</param>
        public static void WriteToString(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv)
        {
            cgd.m_cog.EmitTemplate(SB, "MVtoStringHeader", "NAMESPACE=", (S.m_namespace.Length > 0) ? "::" + S.m_namespace + "::" : "");
        }

        /// <summary>
        /// Writes floating point type definition of a GMV class to 'SB'.
        /// </summary>
        /// <param name="SB">Where the comment goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="className">Mangled name of SMV class.</param>
        /// <param name="smv"></param>
        public static void WriteFloatType(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv, string className)
        {
            cgd.m_cog.EmitTemplate(SB, "classFloatType", "S=", S, "FT=", FT, "className=", className);
        }

        /// <summary>
        /// Writes the SMV class to 'SB' (including comments).
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Not used yet.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="smv">The specialized multivector for which the struct should be written.</param>
        /// <param name="emitCoordIndices">Whether to emit constants for array indices to coordinates.</param>
        public static void WriteSMVclass(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv, bool emitCoordIndices)
        {
            string className = FT.GetMangledName(S, smv.Name);

            SB.AppendLine("");

            WriteComment(SB, S, cgd, FT, smv, emitCoordIndices);

            // typedef
            SB.AppendLine("class " + className);
            SB.AppendLine("{");

            WriteMemberVariables(SB, S, cgd, FT, smv);

            SB.AppendLine("public:");

            WriteFloatType(SB, S, cgd, FT, smv, className);

            if (emitCoordIndices)
                WriteSMVcoordIndices(SB, S, FT, smv);

            WriteCoordinateOrder(SB, S, FT, smv);

            WriteConstructors(SB, S, cgd, FT, smv);

            WriteAssignmentOperators(SB, S, cgd, FT, smv);

            WriteSetDeclarations(SB, S, cgd, FT, smv);

            WriteLargestCoordinateDeclarations(SB, S, cgd, FT, smv);

            WriteToString(SB, S, cgd, FT, smv);

            SB.AppendLine("");

            WriteGetSetCoord(SB, S, cgd, FT, smv);

            SB.AppendLine("}; // end of class " + className);
        }


        /// <summary>
        /// Writes classes for all SMVs to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSMVclasses(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            // for each float type
            bool emitCoordIndices = true; // ( S.m_coordStorage == COORD_STORAGE.ARRAY); // whether to emit defines such as #define VECTOR_E1 0
            foreach (G25.FloatType FT in S.m_floatTypes) 
            {
                // for each smv
                foreach (G25.SMV smv in S.m_SMV) 
                {
                    WriteSMVclass(SB, S, cgd, FT, smv, emitCoordIndices);
                }

                emitCoordIndices = false; // only emit those on the first loop
            }

        }

        /// <summary>
        /// Writes typenames of all SMVs to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSMVtypes(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            // for each float type
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SMV smv in S.m_SMV)
                {
                    string className = FT.GetMangledName(S, smv.Name);
                    // typedef
                    SB.AppendLine("class " + className + ";");
                }
            }
        }



        /// <summary>
        /// Writes a function to set an SMV struct to zero, for all floating point types.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSetZero(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            defSB.AppendLine("");

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SMV smv in S.m_SMV)
                {
                    if (smv.NbNonConstBasisBlade == 0) continue;

                    string className = FT.GetMangledName(S, smv.Name);
                    string funcName = className + "::set";
                    bool mustCast = false;

                    string returnVarName = null;
                    string dstName = G25.CG.Shared.SmvUtil.THIS;
                    bool dstPtr = true;

                    G25.CG.Shared.Functions.WriteAssignmentFunction(S, cgd,
                            S.m_inlineSet, "void", returnVarName, funcName, null, null, FT, mustCast, smv, dstName, dstPtr, new RefGA.Multivector(0.0));
                }
            }
        } // end of WriteSetZero()

        /// <summary>
        /// Writes a function to set an SMV struct to a scalar coordinate, for all floating point types which have a non-constant scalar coordinate.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSetScalar(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            declSB.AppendLine("");
            defSB.AppendLine("");

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SMV smv in S.m_SMV)
                {
                    if (smv.GetElementIdx(RefGA.BasisBlade.ONE) < 0) continue; // if no scalar coordinate, continue

                    string className = FT.GetMangledName(S, smv.Name);
                    string funcName = className + "::set";
                    bool mustCast = false;

                    System.Collections.ArrayList L = new System.Collections.ArrayList();
                    const int NB_COORDS = 1;
                    string[] argTypename = new String[NB_COORDS];
                    string[] argName = new String[NB_COORDS];
                    {
                        RefGA.BasisBlade B = RefGA.BasisBlade.ONE;
                        argTypename[0] = FT.type;
                        argName[0] = "scalarVal";
                        L.Add(new RefGA.BasisBlade(B.bitmap, B.scale, argName[0]));
                    }
                    RefGA.Multivector mvValue = new RefGA.Multivector(L);

                    G25.fgs F = new G25.fgs(funcName, funcName, "", argTypename, argName, new String[] { FT.type }, null, null, null); // null, null = metricName, comment, options
                    F.InitArgumentPtrFromTypeNames(S);
                    bool computeMultivectorValue = false;
                    //G25.CG.Shared.FuncArgInfo returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, FT, smv.Name, computeMultivectorValue);
                    G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(S, F, NB_COORDS, FT, null, computeMultivectorValue);

                    string dstName = G25.CG.Shared.SmvUtil.THIS;
                    bool dstPtr = true;

                    G25.CG.Shared.Functions.WriteAssignmentFunction(S, cgd,
                        S.m_inlineSet, "void", null, funcName, null, FAI, FT, mustCast, smv, dstName, dstPtr, mvValue);
                }
            }
        }

        /// <summary>
        /// Writes a function to set an SMV struct to specified coordinates, for all floating point types.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSet(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            defSB.AppendLine("");

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SMV smv in S.m_SMV)
                {
                    if (smv.NbNonConstBasisBlade == 0) continue;

                    string className = FT.GetMangledName(S, smv.Name);
                    string funcName = className + "::set";
                    bool mustCast = false;


                    System.Collections.ArrayList L = new System.Collections.ArrayList();
                    int NB_ARGS = 1 + smv.NbNonConstBasisBlade;
                    string[] argTypename = new String[NB_ARGS];
                    string[] argName = new String[NB_ARGS];
                    argTypename[0] = G25.CG.Shared.SmvUtil.COORDINATE_ORDER_ENUM;
                    argName[0] = "co";
                    for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                    {
                        RefGA.BasisBlade B = smv.NonConstBasisBlade(i);
                        argTypename[i + 1] = FT.type;
                        string coordStr = "_" + smv.GetCoordLangID(i, S, COORD_STORAGE.VARIABLES);
                        argName[i + 1] = coordStr;
                        L.Add(new RefGA.BasisBlade(B.bitmap, B.scale, coordStr));
                    }
                    RefGA.Multivector mvValue = new RefGA.Multivector(L);


                    G25.fgs F = new G25.fgs(funcName, funcName, "", argTypename, argName, new String[] { FT.type }, null, null, null); // null, null, null = metricName, comment, options
                    F.InitArgumentPtrFromTypeNames(S);
                    bool computeMultivectorValue = false;
                    //G25.CG.Shared.FuncArgInfo returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, FT, smv.Name, computeMultivectorValue);
                    G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(S, F, NB_ARGS, FT, null, computeMultivectorValue);

                    string dstName = G25.CG.Shared.SmvUtil.THIS;
                    bool dstPtr = true;

                    G25.CG.Shared.Functions.WriteAssignmentFunction(S, cgd,
                        S.m_inlineSet, "void", null, funcName, null, FAI, FT, mustCast, smv, dstName, dstPtr, mvValue);
                }
            }
        } // end of WriteSet()

        /// <summary>
        /// Writes a function to set an SMV struct to an array of specified coordinates, for all floating point types.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSetArray(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            defSB.AppendLine("");

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SMV smv in S.m_SMV)
                {
                    if (smv.NbNonConstBasisBlade == 0) continue;

                    String className = FT.GetMangledName(S, smv.Name);
                    String funcName = className + "::set";
                    bool mustCast = false;

                    String[] argTypename = new String[2] { G25.CG.Shared.SmvUtil.COORDINATE_ORDER_ENUM, FT.type };
                    String[] argName = new String[2]{"co", "A"};

                    System.Collections.ArrayList L = new System.Collections.ArrayList();
                    for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                    {
                        RefGA.BasisBlade B = smv.NonConstBasisBlade(i);
                        String coordStr = argName[1] + "[" + i + "]";
                        L.Add(new RefGA.BasisBlade(B.bitmap, B.scale, coordStr));
                    }
                    RefGA.Multivector mvValue = new RefGA.Multivector(L);

                    G25.fgs F = new G25.fgs(funcName, funcName, "", argTypename, argName, new String[] { FT.type }, null, null, null); // null, null, null = metricName, comment, options
                    F.InitArgumentPtrFromTypeNames(S);
                    F.SetArgumentPtr(1, true); // second argument is a pointer to array

                    bool computeMultivectorValue = false;
                    int NB_ARGS = 2; // enum + one array of coordinates
                    G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(S, F, NB_ARGS, FT, null, computeMultivectorValue);

                    string dstName = G25.CG.Shared.SmvUtil.THIS;
                    bool dstPtr = true;

                    G25.CG.Shared.Functions.WriteAssignmentFunction(S, cgd,
                        S.m_inlineSet, "void", null, funcName, null, FAI, FT, mustCast, smv, dstName, dstPtr, mvValue);
                }
            }
        } // end of WriteSetArray()

        /// <summary>
        /// Writes a function to copy the value of one SMV struct to another, for all floating point types.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteCopy(Specification S, G25.CG.Shared.CGdata cgd)
        {
            //StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            defSB.AppendLine("");

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SMV smv in S.m_SMV)
                {
//                            if (smv.NbNonConstBasisBlade == 0) continue;

                    string className = FT.GetMangledName(S, smv.Name);
                    string funcName = className + "::set";
                    bool mustCast = false;

                    G25.fgs F = new G25.fgs(funcName, funcName, "", new String[] { smv.Name }, null, new String[] { FT.type }, null, null, null); // null, null, null = metricName, comment, options
                    F.InitArgumentPtrFromTypeNames(S);
                    bool computeMultivectorValue = false;
                    int nbArgs = 1;
                    G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(S, F, nbArgs, FT, null, computeMultivectorValue);


                    RefGA.Multivector value = G25.CG.Shared.Symbolic.SMVtoSymbolicMultivector(S, smv, FAI[0].Name, FAI[0].Pointer);

                    string dstName = G25.CG.Shared.SmvUtil.THIS;
                    bool dstPtr = true;

                    G25.CG.Shared.Functions.WriteAssignmentFunction(S, cgd,
                        S.m_inlineSet, "void", null, funcName, null, FAI, FT, mustCast, smv, dstName, dstPtr, value);
                }
            }
        } // end of WriteCopy()

        /// <summary>
        /// Writes a function to copy the value of one SMV struct to another with a different floating point type, for all combinations of floating point types.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteCopyCrossFloat(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            defSB.AppendLine("");

            foreach (G25.FloatType srcFT in S.m_floatTypes)
            {
                foreach (G25.FloatType dstFT in S.m_floatTypes)
                {
                    if (srcFT.type == dstFT.type) continue;
                    foreach (G25.SMV smv in S.m_SMV)
                    {
                        if (smv.NbNonConstBasisBlade == 0) continue;

                        //string srcClassName = srcFT.GetMangledName(smv.Name);
                        string dstClassName = dstFT.GetMangledName(S, smv.Name);
                        string funcName = dstClassName + "::set";
                        bool mustCast = dstFT.MustCastIfAssigned(S, srcFT);

                        G25.fgs F = new G25.fgs(funcName, funcName, "", new String[] { smv.Name }, null, new String[] { srcFT.type }, null, null, null); // null, null, null = metricName, comment, options
                        F.InitArgumentPtrFromTypeNames(S);
                        bool computeMultivectorValue = false;
                        int nbArgs = 1;
                        G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(S, F, nbArgs, srcFT, null, computeMultivectorValue);

                        RefGA.Multivector value = G25.CG.Shared.Symbolic.SMVtoSymbolicMultivector(S, smv, FAI[0].Name, FAI[0].Pointer);

                        string dstName = G25.CG.Shared.SmvUtil.THIS;
                        bool dstPtr = true;

                        G25.CG.Shared.Functions.WriteAssignmentFunction(S, cgd,
                            S.m_inlineSet, "void", null, funcName, null, FAI, dstFT, mustCast, smv, dstName, dstPtr, value);
                    }
                }
            }
        } // end of WriteCopyCrossFloat()

        /// <summary>
        /// Writes code for abs largest coordinate
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteLargestCoordinateFunctions(Specification S, G25.CG.Shared.CGdata cgd)
        {
            string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");

            StringBuilder defSB = (S.m_inlineFunctions) ? cgd.m_inlineDefSB : cgd.m_defSB;
            defSB.AppendLine("");


            const string smvName = G25.CG.Shared.SmvUtil.THIS;
            const bool ptr = true;

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                String fabsFunc = "fabs";
                if (FT.type == "float") fabsFunc = "fabsf";

                foreach (G25.SMV smv in S.m_SMV)
                {
                    String[] AS = G25.CG.Shared.CodeUtil.GetAccessStr(S, smv, smvName, ptr);

                    RefGA.BasisBlade maxBasisBlade = smv.AbsoluteLargestConstantBasisBlade();

                    String className = FT.GetMangledName(S, smv.Name);

                    for (int _returnBitmap = 0; _returnBitmap <= 1; _returnBitmap++)
                    {
                        bool returnBitmap = (_returnBitmap != 0);

                        String funcName = className + ((returnBitmap) ? "::largestBasisBlade" : "::largestCoordinate");

                        String funcDecl;
                        if (returnBitmap) {
                            funcDecl = FT.type + " " + funcName + "(unsigned int &bm) const";
                        }
                        else {
                            funcDecl = FT.type + " " + funcName + "() const";
                        }

                        defSB.Append(inlineStr + funcDecl);
                        {
                            defSB.AppendLine(" {");
                            int startIdx = 0;
                            if (maxBasisBlade != null)
                            {
                                defSB.AppendLine("\t" + FT.type + " maxValue = " + FT.DoubleToString(S, Math.Abs(maxBasisBlade.scale)) + ";");
                                if (returnBitmap)
                                    defSB.AppendLine("\tbm = " + maxBasisBlade.bitmap + ";");
                            }
                            else
                            {
                                defSB.AppendLine("\t" + FT.type + " maxValue = " + fabsFunc + "(" + AS[0] + ");");
                                if (returnBitmap)
                                    defSB.AppendLine("\tbm = 0;");
                                startIdx = 1;
                            }

                            for (int c = startIdx; c < smv.NbNonConstBasisBlade; c++)
                            {
                                defSB.Append("\tif (" + fabsFunc + "(" + AS[c] + ") > maxValue) { maxValue = " + fabsFunc + "(" + AS[c] + "); ");
                                if (returnBitmap) defSB.Append("bm = " + smv.NonConstBasisBlade(c).bitmap + "; ");
                                defSB.AppendLine("}");
                            }

                            defSB.AppendLine("\treturn maxValue;");
                            defSB.AppendLine("}");
                        }
                    }
                } // end of loop over all smvs
            } // end of loop over all float types
        } // end of WriteLargestCoordinateFunctions()

        /// <summary>
        /// Writes code to extract the scalar part of an SMV via a non-member function.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteExtractScalarPart(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineFunctions) ? cgd.m_inlineDefSB : cgd.m_defSB;
            declSB.AppendLine("");
            defSB.AppendLine("");

            const string smvName = "x";
            const bool ptr = false;
            RefGA.BasisBlade scalarBlade = RefGA.BasisBlade.ONE;
            string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineFunctions, " ");
            

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SMV smv in S.m_SMV)
                {
                    string[] AS = G25.CG.Shared.CodeUtil.GetAccessStr(S, smv, smvName, ptr);

                    string className = FT.GetMangledName(S, smv.Name);
                    string funcName = "_" + FT.type;
                    string altFuncName = "_Float";
                    string comment = "/// Returns scalar part of  " + className;

                    declSB.AppendLine(comment);
                    string funcDecl = FT.type + " " + funcName + "(const " + className + " &" + smvName + ")";
                    declSB.Append(funcDecl);
                    declSB.AppendLine(";");

                    declSB.AppendLine(comment);
                    string altFuncDecl = FT.type + " " + altFuncName + "(const " + className + " &" + smvName + ")";
                    declSB.Append(G25.CG.Shared.Util.GetInlineString(S, true, " ") + altFuncDecl + " {return " + funcName + "(" + smvName + "); }");
                    declSB.AppendLine(";");

                    defSB.Append(inlineStr + funcDecl);
                    {
                        defSB.AppendLine(" {");

                        int elementIdx = smv.GetElementIdx(scalarBlade);
                        double multiplier = (elementIdx >= 0) ? 1.0 / smv.BasisBlade(0, elementIdx).scale : 1.0;

                        if ((elementIdx >= 0) && (!smv.IsCoordinateConstant(elementIdx)))
                        {
                            string multiplerString = (multiplier != 1.0) ? (FT.DoubleToString(S, multiplier) + " * ") : "";
                            defSB.AppendLine("\treturn " + multiplerString + AS[smv.BladeIdxToNonConstBladeIdx(elementIdx)] + ";");
                        }
                        else
                        {
                            double constValue = (elementIdx >= 0) ? smv.ConstBasisBladeValue(smv.BladeIdxToConstBladeIdx(elementIdx)) : 0.0;
                            defSB.AppendLine("\treturn " + FT.DoubleToString(S, multiplier * constValue) + ";");
                        }
                        defSB.AppendLine("}");
                    }
                } // end of loop over all smvs
            } // end of loop over all float types
        } // end of WriteExtractScalarPart()

        /// <summary>
        /// Writes set, setZero, copy and copyCrossFloat functions for all specialized multivector types.
        /// </summary>
        public void WriteSetFunctions()
        {
            WriteSetZero(m_specification, m_cgd);
            WriteSetScalar(m_specification, m_cgd);
            WriteSet(m_specification, m_cgd);
            WriteSetArray(m_specification, m_cgd);
            WriteCopy(m_specification, m_cgd);
            WriteCopyCrossFloat(m_specification, m_cgd);
            WriteLargestCoordinateFunctions(m_specification, m_cgd);

            WriteExtractScalarPart(m_specification, m_cgd);
        }
#endif
    } // end of class SMV 
} // end of namespace G25.CG.Java

