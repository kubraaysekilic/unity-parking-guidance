# Parking Guidance & Occupancy Tracking

Unity C# simulation: 16 spaces, automatic or manual arrivals, shortest aisle-route
allocation, immediate reservations, animated parking and timed departure.

## Open in Unity
1. Install Unity 6 using Unity Hub and create a **3D Core** project.
2. Copy this repository's `Assets/Scripts` folder into that project's `Assets`.
3. Open any empty scene and press Play. The bootstrap creates the complete demo.
4. Toggle automatic arrivals or click Add vehicle. Green/amber/red show state.
5. For a standalone build, save the scene and add it to Build Profiles.

This is a source import package; generated Unity project files are deliberately
excluded. No paid assets, prefab wiring or scene downloads are required.

## Model
`Free → Reserved → Occupied → Free`. Reservation happens before motion, preventing
two arrivals from taking one space. Release requires the owning vehicle ID and
happens after the departing car clears its bay. Full-lot arrivals are rejected
and counted. Distances use the central aisle plus perpendicular bay access.

## Tests
`dotnet run --project Tests/CoreTests.csproj` (requires .NET 8 SDK).
The pure C# model can be tested independently of Unity.

## Limits
Vehicles are kinematic: they may overlap in the aisle. This is occupancy and
guidance simulation, not collision-safe traffic planning. Real cameras/sensors,
multi-floor routing, obstacle avoidance and persistent telemetry are future work.
This implementation is newly created; the original description's 2025 date is not
claimed as the creation date of this code.

## Reference
[Unity runtime initialization](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/runtimeinitializeonloadmethodattribute)
