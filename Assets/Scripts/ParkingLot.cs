using System;
using System.Collections.Generic;

namespace ParkingGuidance
{
    public enum SpotState { Free, Reserved, Occupied }
    public sealed class Spot
    {
        public int Id { get; private set; }
        public float X { get; private set; }
        public float Z { get; private set; }
        public SpotState State { get; internal set; }
        public string Vehicle { get; internal set; }
        public Spot(int id, float x, float z) { Id=id; X=x; Z=z; }
    }
    // Called on Unity's main thread; allocation reserves before the car starts moving.
    public sealed class ParkingLot
    {
        readonly List<Spot> spots = new List<Spot>();
        public IReadOnlyList<Spot> Spots { get { return spots.AsReadOnly(); } }
        public ParkingLot(IEnumerable<Spot> initial)
        {
            var ids = new HashSet<int>();
            foreach (var spot in initial) {
                if (!ids.Add(spot.Id)) throw new ArgumentException("Duplicate spot ID");
                spots.Add(spot);
            }
        }
        public Spot ReserveNearest(string vehicle, float entryX, float entryZ)
        {
            if (String.IsNullOrWhiteSpace(vehicle)) throw new ArgumentException("Vehicle ID required");
            foreach (var s in spots) if (s.Vehicle == vehicle) throw new InvalidOperationException("Already allocated");
            Spot best=null; float distance=float.MaxValue;
            foreach (var s in spots) {
                // Drive along central aisle, then turn perpendicular into a bay.
                float d=Math.Abs(s.X-entryX)+Math.Abs(s.Z-entryZ);
                if(s.State==SpotState.Free && d<distance) {best=s; distance=d;}
            }
            if(best!=null) {best.State=SpotState.Reserved; best.Vehicle=vehicle;}
            return best;
        }
        public bool Arrive(int id, string vehicle)
        {
            foreach(var s in spots) if(s.Id==id && s.Vehicle==vehicle && s.State==SpotState.Reserved) {
                s.State=SpotState.Occupied; return true;
            }
            return false;
        }
        public bool Release(int id, string vehicle)
        {
            foreach(var s in spots) if(s.Id==id && s.Vehicle==vehicle && s.State!=SpotState.Free) {
                s.State=SpotState.Free; s.Vehicle=null; return true;
            }
            return false;
        }
    }
}
