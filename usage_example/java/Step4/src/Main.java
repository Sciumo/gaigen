import C3ga_pkg.C3ga;
import C3ga_pkg.FlatPoint;
import C3ga_pkg.Line;
import C3ga_pkg.NormalizedPoint;
import C3ga_pkg.Plane;


public class Main {

    /*
    This small example creates a line and a plane from points.
    It then computes the intersection point of the line and the plane.

    In this step, the 'reportUsage' functionality is used to extract what
    specialized functions and types are missing.
    */
	public static void main(String[] args) {
		
        // get five points
        NormalizedPoint linePt1 = C3ga.cgaPoint(1.0f, 0.0f, 0.0f);
        NormalizedPoint linePt2 = C3ga.cgaPoint(1.0f, 1.0f, 0.0f);
        NormalizedPoint planePt1 = C3ga.cgaPoint(1.0f, 2.0f, 0.0f);
        NormalizedPoint planePt2 = C3ga.cgaPoint(1.0f, 2.0f, 1.0f);
        NormalizedPoint planePt3 = C3ga.cgaPoint(0.0f, 2.0f, 1.0f);

        // output text the can be copy-pasted into GAViewer
        System.out.println("linePt1 = " + linePt1 + ",");
        System.out.println("linePt2 = " + linePt2 + ",");
        System.out.println("planePt1 = " + planePt1 + ",");
        System.out.println("planePt2 = " + planePt2 + ",");
        System.out.println("planePt3 = " + planePt3 + ",");

        // create line and plane out of points
        Line L = C3ga.op(linePt1, C3ga.op(linePt2, C3ga.ni));

        Plane P = C3ga.op(planePt1, C3ga.op(planePt2, C3ga.op(planePt3, C3ga.ni)));

        // output text the can be copy-pasted into GAViewer
        System.out.println("L = " + L + ",");
        System.out.println("P = " + P + ",");

        // compute intersection of line and plane
        FlatPoint intersection = C3ga.lc(C3ga.dual(L), P);
    	NormalizedPoint intersectionPt = C3ga.cgaPoint(intersection); // example of how to convert to regular CGA point

        System.out.println("intersection = " + intersection + ",");
	}

}
