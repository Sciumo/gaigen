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
    /// Generates code for sin, cos and exp.
    /// 
    /// The following function names are possible:
    ///   - <c>"sin"</c>: Sine of multivector.
    ///   - <c>"cos"</c>: Cosine of multivector.
    ///   - <c>"exp"</c>: Exponential of multivector.
    ///   - <c>"sinh"</c>: Hyperbolic sine of multivector.
    ///   - <c>"cosh"</c>: Hyperbolic cosine of multivector.
    /// 
    /// For specialized multivector types, the generator tries to figure out the sign of the square
    /// itself. If this fails, the user can specify a value using the <c>optionSquare="value"</c>
    /// option, where <c>value</c> can be -1, 0, +1.
    /// 
    /// If the sign of the square is known, much more effective code can be genererated, avoiding 
    /// a (slow and inprecise) series evaluation.
    /// 
    /// The metric can be specified using the <c>metric="metricName"</c> attribute.
    /// </summary>
    public class SinCosExp : G25.CG.Shared.BaseFunctionGenerator
    {

        /// <returns>true when F.Name == "exp"</returns>
        public static bool IsExp(G25.fgs F)
        {
            return F.Name == "exp";
        }
        /// <returns>true when F.Name == "sin"</returns>
        public static bool IsSin(G25.fgs F)
        {
            return F.Name == "sin";
        }

        /// <returns>true when F.Name == "cos"</returns>
        public static bool IsCos(G25.fgs F)
        {
            return F.Name == "cos";
        }

        /// <returns>true when F.Name == "sinh"</returns>
        public static bool IsSinh(G25.fgs F)
        {
            return F.Name == "sinh";
        }

        /// <returns>true when F.Name == "cosh"</returns>
        public static bool IsCosh(G25.fgs F)
        {
            return F.Name == "cosh";
        }

        // constants, intermediate results
        protected const int NB_ARGS = 1;
        protected bool m_gmvFunc; ///< is this a function over GMVs?
        protected bool m_scalarSquare; ///< is the argument an SMV which squares to a scalar?
        protected G25.VariableType m_returnType; ///< type of returned value
        protected RefGA.Multivector m_returnValue;
        protected int m_signOfSquare; ///< -1, 0 or 1
        protected RefGA.Multivector m_alphaValue; ///< input value squared
        protected RefGA.Multivector m_mulValue;
        protected string m_alphaName = "_alpha";
        protected string m_mulName = "_mul";

        protected string m_inputTypeName;
        protected string m_returnTypeName;

        // dependencies:
        protected Dictionary<string, string> m_gpFuncII = new Dictionary<string, string>(); ///< = mangled name of geometric product of (inputType, inputType) -> returnType (FORCED)
        protected Dictionary<string, string> m_gpFuncRI = new Dictionary<string, string>(); ///<  = mangled name of geometric product of (returnType, inputType) 
        protected Dictionary<string, string> m_gpFuncRR = new Dictionary<string, string>(); ///<  = mangled name of geometric product of (returnType, returnType) 
        protected Dictionary<string, string> m_gpFuncIdouble = new Dictionary<string, string>(); ///< = mangled name of geometric product of (inputType, double)
        protected Dictionary<string, string> m_gpFuncRdouble = new Dictionary<string, string>(); ///< = mangled name of geometric product of (returnType, double) 
        protected Dictionary<string, string> m_addFuncRR = new Dictionary<string, string>(); ///< = mangled name of add of (returnType, returnType)
        protected Dictionary<string, string> m_subFuncRR = new Dictionary<string, string>(); ///< = mangled name of subtract of (returnType, returnType)
        protected Dictionary<string, string> m_normE2funcR = new Dictionary<string, string>(); ///< = mangled name of norm2 of (returnType)
        protected Dictionary<string, string> m_SASfuncI = new Dictionary<string, string>(); ///<  = Scale and add scalar (input type)
        protected Dictionary<string, string> m_copyInputTypeToReturnType = new Dictionary<string, string>(); ///<  = copy input type to return type

        protected G25.SMV m_smv = null; ///< if function over SMV, type goes here
        protected Dictionary<string, string> m_funcName = new Dictionary<string, string>();  ///< generated function name with full mangling, etc


        /// <summary>
        /// Checks if this FunctionGenerator can implement a certain function.
        /// </summary>
        /// <param name="S">The specification of the algebra.</param>
        /// <param name="F">The function to be implemented.</param>
        /// <returns>true if 'F' can be implemented</returns>
        public override bool CanImplement(Specification S, G25.fgs F)
        {
            return ((IsExp(F) || IsCos(F) || IsSin(F) || IsCosh(F) || IsSinh(F)) && (F.MatchNbArguments(1)));
        }

        /// <summary>
        /// If this FunctionGenerator can implement 'F', then this function should complete the (possible)
        /// blanks in 'F'. This means:
        ///  - Fill in F.m_returnTypeName if it is empty
        ///  - Fill in F.m_argumentTypeNames (and m_argumentVariableNames) if it is empty.
        /// </summary>
        public override void CompleteFGS()
        {
            // fill in ArgumentTypeNames
            if (m_fgs.ArgumentTypeNames.Length == 0)
                m_fgs.m_argumentTypeNames = new String[] { m_gmv.Name };

            // init argument pointers from the completed typenames (language sensitive);
            m_fgs.InitArgumentPtrFromTypeNames(m_specification);

            // get all function info
            FloatType FT = m_specification.GetFloatType(m_fgs.FloatNames[0]);
            bool computeMultivectorValue = true;
            G25.CG.Shared.FuncArgInfo[] tmpFAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_specification.m_GMV.Name, computeMultivectorValue);

            m_gmvFunc = !tmpFAI[0].IsScalarOrSMV();

            // compute intermediate results, set return type
            if (m_gmvFunc)
            {
                m_fgs.m_returnTypeName = m_gmv.Name; // sin/cos/sinh/cosh/exp(gmv) = gmv
                m_returnType = m_gmv;
                m_inputTypeName = m_gmv.Name;
                m_returnTypeName = m_gmv.Name;
            }
            else
            {
                m_smv = tmpFAI[0].Type as G25.SMV;

                try  // exceptions are caught below -> in that case, do a series for the SMV
                { // sin/cos/sinh/cosh/exp(smv bivector)
                    RefGA.Multivector value = tmpFAI[0].MultivectorValue[0];
                    RefGA.Multivector squareValue = RefGA.Multivector.gp(value, value, m_M);

                    String userSetSquare = m_fgs.GetOption("square");
                    if (userSetSquare != null) m_signOfSquare = (int)Math.Sign(Double.Parse(userSetSquare));
                    else
                    {
                        // the following line can throw an exception, which is caught below
                        double sqEval = RefGA.Symbolic.SymbolicUtil.EvaluateRandomSymbolicToScalar(squareValue);
                        m_signOfSquare = (int)Math.Sign(sqEval);
                    }
                    m_scalarSquare = true;

                    // compute alpha = sqrt(fabs(value^2))
                    m_alphaValue = RefGA.Symbolic.UnaryScalarOp.Sqrt(RefGA.Symbolic.UnaryScalarOp.Abs(squareValue));

                    // use hyperbolic sin / cos or regular sin / cos for shortcuts?
                    bool hyperbolic =
                            (((IsExp(m_fgs) || IsCosh(m_fgs) || IsSinh(m_fgs)) && (m_signOfSquare > 0)) ||
                            ((IsCos(m_fgs) || IsSin(m_fgs)) && (m_signOfSquare < 0)));

                    // compute mul for SIN and EXP (assuming alpha is not zero)
                    if (IsCos(m_fgs) || m_alphaValue.IsZero()) m_mulValue = RefGA.Multivector.ONE;
                    else if (m_signOfSquare == 0) m_mulValue = RefGA.Multivector.ZERO;
                    else
                    {
                        if (hyperbolic)
                            m_mulValue = RefGA.Multivector.gp(
                                                            RefGA.Symbolic.UnaryScalarOp.Sinh(new RefGA.Multivector(m_alphaName)),
                                                            RefGA.Symbolic.UnaryScalarOp.Inverse(new RefGA.Multivector(m_alphaName)));
                        else m_mulValue = RefGA.Multivector.gp(
                                                            RefGA.Symbolic.UnaryScalarOp.Sin(new RefGA.Multivector(m_alphaName)),
                                                            RefGA.Symbolic.UnaryScalarOp.Inverse(new RefGA.Multivector(m_alphaName)));
                    }


                    // compute sin, cos part
                    RefGA.Multivector cosValue = null, sinValue = null;

                    if (m_signOfSquare == 0)
                    {
                        cosValue = RefGA.Multivector.ONE;
                        sinValue = value;
                    }
                    else
                    {
                        sinValue = RefGA.Multivector.gp(value, new RefGA.Multivector(m_mulName));
                        if (hyperbolic) cosValue = RefGA.Symbolic.UnaryScalarOp.Cosh(new RefGA.Multivector(m_alphaName));
                        else cosValue = RefGA.Symbolic.UnaryScalarOp.Cos(new RefGA.Multivector(m_alphaName));
                    }

                    // compute return value
                    if (IsExp(m_fgs)) m_returnValue = RefGA.Multivector.Add(cosValue, sinValue);
                    else if (IsCos(m_fgs) || IsCosh(m_fgs)) m_returnValue = cosValue;
                    else if (IsSin(m_fgs) || IsSinh(m_fgs)) m_returnValue = sinValue;

                    // get name of return type
                    if (m_fgs.m_returnTypeName.Length == 0)
                    {
                        m_returnType = G25.CG.Shared.SpecializedReturnType.GetReturnType(m_specification, m_cgd, m_fgs, FT, m_returnValue);
                        m_fgs.m_returnTypeName = m_returnType.GetName();
                    }
                    else
                    {
                        m_returnType = m_specification.GetType(m_fgs.m_returnTypeName);
                    }

                }
                catch (System.Exception)
                { // sin/cos/sinh/cosh/exp(any SMV)
                    // determine return type
                    ComputeReturnTypeExpSmv(tmpFAI, FT);
                    m_inputTypeName = tmpFAI[0].TypeName;
                    m_fgs.m_returnTypeName = m_returnTypeName;
                }

            }
        } // end of CompleteFGS()


        /// <summary>
        /// This function should check the dependencies of this function. If dependencies are
        /// missing, the function can complain (throw exception) or fix it (add the required functions).
        /// 
        /// If changes are made to the specification then it must be locked first because
        /// multiple threads run in parallel which may all modify the specification!
        /// </summary>
        public override void CheckDepencies()
        {
            if (m_gmvFunc || (!m_scalarSquare))
            {
                // check dependencies for all float types
                foreach (string floatName in m_fgs.FloatNames)
                {
                    FloatType FT = m_specification.GetFloatType(floatName);

                    //bool returnTrueName = false;
                    m_gpFuncII[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gp", new String[] { m_inputTypeName, m_inputTypeName }, m_returnTypeName, FT, m_G25M.m_name); // TODO: forced return type!
                    m_gpFuncRI[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gp", new String[] { m_returnTypeName, m_inputTypeName }, m_returnTypeName, FT, m_G25M.m_name);
                    m_gpFuncRR[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gp", new String[] { m_returnTypeName, m_returnTypeName }, m_returnTypeName, FT, m_G25M.m_name);

                    m_gpFuncIdouble[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gp", new String[] { m_inputTypeName, FT.type }, FT, null);
                    m_gpFuncRdouble[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "gp", new String[] { m_returnTypeName, FT.type }, FT, null);
                    m_addFuncRR[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "add", new String[] { m_returnTypeName, m_returnTypeName }, FT, null);
                    m_subFuncRR[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "subtract", new String[] { m_returnTypeName, m_returnTypeName }, FT, null);
                    m_normE2funcR[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "norm2", new String[] { m_returnTypeName }, FT, m_G25M.m_name) + G25.CG.Shared.CANSparts.RETURNS_SCALAR;
                    m_SASfuncI[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "sas", new String[] { m_inputTypeName, FT.type, FT.type }, FT, null);

                    if (IsSin(m_fgs) || IsSinh(m_fgs))
                    { // the 'sin' series needs to copy from inputtype to output type
                        if (m_returnTypeName == m_inputTypeName)
                        {
                            m_copyInputTypeToReturnType[floatName] = FT.GetMangledName(m_specification, m_inputTypeName) + "_copy";
                        }
                        else
                        {
                            // why can the converter for the right floattype not be found?
                            // need to use non-mangled name?
                            m_copyInputTypeToReturnType[floatName] = G25.CG.Shared.Dependencies.GetConverterDependency(m_specification, m_cgd, m_inputTypeName, m_returnTypeName, FT);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Writes the declaration/definitions of 'F' to StringBuffer 'SB', taking into account parameters specified in specification 'S'.
        /// </summary>
        public override void WriteFunction()
        {
            foreach (string floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);

                bool computeMultivectorValue = true;
                G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, m_gmv.Name, computeMultivectorValue);

                // because of lack of overloading, function names include names of argument types
                G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, m_fgs, FAI);

                // if bivector type: specialize
                if (m_gmvFunc || (!m_scalarSquare))
                {
                    // sin, cos, exp of GMV, or of SMV which does not square to scalar:
                    WriteFunction(FT, CF);
                }
                else
                {
                    // write simple specialized function:
                    int nbTabs = 1;
                    bool mustCast = false;

                    // setup instructions
                    List<G25.CG.Shared.Instruction> I = new List<G25.CG.Shared.Instruction>();

                    if (m_signOfSquare == 0)
                    {
                        // assign
                        I.Add(new G25.CG.Shared.ReturnInstruction(nbTabs, m_returnType, FT, mustCast, m_returnValue));
                    }
                    else
                    {
                        // alpha
                        bool alphaPtr = false;
                        bool declareAlpha = true;
                        I.Add(new G25.CG.Shared.AssignInstruction(nbTabs, FT, FT, mustCast, m_alphaValue, m_alphaName, alphaPtr, declareAlpha));

                        // mul
                        if (!(IsCos(m_fgs) || IsCosh(m_fgs)))
                        {
                            I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, FT.type + " " + m_mulName + ";"));
                            {
                                bool mulPtr = false;
                                bool declareMul = false;

                                List<G25.CG.Shared.Instruction> ifI = new List<G25.CG.Shared.Instruction>();
                                ifI.Add(new G25.CG.Shared.AssignInstruction(nbTabs + 1, FT, FT, mustCast, m_mulValue, m_mulName, mulPtr, declareMul));

                                List<G25.CG.Shared.Instruction> elseI = new List<G25.CG.Shared.Instruction>();
                                elseI.Add(new G25.CG.Shared.AssignInstruction(nbTabs + 1, FT, FT, mustCast, RefGA.Multivector.ZERO, m_mulName, mulPtr, declareMul));

                                I.Add(new G25.CG.Shared.IfElseInstruction(nbTabs, m_alphaName + " != " + FT.DoubleToString(m_specification, 0.0), ifI, elseI));
                            }
                        }

                        // assign
                        I.Add(new G25.CG.Shared.ReturnInstruction(nbTabs, m_returnType, FT, mustCast, m_returnValue));
                    }

                    Comment comment = new Comment(m_fgs.Name + " of " + FAI[0].TypeName + " (uses fast special case)");

                    m_funcName[FT.type] = CF.OutputName;

                    bool staticFunc = Functions.OutputStaticFunctions(m_specification);
                    G25.CG.Shared.Functions.WriteFunction(m_specification, m_cgd, CF, m_specification.m_inlineFunctions, staticFunc, CF.OutputName, FAI, I, comment);
                } // end of 'if bivector SMV'
            }
        } // end of WriteFunction

        /// <summary>
        /// Computes the return type for exp(smv).
        /// Sets m_returnTypeName and m_returnType.
        /// </summary>
        protected void ComputeReturnTypeExpSmv(G25.CG.Shared.FuncArgInfo[] FAI, G25.FloatType FT)
        {
            m_returnTypeName = m_fgs.ReturnTypeName;
            if (m_returnTypeName.Length == 0)
            {
                // find out returntype
                G25.SMV inputType = FAI[0].Type as G25.SMV;
                { // determine tight return type (i.e., keep repeating the geometric product of the input type until no more new basis blades are found)
                    // We want to known which basis blades are present in inputType-to-the-power-of-infinity
                    // So which bases blades will be present in the return type?
                    bool[] present = new bool[1 << m_specification.m_dimension];
                    {
                        bool[] tried = new bool[(1 << m_specification.m_dimension) * (1 << m_specification.m_dimension)];

                        // init 'present' with the inputType
                        for (int i = 0; i < inputType.NbCoordinates; i++)
                        {
                            present[inputType.Group(0)[i].bitmap] = true;
                        }

                        // combine bitmaps (gp) until no more new bitmaps found
                        bool newBBfound = true;
                        while (newBBfound)
                        {
                            newBBfound = false;
                            for (uint i = 0; i < present.Length; i++)
                            {
                                if (!present[i]) continue;
                                for (uint j = 0; j < present.Length; j++)
                                {
                                    if (!present[j]) continue;
                                    if (tried[j * (1 << m_specification.m_dimension) + i]) continue;

                                    // remember that we tried this combo of input blades:
                                    tried[j * (1 << m_specification.m_dimension) + i] = true;

                                    RefGA.Multivector M = RefGA.Multivector.gp(new RefGA.Multivector(new RefGA.BasisBlade(i)),
                                        new RefGA.Multivector(new RefGA.BasisBlade(j)), m_G25M.m_metric);

                                    for (int k = 0; k < M.BasisBlades.Length; k++)
                                    {
                                        uint b = M.BasisBlades[k].bitmap;
                                        if (!present[b])
                                        { // this bitmap wasn't found yet, so mark it as present
                                            newBBfound = true;
                                            present[b] = true;
                                        }
                                    }
                                }
                            }
                        } // end of combine bitmaps (gp) until no more new bitmaps found
                    }

                    // construct multivector of return type, get return type
                    {
                        System.Collections.ArrayList L = new System.Collections.ArrayList();
                        for (uint i = 0; i < present.Length; i++)
                        {
                            if (!present[i]) continue;
                            else L.Add(new RefGA.BasisBlade(i, 1.0, "c" + i));
                        }
                        RefGA.Multivector RTMV = new RefGA.Multivector(L);
                        m_returnType = CG.Shared.SpecializedReturnType.GetReturnType(m_specification, m_cgd, m_fgs, FT, RTMV) as G25.SMV;
                        m_returnTypeName = m_returnType.GetName();
                    }
                }
            } // end of 'find out return type name'
            else
            {
                m_returnType = m_specification.GetType(m_returnTypeName);
            }
        } // end of ComputeReturnTypeExpSmv()

        /// <summary>
        /// Emits a returnType sincosexp(inputType) function.
        /// </summary>
        /// <param name="FT"></param>
        /// <param name="CF"></param>
        protected void WriteFunction(FloatType FT, G25.fgs CF)
        {
            // get template name
            string templateName = "not set";
            if (IsCos(m_fgs)) templateName = "seriesCos";
            else if (IsSin(m_fgs)) templateName = "seriesSin";
            else if (IsCosh(m_fgs)) templateName = "seriesCosh";
            else if (IsSinh(m_fgs)) templateName = "seriesSinh";
            else if (IsExp(m_fgs)) templateName = "seriesExp";

            // library functions over scalars:
            string sqrtFunc = CodeUtil.OpNameToLangString(m_specification, FT, "sqrt");
            string cosFunc = CodeUtil.OpNameToLangString(m_specification, FT, "cos");
            string sinFunc = CodeUtil.OpNameToLangString(m_specification, FT, "sin");
            string coshFunc = CodeUtil.OpNameToLangString(m_specification, FT, "cosh");
            string sinhFunc = CodeUtil.OpNameToLangString(m_specification, FT, "sinh");

            // get arguments setup:
            System.Collections.Hashtable args = new System.Collections.Hashtable();
            args["S"] = m_specification;
            args["FT"] = FT;
            args["funcName"] = CF.OutputName;
            args["inputType"] = FT.GetMangledName(m_specification, m_inputTypeName);
            args["returnType"] = FT.GetMangledName(m_specification, m_returnTypeName);
            args["gpFuncII"] = m_gpFuncII[FT.type];
            args["gpFuncRI"] = m_gpFuncRI[FT.type];
            args["gpFuncRR"] = m_gpFuncRR[FT.type];
            args["gpFuncIdouble"] = m_gpFuncIdouble[FT.type];
            args["gpFuncRdouble"] = m_gpFuncRdouble[FT.type];
            args["addFuncRR"] = m_addFuncRR[FT.type];
            args["subFuncRR"] = m_subFuncRR[FT.type];
            args["normE2funcR"] = m_normE2funcR[FT.type];
            args["SASfuncI"] = m_SASfuncI[FT.type];
            args["sqrtFunc"] = sqrtFunc;
            args["cosFunc"] = cosFunc;
            args["sinFunc"] = sinFunc;
            args["coshFunc"] = coshFunc;
            args["sinhFunc"] = sinhFunc;
            args["mathFuncName"] = m_fgs.Name;
            if (IsSin(m_fgs) || IsSinh(m_fgs)) // the 'sin' series needs to copy from inputtype to output type
                args["copyInputToReturnFunc"] = m_copyInputTypeToReturnType[FT.type];

            // how to access the scalar coordinate of the return type:
            if (m_gmv.GetName() == m_returnType.GetName())
            {
                string emp = (m_specification.OutputC()) ? "&" : "";
                args["scalarTmp1"] = FT.GetMangledName(m_specification, m_returnTypeName) + "_scalar(" + emp + "tmp1)";
            }
            else
            {
                // return type is SMV: get access strings for all coordinates, and use the one for the scalar
                string varName = "tmp1";
                bool ptr = false;
                G25.SMV smv = m_returnType as G25.SMV;
                string[] accessStr = G25.CG.Shared.CodeUtil.GetAccessStr(m_specification, smv, varName, ptr);
                args["scalarTmp1"] = accessStr[smv.GetElementIdx(RefGA.BasisBlade.ONE)];
            }

            args["userComment"] = m_fgs.Comment;

            m_cgd.m_cog.EmitTemplate(m_cgd.m_defSB, templateName, args);

            if (m_specification.OutputCppOrC())
                m_cgd.m_cog.EmitTemplate(m_cgd.m_declSB, "seriesDecl", args);

            m_funcName[FT.type] = CF.OutputName;
        }

        // used for testing:
        protected Dictionary<string, string> m_randomScalarFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomSmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_addGmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_subtractGmvFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_gpSinCosExpFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_randomBladeFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_sinhFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_coshFuncName = new Dictionary<string, string>();
        protected Dictionary<string, string> m_expFuncName = new Dictionary<string, string>();

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

                if (m_gmvFunc)
                {
                    m_addGmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "add", new String[] { m_specification.m_GMV.Name, m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, null);
                    m_randomBladeFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_blade", new String[0], m_specification.m_GMV.Name, FT, null);
                    m_sinhFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "sinh", new String[] { m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, m_G25M.m_name);
                    m_coshFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "cosh", new String[] { m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, m_G25M.m_name);
                    m_expFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "exp", new String[] { m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, m_G25M.m_name);
                }
                else if (m_smv != null)
                {
                    string defaultReturnTypeName = null;
                    m_randomSmvFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "random_" + m_smv.Name, new String[0], defaultReturnTypeName, FT, null);
                    m_gpSinCosExpFuncName[FT.type] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, m_fgs.Name, new String[] { m_specification.m_GMV.Name }, m_specification.m_GMV.Name, FT, m_G25M.m_name);
                }
            }
        }

        /// <summary>
        /// Writes the testing function for 'F' to 'm_defSB'.
        /// The generated function returns success (1) or failure (0).
        /// </summary>
        /// <returns>The list of name name of the int() function which tests the function.</returns>
        public override List<string> WriteTestFunction()
        {
            StringBuilder defSB = (m_specification.m_inlineFunctions) ? m_cgd.m_inlineDefSB : m_cgd.m_defSB;

            List<string> testFuncNames = new List<string>();


            foreach (string floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);

                string testFuncName = Util.GetTestingFunctionName(m_specification, m_cgd, m_funcName[FT.type]);

                if (m_gmvFunc) // GMV test
                {
                    bool canTest = IsExp(m_fgs) || IsCosh(m_fgs) || IsSinh(m_fgs);
                    canTest = canTest && m_specification.m_dimension >= 2;
                    if (canTest)
                    {
                        testFuncNames.Add(testFuncName);
                        System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                        argTable["S"] = m_specification;
                        argTable["FT"] = FT;
                        argTable["gmv"] = m_specification.m_GMV;
                        argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                        argTable["testFuncName"] = testFuncName;
                        argTable["targetFuncName"] = m_funcName[FT.type];
                        argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                        argTable["randomBladeFuncName"] = m_randomBladeFuncName[FT.type];
                        argTable["subtractGmvFuncName"] = m_subtractGmvFuncName[FT.type];
                        argTable["addGmvFuncName"] = m_addGmvFuncName[FT.type];
                        argTable["sinhFuncName"] = m_sinhFuncName[FT.type];
                        argTable["coshFuncName"] = m_coshFuncName[FT.type];
                        argTable["expFuncName"] = m_expFuncName[FT.type];

                        m_cgd.m_cog.EmitTemplate(defSB, "testSinhCoshExpGMV", argTable);
                    }
                }
                else if ((m_smv != null) && m_smv.CanConvertToGmv(m_specification)) // SMV test
                {
                    testFuncNames.Add(testFuncName);
                    System.Collections.Hashtable argTable = new System.Collections.Hashtable();
                    argTable["S"] = m_specification;
                    argTable["FT"] = FT;
                    argTable["gmvName"] = FT.GetMangledName(m_specification, m_specification.m_GMV.Name);
                    argTable["smv"] = m_smv;
                    argTable["smvName"] = FT.GetMangledName(m_specification, m_smv.Name);
                    argTable["resultIsScalar"] = m_returnType is G25.FloatType;
                    argTable["resultSmvName"] = FT.GetMangledName(m_specification, m_fgs.m_returnTypeName);
                    argTable["testFuncName"] = testFuncName;
                    argTable["targetFuncName"] = m_funcName[FT.type];
                    argTable["randomScalarFuncName"] = m_randomScalarFuncName[FT.type];
                    argTable["randomSmvFuncName"] = m_randomSmvFuncName[FT.type];
                    argTable["subtractGmvFuncName"] = m_subtractGmvFuncName[FT.type];
                    argTable["gpSinCosExpFuncName"] = m_gpSinCosExpFuncName[FT.type];
                    argTable["scalarSquare"] = m_scalarSquare;
                    
                    m_cgd.m_cog.EmitTemplate(defSB, "testSinCosExpSMV", argTable);
                }
            }

            return testFuncNames;
        } // end of WriteTestFunction()


    } // end of class SinCosExp

} // end of namespace G25.CG.Shared.Func

