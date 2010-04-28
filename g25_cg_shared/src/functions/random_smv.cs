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
    /// Generates code for generating random SMV blades. 
    /// 
    /// For scalar, pseudoscalar, vector, dual vector and <c>G25.SMV.MULTIVECTOR_TYPE.MULTIVECTOR</c> types, 
    /// random coordinates are generated and the result is rescaled to a random scalar.
    /// 
    /// For <c>G25.SMV.MULTIVECTOR_TYPE.BLADE</c>, <c>ROTOR</c> and <c>VERSOR</c> multivector types, 
    /// random vectors are generated and multiplied using the geometric product of
    /// specified metric. NOTE THAT THE RESULT OF THIS PROCEDURE IS NOT NECESSARILY 
    /// A ROTOR, VERSOR OR BLADE DEPENDING ON THE DEFINITION OF THE SMV!
    /// 
    /// 
    /// The function name should be <c>"random_TYPENAME"</c>.
    /// 
    /// <code>
    ///   <function name="random_dualSphere" floatType="double"/>
    ///   <function name="random_normalizedPoint" floatType="double"/>
    ///   <function name="random_sphere"/>
    /// </code>
    /// </summary>
    public class RandomSMV : G25.CG.Shared.BaseFunctionGenerator
    {


        protected const int NB_ARGS = 1;
        public G25.SMV m_smvType;
        public string m_SMVname;
        protected Dictionary<string, string> m_randomScalarFunc = new Dictionary<string, string>(); ///< = mangled name of random scalar func 
        protected Dictionary<string, string> m_normFunc = new Dictionary<string, string>(); ///< = mangled name of norm func of m_SMVname
        protected const string MINIMUM_NORM = "minimumNorm";
        protected const string LARGEST_COORDINATE = "largestCoordinate";

        public string getSMVnameFromFGS(G25.fgs F)
        {
            return F.Name.Substring(RandomScalar.RANDOM.Length);
        }

        /// <summary>
        /// Return true if  <c>smv</c> can be initialized with random values for each coordinate:
        ///   - scalar types
        ///   - vector types
        ///   - dual vector types
        ///   - pseudoscalar types
        ///   - G25.SMV.MULTIVECTOR_TYPE.MULTIVECTOR types
        /// </summary>
        /// <param name="S"></param>
        /// <param name="smv"></param>
        /// <returns>true if  <c>smv</c> can be initialized with random values for each coordinate.</returns>
        public static bool InitWithRandomCoordinates(Specification S, G25.SMV smv)
        {
            if (smv.MvType == G25.SMV.MULTIVECTOR_TYPE.MULTIVECTOR) return true;
            int lg = smv.LowestGrade();
            int hg = smv.HighestGrade();
            if (lg != hg) return false;
            return ((lg == 0) || (lg == 1) || (lg == (S.m_dimension - 1)) || (lg == S.m_dimension));
        }

        /// <summary>
        /// Checks if this FunctionGenerator can implement a certain function.
        /// </summary>
        /// <param name="S">The specification of the algebra.</param>
        /// <param name="F">The function to be implemented.</param>
        /// <returns>true if 'F' can be implemented</returns>
        public override bool CanImplement(Specification S, G25.fgs F)
        {
            if (!(F.Name.StartsWith(RandomScalar.RANDOM) &&
                S.IsSpecializedMultivectorName(getSMVnameFromFGS(F)))) return false;
            if (F.NbArguments > 1) return false;
            return true;
        }

        /// <summary>
        /// If this FunctionGenerator can implement 'F', then this function should complete the (possible)
        /// blanks in 'F'. This means:
        ///  - Fill in F.m_returnTypeName if it is empty
        ///  - Fill in F.m_argumentTypeNames (and m_argumentVariableNames) if it is empty.
        /// </summary>
        public override void CompleteFGS()
        {
            // compute intermediate results, set return type
            m_fgs.m_returnTypeName = getSMVnameFromFGS(m_fgs);

            // fill in ArgumentTypeNames
            m_fgs.m_argumentTypeNames = new String[NB_ARGS];
            m_fgs.m_argumentTypeNames[0] = m_fgs.FloatNames[0];
            if (m_fgs.m_argumentVariableNames.Length != 1)
                m_fgs.m_argumentVariableNames = new string[1] { "scale" };

            // init argument pointers from the completed typenames (language sensitive);
            m_fgs.InitArgumentPtrFromTypeNames(m_specification);

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
            m_SMVname = getSMVnameFromFGS(m_fgs);
            m_smvType = m_specification.GetSMV(m_SMVname);

            // check dependencies for all float types
            foreach (string floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);
                //bool returnTrueName = false;
                m_randomScalarFunc[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, RandomScalar.RANDOM + floatName, new string[0], floatName, FT, null);
                m_normFunc[floatName] = G25.CG.Shared.Dependencies.GetDependency(m_specification, m_cgd, "norm", new string[] { m_SMVname }, FT, m_G25M.m_name) + G25.CG.Shared.CANSparts.RETURNS_SCALAR;
            }
        }



        /// <summary>
        /// Writes the 'explicit' declaration/definitions of 'F' to StringBuffer 'SB', taking into account parameters specified in specification 'S'.
        /// 
        /// This function is called by the non-explicit version of the function.
        /// </summary>
        public void WriteExFunction(FloatType FT, string exFuncName)
        {
            G25.SMV smv = m_specification.GetSMV(m_SMVname);

            string randFuncCall = FT.DoubleToString(m_specification, -1.0) + " + " + FT.DoubleToString(m_specification, 2.0) + " * " + m_randomScalarFunc[FT.type] + "()";

            // construct a explicit FGS
            string[] exArgumentTypeNames = new string[] { FT.type, FT.type, FT.type };
            string[] exVariableNames = { m_fgs.ArgumentVariableNames[0], MINIMUM_NORM, LARGEST_COORDINATE };
            fgs exFgs = new fgs(m_fgs.Name, m_fgs.OutputName, m_fgs.ReturnTypeName, exArgumentTypeNames, exVariableNames, m_fgs.FloatNames, m_fgs.MetricName, m_fgs.Comment, m_fgs.Options);
            exFgs.InitArgumentPtrFromTypeNames(m_specification);

            bool computeMultivectorValue = false;
            int nbArgs = 3;
            G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, exFgs, nbArgs, FT, FT.type, computeMultivectorValue);

            // because of lack of overloading, function names include names of argument types
            //G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, exFgs, FAI);
            G25.fgs CF = new G25.fgs(exFgs, exFuncName);
            //string exFuncName = funcName;

            // setup code to recursively call exFuncName
            string exFuncCall = "";
            {
                StringBuilder exFuncCallSB = new StringBuilder();
                if (m_specification.m_outputLanguage != OUTPUT_LANGUAGE.C)
                    exFuncCallSB.Append("return ");

                exFuncCallSB.Append(exFuncName);
                exFuncCallSB.Append("(");
                if (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C)
                {
                    exFuncCallSB.Append(fgs.RETURN_ARG_NAME);
                    exFuncCallSB.Append(", ");
                }
                for (int i = 0; i < exVariableNames.Length; i++)
                {
                    if (i > 0) exFuncCallSB.Append(", ");
                    exFuncCallSB.Append(exVariableNames[i]);
                }
                exFuncCallSB.AppendLine(");");
                if (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C) {
                    exFuncCallSB.AppendLine("return;");
                }
                exFuncCall = exFuncCallSB.ToString();
            }

            // generate a random SMV:
            RefGA.Multivector randomSMV = null;

            // get highest grade and bitmap of
            int hg = smv.HighestGrade(); // what is the highest non-zero grade of the type?
            uint bvBitmap = smv.BasisVectorBitmap(); // which basis vectors are used in the type?

            // setup random value to be returned
            if (InitWithRandomCoordinates(m_specification, smv))
            {
                // set all coordinates to random values
                randomSMV = RefGA.Multivector.ZERO;
                for (int b = 0; b < smv.NbNonConstBasisBlade; b++)
                {
                    randomSMV = RefGA.Multivector.Add(randomSMV,
                        RefGA.Multivector.gp(new RefGA.Multivector(smv.NonConstBasisBlade(b)),
                        new RefGA.Multivector("r" + smv.NonConstBasisBlade(b).ToLangString(m_specification.m_basisVectorNames))));
                }
            }
            else
            {
                // set to geometric product of random vectors
                randomSMV = RefGA.Multivector.ONE;
                RefGA.Multivector randomVector;
                RefGA.BasisBlade[] B = new RefGA.BasisBlade[(int)RefGA.Bits.BitCount(bvBitmap)];
                for (int g = 0; g < hg; g++)
                {
                    int cnt = 0;
                    for (int v = 0; v < m_specification.m_dimension; v++)
                    {
                        if ((bvBitmap & (uint)(1 << v)) != 0)
                        {
                            B[cnt] = new RefGA.BasisBlade((uint)(1 << v), 1.0, "r" + g + "_" + m_specification.m_basisVectorNames[v]);
                            cnt++;
                        }
                    }
                    randomVector = new RefGA.Multivector(B);
                    randomSMV = RefGA.Multivector.gp(randomSMV, randomVector, m_G25M.m_metric);
                }

                // round value if required by metric
                if (m_G25M.m_round) randomSMV = randomSMV.Round(1e-14);
            }

            // setup instructions
            System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
            {
                int nbTabs = 1;

                if (!smv.IsConstant())
                {
                    string smvTypeName = FT.GetMangledName(m_specification, m_SMVname);

                    // SMV tmp;
                    // double n, mul, lc;
                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, smvTypeName + " tmp;"));
                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, FT.type + " n, mul, lc;"));
                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C ? "int" : "bool") + " nullBlade;"));

                    // double rCoord = randomValue(), ....;
                    StringBuilder randSB = new StringBuilder();
                    randSB.Append(FT.type);
                    randSB.Append(" ");
                    if (InitWithRandomCoordinates(m_specification, smv))
                    { // random coordinates
                        for (int b = 0; b < smv.NbNonConstBasisBlade; b++)
                        {
                            if (b > 0) randSB.Append(", ");
                            randSB.Append("r" + smv.NonConstBasisBlade(b).ToLangString(m_specification.m_basisVectorNames));
                            randSB.Append(" = ");
                            randSB.Append(randFuncCall);
                        }
                    }
                    else
                    { // random vectors
                        bool first = true;
                        for (int g = 0; g < hg; g++)
                        {
                            for (int v = 0; v < m_specification.m_dimension; v++)
                            {
                                if (!first) randSB.Append(", "); first = false;
                                if ((g > 0) && (v == 0))
                                {
                                    randSB.AppendLine(""); // newline 
                                    randSB.Append("\t\t");
                                }
                                randSB.Append("r" + g + "_" + m_specification.m_basisVectorNames[v]);
                                randSB.Append(" = ");
                                randSB.Append(randFuncCall);
                            }
                        }
                    }
                    randSB.Append(";");
                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, randSB.ToString()));


                    // SMV tmp = random;
                    bool mustCast = false;
                    bool randomPtr = false;
                    bool declareRandom = false;
                    I.Add(new G25.CG.Shared.AssignInstruction(nbTabs, smv, FT, mustCast, randomSMV, "tmp", randomPtr, declareRandom));

                    // n = norm_ret_scalar(tmp);
                    string emp = (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C) ? "&" : "";
                    I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, "n = " + m_normFunc[FT.type] + "(" + emp + "tmp);"));

                    // lc = largestCoordinate(tmp);
                    if (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C)
                    {
                        I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, "lc = " + smvTypeName + "_largestCoordinate(&tmp);"));
                    }
                    else
                    {
                        I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, "lc = tmp.largestCoordinate();"));
                    }

                    // null = (n == 0) && (lc ! 0)
                    if ((m_smvType.MvType == SMV.MULTIVECTOR_TYPE.ROTOR) || (m_smvType.MvType == SMV.MULTIVECTOR_TYPE.VERSOR))
                        I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, "nullBlade = " + (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C ? "0" : "false") + ";" ));
                    else I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, "nullBlade = ((n == " + FT.DoubleToString(m_specification, 0.0) + ") && (lc != " + FT.DoubleToString(m_specification, 0.0) + "));"));


                    // test minimumNorm
                    I.Add(new CommentInstruction(nbTabs, "Recurse if generated random value has a norm below user-supplied limit, unless this is a null blade"));
                    I.Add(new G25.CG.Shared.IfElseInstruction(nbTabs, "(n < " + exVariableNames[1] + ") && (!nullBlade)", 
                        new List<Instruction>() {new G25.CG.Shared.VerbatimCodeInstruction(nbTabs + 1, exFuncCall)},  // if instructions
                        new List<Instruction>())); // else instructions

                    // compute multiplier
                    string lcCondition = "(lc * " + CodeUtil.OpNameToLangString(m_specification, FT, "abs") + "(mul)) > largestCoordinate ";
                    I.Add(new CommentInstruction(nbTabs, "Compute multiplier"));
                    I.Add(new G25.CG.Shared.IfElseInstruction(nbTabs, "n < " + FT.DoubleToString(m_specification, 0.0001),
                        new List<Instruction>() { new G25.CG.Shared.VerbatimCodeInstruction(nbTabs + 1, "mul = " + FT.DoubleToString(m_specification, 1.0) + ";") }, // if instructions
                        new List<Instruction>() { // else instructions
                            new G25.CG.Shared.VerbatimCodeInstruction(nbTabs + 1, "mul = " + exFgs.ArgumentVariableNames[0] + " * (" + randFuncCall + ") / n;"),
                            new CommentInstruction(nbTabs+1, "Test largest coordinate"),
                            new G25.CG.Shared.IfElseInstruction(nbTabs+1, lcCondition,
                                new List<Instruction>() { new G25.CG.Shared.VerbatimCodeInstruction(nbTabs + 2, exFuncCall) },  // if instructions
                                new List<Instruction>())
                        })); 

                    // test largest coordinate
                    //I.Add(new CommentInstruction(nbTabs, "Test largest coordinate"));
                    //I.Add(new G25.CG.Shared.IfElseInstruction(nbTabs, lcCondition,
                    //   new List<Instruction>() { new G25.CG.Shared.VerbatimCodeInstruction(nbTabs + 1, exFuncCall) },  // if instructions
                    //    new List<Instruction>())); // else instructions

                    // return tmp*mul;
                    I.Add(new CommentInstruction(nbTabs, "Apply multiplier, return"));
                    RefGA.Multivector tmpValue = G25.CG.Shared.Symbolic.SMVtoSymbolicMultivector(m_specification, smv, "tmp", false);
                    RefGA.Multivector returnValue = RefGA.Multivector.gp(tmpValue, new RefGA.Multivector("mul"));
                    I.Add(new G25.CG.Shared.ReturnInstruction(nbTabs, smv, FT, mustCast, returnValue));
                } // end of '!IsConstant()'?
                else
                {
                    // the user wants a 'random' constant: simply return the constant itself.
                    RefGA.Multivector returnValue = G25.CG.Shared.Symbolic.SMVtoSymbolicMultivector(m_specification, smv, "tmp", false);
                    bool mustCast = false;
                    I.Add(new G25.CG.Shared.ReturnInstruction(nbTabs, smv, FT, mustCast, returnValue));
                }

            }

            // because of lack of overloading, function names include names of argument types
            //G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, exFgs, FAI);

            // generate comment
            string comment = "/** " +
                exFgs.AddUserComment("Returns random " + m_SMVname + " with a scale in the interval [0, scale)") + " */";

            G25.CG.Shared.Functions.WriteFunction(m_specification, m_cgd, CF, m_specification.m_inlineFunctions, exFuncName, FAI, I, comment);
        } // end of WriteExFunction

        /// <summary>
        /// Writes the declaration/definitions of 'F' to StringBuffer 'SB', taking into account parameters specified in specification 'S'.
        /// </summary>
        public override void WriteFunction()
        {

            //G25.SMV smv = m_specification.GetSMV(m_SMVname);

            foreach (string floatName in m_fgs.FloatNames)
            {
                FloatType FT = m_specification.GetFloatType(floatName);

                m_fgs.m_argumentTypeNames[0] = FT.type;
                bool computeMultivectorValue = false;
                G25.CG.Shared.FuncArgInfo[] FAI = G25.CG.Shared.FuncArgInfo.GetAllFuncArgInfo(m_specification, m_fgs, NB_ARGS, FT, FT.type, computeMultivectorValue);

                // because of lack of overloading, function names include names of argument types
                G25.fgs CF = G25.CG.Shared.Util.AppendTypenameToFuncName(m_specification, FT, m_fgs, FAI);

                WriteExFunction(FT, CF.OutputName + "_ex");

                System.Collections.Generic.List<G25.CG.Shared.Instruction> I = new System.Collections.Generic.List<G25.CG.Shared.Instruction>();
                int nbTabs = 1;
                I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, FT.type + " " + MINIMUM_NORM + " = " + FT.DoubleToString(m_specification, 0.0001) + ";"));
                I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs, FT.type + " " + LARGEST_COORDINATE + " = " + FT.DoubleToString(m_specification, 4.0) + ";"));
                I.Add(new G25.CG.Shared.VerbatimCodeInstruction(nbTabs,
                    ((m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C) ? "" : "return ") + 
                    CF.OutputName + "_ex(" +
                    ((m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C) ? (G25.fgs.RETURN_ARG_NAME + ", ") : "") + 
                    FAI[0].Name + ", minimumNorm, " + FAI[0].Name + " * largestCoordinate);"));

                // generate comment
                string comment = "/** " +
                    m_fgs.AddUserComment("Returns random " + m_SMVname + " with a scale in the interval [0, scale)") + " */";

                G25.CG.Shared.FuncArgInfo returnArgument = null;
                if (m_specification.m_outputLanguage == OUTPUT_LANGUAGE.C) 
                    returnArgument = new G25.CG.Shared.FuncArgInfo(m_specification, m_fgs, -1, FT, m_SMVname, false); // false = compute value

                string returnTypeName = FT.GetMangledName(m_specification, m_SMVname);
                G25.CG.Shared.Functions.WriteFunction(m_specification, m_cgd, CF, m_specification.m_inlineFunctions, returnTypeName, CF.OutputName, returnArgument, FAI, I, comment);
            }
        } // end of WriteFunction


    } // end of class RandomSMV
} // end of namespace G25.CG.Shared.Func

