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

namespace G25.CG.CSJ
{
    public class SMV
    {

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
            int nbTabs = 1;
            string accessModifier = Keywords.PackageProtectedAccessModifier(S);

            // individual coordinates or one array?
            if (S.m_coordStorage == COORD_STORAGE.VARIABLES)
            {
                for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
                {
                    string comment = "The " + smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames) + " coordinate.";
                    G25.CG.Shared.Util.WriteFunctionComment(SB, S, nbTabs, comment, null, null);
                    SB.AppendLine("\t" + accessModifier + " " + FT.type + " m_" + smv.GetCoordLangID(i, S) + ";");
                }
            }
            else
            {
                if (smv.NbNonConstBasisBlade > 0)
                {
                    // emit: float c[3]; // e1, e2, e3
                    string comment = " The coordinates (stored in an array).";
                    G25.CG.Shared.Util.WriteFunctionComment(SB, S, nbTabs, comment, null, null);

                    SB.Append("\t" + accessModifier + " " + FT.type + "[] m_c = new " +  FT.type + "[" + smv.NbNonConstBasisBlade + "]; // ");
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

            int nbTabs = 1;
            G25.CG.Shared.Util.WriteFunctionComment(SB, S, nbTabs, "Array indices of " + className + " coordinates.", null, null);


            string AccessModifier = Keywords.ConstAccessModifier(S);

            for (int i = 0; i < smv.NbNonConstBasisBlade; i++)
            {
                SB.AppendLine();
                G25.CG.Shared.Util.WriteFunctionComment(SB, S, nbTabs, "index of coordinate for " + smv.NonConstBasisBlade(i).ToString(S.m_basisVectorNames) + " in " + className, null, null);
                SB.AppendLine("\tpublic " + AccessModifier + " int " + GetCoordIndexDefine(S, FT, smv, i) + " = " + i + ";");
            }


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

            string AccessModifier = Keywords.ConstAccessModifier(S);
            string typeName = G25.CG.Shared.SmvUtil.COORDINATE_ORDER_ENUM;
            string constantName = G25.CG.Shared.SmvUtil.GetCoordinateOrderConstant(S, smv);

            SB.AppendLine();

            int nbTabs = 1;
            G25.CG.Shared.Util.WriteFunctionComment(SB, S, nbTabs, "The order of coordinates (this is the type of the first argument of coordinate-handling functions.", null, null);

            string enumAccessModifier = (S.m_outputLanguage == OUTPUT_LANGUAGE.CSHARP) ? "public" : "private";

            SB.AppendLine("\t" + enumAccessModifier + " enum " + typeName + " {");
            SB.AppendLine("\t\t" + constantName);
            SB.AppendLine("\t};");
            SB.AppendLine("\tpublic " + AccessModifier + " " + typeName + " " + constantName + " = " + typeName + "." + constantName + ";");


        }

        /// <summary>
        /// Writes the function to get the array of coordinates.
        /// Does nothing when coordinate storage is not array based or when the type has no variable coordinates.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names.</param>
        /// <param name="FT"></param>
        /// <param name="smv">The specialized multivector for which the coordinate indices should be written.</param>
        public static void WriteGetCoordinates(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv)
        {
            if (S.m_coordStorage != COORD_STORAGE.ARRAY) return;
            if (smv.NbNonConstBasisBlade == 0) return;

            //string typeName = G25.CG.Shared.SmvUtil.COORDINATE_ORDER_ENUM;
            string constantName = G25.CG.Shared.SmvUtil.GetCoordinateOrderConstant(S, smv);

            cgd.m_cog.EmitTemplate(SB, "SMVgetCoords", "FT=", FT, "COORD_TYPE_STRING=", constantName);
        }

        /// <summary>
        /// Writes the implementation of the multivector interface.
        /// </summary>
        /// <param name="SB"></param>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT"></param>
        public static void WriteMultivectorInterface(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv)
        {
            string gmvName = FT.GetMangledName(S, S.m_GMV.Name);
            cgd.m_cog.EmitTemplate(SB, "SMVmvInterfaceImpl", "gmvName=", gmvName);
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
        /// Writes constructors.
        /// </summary>
        /// <param name="SB">Where the code goes.</param>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Not used yet.</param>
        /// <param name="FT">Float point type of 'SMV'.</param>
        /// <param name="smv">The specialized multivector for which the struct should be written.</param>
        public static void WriteSetFunctions(StringBuilder SB, Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SMV smv)
        {
            WriteSetZero(S, cgd, FT, smv);
        }

        /// <summary>
        /// Writes a function to set an SMV struct to zero, for all floating point types.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        private static void WriteSetZero(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, G25.SMV smv)
        {
            if (smv.NbNonConstBasisBlade == 0) return;

            cgd.m_defSB.AppendLine("");

            string funcName = GMV.GetSetFuncName(S);
            bool mustCast = false;

            string returnVarName = null;
            string dstName = G25.CG.Shared.SmvUtil.THIS;
            bool dstPtr = true;

            G25.CG.Shared.Functions.WriteAssignmentFunction(S, cgd,
                    S.m_inlineSet, "void", returnVarName, funcName, null, null, FT, mustCast, smv, dstName, dstPtr, new RefGA.Multivector(0.0));
        } // end of WriteSetZero()

#if RIEN
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
                    string funcName = GMV.GetSetFuncName(S);
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
                    string funcName = GMV.GetSetFuncName(S);
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

                    string className = FT.GetMangledName(S, smv.Name);
                    string funcName = GMV.GetSetFuncName(S);
                    bool mustCast = false;

                    string[] argTypename = new string[2] { G25.CG.Shared.SmvUtil.COORDINATE_ORDER_ENUM, FT.type };
                    string[] argName = new string[2] { "co", "A" };

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
                    string funcName = GMV.GetSetFuncName(S);
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
                        string funcName = GMV.GetSetFuncName(S);
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

#endif 


#if RIEN
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
#endif


    } // end of class SMV
} // end of namespace G25.CG.CSJ
