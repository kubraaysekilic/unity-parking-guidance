using System.Collections.Generic;
using UnityEngine;
using ParkingGuidance;

public sealed class ParkingDemo : MonoBehaviour
{
    sealed class Car { public GameObject View; public Spot Spot; public string Id; public int Stage; public float Stay; }
    ParkingLot lot;
    readonly List<Car> cars = new List<Car>();
    readonly List<Renderer> bays = new List<Renderer>();
    int serial, rejected;
    float nextSpawn;
    bool automatic=true;
    Material free, reserved, occupied, vehicle, asphalt;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot() { if(FindFirstObjectByType<ParkingDemo>()==null) new GameObject("Parking simulation").AddComponent<ParkingDemo>(); }
    Material Mat(Color color) {
        var shader=Shader.Find("Universal Render Pipeline/Unlit");
        if(shader==null) shader=Shader.Find("Unlit/Color");
        var material=new Material(shader); material.color=color; return material;
    }
    GameObject Box(string title, Vector3 position, Vector3 scale, Material material) {
        var obj=GameObject.CreatePrimitive(PrimitiveType.Cube); obj.name=title;
        obj.transform.position=position; obj.transform.localScale=scale;
        obj.GetComponent<Renderer>().sharedMaterial=material; return obj;
    }
    void Start() {
        free=Mat(new Color(.2f,.8f,.6f)); reserved=Mat(new Color(1,.7f,.2f));
        occupied=Mat(new Color(.9f,.3f,.4f)); vehicle=Mat(new Color(.3f,.6f,1)); asphalt=Mat(new Color(.08f,.12f,.2f));
        Box("Ground",Vector3.zero,new Vector3(36,.1f,17),asphalt);
        var spots=new List<Spot>();
        for(int row=0;row<2;row++) for(int i=0;i<8;i++) {
            var s=new Spot(spots.Count,-12+i*3.4f,row==0 ? -5 : 5); spots.Add(s);
            bays.Add(Box("Bay "+s.Id,new Vector3(s.X,.15f,s.Z),new Vector3(2.5f,.1f,3.5f),free).GetComponent<Renderer>());
        }
        lot=new ParkingLot(spots);
        var cam=Camera.main;
        if(cam==null) cam=new GameObject("Camera").AddComponent<Camera>();
        cam.transform.position=new Vector3(0,28,-14); cam.transform.LookAt(Vector3.zero);
        cam.orthographic=true; cam.orthographicSize=20;
        cam.clearFlags=CameraClearFlags.SolidColor; cam.backgroundColor=new Color(.04f,.07f,.12f);
    }
    void Spawn() {
        string id="CAR-"+(++serial);
        var spot=lot.ReserveNearest(id,-17,0);
        if(spot==null) {rejected++; return;}
        cars.Add(new Car {Id=id,Spot=spot,Stay=Random.Range(12f,24f),View=Box(id,new Vector3(-17,.7f,0),new Vector3(1.8f,1,1),vehicle)});
    }
    void Update() {
        if(lot==null) return;
        nextSpawn-=Time.deltaTime;
        if(automatic && nextSpawn<=0) {Spawn(); nextSpawn=2;}
        for(int i=cars.Count-1;i>=0;i--) {
            var c=cars[i];
            if(c.Stage==2) { c.Stay-=Time.deltaTime; if(c.Stay<=0)c.Stage=3; continue; }
            var target=c.Stage==0 || c.Stage==3 ? new Vector3(c.Spot.X,.7f,0) : c.Stage==1 ? new Vector3(c.Spot.X,.7f,c.Spot.Z) : new Vector3(17,.7f,0);
            c.View.transform.position=Vector3.MoveTowards(c.View.transform.position,target,5*Time.deltaTime);
            if(Vector3.Distance(c.View.transform.position,target)<.05f) {
                c.Stage++;
                if(c.Stage==2)lot.Arrive(c.Spot.Id,c.Id);
                if(c.Stage==4)lot.Release(c.Spot.Id,c.Id);
                if(c.Stage==5){Destroy(c.View); cars.RemoveAt(i);}
            }
        }
        foreach(var s in lot.Spots) bays[s.Id].sharedMaterial=s.State==SpotState.Free?free:s.State==SpotState.Reserved?reserved:occupied;
    }
    void OnGUI() {
        if(lot==null) return;
        int f=0,r=0,o=0;
        foreach(var s in lot.Spots) {if(s.State==SpotState.Free)f++; else if(s.State==SpotState.Reserved)r++; else o++;}
        GUI.Box(new Rect(18,18,340,190),"PARKING / LIVE GUIDANCE");
        GUI.Label(new Rect(32,50,310,30),"Free: "+f+"  Reserved: "+r+"  Occupied: "+o);
        GUI.Label(new Rect(32,80,310,30),"Rejected (full): "+rejected+" | Capacity: 16");
        automatic=GUI.Toggle(new Rect(32,110,280,25),automatic,"Automatic arrivals");
        if(GUI.Button(new Rect(32,145,140,32),"Add vehicle"))Spawn();
        GUI.Label(new Rect(18,215,500,30),"Green = free   Amber = reserved   Red = occupied");
    }
    void OnDestroy() {foreach(var m in new[]{free,reserved,occupied,vehicle,asphalt}) if(m!=null)Destroy(m);}
}
