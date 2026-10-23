using System;
using ParkingGuidance;
class CoreTests {
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    static void Main(){
        var lot=new ParkingLot(new[]{new Spot(0,3,5),new Spot(1,1,5)});
        var a=lot.ReserveNearest("A",0,0); Check(a.Id==1,"Nearest path");
        var b=lot.ReserveNearest("B",0,0); Check(b.Id==0,"Reservation exclusion");
        Check(lot.ReserveNearest("C",0,0)==null,"Full lot");
        Check(!lot.Arrive(1,"B"),"Ownership");
        Check(lot.Arrive(1,"A"),"Arrival");
        Check(!lot.Release(1,"B"),"Release ownership");
        Check(lot.Release(1,"A"),"Departure");
        Check(lot.ReserveNearest("C",0,0).Id==1,"Reuse");
        bool duplicate=false;try{lot.ReserveNearest("C",0,0);}catch(InvalidOperationException){duplicate=true;}
        Check(duplicate,"Duplicate vehicle");
        Console.WriteLine("PASS: 9 parking allocation checks");
    }
}
