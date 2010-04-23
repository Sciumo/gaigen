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

// Copyright 2008, Daniel Fontijne, University of Amsterdam -- fontijne@science.uva.nl


/**
 * This file just contains a bunch of testing code used during development.
 * */
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace RefGA
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello world");


            {
              ArrayList AL = new ArrayList();
              AL.Add(new BasisBlade(1, 1.0, "a1"));
              AL.Add(new BasisBlade(2, 1.0, "a2"));
              Multivector A = new Multivector(AL);
              System.Console.WriteLine("A = " + A.ToString());
              
              ArrayList BL = new ArrayList();
              BL.Add(new BasisBlade(2, 1.0, "b1"));
              BL.Add(new BasisBlade(3, 1.0, "b2"));
              Multivector B = new Multivector(BL);
              System.Console.WriteLine("B = " + B.ToString());
              
              Multivector AB = Multivector.gp(A, B);
              System.Console.WriteLine("AB = " + AB.ToString());

              Symbolic.HashtableSymbolicEvaluator HSE = new Symbolic.HashtableSymbolicEvaluator();
              HSE.Add("a1", 3.0);
              HSE.Add("a2", -2.0);
              HSE.Add("b1", 4.0);
              HSE.Add("b2", 1.0);
              Multivector ABeval = AB.SymbolicEval(HSE);
              System.Console.WriteLine("ABeval = " + ABeval.ToString());

                return;

            }

            Metric M;
            try
            {
                // Conformal metric (no, e1, e2, e3, ni)
                /*                int N = 5;
                                double[] EVM = new double[]{
                                    -0.707107, -0.707107, 0, 0, 0,
                                     0, 0, 0, 0, 1,
                                     0, 0, 1, 0, 0,
                                     0, 0, 0, -1, 0,
                                    -0.707107, 0.707107, 0, 0, 0};

                                double[] EV = new double[] { -1.0, 1.0, 1.0, 1.0, 1.0 };*/

                /*// Euclidean metric (e1, e2, e3)
               double[] m = new double[]{
                   1.0, 0.0, 0.0,
                   0.0, 1.0, 0.0,
                   0.0, 0.0, 1.0};*/

                // Conformal metric (no, e1, e2, e3, ni)
                double[] m = new double[]{
                    0.0, 0.0, 0.0, 0.0, -1.0, 
                    0.0, 1.0, 0.0, 0.0, 0.0,
                    0.0, 0.0, 1.0, 0.0, 0.0,
                    0.0, 0.0, 0.0, 1.0, 0.0,
                    -1.0, 0.0, 0.0, 0.0, 0.0};
                M = new Metric(m);

            }
            catch (Exception E)
            {
                System.Console.WriteLine(E.ToString());
                return;
            }

            /*{
                ArrayList AL = new ArrayList();
                AL.Add(new BasisBlade(1 << 3)); // e1
                Multivector A = new Multivector(AL);

                ArrayList BL = new ArrayList();
                BL.Add(new BasisBlade(1 << 3)); // e1
                Multivector B = new Multivector(BL);

                Multivector V = Multivector.InnerProduct(A, B, M, BasisBlade.InnerProductType.LEFT_CONTRACTION);

                System.Console.WriteLine("A = " + A.ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));
                System.Console.WriteLine("B = " + B.ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));
                System.Console.WriteLine("A . B = " + V.ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));
            }*/

     /*       ArrayList AL = new ArrayList();
            //            AL.Add(new BasisBlade(2 + 16, 1.0, "a_e1ni"));
            //          AL.Add(new BasisBlade(4 + 16, 1.0, "a_e2ni"));
            //        AL.Add(new BasisBlade(8 + 16, 1.0, "a_e3ni"));
            //            AL.Add(new BasisBlade(2 + 4, 1.0, "a_e1e2"));
            //          AL.Add(new BasisBlade(4 + 8, 1.0, "a_e2e3"));
            //        AL.Add(new BasisBlade(8 + 2, 1.0, "a_e1e3"));
            //            AL.Add(new BasisBlade(1 + 16, 1.0, "a_noni"));
            AL.Add(new BasisBlade(2 + 16, 1.0));
            AL.Add(new BasisBlade(4 + 16, 2.0));
            AL.Add(new BasisBlade(8 + 16, 3.0));
            AL.Add(new BasisBlade(2 + 4, 4.0));
            AL.Add(new BasisBlade(4 + 8, 5.0));
            AL.Add(new BasisBlade(8 + 2, 6.0));
            Multivector A = new Multivector(AL);

            Multivector Aexp = A.Exp(M, false, 0.0);

            System.Console.WriteLine("A = " + A.ToString(new string[] { "no", "e1", "e2", "e3", "ni" }) + ",");
            System.Console.WriteLine("expA = " + Aexp.ToString(new string[] { "no", "e1", "e2", "e3", "ni" }) + ",");*/


            //            Symbolic.HashtableSymbolicEvaluator HSE = new Symbolic.HashtableSymbolicEvaluator();
            //            HSE.Add("a_e1ni", 1.0);
            //          HSE.Add("a_e2ni", 2.0);
            //        HSE.Add("a_e3ni", 3.0);
            //            HSE.Add("a_e1e2", 4.0);
            //          HSE.Add("a_e2e3", 5.0);
            //        HSE.Add("a_e1e3", 6.0);
            //            HSE.Add("a_noni", 6.0);
            //          System.Console.WriteLine("A = " + A.SymbolicEval(HSE).ToString(new string[] { "no", "e1", "e2", "e3", "ni" }) + ",");
            //        System.Console.WriteLine("expA = " + Aexp.SymbolicEval(HSE).ToString(new string[] { "no", "e1", "e2", "e3", "ni" }) + ",");




            /*            // compute square
                        Multivector A2 = Multivector.gp(A, A, M);
                        System.Console.WriteLine("A2 = " + A2.ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));


                        // evaluate with random coords
                        Symbolic.RandomSymbolicEvaluator RSE = new Symbolic.RandomSymbolicEvaluator(-100.0, 100.0);
                        int pos = 0, neg = 0, zero = 0;
                        for (int i = 0; i < 1; i++)
                        {
                            Multivector MV = A2.SymbolicEval(RSE);
                            double val = MV.RealScalarPart();
                            System.Console.WriteLine("MV = " + MV);
                            System.Console.WriteLine("val = " + val);
                            if (val > 1e-7) pos++;
                            else if (val < -1e-7) neg++;
                            else zero++;
                            RSE.reset();
                        }
                        System.Console.WriteLine("pos = " + pos);
                        System.Console.WriteLine("neg = " + neg);
                        System.Console.WriteLine("zero = " + zero);*/





            /*
            ArrayList AL = new ArrayList();
            AL.Add(new BasisBlade(1 << 0, 1.0, "a_e1"));
            AL.Add(new BasisBlade(1 << 1, 1.0, "a_e2"));
            AL.Add(new BasisBlade(1 << 2, 1.0, "a_e3"));
            Multivector A = new Multivector(AL);

            ArrayList BL = new ArrayList();
            BL.Add(new BasisBlade(1 << 0, 1.0, "b_e1"));
            BL.Add(new BasisBlade(1 << 1, 1.0, "b_e2"));
            BL.Add(new BasisBlade(1 << 2, 1.0, "b_e3"));
            Multivector B = new Multivector(BL);

            System.Console.WriteLine("A = " + A.ToString());
            System.Console.WriteLine("B = " + B.ToString());
            System.Console.WriteLine("A B = " + Multivector.gp(A, B).ToString());
            */


            /*
            BasisBlade Z = new BasisBlade(1 << 0, 1.0, new Object[][] {
                new Object[] { "blah", "aaaarg", 3.0,  2.0, 1 }, 
                new Object[] { "bloooey", 'x', "crap", true, false},
                new Object[] { "blah", "aaaarg", -10.0, 1 }
            });

            System.Console.WriteLine("Z = " + Z.ToString());
            return;*/

            // OK: now check for harder objects . . . 
            /*
            Symbolic.HashtableSymbolicEvaluator HSE = new Symbolic.HashtableSymbolicEvaluator();
            HSE.Add("a_e1", 1.0);
            HSE.Add("a_e2", 2.0);
            HSE.Add("a_e3", 3.0);
            //Symbolic.RandomSymbolicEvaluator HSE = new Symbolic.RandomSymbolicEvaluator(-100.0, 100.0);


            ArrayList AL = new ArrayList();
            //AL.Add(new BasisBlade(1 << 0, 1.0, "a_no"));
            AL.Add(new BasisBlade(1 << 1, 1.0, "a_e1"));
            AL.Add(new BasisBlade(1 << 2, 1.0, "a_e2"));
            AL.Add(new BasisBlade(1 << 3, 1.0, "a_e3"));
            AL.Add(new BasisBlade(1 << 4, 1.0, "a_e1"));
            Multivector A = new Multivector(AL);
            A = Multivector.gp(A, Symbolic.ScalarOp.Exp(new Multivector(new BasisBlade(0, 1.0, "a_e2"))));
            System.Console.WriteLine("A = " + A.ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));
            // todo: check if exp, etc is working, then check why not collapse (by Multivector simplify, basis blade simplify) to single scalar value . . 
            Multivector Ae = A.SymbolicEval(HSE);
            System.Console.WriteLine("Ae = " + Ae.ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));
            return;

            ArrayList BL = new ArrayList();
//            BL.Add(new BasisBlade(1 << 0, 1.0));//, "b_no"));
            BL.Add(new BasisBlade(1 << 1, -2.0));//, "b_e1"));
            BL.Add(new BasisBlade(1 << 2, 3.0));//, "b_e2"));
            BL.Add(new BasisBlade(1 << 3, -4.0));//, "b_e3"));
   //         BL.Add(new BasisBlade(1 << 4, 1.0));//, "b_ni"));
           Multivector B = new Multivector(BL);


            A = Multivector.op(A, B);

            B = Multivector.gp(A, A);

            System.Console.WriteLine("A = " + A.ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));
            System.Console.WriteLine("B = " + B.ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));

            // how to know whether outcome of symbolic computation is positive, negative or zero???
            // simply fill in values for random variables?


            //Multivector unitA = A.Unit_r(M);
            //System.Console.WriteLine("unitA = " + unitA.ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));
            */

            /*            Multivector ApAgB = Multivector.Add(Multivector.Add(A, Multivector.gp(A, B, M)), Multivector.gp(Multivector.GetPseudoscalar(4), 0.01));

                        // still testing . . .
                        System.Console.WriteLine("ApAgB = " + ApAgB.ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));
                        int g = ApAgB.LargestGradePartIndex();

                        System.Console.WriteLine("Largest grade part = " + g);
                        System.Console.WriteLine("Largest grade part = " + ApAgB.ExtractGrade(g).ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));

                        System.Console.WriteLine("Top grade part = " + ApAgB.TopGradePartIndex(0.02));*/

            /*
            BasisBlade se1 = new BasisBlade(1 << 1, 1.0, "ae1");
            BasisBlade se2 = new BasisBlade(1 << 2, 1.0, "ae2");
            BasisBlade X = BasisBlade.op(se1, se2);
            System.Console.WriteLine("se1 = " + se1);
            System.Console.WriteLine("se2 = " + se2);
            System.Console.WriteLine("X = " + X);
            */


            /*
            Multivector no = new Multivector(new BasisBlade(1, 1.0));
            Multivector ni = new Multivector(new BasisBlade(1 << 4, 1.0));
            Multivector noni = Multivector.GeometricProduct(no, ni, M);

            System.Console.WriteLine("no = " + no.ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));
            System.Console.WriteLine("ni = " + ni.ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));
            System.Console.WriteLine("noni = " + noni.ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));
            System.Console.WriteLine("noni2 = " + noni.ExtractGrade(2).ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));
            System.Console.WriteLine("dual(noni) = " + noni.Dual(M).ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));

            System.Console.WriteLine("dual(I) = " + Multivector.GetPseudoscalar(5).Dual(M).ToString(new string[] { "no", "e1", "e2", "e3", "ni" }));
            */

            /*BasisBlade no = new BasisBlade(1);
            BasisBlade ni = new BasisBlade(1 << 4);

            System.Console.WriteLine("no = " + no);
            System.Console.WriteLine("ni = " + ni);

            System.Collections.ArrayList L = BasisBlade.GeometricProduct(ni, no, M);
            System.Console.WriteLine("L = " + BasisBlade.ToString(L, new string[] {"no", "e1", "e2", "e3", "ni"}));
            */
            /*
            BasisBlade A = new BasisBlade(1 | 4, 2.0);
            BasisBlade B = new BasisBlade(2, 2.0);
            BasisBlade C = new BasisBlade(1 | 4, 3.0);
            BasisBlade D =new BasisBlade(1, 1.0);
            System.Collections.ArrayList L = new System.Collections.ArrayList();
            L.Add(A); L.Add(B); L.Add(C); L.Add(D); L.Add(new BasisBlade(2 | 4, 2.0));

            System.Console.WriteLine("Before: ");
            foreach (BasisBlade BB in L)
            {
                System.Console.WriteLine(BB.ToString());
            }
            L = BasisBlade.Simplify(L);
            System.Console.WriteLine("After: ");
            foreach (BasisBlade BB in L)
            {
                System.Console.WriteLine(BB.ToString());
            }
            */


            //            System.Console.WriteLine("A = " + A);
            //          System.Console.WriteLine("B = " + B); // .ToString(new string[] { "no", "e1", "ni" })

            //        System.Console.WriteLine("A ^ B = " + (A ^ B));




            /*            try
                        {
                            double[][] MA = new double[][]{
                                new double[]{0.0, 0.0, 0.0, 0.0, -1.0},
                                new double[]{0.0, 1.0, 0.0, 0.0, 0.0},
                                new double[]{0.0, 0.0, 1.0, 0.0, 0.0},
                                new double[]{0.0, 0.0, 0.0, 1.0, 0.0},
                                new double[]{-1.0, 0.0, 0.0, 0.0, 0.0}};
                            new Metric(MA);
                        }
                        catch (Exception E)
                        {
                            System.Console.WriteLine(E.ToString());
                        }*/
            /*

                        double[] A = new double[]{
                            1.0, 0.0, 0.0,
                            0.0, 1.0, 0.0,
                            0.0, 0.0, 1.0};
                        int nbRows = 3;
                        int nbColumns = 3;
                        bool columnWise = false;
                        dnAnalytics.LinearAlgebra.Matrix M = dnAnalytics.LinearAlgebra.MatrixBuilder.CreateMatrix(A, nbRows, nbColumns, columnWise);

                        System.Console.WriteLine("Hi hoi!" + M);

                        */

        }
    }
}
