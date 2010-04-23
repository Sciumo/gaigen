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


namespace G25
{
    namespace CG
    {
        namespace Shared
        {

            /// <summary>
            /// This class contains functions for finding the (best) return type 
            /// of specialized functions.
            /// </summary>
            public class SpecializedReturnType
            {
                /// <summary>
                /// This function contains a function to find a tightly matching specialized multivector.
                /// Given a Multivector 'M', it find the best match which can contain it.
                /// 
                /// The function tries each SMV in the specification. It rates them as follows:
                /// -missing variable basis blade: disqualified
                /// -constant coordinate mismatch: disqualified
                /// -constant coordinate match: +1
                /// -excess variable coordinate : -1
                /// 
                /// If the symbolic multivector 'M' has a scalar return type, the function will insist on returning
                /// a scalar type! It will not (for example) return a rotor.
                /// </summary>
                /// <param name="S">Specification of algebra, used for the m_SMV</param>
                /// <param name="M">The multivector for which a matching SMV is sought.</param>
                /// <param name="FT">The preferred floating point type (only used when scalar is returned).</param>
                /// <returns>null when no match found; the selected G25.SMV or G25.FloatType otherwise.</returns>
                public static G25.VariableType FindTightestMatch(G25.Specification S, RefGA.Multivector M, FloatType FT) {
                    G25.SMV bestSMV = null;
                    const int LOWEST_RATING = -10000000; // rating of good-enough blades _can_ be negative, if they have excess variable coordinates
                    int bestRating = LOWEST_RATING;

                    const double EPSILON = 1e-6; // make this user controllable??

                    foreach (G25.SMV smv in S.m_SMV)
                    {
                        int rating = 0;
                        int nbConstMatched = 0; // number of elements from M.BasisBlades matched with const coordinates
                        for (int i = 0; i < M.BasisBlades.Length; i++)
                        {
                            // get blade, see if it is constant
                            RefGA.BasisBlade B = M.BasisBlades[i];
                            bool constant = (B.symScale == null);

                            // if constant, then try constant blades first
                            bool found = false; // set to true when matching basis blade is found
                            if (constant)
                            {
                                // loop over all constant basis blades
                                for (int j = 0; j < smv.NbConstBasisBlade; j++)
                                {
                                    // get basis blade, compare bitmaps 
                                    RefGA.BasisBlade C = smv.ConstBasisBlade(j);
                                    if (C.bitmap == B.bitmap)
                                    {
                                        if (Math.Abs(C.scale - B.scale) / Math.Abs(C.scale) > EPSILON)
                                        {
                                            rating = LOWEST_RATING;
                                            break; // this break the outer loop (over 'i') too
                                        }
                                        else {
                                            rating++;
                                            nbConstMatched++;
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (rating == LOWEST_RATING) break;

                            // if no constant basis blade was found, loop over all variable basis blades
                            if (!found) 
                            {
                                for (int j = 0; j < smv.NbNonConstBasisBlade; j++)
                                {
                                    // get basis blade, compare bitmaps 
                                    RefGA.BasisBlade C = smv.NonConstBasisBlade(j);
                                    if (C.bitmap == B.bitmap)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                            }

                            if (!found)
                            {// if neither constant nor variable basis blade found: no match
                                rating = LOWEST_RATING;
                                break;
                            }
                        }

                        // disqualify 'smv' when it has const coords not present in 'M'
                        if (nbConstMatched < smv.NbConstBasisBlade) 
                            rating = LOWEST_RATING;

                        if (rating != LOWEST_RATING)
                        {
                            // adjust rating for excess variable coordinates:
                            int nbNonConstCoords = M.BasisBlades.Length - nbConstMatched;
                            int nbExcess = smv.NbNonConstBasisBlade - nbNonConstCoords;
                            if (nbExcess > 0) rating -= nbExcess; // subtract 1 for each non-const coordinate that is in 'smv' without actually being useful
                        }

                        // keep best SMV so far
                        if (rating > bestRating)
                        {
                            bestRating = rating;
                            bestSMV = smv;
                        }
                    }

                    if ((M.IsZero() || M.IsScalar()) && (bestRating < 0)) return FT; // insist on retuning a scalar type
                    else return bestSMV;

                } // end of function FindTightestMatch()

                /// <summary>
                /// The user may have requested a return type (F.ReturnTypeName). 
                /// In that case, the type with the same name is found, or an exception 
                /// is thrown if it does not exists.
                /// 
                /// Otherwise, finds the (tightest, best) return type for a function 'F' which 
                /// returning the (symbolic) value 'value'. 
                /// This  may or may not result in an SMV being found. If no 
                /// type is found and the return type is a scalar, then a floating
                /// point type may be returned.
                /// 
                /// If no SMV type is found, then a new type is created, and an exception
                /// is thrown which described the missing type.
                /// </summary>
                /// <param name="S">The specification (used to find SMVs)</param>
                /// <param name="cgd">Not used currently.</param>
                /// <param name="F">Function description (used for user-specified return type).</param>
                /// <param name="FT">Floating point type used in function.</param>
                /// <param name="value">The symbolic return value.</param>
                /// <returns>The SMV, FloatType or null if none found.</returns>
                public static G25.VariableType GetReturnType(Specification S, CGdata cgd, G25.fgs F, FloatType FT, RefGA.Multivector value)
                {
                    if ((F.ReturnTypeName != null) && (F.ReturnTypeName.Length > 0))
                    {
                        // The user specified a return type
                        G25.SMV returnSmv = S.GetSMV(F.ReturnTypeName); // is it a SMV?
                        if (returnSmv != null) return returnSmv;
                        G25.FloatType returnFT = S.GetFloatType(F.ReturnTypeName); // is it a specific float?
                        if (returnFT != null) return FT; //returnFT;
                        else if (F.ReturnTypeName == G25.Specification.XML_SCALAR) // is it a unspecified float?
                            return FT;

                        // type does not exist: abort (TODO: error message)
                        else throw new G25.UserException("Non-existant user-specified return type: " + F.ReturnTypeName,
                            S.FunctionToXmlString(F)); // non-existant return type
                    }
                    else
                    {
                        G25.VariableType RT = FindTightestMatch(S, value, FT);
                        if (RT != null) return RT;
                        else return CreateSyntheticSMVtype(S, cgd, FT, value);
                    }
                }


                public static G25.VariableType CreateSyntheticSMVtype(Specification S, CGdata cgd, FloatType FT, RefGA.Multivector value)
                {
                    // make up list of basis blades
                    rsbbp.BasisBlade[] L = new rsbbp.BasisBlade[value.BasisBlades.Length];
                    for (int i = 0 ; i < value.BasisBlades.Length; i++) 
                    {
                        RefGA.BasisBlade B = value.BasisBlades[i];
                        if (B.symScale == null) L[i] = new rsbbp.BasisBlade(new RefGA.BasisBlade(B.bitmap), B.scale); // constant value
                        else L[i] = new rsbbp.BasisBlade(new RefGA.BasisBlade(B.bitmap)); // non-const value
                    }

                    // get other required info
                    String name = "nameOfType";
                    SMV.MULTIVECTOR_TYPE mvType = SMV.MULTIVECTOR_TYPE.MULTIVECTOR;
                    String comment = "MISSING; PLEASE ADD TO SPECIFICATION";
                    //String constantName = null;

                    // create the type
                    G25.SMV synSMV = new G25.SMV(name, L, mvType, comment);

                    // throw exception
                    throw new G25.UserException("Missing specialized multivector type.\n" + 
                        "Please add the following XML to the specification to fix the dependency:\n" + 
                        S.SMVtoXmlString(synSMV));

                }


            } // end of class SpecializedReturnType
        } // end of namepace Shared
    } // end of namespace CG
} // end of namespace G25
