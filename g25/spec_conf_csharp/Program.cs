using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using c3ga;

namespace spec_conf_csharp
{
    class Program
    {
        static void Main(string[] args)
        {
		    GroupBitmap gu = GroupBitmap.GRADE_0 | GroupBitmap.GRADE_2 | GroupBitmap.GRADE_5;
		    double[] coordinates = new double[] {-20.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 0.000000001};
		    mv X = new mv(gu, coordinates);
    		
		    String str = c3ga.c3ga.String(X, "%e");
    		
            Console.WriteLine(str);
		    Console.WriteLine(X);
        }
    }
}
