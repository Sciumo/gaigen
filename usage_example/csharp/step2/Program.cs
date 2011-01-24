using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using c3ga_ns;

namespace c3ga_csharp_example_step2
{
    class Program
    {
        /*
        This small example creates a line and a plane from points.
        It then computes the intersection point of the line and the plane.

        In this step, all GA variables are stored in specialized multivector types,
        such as 'line', 'plane' and 'flatPoint'.
        */
        static void Main(string[] args)
        {
            // get five points
            normalizedPoint linePt1 = c3ga.cgaPoint(1.0f, 0.0f, 0.0f);
            normalizedPoint linePt2 = c3ga.cgaPoint(1.0f, 1.0f, 0.0f);
            normalizedPoint planePt1 = c3ga.cgaPoint(1.0f, 2.0f, 0.0f);
            normalizedPoint planePt2 = c3ga.cgaPoint(1.0f, 2.0f, 1.0f);
            normalizedPoint planePt3 = c3ga.cgaPoint(0.0f, 2.0f, 1.0f);

            // output text the can be copy-pasted into GAViewer
            Console.WriteLine("linePt1 = " + linePt1 + ",");
            Console.WriteLine("linePt2 = " + linePt2 + ",");
            Console.WriteLine("planePt1 = " + planePt1 + ",");
            Console.WriteLine("planePt2 = " + planePt2 + ",");
            Console.WriteLine("planePt3 = " + planePt3 + ",");

            // create line and plane out of points
            line L = new line((mv)linePt1 ^ (mv)linePt2 ^ (mv)c3ga.ni);
            //line L = new line(c3ga.op(linePt1, c3ga.op(linePt2, c3ga.ni))); // alternative, no explicit conversion required

            plane P = new plane((mv)planePt1 ^ (mv)planePt2 ^ (mv)planePt3 ^ (mv)c3ga.ni);
            //plane P = new plane(c3ga.op(planePt1, c3ga.op(planePt2, c3ga.op(planePt3, c3ga.ni)))); // alternative, no explicit conversion required

            // output text the can be copy-pasted into GAViewer
            Console.WriteLine("L = " + L + ",");
            Console.WriteLine("P = " + P + ",");

            // compute intersection of line and plane
            flatPoint intersection = new flatPoint(c3ga.lc(c3ga.dual(L), P));

            Console.WriteLine("intersection = " + intersection + ",");
        }
    }
}
