import C3ga_pkg.C3ga;
import C3ga_pkg.Mv;


public class Main {

    /**
    This small example creates a line and a plane from points.
    It then computes the intersection point of the line and the plane.

    In this step, all GA variables are stored in 'mv' (the multivector type).
    */
	public static void main(String[] args) {
		
        // get five points
        Mv linePt1 = new Mv(C3ga.cgaPoint(1.0f, 0.0f, 0.0f));
        Mv linePt2 = new Mv(C3ga.cgaPoint(1.0f, 1.0f, 0.0f));
        Mv planePt1 = new Mv(C3ga.cgaPoint(1.0f, 2.0f, 0.0f));
        Mv planePt2 = new Mv(C3ga.cgaPoint(1.0f, 2.0f, 1.0f));
        Mv planePt3 = new Mv(C3ga.cgaPoint(0.0f, 2.0f, 1.0f));

        // output text the can be copy-pasted into GAViewer
        System.out.println("linePt1 = " + linePt1 + ",");
        System.out.println("linePt2 = " + linePt2 + ",");
        System.out.println("planePt1 = " + planePt1 + ",");
        System.out.println("planePt2 = " + planePt2 + ",");
        System.out.println("planePt3 = " + planePt3 + ",");

        // create line and plane out of points
        Mv L = C3ga.op(linePt1, C3ga.op(linePt2, C3ga.ni));
        Mv P = C3ga.op(planePt1, C3ga.op(planePt2, C3ga.op(planePt3, C3ga.ni)));

        // output text the can be copy-pasted into GAViewer
        System.out.println("L = " + L + ",");
        System.out.println("P = " + P + ",");

        // compute intersection of line and plane
        Mv intersection = C3ga.lc(C3ga.dual(L), P);

        System.out.println("intersection = " + intersection + ",");
	}
}
