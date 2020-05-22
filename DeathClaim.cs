using UnityEngine;
using ICities;
using ColossalFramework;

namespace SuperHearseAI
{
    public class DeathClaim
    {
        public Citizen.Location location;
        public ushort buildingID;
        public uint citizenID;
        public Vector3 pos;
        public bool vehicleArriving;
    }
}