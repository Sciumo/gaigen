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
    /// Contains a function that 'plans' how to initialize an outermorphism from vector images.
    /// 
    /// </summary>
    public class OMinit
    {

        private static string GetFunctionName(Specification S, string typeName, string funcName, string funcNameC)
        {
            switch (S.m_outputLanguage)
            {
                case OUTPUT_LANGUAGE.C:
                    return typeName + funcNameC;
                case OUTPUT_LANGUAGE.CPP:
                    return typeName + "::" + funcName;
                case OUTPUT_LANGUAGE.CSHARP:
                    return funcName.Substring(0, 1).ToUpper() + funcName.Substring(1);
                case OUTPUT_LANGUAGE.JAVA:
                    return funcName.Substring(0, 1).ToLower() + funcName.Substring(1);
                default:
                    return "GetFunctionName(): not implemented yet";
            }

        }

        /// <summary>
        /// 'Plans' how to initialize an outermorphism from vector images.
        /// 
        /// The plan is returned in the form of nested array <c>uint[grade][domain][plan steps]</c>.
        /// The <c>plan steps</c> are the bitmaps of the basis blades that should be wedged together
        /// to form the basis blade for <c>grade,domain</c>.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="om"></param>
        /// <returns>plan for initialization of <c>om</c> from vector images.</returns>
        public static uint[][][] ComputeOmInitFromVectorsPlan(Specification S, OM om)
        {
            // keep track of which basis blades are available (domain vectors are the input, so they are always available)
            bool[] availableBasisBlades = new bool[1 << S.m_dimension];
            for (int i = 0; i < om.DomainVectors.Length; i++)
                availableBasisBlades[om.DomainVectors[i].bitmap] = true;

            // for each grade, figure out how to construct the basis blades of that grade from basis blades which are already available
            uint[][][] result = new uint[S.m_dimension + 1][][];
            for (int g = 1; g <= S.m_dimension; g++)
            {
                result[g] = new uint[om.DomainForGrade(g).Length][];
                for (int d = 0; d < om.DomainForGrade(g).Length; d++)
                {
                    
                    RefGA.BasisBlade D = om.DomainForGrade(g)[d];
                    // find the largest part of D which is already known
                    List<uint> plan = new List<uint>();
                    uint bitmap = D.bitmap;
                    while (bitmap != 0)
                    {
                        // find first bit, plan it, see if the rest is available
                        uint lowestBitIdx = (uint)RefGA.Bits.LowestOneBit(bitmap);
                        uint lowestBitmap = (uint)(1 << (int)lowestBitIdx);
                        plan.Add(lowestBitmap);
                        bitmap ^= lowestBitmap;

                        if (availableBasisBlades[bitmap]) // if remaining bitmap has already been computed, use that
                        {
                            plan.Add(bitmap);
                            bitmap ^= bitmap;
                        }
                    }

                    // plan for this basis blade is done:
                    result[g][d] = plan.ToArray();
                    availableBasisBlades[D.bitmap] = true;
                }
            }

            return result;
        } // end of ComputeOmInitFromVectorsPlan()

        /// <summary>
        /// Computes the signs for the plan as computed by GetOmInitFromVectorsPlan().
        /// 
        /// Because the basis blades in the domain of 'om' do not have to be in canonical order,
        /// there may be some signs required to correctly executed the plan
        /// </summary>
        /// <param name="S"></param>
        /// <param name="om"></param>
        /// <param name="plan"></param>
        /// <returns>signs for the plan</returns>
        public static double[][] ComputeOmInitFromVectorsSigns(Specification S, OM om, uint[][][] plan)
        {
            double[][] result = new double[S.m_dimension + 1][];

            for (int g = 1; g <= S.m_dimension; g++)
            {
                result[g] = new double[om.DomainForGrade(g).Length];
                for (int d = 0; d < om.DomainForGrade(g).Length; d++)
                {
                    // get domain blade we have to construct
                    RefGA.BasisBlade D = om.DomainForGrade(g)[d];

                    // follow the plan to construct the blade
                    RefGA.BasisBlade P = RefGA.BasisBlade.ONE;
                    for (int p = 0; p < plan[g][d].Length; p++)
                    {
                        G25.Tuple<RefGA.BasisBlade, int, int> info = GetDomainBladeInfo(S, om, plan[g][d][p]);
                        P = RefGA.BasisBlade.op(P, info.Value1);
                    }

                    // compute sign difference & store
                    double s = D.scale / P.scale;
                    result[g][d] = s;
                }
            }

            return result;
        } // end of ComputeOmInitFromVectorsSigns()

        /// <summary>
        /// Returns info about <c>bitmap</c>. 
        /// 
        /// If bitmap is not in the domain of <c>om</c>, the basis blade in the returned value will be null,
        /// and the domainIdx will be -1.
        /// 
        /// If the grade of the bitmap is 1, the domainIdx will be -1 because it is
        /// assumed to be an input vector.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="om"></param>
        /// <param name="bitmap"></param>
        /// <returns><c>BasisBlade, grade, domainIdx</c> for <c>bitmap</c></returns>
        public static G25.Tuple<RefGA.BasisBlade, int, int> GetDomainBladeInfo(Specification S, OM om, uint bitmap)
        {
            int grade = (int)RefGA.Bits.BitCount(bitmap);

            if (grade == 1)  // basis vector
                return new G25.Tuple<RefGA.BasisBlade, int, int>(new RefGA.BasisBlade(bitmap), grade, -1);

            // find the bitmap in the domain
            RefGA.BasisBlade B = null;
            int domainIdx = -1;
            for (int i = 0; i < om.DomainForGrade(grade).Length; i++)
            {
                if (bitmap == om.DomainForGrade(grade)[i].bitmap)
                {
                    domainIdx = i;
                    B = om.DomainForGrade(grade)[i];
                    break;
                }
            }

            return new G25.Tuple<RefGA.BasisBlade, int, int>(B, grade, domainIdx);
        } // end of GetDomainBladeInfo()

        /// <summary>
        /// Returns the SMV type that can hold an image of a basis vector.
        /// </summary>
        public static G25.SMV getRangeVectorType(Specification S, G25.FloatType FT, CGdata cgd, G25.GOM gom)
        {
            RefGA.Multivector rangeVectorValue = new RefGA.Multivector(gom.RangeVectors);
            G25.SMV rangeVectorType = (G25.SMV)G25.CG.Shared.SpecializedReturnType.FindTightestMatch(S, rangeVectorValue, FT);
            if (rangeVectorType == null) // type is missing, add it and tell user to add it to XML
            {
                rangeVectorType = (G25.SMV)G25.CG.Shared.SpecializedReturnType.CreateSyntheticSMVtype(S, cgd, FT, rangeVectorValue);
            }
            return rangeVectorType;
        }

        /// <summary>
        /// Returns the SMV type that can hold an image of a basis vector.
        /// </summary>
        public static G25.SMV getRangeVectorType(Specification S, G25.FloatType FT, CGdata cgd, G25.SOM som)
        {
            RefGA.Multivector rangeVectorValue = new RefGA.Multivector(som.RangeVectors);
            G25.SMV rangeVectorType = (G25.SMV)G25.CG.Shared.SpecializedReturnType.FindTightestMatch(S, rangeVectorValue, FT);
            if (rangeVectorType == null) // type is missing, add it and tell user to add it to XML
            {
                rangeVectorType = (G25.SMV)G25.CG.Shared.SpecializedReturnType.CreateSyntheticSMVtype(S, cgd, FT, rangeVectorValue);
            }
            return rangeVectorType;
        }

        /// <summary>
        /// Returns a dictionary (value -> coordinateAccessString) that can be used to initialize a general OM to identity.
        /// 
        /// Use the dictionary as follows: foreach(string str in dictionary.value) generateCode{str = dictionary.key}
        /// </summary>
        public static Dictionary<double, List<string>> GetGomIdentityInitCode(Specification S, G25.GOM gom, string gomName, string matrixName)
        {
            Dictionary<double, List<string>> nonZero = new Dictionary<double, List<string>>(); // collect a.m[...] = value in here, by value
            string refStr = (S.OutputC()) ? gomName + "->" : "";
            // figure out which elements need to get something assigned
            // for all blades in the domain, find matching blade in range, and add it to nonZero
            for (int g = 1; g < S.m_GOM.Domain.Length; g++)
            {
                for (int d = 0; d < S.m_GOM.Domain[g].Length; d++)
                {
                    RefGA.BasisBlade D = S.m_GOM.Domain[g][d];
                    for (int r = 0; r < S.m_GOM.Range[g].Length; r++)
                    {
                        RefGA.BasisBlade R = S.m_GOM.Domain[g][r];

                        if (D.bitmap == R.bitmap)
                        {
                            double val = D.scale / R.scale;
                            string coordStr = refStr + matrixName + g + "[" + (r * S.m_GOM.Domain[g].Length + d) + "]";
                            if (!nonZero.ContainsKey(val)) nonZero[val] = new List<string>();
                            nonZero[val].Add(coordStr);
                            continue; // no need to search other blades in range
                        }
                    }
                }

            } // end of loop over all grades of the OM
            return nonZero;
        }

        /// <summary>
        /// Writes a function to set a GOM struct according to vector images, for all floating point types.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="matrixMode">When true, generates code for setting from matrix instead of vector images.</param>
        /// <param name="transpose">When this parameter is true and <c>matrixMode</c> is true, generates code for setting from transpose matrix.</param>
        public static void WriteSetVectorImages(Specification S, G25.CG.Shared.CGdata cgd, bool matrixMode, bool transpose)
        {
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                WriteSetVectorImages(S, cgd, FT, matrixMode, transpose);
            }
        }

        /// <summary>
        /// Writes a function to set a GOM struct according to vector images, for all floating point types.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT">Float type.</param>
        /// <param name="matrixMode">When true, generates code for setting from matrix instead of vector images.</param>
        /// <param name="transpose">When this parameter is true and <c>matrixMode</c> is true, generates code for setting from transpose matrix.</param>
        public static void WriteSetVectorImages(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, bool matrixMode, bool transpose)
        {
            G25.GOM gom = S.m_GOM;

            // get the 'plan' on how to initialize all domain basis blades efficiently:
            uint[][][] plan = G25.CG.Shared.OMinit.ComputeOmInitFromVectorsPlan(S, gom);
            double[][] signs = G25.CG.Shared.OMinit.ComputeOmInitFromVectorsSigns(S, gom, plan);

            // get range vector type
            G25.SMV rangeVectorType = G25.CG.Shared.OMinit.getRangeVectorType(S, FT, cgd, gom);

            // setup array of arguments, function specification, etc
            int NB_ARGS = (matrixMode) ? 1 : gom.DomainVectors.Length;
            string[] argTypes = new string[NB_ARGS], argNames = new string[NB_ARGS];
            RefGA.Multivector[] symbolicBBvalues = new RefGA.Multivector[1 << S.m_dimension]; // symbolic basis blade values go here
            if (matrixMode)
            {
                argTypes[0] = FT.type;
                argNames[0] = "M";

                // convert matrix columns to symbolic Multivector values
                for (int d = 0; d < gom.DomainVectors.Length; d++)
                {
                    RefGA.BasisBlade[] IV = new RefGA.BasisBlade[gom.RangeVectors.Length];
                    for (int r = 0; r < gom.RangeVectors.Length; r++)
                    {
                        int matrixIdx = (transpose) ? (d * gom.RangeVectors.Length + r) : (r * gom.DomainVectors.Length + d);
                        string entryName = argNames[0] + "[" + matrixIdx + "]";
                        IV[r] = new RefGA.BasisBlade(gom.RangeVectors[r].bitmap, 1.0, entryName);
                    }
                    symbolicBBvalues[gom.DomainVectors[d].bitmap] = new RefGA.Multivector(IV);
                }
            }
            else
            {
                for (int d = 0; d < NB_ARGS; d++)
                {
                    argTypes[d] = rangeVectorType.Name;
                    argNames[d] = "i" + gom.DomainVectors[d].ToLangString(S.m_basisVectorNames);
                    bool ptr = S.OutputC();

                    symbolicBBvalues[gom.DomainVectors[d].bitmap] = G25.CG.Shared.Symbolic.SMVtoSymbolicMultivector(S, rangeVectorType, argNames[d], ptr);
                }
            }

            // generate function names for all grades
            string typeName = FT.GetMangledName(S, gom.Name);
            string[] funcNames = new string[gom.Domain.Length];
            for (int g = 1; g < gom.Domain.Length; g++)
            {
                string nameC = (g > 1) ? "_set" : ((matrixMode) ? "_setMatrix" : "_setVectorImages");
                string suffix = (g > 1) ? ("_grade_" + g) : "";
                funcNames[g] = GetFunctionName(S, typeName, "set" + suffix, nameC + suffix);
            }


            // setup instructions (for main function, and subfunctions for grades)
            List<G25.CG.Shared.Instruction> mainI = new List<G25.CG.Shared.Instruction>();
            List<G25.CG.Shared.Instruction>[] bladeI = new List<G25.CG.Shared.Instruction>[1 << S.m_dimension];
            {
                bool mustCast = false;
                int nbTabs = 1;
                string dstName = (S.OutputC()) ? G25.fgs.RETURN_ARG_NAME : SmvUtil.THIS;
                bool dstPtr = S.OutputCppOrC();
                bool declareDst = false;
                for (int g = 1; g < gom.Domain.Length; g++)
                {

                    for (int d = 0; d < gom.DomainForGrade(g).Length; d++)
                    {
                        G25.SMVOM smvOM = gom.DomainSmvForGrade(g)[d];
                        RefGA.BasisBlade domainBlade = gom.DomainForGrade(g)[d];


                        if (g > 1)
                        {
                            bladeI[domainBlade.bitmap] = new List<G25.CG.Shared.Instruction>();

                            string funcCallCode = funcNames[g] + "_" + d + "(";
                            if (S.OutputC()) funcCallCode += G25.fgs.RETURN_ARG_NAME;
                            funcCallCode += ");";
                            mainI.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, funcCallCode));
                        }

                        // follow the plan
                        RefGA.Multivector value = new RefGA.Multivector(signs[g][d]);
                        uint[] P = plan[g][d];
                        for (int p = 0; p < P.Length; p++)
                            value = RefGA.Multivector.op(value, symbolicBBvalues[P[p]]);

                        // add instructions
                        List<G25.CG.Shared.Instruction> I = (g == 1) ? mainI : bladeI[domainBlade.bitmap];
                        I.Add(new G25.CG.Shared.CommentInstruction(nbTabs, "Set image of " + domainBlade.ToString(S.m_basisVectorNames)));
                        I.Add(new G25.CG.Shared.AssignInstruction(nbTabs, smvOM, FT, mustCast, value, dstName, dstPtr, declareDst));

                        // store symbolic value
                        symbolicBBvalues[domainBlade.bitmap] = G25.CG.Shared.Symbolic.SMVtoSymbolicMultivector(S, smvOM, dstName, dstPtr);
                    }
                }
            }

            // output grade > 1 functions
            if (cgd.generateOmInitCode(FT.type))
            {
                for (int g = 2; g < gom.Domain.Length; g++)
                {
                    for (int d = 0; d < gom.DomainForGrade(g).Length; d++)
                    {
                        RefGA.BasisBlade domainBlade = gom.DomainForGrade(g)[d];

                        string funcName = funcNames[g] + "_" + d;
                        G25.fgs F = new G25.fgs(funcName, funcName, "", new string[0], new string[0], new string[] { FT.type }, null, null, null); // null, null = metricName, comment, options
                        //F.InitArgumentPtrFromTypeNames(S);

                        bool computeMultivectorValue = false;

                        G25.CG.Shared.FuncArgInfo returnArgument = null;
                        if (S.OutputC())
                            returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, FT, gom.Name, computeMultivectorValue);

                        int nbArgs = 0;
                        G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(S, F, nbArgs, FT, S.m_GMV.Name, computeMultivectorValue);

                        Comment comment;
                        comment = new Comment("Sets grade " + g + " part of outermorphism matrix based on lower grade parts.");
                        bool inline = false; // do not inline this potentially huge function
                        bool staticFunc = false;
                        bool writeDecl = S.OutputC();
                        G25.CG.Shared.Functions.WriteFunction(S, cgd, F, inline, staticFunc, "void", funcName, returnArgument, FAI, bladeI[domainBlade.bitmap], comment, writeDecl);
                    }
                }
            }


            { // output grade 1 function
                G25.fgs F = new G25.fgs(funcNames[1], funcNames[1], "", argTypes, argNames, new string[] { FT.type }, null, null, null); // null, null = metricName, comment, options
                F.InitArgumentPtrFromTypeNames(S);
                if (matrixMode)
                {
                    F.m_argumentPtr[0] = S.OutputCppOrC();
                    F.m_argumentArr[0] = S.OutputCSharpOrJava();
                }

                bool computeMultivectorValue = false;

                G25.CG.Shared.FuncArgInfo returnArgument = null;
                if (S.OutputC())
                    returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, FT, gom.Name, computeMultivectorValue);

                G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(S, F, NB_ARGS, FT, S.m_GMV.Name, computeMultivectorValue);

                Comment comment;
                if (!matrixMode) comment = new Comment("Sets " + typeName + " from images of the domain vectors.");
                else comment = new Comment("Sets " + typeName + " from a " + (transpose ? "transposed " : "") + "matrix");
                bool inline = false; // do not inline this potentially huge function
                bool staticFunc = false;
                bool writeDecl = S.OutputC();
                G25.CG.Shared.Functions.WriteFunction(S, cgd, F, inline, staticFunc, "void", funcNames[1], returnArgument, FAI, mainI, comment, writeDecl);
            }
        } // end of WriteSetVectorImages()

        public static void WriteOMtoOMcopy(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, OM srcOm, OM dstOm)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;
            string srcTypeName = FT.GetMangledName(S, srcOm.Name);
            string dstTypeName = FT.GetMangledName(S, dstOm.Name);
            bool writeDecl = S.OutputC();

            string funcName = GetFunctionName(S, (S.OutputC() ? "" : dstTypeName), "set", srcTypeName + "_to_" + dstTypeName);

            // do we inline this func?
            string inlineStr = G25.CG.Shared.Util.GetInlineString(S, S.m_inlineSet, " ");
            string dstArgStr = (S.OutputC()) ? (dstTypeName + " *dst, ") : "";
            string refStr = (S.OutputC()) ? "*" : ((S.OutputCpp()) ? "&" : "");
            string CONST = (S.OutputCppOrC()) ? "const " : "";

            string funcDecl = inlineStr + "void " + funcName + "(" + dstArgStr + CONST + srcTypeName + " " + refStr + "src)";

            // write comment, function declaration
            Comment comment = new Comment("Copies a " + srcTypeName + " to a " + dstTypeName + "\n" + 
                "Warning 1: coordinates which cannot be represented are silenty lost.\n" +
                "Warning 2: coordinates which are not present in 'src' are set to zero in 'dst'.\n");
            if (writeDecl)
            {
                comment.Write(declSB, S, 0);
                declSB.Append(funcDecl);
                declSB.AppendLine(";");
            }

            if (S.OutputCSharpOrJava())
                comment.Write(defSB, S, 0);


            defSB.Append(funcDecl);
            {
                defSB.AppendLine(" {");

                Dictionary<Tuple<int, int, int>, Tuple<int, int, double>> D = dstOm.getMapping(srcOm);

                StringBuilder copySB = new StringBuilder();
                List<string> setToZero = new List<string>();

                string matrixStr = (S.OutputC()) ? "m" : "m_m";
                string dstMatrixStr = (S.OutputC()) ? "dst->" + matrixStr : matrixStr;
                string ptrStr = (S.OutputC()) ? "->" : ".";

                // For all grades of som, for all columns, for all rows, check D, get entry, set; otherwise set to null
                // Do not use foreach() on D because we want to fill in coordinates in their proper order.
                for (int gradeIdx = 1; gradeIdx < dstOm.Domain.Length; gradeIdx++)
                {
                    for (int somRangeIdx = 0; somRangeIdx < dstOm.Range[gradeIdx].Length; somRangeIdx++)
                    {
                        for (int somDomainIdx = 0; somDomainIdx < dstOm.Domain[gradeIdx].Length; somDomainIdx++)
                        {
                            Tuple<int, int, int> key = new Tuple<int, int, int>(gradeIdx, somDomainIdx, somRangeIdx);

                            int somMatrixIdx = dstOm.getCoordinateIndex(gradeIdx, somDomainIdx, somRangeIdx);
                            string dstString = dstMatrixStr + gradeIdx + "[" + somMatrixIdx + "] = ";
                            if (D.ContainsKey(key))
                            {
                                Tuple<int, int, double> value = D[key];
                                int gomMatrixIdx = srcOm.getCoordinateIndex(gradeIdx, value.Value1, value.Value2);
                                double multiplier = value.Value3;
                                string multiplierString = (multiplier == 1.0) ? "" : (FT.DoubleToString(S, multiplier) + " * ");

                                copySB.AppendLine("\t" + dstString + multiplierString + " src" + ptrStr + matrixStr + gradeIdx + "[" + gomMatrixIdx + "];");
                            }
                            else
                            {
                                setToZero.Add(dstString);
                            }
                        }
                    }
                }

                // append copy statements
                defSB.Append(copySB);

                // append statements to set coordinates to zero
                if (setToZero.Count > 0)
                {
                    int cnt = 0;
                    defSB.Append("\t");
                    foreach (string str in setToZero)
                    {
                        defSB.Append(str);
                        cnt++;
                        if (cnt > 8)
                        {
                            cnt = 0;
                            defSB.AppendLine("");
                            defSB.Append("\t\t");
                        }
                    }
                    defSB.AppendLine(FT.DoubleToString(S, 0.0) + ";");
                }

                defSB.AppendLine("}");
            }
        }

        /// <summary>
        /// Writes functions to copy GOM to SOMs.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteGOMtoSOMcopy(Specification S, G25.CG.Shared.CGdata cgd)
        {
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                WriteGOMtoSOMcopy(S, cgd, FT);
            } // end of loop over all float types
        } // end of WriteGOMtoSOMcopy()

        /// <summary>
        /// Writes functions to copy SOM to GOMs.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSOMtoGOMcopy(Specification S, G25.CG.Shared.CGdata cgd)
        {
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                WriteSOMtoGOMcopy(S, cgd, FT);
            } // end of loop over all float types
        } // end of WriteGOMtoSOMcopy()

        /// <summary>
        /// Writes functions to copy GOM to SOMs.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT"></param>
        public static void WriteGOMtoSOMcopy(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT)
        {
            G25.GOM gom = S.m_GOM;
            for (int s = 0; s < S.m_SOM.Count; s++)
            {
                G25.SOM som = S.m_SOM[s];

                WriteOMtoOMcopy(S, cgd, FT, gom, som);
            } // end of loop over all SMVs
        }

        /// <summary>
        /// Writes functions to copy SOM to GOMs.
        /// </summary>
        /// <param name="S"></param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT"></param>
        public static void WriteSOMtoGOMcopy(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT)
        {
            G25.GOM gom = S.m_GOM;
            for (int s = 0; s < S.m_SOM.Count; s++)
            {
                G25.SOM som = S.m_SOM[s];

                WriteOMtoOMcopy(S, cgd, FT, som, gom);
            } // end of loop over all SMVs
        }

        /// <summary>
        /// Writes a function to set an SOM struct/class to identity, for all SOMs and floating point types.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSetIdentity(Specification S, G25.CG.Shared.CGdata cgd)
        {
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SOM som in S.m_SOM)
                {
                    WriteSetIdentity(S, cgd, FT, som);
                }
            }
        }

        /// <summary>
        /// Writes a function to set an SOM struct/class to identity
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="FT"></param>
        /// <param name="som"></param>
        public static void WriteSetIdentity(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SOM som)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            if (S.OutputC())
                declSB.AppendLine();
            defSB.AppendLine();

            string typeName = FT.GetMangledName(S, som.Name);
            string funcName = GetFunctionName(S, typeName, "setIdentity", "_setIdentity");

            bool mustCast = false;

            G25.fgs F = new G25.fgs(funcName, funcName, "", null, null, new String[] { FT.type }, null, null, null); // null, null = metricName, comment, options
            F.InitArgumentPtrFromTypeNames(S);
            bool computeMultivectorValue = false;

            G25.CG.Shared.FuncArgInfo returnArgument = null;
            if (S.OutputC())
                returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, FT, som.Name, computeMultivectorValue);

            // setup instructions
            List<G25.CG.Shared.Instruction> I = new List<G25.CG.Shared.Instruction>();
            {
                int nbTabs = 1;
                mustCast = false;
                string valueName = (S.OutputC()) ? G25.fgs.RETURN_ARG_NAME : SmvUtil.THIS;
                bool valuePtr = S.OutputCppOrC();
                bool declareValue = false;
                for (int g = 1; g < som.Domain.Length; g++)
                {
                    for (int c = 0; c < som.DomainForGrade(g).Length; c++)
                    {
                        G25.SMVOM smvOM = som.DomainSmvForGrade(g)[c];
                        I.Add(new G25.CG.Shared.AssignInstruction(nbTabs, smvOM, FT, mustCast, new RefGA.Multivector(som.DomainForGrade(g)[c]), valueName, valuePtr, declareValue));
                    }
                }
            }

            Comment comment = new Comment("Sets " + typeName + " to identity.");
            bool writeDecl = S.OutputC();
            bool staticFunc = false;
            G25.CG.Shared.Functions.WriteFunction(S, cgd, F, S.m_inlineSet, staticFunc, "void", funcName, returnArgument, new G25.CG.Shared.FuncArgInfo[0], I, comment, writeDecl);

        }

        /// <summary>
        /// Writes a function to copy an SOM struct, for all floating point types.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSetCopy(Specification S, G25.CG.Shared.CGdata cgd)
        {

            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SOM som in S.m_SOM)
                {
                    WriteSetCopy(S, cgd, FT, som);
                }
            }
        }

        public static void WriteSetCopy(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SOM som)
        {
            StringBuilder declSB = cgd.m_declSB;
            StringBuilder defSB = (S.m_inlineSet) ? cgd.m_inlineDefSB : cgd.m_defSB;

            if (S.OutputC())
                declSB.AppendLine();
            defSB.AppendLine();

            string typeName = FT.GetMangledName(S, som.Name);
            string funcName = GetFunctionName(S, typeName, "set", "_set");

            bool mustCast = false;

            const int NB_ARGS = 1;
            string srcName = "src";
            bool srcPtr = S.OutputC();
            G25.fgs F = new G25.fgs(funcName, funcName, "", new String[] { som.Name }, new String[] { srcName }, new String[] { FT.type }, null, null, null); // null, null = metricName, comment, options
            F.InitArgumentPtrFromTypeNames(S);
            bool computeMultivectorValue = false;
            G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(S, F, NB_ARGS, FT, S.m_GMV.Name, computeMultivectorValue);

            G25.CG.Shared.FuncArgInfo returnArgument = null;
            if (S.OutputC())
                returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, FT, som.Name, computeMultivectorValue);

            // setup instructions
            List<G25.CG.Shared.Instruction> I = new List<G25.CG.Shared.Instruction>();
            {
                int nbTabs = 1;
                mustCast = false;
                string dstName = (S.OutputC()) ? G25.fgs.RETURN_ARG_NAME : SmvUtil.THIS;
                bool dstPtr = (S.OutputCppOrC());
                bool declareDst = false;
                for (int g = 1; g < som.Domain.Length; g++)
                {
                    for (int c = 0; c < som.DomainForGrade(g).Length; c++)
                    {
                        G25.SMVOM smvOM = som.DomainSmvForGrade(g)[c];
                        RefGA.Multivector srcValue = G25.CG.Shared.Symbolic.SMVtoSymbolicMultivector(S, smvOM, srcName, srcPtr);
                        I.Add(new G25.CG.Shared.AssignInstruction(nbTabs, smvOM, FT, mustCast, srcValue, dstName, dstPtr, declareDst));
                    }
                }
            }

            Comment comment = new Comment("Copies " + typeName + "."); ;
            bool writeDecl = (S.OutputC());
            bool staticFunc = false;
            G25.CG.Shared.Functions.WriteFunction(S, cgd, F, S.m_inlineSet, staticFunc, "void", funcName, returnArgument, FAI, I, comment, writeDecl);
        }

        /// <summary>
        /// Writes a function to set a SOM struct according to vector images, for all floating point types.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        public static void WriteSetVectorImages(Specification S, G25.CG.Shared.CGdata cgd)
        {
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SOM som in S.m_SOM)
                {
                    WriteSetVectorImages(S, cgd, FT, som);
                }
            }
        }

        public static void WriteSetVectorImages(Specification S, G25.CG.Shared.CGdata cgd, G25.FloatType FT, G25.SOM som)
        {
            G25.SMV rangeVectorType = G25.CG.Shared.OMinit.getRangeVectorType(S, FT, cgd, som);

            // loop over som.DomainVectors
            // setup array of arguments, function specification, etc
            int NB_ARGS = som.DomainVectors.Length;
            string[] argTypes = new string[NB_ARGS];
            string[] argNames = new string[NB_ARGS];
            RefGA.Multivector[] argValue = new RefGA.Multivector[NB_ARGS];
            for (int d = 0; d < NB_ARGS; d++)
            {
                argTypes[d] = rangeVectorType.Name;
                argNames[d] = "i" + som.DomainVectors[d].ToLangString(S.m_basisVectorNames);
                bool ptr = (S.OutputC());
                argValue[d] = G25.CG.Shared.Symbolic.SMVtoSymbolicMultivector(S, rangeVectorType, argNames[d], ptr);
            }


            string typeName = FT.GetMangledName(S, som.Name);
            string funcName = GetFunctionName(S, typeName, "set", "_setVectorImages");


            G25.fgs F = new G25.fgs(funcName, funcName, "", argTypes, argNames, new String[] { FT.type }, null, null, null); // null, null = metricName, comment, options
            F.InitArgumentPtrFromTypeNames(S);
            bool computeMultivectorValue = false;
            G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(S, F, NB_ARGS, FT, S.m_GMV.Name, computeMultivectorValue);

            G25.CG.Shared.FuncArgInfo returnArgument = null;
            if (S.OutputC())
                returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, FT, som.Name, computeMultivectorValue);

            // setup instructions
            List<G25.CG.Shared.Instruction> I = new List<G25.CG.Shared.Instruction>();
            {
                bool mustCast = false;
                int nbTabs = 1;
                string dstName = (S.OutputC()) ? G25.fgs.RETURN_ARG_NAME : SmvUtil.THIS;
                bool dstPtr = S.OutputCppOrC();
                bool declareDst = false;
                for (int g = 1; g < som.Domain.Length; g++)
                {
                    for (int c = 0; c < som.DomainForGrade(g).Length; c++)
                    {
                        G25.SMVOM smvOM = som.DomainSmvForGrade(g)[c];
                        RefGA.BasisBlade domainBlade = som.DomainForGrade(g)[c];
                        RefGA.Multivector value = new RefGA.Multivector(new RefGA.BasisBlade(domainBlade, 0)); // copy the scalar part, ditch the basis blade
                        for (uint v = 0; v < som.DomainVectors.Length; v++)
                        {
                            if ((domainBlade.bitmap & som.DomainVectors[v].bitmap) != 0)
                            {
                                value = RefGA.Multivector.op(value, argValue[v]);
                            }
                        }

                        I.Add(new G25.CG.Shared.CommentInstruction(nbTabs, "Set image of " + domainBlade.ToString(S.m_basisVectorNames)));
                        I.Add(new G25.CG.Shared.AssignInstruction(nbTabs, smvOM, FT, mustCast, value, dstName, dstPtr, declareDst));
                    }
                }

            }

            Comment comment = new Comment("Sets " + typeName + " from images of the domain vectors.");
            bool writeDecl = false;
            bool staticFunc = false;
            G25.CG.Shared.Functions.WriteFunction(S, cgd, F, S.m_inlineSet, staticFunc, "void", funcName, returnArgument, FAI, I, comment, writeDecl);
        }
        
        


        /// <summary>
        /// Writes a function to set a SOM struct according to a matrix. The columns (or rows, in transposed mode)
        /// of the matrix are the images of the domain vectors.
        /// </summary>
        /// <param name="S">Used for basis vector names and output language.</param>
        /// <param name="cgd">Results go here. Also intermediate data for code generation. Also contains plugins and cog.</param>
        /// <param name="transpose">Whether generate for a transposed matrix or not.</param>
        public static void WriteSetMatrix(Specification S, G25.CG.Shared.CGdata cgd, bool transpose)
        {
            foreach (G25.FloatType FT in S.m_floatTypes)
            {
                foreach (G25.SOM som in S.m_SOM)
                {
                }
            }
        }

        public static void WriteSetMatrix(Specification S, G25.CG.Shared.CGdata cgd, FloatType FT, G25.SOM som, bool transpose)
        {
            int NB_ARGS = 1;
            string[] argTypes = new string[NB_ARGS];
            string[] argNames = new string[NB_ARGS];
            argTypes[0] = FT.type;
            argNames[0] = "M";

            // construct image values
            RefGA.Multivector[] imageValue = new RefGA.Multivector[som.DomainVectors.Length];
            for (int d = 0; d < som.DomainVectors.Length; d++)
            {
                //imageValue[d] = RefGA.Multivector.ZERO;
                RefGA.BasisBlade[] IV = new RefGA.BasisBlade[som.RangeVectors.Length];
                for (int r = 0; r < som.RangeVectors.Length; r++)
                {
                    int matrixIdx = (transpose) ? (d * som.RangeVectors.Length + r) : (r * som.DomainVectors.Length + d);
                    string entryName = argNames[0] + "[" + matrixIdx + "]";
                    IV[r] = new RefGA.BasisBlade(som.RangeVectors[r].bitmap, 1.0, entryName);
                }
                imageValue[d] = new RefGA.Multivector(IV);
            }


            string typeName = FT.GetMangledName(S, som.Name);
            string funcName = GetFunctionName(S, typeName, "set", "_setMatrix");
            if (transpose) funcName = funcName + "Transpose";

            //argNames[0] = "*" + argNames[0]; // quick hack: add pointer to name instead of type!
            G25.fgs F = new G25.fgs(funcName, funcName, "", argTypes, argNames, new String[] { FT.type }, null, null, null); // null, null = metricName, comment, options
            F.InitArgumentPtrFromTypeNames(S);
            if (S.OutputCppOrC())
                F.m_argumentPtr[0] = true;
            else F.m_argumentArr[0] = true;
            bool computeMultivectorValue = false;
            G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(S, F, NB_ARGS, FT, S.m_GMV.Name, computeMultivectorValue);

            G25.CG.Shared.FuncArgInfo returnArgument = null;
            if (S.OutputC())
                returnArgument = new G25.CG.Shared.FuncArgInfo(S, F, -1, FT, som.Name, computeMultivectorValue);

            // setup instructions
            List<G25.CG.Shared.Instruction> I = new List<G25.CG.Shared.Instruction>();
            {
                bool mustCast = false;
                int nbTabs = 1;
                string dstName = (S.OutputC()) ? G25.fgs.RETURN_ARG_NAME : SmvUtil.THIS;
                bool dstPtr = S.OutputCppOrC();
                bool declareDst = false;
                for (int g = 1; g < som.Domain.Length; g++)
                {
                    for (int c = 0; c < som.DomainForGrade(g).Length; c++)
                    {
                        G25.SMVOM smvOM = som.DomainSmvForGrade(g)[c];
                        RefGA.BasisBlade domainBlade = som.DomainForGrade(g)[c];
                        RefGA.Multivector value = new RefGA.Multivector(new RefGA.BasisBlade(domainBlade, 0)); // copy the scalar part, ditch the basis blade
                        for (uint v = 0; v < som.DomainVectors.Length; v++)
                        {
                            if ((domainBlade.bitmap & som.DomainVectors[v].bitmap) != 0)
                            {
                                value = RefGA.Multivector.op(value, imageValue[v]);
                            }
                        }

                        I.Add(new G25.CG.Shared.CommentInstruction(nbTabs, "Set image of " + domainBlade.ToString(S.m_basisVectorNames)));
                        I.Add(new G25.CG.Shared.AssignInstruction(nbTabs, smvOM, FT, mustCast, value, dstName, dstPtr, declareDst));
                    }
                }

            }

            Comment comment = new Comment("Sets " + typeName + " from a " + (transpose ? "transposed " : "") + "matrix.");
            bool writeDecl = (S.OutputC());
            bool staticFunc = false;
            G25.CG.Shared.Functions.WriteFunction(S, cgd, F, S.m_inlineSet, staticFunc, "void", funcName, returnArgument, FAI, I, comment, writeDecl);
        }

    } // end of class OMinit
} // end of namepace G25.CG.Shared
