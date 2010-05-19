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

namespace G25.CG.C
{
    /// <summary>
    /// Handles code generation for specialized multivectors (classes, constructors, set functions, etc).
    /// </summary>
    public class SMV
    {
        public SMV(Specification S, CG.Shared.CGdata cgd)
        {
            m_specification = S;
            m_cgd = cgd;
        }

        protected Specification m_specification;
        protected CG.Shared.CGdata m_cgd;

        public static void WriteSMVtypeConstants(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            SB.AppendLine("");

            SB.AppendLine("// These constants define a unique number for each specialized multivector type.");
            SB.AppendLine("// They are used to report usage of non-optimized functions.");

            {
                int idx = 0;
                // for each floating point type
                foreach (G25.FloatType FT in S.m_floatTypes)
                {
                    // for each smv
                    foreach (G25.SMV smv in S.m_SMV)
                    {
                        String name = (S.m_namespace + "_" + FT.GetMangledName(S, smv.Name)).ToUpper();
                        SB.AppendLine("#define " + name + " " + idx);
                        idx++;

                    }
                }
            }

            SB.AppendLine("");
            SB.AppendLine("/// For each specialized multivector type, the mangled typename.");
            SB.AppendLine("/// This is used to report usage of non-optimized functions.");
            SB.AppendLine("extern const char *g_" + S.m_namespace + "Typenames[];");

        }


        public static void WriteSMVtypenames(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            SB.AppendLine("");
            SB.AppendLine("const char *g_" + S.m_namespace + "Typenames[] = ");
            SB.AppendLine("{");
            {
                int idx = 0;
                // for each floating point type
                foreach (G25.FloatType FT in S.m_floatTypes)
                {
                    // for each smv
                    foreach (G25.SMV smv in S.m_SMV)
                    {
                        if (idx > 0) SB.AppendLine(",");
                        SB.Append("\t\"" + FT.GetMangledName(S, smv.Name) + "\"");
                        idx++;
                    }
                }
                if (idx == 0) SB.Append("\t\"There are no specialized types defined\"");
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
        /// <returns>The symbol for the define for coordinate index 'idx' of 'smv'.</returns>
        public static String GetCoordIndexDefine(Specification S, G25.SMV smv, int idx)
        {
            return smv.Name.ToUpper() + "_" + smv.GetCoordLangID(idx, S, COORD_STORAGE.VARIABLES).ToUpper();
        }

        /// <summary>
        /// Writes the <c>defines</c> for indices of the smv struct to 'SB'. For example,  <c>define VECTOR_E1 0</c>.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names.</param>
        /// <param name="smv">The specialized multivector for which the coordinate indices should be written.</param>
        public static void WriteSMVcoordIndices(StringBuilder SB, Specification S, G25.SMV smv) {
            SB.AppendLine("");
            for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
            {
                // write comment about what the #define is
                SB.AppendLine("/** index of coordinate for " + smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames) + " in " + smv.Name + ".c */");
                SB.AppendLine("#define " + GetCoordIndexDefine(S, smv, i) + " " + i);
            }
        }

        /// <summary>
        /// Writes the definition of an SMV struct to 'SB' (including comments).
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Not used yet.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="smv">The specialized multivector for which the struct should be written.</param>
        public static void WriteSMVstruct(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv)
        {
            SB.AppendLine("");

            { // comments for type: 
                SB.AppendLine("/**");
                SB.AppendLine(" * This struct can hold a specialized multivector of type " + FT.GetMangledName(S, smv.Name) + ".");
                SB.AppendLine(" * ");

                SB.AppendLine(" * The coordinates are stored in type  " + FT.type + ".");
                SB.AppendLine(" * ");

                if (smv.NbNonConstBasisBlade > 0)
                {
                    SB.AppendLine(" * The variable non-zero coordinates are:");
                    for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                    {
                        SB.AppendLine(" *   - coordinate " + smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames) + "  (array index: " + GetCoordIndexDefine(S, smv, i) + " = " + i + ")");
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

            // typedef
            SB.AppendLine("typedef struct ");
            SB.AppendLine("{");
            // individual coordinates or one array?
            if (smv.NbNonConstBasisBlade > 0)
            {
                if (S.m_coordStorage == COORD_STORAGE.VARIABLES)
                {
                    // emit float e1; float e2; float e3; 
                    for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                    {
                        SB.AppendLine("\t/** The " + smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames) + " coordinate. */");
                        SB.AppendLine("\t" + FT.type + " " + smv.GetCoordLangID(i, S) + ";");
                        //G25.CG.Shared.Util.BasisBladeToLangString(smv.NonConstBasisBlade(i), S.m_basisVectorNames) + ";");
                    }
                }
                else
                {
                    // emit: float c[3]; // e1, e2, e3
                    SB.AppendLine("\t/** The coordinates (stored in an array). */");
                    SB.Append("\t" + FT.type + " c[" + smv.NbNonConstBasisBlade + "]; // ");
                    for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                    {
                        if (i > 0) SB.Append(", ");
                        SB.Append(smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames));
                    }
                    SB.AppendLine("");
                }
            }
            else
            {
                SB.AppendLine("\tint filler; ///< Filler, because C does not allow empty structs.");
            }

            // emit: // no=1
            for (int i = 0; i < smv.NbConstBasisBlade; i++)
            {

                SB.AppendLine("\t// " + smv.ConstBasisBlade(i).ToString(S.m_basisVectorNames) + " = " + smv.ConstBasisBladeValue(i).ToString());

            }

            SB.AppendLine("} " + FT.GetMangledName(S, smv.Name) + ";");
        }


        /// <summary>
        /// Writes structs for all SMVs to 'SB'.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSMVstructs(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd)
        {
            // for each float type
            bool emitCoordIndices = true; // ( S.m_coordStorage == COORD_STORAGE.ARRAY); // whether to emit defines such as #define VECTOR_E1 0
            foreach (G25.FloatType FT in S.m_floatTypes) 
            {
                // for each smv
                foreach (G25.SMV smv in S.m_SMV) 
                {
                    if (emitCoordIndices)
                        WriteSMVcoordIndices(SB, S, smv);

                    WriteSMVstruct(SB, S, cgd, FT, smv);
                }

                emitCoordIndices = false; // only emit those on the first loop
            }
        }

        /// <summary>
        /// Writes a function to set an SMV struct to zero, for all floating point types.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSetZero(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            declSB.AppendLine("");
            defSB.AppendLine("");

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SMV smv in S.m_SMV)
                {
                    if (smv.NbNonConstBasisBlade == 0) continue;

                    string typeName = FT.GetMangledName(S, smv.Name);
                    string funcName = typeName + "_setZero";
                    bool mustCast = false;

                    G25.fgs F = new G25.fgs(funcName, funcName, "", null, null, new string[] { FT.type }, null, null, null); // null, null = metricName, comment, options
                    F.InitArgumentPtrFromTypeNames(S);
                    bool computeMultivectorValue = false;
                    G25.CG.Shared.FuncArgInfo returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, FT, smv.Name, computeMultivectorValue);

                    declSB.AppendLine("/** Sets " + typeName + " to zero */");

                    string returnVarName = null;
                    bool staticFunc = false;
                    G25.CG.Shared.Functions.WriteAssignmentFunction(S, cgd,
                            S.m_inlineSet, staticFunc, "void", returnVarName, funcName, returnArgument, null, FT, mustCast, smv, returnArgument.Name, returnArgument.Pointer, new RefGA.Multivector(0.0));
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
                    // if (smv.NbNonConstBasisBlade == 0) continue;
                    if (smv.GetElementIdx(RefGA.BasisBlade.ONE) < 0) continue; // if no scalar coordinate, continue

                    string typeName = FT.GetMangledName(S, smv.Name);
                    string funcName = typeName + "_setScalar";
                    bool mustCast = false;


                    System.Collections.ArrayList L = new System.Collections.ArrayList();
                    const int NB_COORDS = 1;
                    string[] argTypename = new string[NB_COORDS];
                    string[] argName = new string[NB_COORDS];
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
                    G25.CG.Shared.FuncArgInfo returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, FT, smv.Name, computeMultivectorValue);
                    G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(S, F, NB_COORDS, FT, null, computeMultivectorValue);

                    declSB.AppendLine("/** Sets " + typeName + " to a scalar value */");

                    bool staticFunc = false;
                    G25.CG.Shared.Functions.WriteAssignmentFunction(S, cgd,
                        S.m_inlineSet, staticFunc, "void", null, funcName, returnArgument, FAI, FT, mustCast, smv, returnArgument.Name, returnArgument.Pointer, mvValue);
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
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            declSB.AppendLine("");
            defSB.AppendLine("");

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SMV smv in S.m_SMV)
                {
                    if (smv.NbNonConstBasisBlade == 0) continue;

                    string typeName = FT.GetMangledName(S, smv.Name);
                    string funcName = typeName + "_set";
                    bool mustCast = false;


                    System.Collections.ArrayList L = new System.Collections.ArrayList();
                    string[] argTypename = new String[smv.NbNonConstBasisBlade];
                    string[] argName = new String[smv.NbNonConstBasisBlade];
                    for (int i = 0; i < smv.NbNonConstBasisBlade; i++) {
                        RefGA.BasisBlade B = smv.NonConstBasisBlade(i);
                        argTypename[i] = FT.type;
                        String coordStr = "_" + smv.GetCoordLangID(i, S, COORD_STORAGE.VARIABLES);
                        argName[i] = coordStr;
                        L.Add(new RefGA.BasisBlade(B.bitmap, B.scale, coordStr));
                    }
                    RefGA.Multivector mvValue = new RefGA.Multivector(L);


                    G25.fgs F = new G25.fgs(funcName, funcName, "", argTypename, argName, new String[] { FT.type }, null, null, null); // null, null, null = metricName, comment, options
                    F.InitArgumentPtrFromTypeNames(S);
                    bool computeMultivectorValue = false;
                    G25.CG.Shared.FuncArgInfo returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, FT, smv.Name, computeMultivectorValue);
                    G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(S, F, smv.NbNonConstBasisBlade, FT, null, computeMultivectorValue);


                    declSB.AppendLine("/** Sets " + typeName + " to specified coordinates */");

                    bool staticFunc = false;
                    G25.CG.Shared.Functions.WriteAssignmentFunction(S, cgd,
                        S.m_inlineSet, staticFunc, "void", null, funcName, returnArgument, FAI, FT, mustCast, smv, returnArgument.Name, returnArgument.Pointer, mvValue);
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
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            declSB.AppendLine("");
            defSB.AppendLine("");

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SMV smv in S.m_SMV)
                {
                    if (smv.NbNonConstBasisBlade == 0) continue;

                    string typeName = FT.GetMangledName(S, smv.Name);
                    string funcName = typeName + "_setArray";
                    bool mustCast = false;

                    string[] argTypename = new string[1] { FT.type };
                    string[] argName = new string[1] { "A" };

                    System.Collections.ArrayList L = new System.Collections.ArrayList();
                    for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                    {
                        RefGA.BasisBlade B = smv.NonConstBasisBlade(i);
                        //argTypename[i] = FT.type;
                        String coordStr = argName[0] + "[" + i + "]";
                        //argName[i] = coordStr;
                        L.Add(new RefGA.BasisBlade(B.bitmap, B.scale, coordStr));
                    }
                    RefGA.Multivector mvValue = new RefGA.Multivector(L);


                    G25.fgs F = new G25.fgs(funcName, funcName, "", argTypename, argName, new String[] { FT.type }, null, null, null); // null, null, null = metricName, comment, options
                    F.InitArgumentPtrFromTypeNames(S);
                    F.SetArgumentPtr(0, true); // first argument is a pointer to array

                    bool computeMultivectorValue = false;
                    G25.CG.Shared.FuncArgInfo returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, FT, smv.Name, computeMultivectorValue);
                    int nbArgs = 1; // one array of coordinates
                    G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(S, F, nbArgs, FT, null, computeMultivectorValue);


                    declSB.AppendLine("/** Sets " + typeName + " to specified coordinates */");

                    bool staticFunc = false;
                    G25.CG.Shared.Functions.WriteAssignmentFunction(S, cgd,
                        S.m_inlineSet, staticFunc, "void", null, funcName, returnArgument, FAI, FT, mustCast, smv, returnArgument.Name, returnArgument.Pointer, mvValue);
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
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            declSB.AppendLine("");
            defSB.AppendLine("");

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SMV smv in S.m_SMV)
                {
                    if (smv.NbNonConstBasisBlade == 0) continue;

                    String typeName = FT.GetMangledName(S, smv.Name);
                    String funcName = typeName + "_copy";
                    bool mustCast = false;

                    G25.fgs F = new G25.fgs(funcName, funcName, "", new String[] { smv.Name }, null, new String[] { FT.type }, null, null, null); // null, null, null = metricName, comment, options
                    F.InitArgumentPtrFromTypeNames(S);
                    bool computeMultivectorValue = false;
                    G25.CG.Shared.FuncArgInfo returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, FT, smv.Name, computeMultivectorValue);
                    int nbArgs = 1;
                    G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(S, F, nbArgs, FT, null, computeMultivectorValue);


                    declSB.AppendLine("/** Copies " + typeName + ": " + FAI[0].Name + " = " + returnArgument.Name + " */");

                    RefGA.Multivector value = G25.CG.Shared.Symbolic.SMVtoSymbolicMultivector(S, smv, FAI[0].Name, FAI[0].Pointer);

                    bool staticFunc = false;
                    G25.CG.Shared.Functions.WriteAssignmentFunction(S, cgd,
                        S.m_inlineSet, staticFunc, "void", null, funcName, returnArgument, FAI, FT, mustCast, smv, returnArgument.Name, returnArgument.Pointer, value);
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
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            declSB.AppendLine("");
            defSB.AppendLine("");

            foreach (G25.FloatType srcFT in S.m_floatTypes)
            {
                foreach (G25.FloatType dstFT in S.m_floatTypes)
                {
                    if (srcFT.type == dstFT.type) continue;
                    foreach (G25.SMV smv in S.m_SMV)
                    {
                        if (smv.NbNonConstBasisBlade == 0) continue;

                        String srcTypeName = srcFT.GetMangledName(S, smv.Name);
                        String dstTypeName = dstFT.GetMangledName(S, smv.Name);
                        G25.fgs tmpFgs = null;
                        String funcName = G25.CG.Shared.Converter.GetConverterName(S, tmpFgs, srcTypeName, dstTypeName);  
                        bool mustCast = dstFT.MustCastIfAssigned(S, srcFT);

                        G25.fgs F = new G25.fgs(funcName, funcName, "", new String[] { smv.Name }, null, new String[] { srcFT.type }, null, null, null); // null, null, null = metricName, comment, options
                        F.InitArgumentPtrFromTypeNames(S);
                        bool computeMultivectorValue = false;
                        G25.CG.Shared.FuncArgInfo returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, dstFT, smv.Name, computeMultivectorValue);
                        int nbArgs = 1;
                        G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(S, F, nbArgs, srcFT, null, computeMultivectorValue);

                        declSB.AppendLine("/** Copies " + srcTypeName + " to " + dstTypeName + ": " + returnArgument.Name + " = " + FAI[0].Name + " */");

                        RefGA.Multivector value = G25.CG.Shared.Symbolic.SMVtoSymbolicMultivector(S, smv, FAI[0].Name, FAI[0].Pointer);

                        bool staticFunc = false;
                        G25.CG.Shared.Functions.WriteAssignmentFunction(S, cgd, 
                            S.m_inlineSet, staticFunc, "void", null, funcName, returnArgument, FAI, dstFT, mustCast, smv, returnArgument.Name, returnArgument.Pointer, value);
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
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineFunctions) ? cgd.m_inlineDefSB : cgd.m_defSB;
            declSB.AppendLine("");
            defSB.AppendLine("");

            const String smvName = "x";
            const bool ptr = true;

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                String fabsFunc = "fabs";
                if (FT.type == "float") fabsFunc = "fabsf";
    

                foreach (G25.SMV smv in S.m_SMV)
                {
                    String[] AS = G25.CG.Shared.CodeUtil.GetAccessStr(S, smv, smvName, ptr);

                    double maxValue = smv.AbsoluteLargestConstantCoordinate();

                    String typeName = FT.GetMangledName(S, smv.Name);
                    String funcName = typeName + "_largestCoordinate";

                    declSB.AppendLine("/** Returns abs largest coordinate of " + typeName + " */");
                    String funcDecl = FT.type + " " + funcName + "(const " + typeName + " *" + smvName + ")";

                    declSB.Append(funcDecl);
                    declSB.AppendLine(";");

                    defSB.Append(funcDecl);
                    {
                        defSB.AppendLine(" {");
                        int startIdx = 0;
                        if (maxValue != 0.0)
                            defSB.AppendLine("\t" + FT.type + " maxValue = " + FT.DoubleToString(S, maxValue) + ";");
                        else
                        {
                            defSB.AppendLine("\t" + FT.type + " maxValue = " + fabsFunc + "(" + AS[0] + ");");
                            startIdx = 1;
                        }

                        for (int c = startIdx; c < smv.NbNonConstBasisBlade; c++)
                        {
                            defSB.AppendLine("\tif (" + fabsFunc + "(" + AS[c] + ") > maxValue) maxValue = " + fabsFunc + "(" + AS[c] + ");"); 
                        }

                        defSB.AppendLine("\treturn maxValue;");
                        defSB.AppendLine("}");
                    }
                } // end of loop over all smvs
            } // end of loop over all float types
        } // end of WriteLargestCoordinateFunctions()

        /// <summary>
        /// Writes code to extract the scalar part of an SMV
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteExtractScalarPart(Specification S, G25.CG.Shared.CGdata cgd)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineFunctions) ? cgd.m_inlineDefSB : cgd.m_defSB;
            declSB.AppendLine("");
            defSB.AppendLine("");

            const String smvName = "x";
            const bool ptr = true;
            RefGA.BasisBlade scalarBlade = new RefGA.BasisBlade(1.0);
            

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SMV smv in S.m_SMV)
                {
                    String[] AS = G25.CG.Shared.CodeUtil.GetAccessStr(S, smv, smvName, ptr);

                    String typeName = FT.GetMangledName(S, smv.Name);
                    String funcName = typeName + "_" + FT.type;

                    declSB.AppendLine("/** Returns scalar part of  " + typeName + " */");
                    String funcDecl = FT.type + " " + funcName + "(const " + typeName + " *" + smvName + ")";

                    declSB.Append(funcDecl);
                    declSB.AppendLine(";");

                    defSB.Append(funcDecl);
                    {
                        defSB.AppendLine(" {");
                        int scalarCoordIdx = smv.GetElementIdx(scalarBlade);
                        if (scalarCoordIdx >= 0)
                        {
                            double multiplier = 1.0 / smv.BasisBlade(0, scalarCoordIdx).scale;
                            string multiplerString = (multiplier != 1.0) ? (FT.DoubleToString(S, multiplier) + " * "): "";
                            defSB.AppendLine("\treturn " + multiplerString + AS[scalarCoordIdx] + ";");
                        }
                        else defSB.AppendLine("\treturn " + FT.DoubleToString(S, 0.0) + ";");
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

    } // end of class SMV
} // end of namespace G25.CG.C

