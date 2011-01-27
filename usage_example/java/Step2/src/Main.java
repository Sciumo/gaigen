import C3ga_pkg.C3ga;
import C3ga_pkg.FlatPoint;
import C3ga_pkg.Line;
import C3ga_pkg.NormalizedPoint;
import C3ga_pkg.Plane;


public class Main {

    /**
    This small example creates a line and a plane from points.
    It then computes the intersection point of the line and the plane.

    In this step, all GA variables are stored in specialized multivector types,
    such as 'line', 'plane' and 'flatPoint'.
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
        Line L = new Line(C3ga.op(linePt1, C3ga.op(linePt2, C3ga.ni)));

        Plane P = new Plane(C3ga.op(planePt1, C3ga.op(planePt2, C3ga.op(planePt3, C3ga.ni))));

        // output text the can be copy-pasted into GAViewer
        System.out.println("L = " + L + ",");
        System.out.println("P = " + P + ",");

        // compute intersection of line and plane
        FlatPoint intersection = new FlatPoint(C3ga.lc(C3ga.dual(L), P));

        System.out.println("intersection = " + intersection + ",");
	}

}
