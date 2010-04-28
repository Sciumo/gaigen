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

namespace G25.CG.Shared.Func
{
    /// <summary>
    /// Generates code for applying a (unit) versor to another multivector.
    /// 
    /// The function should be called either <c>"applyVersor"</c>, <c>"applyUnitVersor"</c> or <c>"applyVersorWI"</c>.
    /// <c>"applyUnitVersor"</c> assumes the versor is unit (which avoids having to invert it).
    /// The <c>"applyVersorWI"</c> functions takes 3 argument: versor, operand, inverseVersor. The <c>WI</c> part
    /// stands for 'With Inverse', i.e., the inverse is specified by the user..
    /// 
    /// The first argument of the generated function is the versor, the second is the blade or versor
    /// to be transformed. The third argument (<c>WI</c> only) is the inverse of the versor.
    /// 
    /// The metric can be specified using the <c>metric="metricName"</c> attribute.
    /// </summary>
    public class ApplyVersor : G25.CG.Shared.BaseFunctionGenerator
    {

        protected String m_normSquaredName = "_n2_";
        protected bool m_isUnit; ///< set to true when the versor is unit, or when explicit inverse is provided
        protected int NB_ARGS; ///< set to 2 or 3, depending on whether explicit inverse is provided
        protected bool m_gmvFunc; ///< is this a function over GMVs?
        protected RefGA.Multivector m_versorValue; ///< symbolic value of input versor
        protected RefGA.Multivector m_reverseVersorValue; ///< symbolic value of reverse or inverse of versor
        protected RefGA.Multivector m_transformedValue; ///< symbolic value of transformed input multivector
        protected RefGA.Multivector m_n2Value = null; ///< symbolic value of norm squared or versor (can be null)
        protected int m_inputGradeUsage; ///< grade usage bitmap of input (arg #2) multivector
        protected Dictionary<string, string> m_funcName = new Dictionary<string, string>();  ///< generated function name with full mangling, etc

        protected G25.MV m_versorMv; ///< the versor type
        protected G25.MV m_subjectMv; ///< the type of the multivector to which the versor is applied
        protected G25.MV m_inverseVersorMv; ///< the inverse versor type (can be null)
        protected G25.VariableType m_returnType; ///< return type


        /// <returns>true when <c>F.Name == "applyVersor"</c>.</returns>
        public static bool IsApplyVersor(G25.fgs F)
        {
            return F.Name == "applyVersor";
        }

        /// <returns>true when <c>F.Name == "applyUnitVersor"</c>.</returns>
        public static bool IsApplyUnitVersor(G25.fgs F)
        {
            return F.Name == "applyUnitVersor";
        }

        /// <returns>true when <c>F.Name == "applyVersorWI"</c>.</returns>
        public static bool IsApplyVersorWI(G25.fgs F)
        {
            return F.Name == "applyVersorWI";
        }

        /// <returns>3 when IsApplyVersorWI() returns true, 2 otherwise.</returns>
        public static int GetNbArgs(G25.fgs F)
        {
            return (IsApplyVersorWI(F)) ? 3 : 2;
        }

        /// <summary>
        /// Checks if this FunctionGenerator can implement a certain function.
        /// </summary>
        /// <param name="S">The specification of the algebra.</param>
        /// <param name="F">The function to be implemented.</param>
        /// <returns>true if 'F' can be implemented</returns>
        public override bool CanImplement(Specification S, G25.fgs F)
        {
            if (!(IsApplyVersor(F) || IsApplyVersorWI(F) || IsApplyUnitVersor(F))) return false;

            int nbArgs = GetNbArgs(F);

            // don't allow mixing of GMV/SMV or Scalar/SMV
            if (!G25.CG.Shared.Functions.NotMixSmvGmv(S, F, nbArgs, S.m_GMV.Name)) return false;
            if (!G25.CG.Shared.Functions.NotMixScalarGmv(S, F, nbArgs, S.m_GMV.Name)) return false;

            return (((IsApplyVersor(F) || IsApplyUnitVersor(F)) && F.MatchNbArguments(2)) ||
                (IsApplyVersorWI(F) && F.MatchNbArguments(3)));
        }


        /// <summary>
        /// If this FunctionGenerator can implement 'F', then this function should complete the (possible)
        /// blanks in 'F'. This means:
        ///  - Fill in F.m_returnTypeName if it is empty
        ///  - Fill in F.m_argumentTypeNames (and m_argumentVariableNames) if it is empty.
        /// </summary>
        public override void CompleteFGS()
        {
            // get info on # arguments
            m_isUnit = IsApplyUnitVersor(m_fgs) || IsApplyVersorWI(m_fgs); // unit or inverse provided?
            NB_ARGS = GetNbArgs(m_fgs); // number of arguments (2 or 3)

            // fill in ArgumentTypeNames
            if (m_fgs.ArgumentTypeNames.Length == 0)
            {
                m_fgs.m_argumentTypeNames = new String[NB_ARGS];
                for (int i = 0; i < NB_ARGS; i++)
                    m_fgs.m_argumentTypeNames[i] = m_gmv.Name;
            }

            // init argument pointers from the completed typenames (language sensitive);
            m_fgs.InitArgumentPtrFromTypeNames(m_specification);

            // get all function info
            FloatType FT = m_specification.GetFloatType(m_fgs.FloatNames[0]);
            bool computeMultivectorValue = true;
            G25.CG.Shared.FuncArgInfo[] tmpFAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);
            m_gmvFunc = !(tmpFAI[0].IsScalarOrSMV() && tmpFAI[1].IsScalarOrSMV());


            m_versorMv = (G25.MV)tmpFAI[0].Type;
            m_subjectMv = (G25.MV)tmpFAI[1].Type;
            if (tmpFAI.Length > 2)
                m_inverseVersorMv = (G25.MV)tmpFAI[2].Type;


            { // get symbolic result
                // get grade of input blade:
                m_inputGradeUsage = tmpFAI[1].MultivectorValue[0].GradeUsage();

                // get basic transformed value
                m_versorValue = tmpFAI[0].MultivectorValue[0];
                m_reverseVersorValue = (IsApplyVersorWI(m_fgs)) ? tmpFAI[2].MultivectorValue[0] : RefGA.Multivector.Reverse(m_versorValue);
                m_transformedValue =
                    RefGA.Multivector.gp(
                    RefGA.Multivector.gp(m_versorValue, tmpFAI[1].MultivectorValue[0], m_M),
                    m_reverseVersorValue, m_M);


                // apply grade part selection
                m_transformedValue = m_transformedValue.ExtractGrade(RefGA.Multivector.GradeBitmapToArray(m_inputGradeUsage));

                // if rotor is not guaranteed by the user to be unit, compute norm squared
                if (!m_isUnit)
                    m_n2Value = RefGA.Multivector.ip(m_reverseVersorValue, m_versorValue, m_M, RefGA.BasisBlade.InnerProductType.LEFT_CONTRACTION);

                // round value if required by metric
                if (m_G25M.m_round)
                {
                    m_transformedValue = m_transformedValue.Round(1e-14);
                    if (m_n2Value != null)
                        m_n2Value = m_n2Value.Round(1e-14);
                }
            }

            // compute intermediate results, set return type
            if (m_gmvFunc) m_fgs.m_returnTypeName = m_gmv.Name; // gmv * gmv / gmv = gmv
            else
            {
                // get name of return type
                if (m_fgs.m_returnTypeName.Length == 0)
                    m_fgs.m_returnTypeName = G25.CG.Shared.SpecializedReturnType.GetReturnType(m_specification, m_cgd, m_fgs, FT, m_transformedValue).GetName();
            }
            m_returnType = m_specification.GetType(m_fgs.m_returnTypeName);

        }

        /// <summary>
        /// This function should check the dependencies of this function. If dependencies are
        /// missing, the function can complain (throw exception) or fix it (add the required functions).
        /// 
        /// If changes are made to the specification then it must be locked first because
        /// multiple threads run in parallel which may all modify the specification!
        /// </summary>
        public override void CheckDepencies()
        {
            // check dependencies for all float types
            foreach (String floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);
                //bool returnTrueName = false;
                G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gp", new String[] { m_gmv.Name, m_gmv.Name }, FT, m_G25M.m_name);
                G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, G25.CG.Shared.CANSparts.EXTRACT_GRADE, new String[] { m_gmv.Name }, FT, null);
                if (IsApplyVersor(m_fgs)) G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "versorInverse", new String[] { m_gmv.Name }, FT, m_G25M.m_name);
                else if (IsApplyUnitVersor(m_fgs)) G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "reverse", new String[] { m_gmv.Name }, FT, null);
            }
        }


        /// <summary>
        /// Writes the declaration/definitions of 'F' to StringBuffer 'SB', taking into account parameters specified in specification 'S'.
        /// </summary>
        public override void WriteFunction()
        {
            // get metric
            G25.Metric G25M = m_specification.GetMetric(m_fgs.MetricName);
            //RefGA.Metric M = G25M.m_metric;

            bool isUnit = IsApplyUnitVersor(m_fgs) || IsApplyVersorWI(m_fgs);

            foreach (string floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);

                bool computeMultivectorValue = true;
                G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);

                // generate comment
                string warningComment = (isUnit) ? " Only gives the correct result when the versor has a positive squared norm.\n" : "";
                string comment = "/** " +
                    m_fgs.AddUserComment("Returns " + FAI[0].Name + " * " + FAI[1].Name + " * " + ((isUnit) ? "reverse" : "inverse") + "(" + FAI[0].Name + ") using " + m_G25M.m_name + " metric." + warningComment) + " */";

                if (m_gmvFunc)
                { // generate function over GMVs
                    // determine what type of function is requested:
                    G25.CG.Shared.GPparts.ApplyVersorTypes AVtype;
                    if (IsApplyVersor(m_fgs)) AVtype = G25.CG.Shared.GPparts.ApplyVersorTypes.INVERSE;
                    else if (IsApplyUnitVersor(m_fgs)) AVtype = G25.CG.Shared.GPparts.ApplyVersorTypes.REVERSE;
                    else AVtype = G25.CG.Shared.GPparts.ApplyVersorTypes.EXPLICIT_INVERSE;

                    // generate code:
                    m_funcName[FT.type] = G25.CG.Shared.GmvGpParts.WriteGMVapplyVersorFunction(m_specification, m_cgd, FT, G25M, FAI, m_fgs, comment, AVtype);
                }
                else
                { // write  specialized function:
                    // setup instructions
                    System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
                    {
                        int nbTabs = 1;
                        bool mustCast = false;
                        bool n2Ptr = false;
                        bool declareN2 = true;

                        if (!isUnit)
                        {
                            // n2 = reverse(versor) * versor
                            I.Add(new G25.CG.Shared.AssignInstruction(nbTabs, FT, FT, mustCast, m_n2Value, m_normSquaredName, n2Ptr, declareN2));
                        }

                        //bool transformedPtr = !(FAI[1].Type is G25.FloatType);
                        //bool declareTransformed = false;


                        if (isUnit)
                        {
                            // result = versor * X * reverse(versor)
                            I.Add(new G25.CG.Shared.ReturnInstruction(nbTabs, m_returnType, FT, mustCast, m_transformedValue));
                        }
                        else
                        {
                            // result = versor * X * reverse(versor) / (reverse(versor).versor)
                            I.Add(new G25.CG.Shared.ReturnInstruction(nbTabs, m_returnType, FT, mustCast, m_transformedValue, "/", new RefGA.Multivector(m_normSquaredName)));
                        }
                    }

                    // because of lack of overloading, function names include names of argument types
                    G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, m_fgs, FAI);
                    m_funcName[FT.type] = CF.OutputName;

                    G25.CG.Shared.Functions.WriteFunction(m_specification, m_cgd, CF, m_specification.m_inlineFunctions, CF.OutputName, FAI, I, comment);
                }
            }
        } // end of WriteFunction

        // used for testing:
        protected Dictionary<string, string> m_randomScalarFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomBladeFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomVersorFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomSmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_subtractGmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_unitVersorFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_innerProductFuncName = new Dictionary<string, string>();
        //protected Dictionary<string, string> m_inverseGmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_applyVersorGmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_versorInverseFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_norm2VersorFuncName = new Dictionary<string, string>();

        /// <summary>
        /// This function checks the dependencies for the _testing_ code of this function. If dependencies are
        /// missing, the function adds the required functions (this is done simply by asking for them . . .).
        /// </summary>
        public override void CheckTestingDepencies()
        {
            //bool returnTrueName = true;
            foreach (string floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);

                m_randomScalarFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + FT.type, new String[0], FT.type, FT, null);
                m_subtractGmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "subtract", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, null);

                // actual requirements:
                if (m_gmvFunc)
                {
                    m_randomBladeFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_blade", new String[0], m_specification.m_GMV.Name, FT, null);
                    m_randomVersorFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_versor", new String[0], m_specification.m_GMV.Name, FT, m_G25M.m_name);
                    m_unitVersorFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "unit", new String[] { m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, m_G25M.m_name);
                    m_innerProductFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "mhip", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, m_G25M.m_name);
                    m_versorInverseFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "versorInverse", new String[] { m_versorMv.Name }, m_specification.m_GMV.Name, FT, m_G25M.m_name);
                }
                else
                {
                    string defaultReturnTypeName = null;

                    m_applyVersorGmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "applyVersor", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, defaultReturnTypeName, FT, m_G25M.m_name);
                    m_randomVersorFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + m_versorMv.Name, new String[0], defaultReturnTypeName, FT, m_G25M.m_name);
                    m_randomSmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + m_subjectMv.Name, new String[0], defaultReturnTypeName, FT, m_G25M.m_name);
                    m_unitVersorFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "unit", new String[] { m_versorMv.Name }, defaultReturnTypeName, FT, m_G25M.m_name);
                    m_versorInverseFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "versorInverse", new String[] { m_versorMv.Name }, defaultReturnTypeName, FT, m_G25M.m_name);
                    m_norm2VersorFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "norm2", new String[] { m_versorMv.Name }, defaultReturnTypeName, FT, m_G25M.m_name);
                }
            }
        }




        public override List<string> WriteTestFunction()
        {
            StringBuilder defSB = (m_specification.m_inlineFunctions) ? m_cgd.m_inlineDefSB : m_cgd.m_defSB;

            List<string> testFuncNames = new List<string>();

            foreach (string floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);

                string testFuncName = Util.GetTestingFunctionName(m_specification, m_cgd, m_funcName[FT.type]);

                testFuncNames.Add(testFuncName);

                if (m_gmvFunc)
                {
                    System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                    argTable["S"] = m_specification;
                    argTable["FT"] = FT;
                    argTable["gmv"] = m_specification.m_GMV;
                    argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                    argTable["testFuncName"] = testFuncName;
                    argTable["targetFuncName"] = m_funcName[FT.type];

                    argTable["unit"] = m_isUnit;
                    argTable["explicitInverse"] = IsApplyVersorWI(m_fgs);

                    argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                    argTable["subtractGmvFuncName"] = m_subtractGmvFuncName[FT.type];
                    argTable["randomVersorFuncName"] = m_randomVersorFuncName[FT.type];
                    argTable["randomBladeFuncName"] = m_randomBladeFuncName[FT.type];
                    argTable["unitVersorFuncName"] = m_unitVersorFuncName[FT.type];
                    argTable["innerProductFuncName"] = m_innerProductFuncName[FT.type];
                    argTable["subtractGmvFuncName"] = m_subtractGmvFuncName[FT.type];
                    argTable["inverseVersorFuncName"] = m_versorInverseFuncName[FT.type];
                    argTable["basisVectorBitmap"] = m_G25M.GetEuclideanBasisVectorBitmap();

                    m_cgd.m_cog.EmitTemplate(defSB, "testApplyVersor_GMV", argTable);
                }
                else if (m_versorMv.CanConvertToGmv(m_specification) && m_subjectMv.CanConvertToGmv(m_specification))
                {
                    System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                    argTable["S"] = m_specification;
                    argTable["FT"] = FT;
                    argTable["gmv"] = m_specification.m_GMV;
                    argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                    argTable["testFuncName"] = testFuncName;
                    argTable["targetFuncName"] = m_funcName[FT.type];

                    argTable["unit"] = m_isUnit;
                    argTable["explicitInverse"] = IsApplyVersorWI(m_fgs);

                    argTable["versorType"] = m_versorMv;
                    argTable["versorName"] = FT.GetMangledName(m_specification, m_versorMv.Name);
                    argTable["smvName"] = FT.GetMangledName(m_specification, m_subjectMv.Name);
                    argTable["transformedSmvName"] = FT.GetMangledName(m_specification, m_returnType.GetName());
                    argTable["inverseVersorName"] = FT.GetMangledName(m_specification, ((m_inverseVersorMv == null) ? m_versorMv : m_inverseVersorMv).Name);

                    argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                    argTable["subtractGmvFuncName"] = m_subtractGmvFuncName[FT.type];
                    argTable["applyVersorGmvFuncName"] = m_applyVersorGmvFuncName[FT.type];
                    argTable["randomVersorFunc"] = m_randomVersorFuncName[FT.type];
                    argTable["randomSmvFunc"] = m_randomSmvFuncName[FT.type];
                    argTable["unitVersorFuncName"] = m_unitVersorFuncName[FT.type];
                    argTable["norm2VersorFuncName"] = m_norm2VersorFuncName[FT.type] + CANSparts.RETURNS_SCALAR;
                    argTable["versorInverseFuncName"] = m_versorInverseFuncName[FT.type];

                    m_cgd.m_cog.EmitTemplate(defSB, "testApplyVersor_SMV", argTable);
                }
            }

            return testFuncNames;
        }


    } // end of class ApplyVersor
} // end of namespace G25.CG.Shared.Func

