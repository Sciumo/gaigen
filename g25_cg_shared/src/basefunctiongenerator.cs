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


            public class BaseFunctionGenerator 
            {
                /// <summary>
                /// Checks if this FunctionGenerator can implement a certain function.
                /// </summary>
                /// <param name="S">The specification of the algebra.</param>
                /// <param name="F">The function to be implemented.</param>
                /// <returns>true if 'F' can be implemented</returns>
                public virtual bool CanImplement(Specification S, G25.fgs F)
                {
                    // subclass should override
                    return false;
                }


                /// <summary>
                /// Must be called before CompleteFGS(), CheckDepencies() or WriteFunction() is called.
                /// Subclass can override, but if so, must always call superclass version of Init()
                /// </summary>
                /// <param name="S"></param>
                /// <param name="F"></param>
                /// <param name="cgd">Where the generate code goes.</param>
                public virtual void Init(Specification S, G25.fgs F, G25.CG.Shared.CGdata cgd)
                {
                    m_specification = S;
                    m_fgs = F;
                    m_cgd = cgd;
                    m_gmv = m_specification.m_GMV;
                    m_G25M = m_specification.GetMetric(m_fgs.MetricName);
                    m_M = m_G25M.m_metric;
                    m_sane = true;
                }


                /// <summary>
                /// If this FunctionGenerator can implement 'F', then this function should complete the (possible)
                /// blanks in 'F'. This means:
                ///  - Fill in F.m_returnTypeName if it is empty
                ///  - Fill in F.m_argumentTypeNames (and m_argumentVariableNames) if it is empty.
                ///  - Optionally fill in F.m_argumentPtr
                /// 
                /// As the return type is required, many functions will also compute the return value and other info 
                /// needed for generating the code inside this function. These intermediate values are then
                /// stored in class variables so they can be reused in WriteFunction()
                /// </summary>
                public virtual void CompleteFGS()
                {
                    // subclass should override
                }

                /// <summary>
                /// This function should check the dependencies of this function. If dependencies are
                /// missing, the function can complain (throw DependencyException) or fix it (add the required functions).
                /// 
                /// If changes are made to the specification then it must be locked first because
                /// multiple threads run in parallel which may all modify the specification!
                /// </summary>
                public virtual void CheckDepencies()
                {
                    // subclass should override
                }

                /// <summary>
                /// Should write the declaration/definition of 'F' to 'm_declSB', 'm_defSB' and 'm_inlineDefSB',
                /// taking into account parameters specified in specification 'S'.
                /// </summary>
                public virtual void WriteFunction()
                {
                    // subclass should override
                }

                /// <summary>
                /// This function should check the dependencies for the _testing_ code of this function. If dependencies are
                /// missing, the function should add the required functions (this is done simply by asking for them . . .).
                /// </summary>
                public virtual void CheckTestingDepencies()
                {
                    // subclass should override
                }

                /// <summary>
                /// Should write the testing function for 'F' to 'm_defSB'.
                /// 
                /// The generated function should return success (1) or failure (0).
                /// </summary>
                /// <returns>The list of name name of the int() function which tests the function.</returns>
                public virtual List<string> WriteTestFunction()
                {
                    return null; // this means no testing function
                }


                /// <summary>
                /// Entry point for new thread. Catches G25.ErrorExceptions and puts them in m_cgd.
                /// If an exception is caught, m_sane is set to false.
                /// Aborts the call when m_sane is false on entry.
                /// </summary>
                public void CompleteFGSentryPoint()
                {
                    if (!m_sane) return;
                    try
                    {
                        CompleteFGS();
                    }
                    catch (G25.UserException E)
                    {
                        ErrorDetected(E);
                    }
                }

                /// <summary>
                /// Entry point for new thread. Catches G25.ErrorExceptions and puts them in m_cgd.
                /// If an exception is caught, m_sane is set to false.
                /// Aborts the call when m_sane is false on entry.
                /// </summary>
                public void CheckDepenciesEntryPoint()
                {
                    if (!m_sane) return;
                    try
                    {
                        CheckDepencies();
                    }
                    catch (G25.UserException E)
                    {
                        ErrorDetected(E);
                    }
                }

                /// <summary>
                /// Entry point for new thread. Catches G25.ErrorExceptions and puts them in m_cgd.
                /// If an exception is caught, m_sane is set to false.
                /// Aborts the call when m_sane is false on entry.
                /// </summary>
                public void WriteFunctionEntryPoint()
                {
                    if (!m_sane) return;
                    try
                    {
                        WriteFunction();
                    }
                    catch (G25.UserException E)
                    {
                        ErrorDetected(E);
                    }
                }

                /// <summary>
                /// Entry point for new thread. Catches G25.ErrorExceptions and puts them in m_cgd.
                /// If an exception is caught, m_sane is set to false.
                /// Aborts the call when m_sane is false on entry.
                /// </summary>
                /// <returns>the name of the test function, or null if none generated.</returns>
                public void WriteTestFunctionEntryPoint()
                {
                    List<string> funcNames = null;
                    if (m_sane)
                    {
                        try
                        {
                            funcNames = WriteTestFunction();
                        }
                        catch (G25.UserException E)
                        {
                            ErrorDetected(E);
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine("Temp code (while developing C++ test templates");
                            Console.WriteLine(E.Message);
                        }
                    }
                    m_cgd.m_generatedTestFunctions = funcNames;
                }

                /// <summary>
                /// Entry point for new thread. Catches G25.ErrorExceptions and puts them in m_cgd.
                /// If an exception is caught, m_sane is set to false.
                /// Aborts the call when m_sane is false on entry.
                /// </summary>
                public void CheckTestingDepenciesEntryPoint()
                {
                    if (!m_sane) return;
                    try
                    {
                        CheckTestingDepencies();
                    }
                    catch (G25.UserException E)
                    {
                        ErrorDetected(E);
                    }
                }

                protected void ErrorDetected(G25.UserException E)
                {
                    m_sane = false; // do not continue code generation for this type
                    if (E.m_XMLerrorSource.Length == 0)
                    {
                        String XMLstring = m_specification.FunctionToXmlString(m_fgs);
                        m_cgd.AddError(new G25.UserException(E.m_message, XMLstring, E.m_filename, E.m_line, E.m_column));
                    }
                    else m_cgd.AddError(E);
                }

                /// <summary>
                /// Resets all code (any other stuff?) generated inside the <c>m_cgd</c>.
                /// This is useful when you want to recycle this FunctionGenerator,
                /// as TestSuite.GenerateCode() does.
                /// </summary>
                public void ResetCGdata()
                {
                    // reset code string buffers
                    m_cgd.ResetSB();

                }


                /// <summary>Set by Init()</summary>
                protected Specification m_specification;
                /// <summary>Set by Init()</summary>
                protected G25.fgs m_fgs;
                /// <summary>Set by Init()</summary>
                protected G25.CG.Shared.CGdata m_cgd;
                /// <summary>Set by Init(); used by so many code generator that it is handy to have a direct reference here.</summary>
                protected G25.GMV m_gmv;
                /// <summary>Set by Init(); used by so many code generator that it is handy to have a direct reference here.</summary>
                protected G25.Metric m_G25M;
                /// <summary>Set by Init(); used by so many code generator that it is handy to have a direct reference here.</summary>
                protected RefGA.Metric m_M;
                /// <summary>Whether this function generator can continue generating code. 
                /// Set to true by default, but set to false when an error occurs.
                /// </summary>
                protected bool m_sane;



            } // end of class BaseFunctionGenerator
        } // end of namespace 'Shared'
    } // end of namespace CG
} // end of namespace G25
