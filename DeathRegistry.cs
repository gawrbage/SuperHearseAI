using UnityEngine;
using ICities;
using ColossalFramework;
using System.Collections.Generic;
using System;

namespace SuperHearseAI
{
    public static class DeathRegistry
    {
        private static List<DeathList> districts = new List<DeathList>(256);
        public static DeathClaim[] claims;
        private static bool[] registeredBuildings;
        private static byte[] registeredBuildingsDistrict;
        private static bool initialized = false;

        private static readonly object locker;

        public static void Initialize()
        {
            try { if (initialized) return; } catch (Exception E) { }
            for (uint i = 0; i < districts.Capacity; i++)
            {
                districts.Add(new DeathList());
            }
            claims = new DeathClaim[ushort.MaxValue + 1];
            registeredBuildings = new bool[ushort.MaxValue + 1];
            registeredBuildingsDistrict = new byte[ushort.MaxValue + 1];
            initialized = true;
        }

        public static DeathClaim GetDeathClaim(byte districtNo, Vector3 pos)
        {
            return districts[districtNo].GetClosestAndPop(pos);
        }

        public static void RecallDeathClaim(byte districtNo, DeathClaim claim)
        {
            districts[districtNo].FindAndDelete(claim);
            claims[claim.buildingID] = null;
            registeredBuildings[claim.buildingID] = false;
            registeredBuildingsDistrict[claim.buildingID] = 0;
        }

        public static bool IsRegistered(ushort buildingID)
        {
            return registeredBuildings[buildingID];
        }

        public static byte GetBuildingDistrict(ushort buildingID)
        {
            return registeredBuildingsDistrict[buildingID];
        }

        public static void RestClaim(DeathClaim claim)
        {
            registeredBuildings[claim.buildingID] = false;
            registeredBuildingsDistrict[claim.buildingID] = 0;
            claims[claim.buildingID] = null;
        }

        public static void SubmitDeathClaim(byte districtNo, DeathClaim claim)
        {
            districts[districtNo].Add(claim);
            claims[claim.buildingID] = claim;
            registeredBuildings[claim.buildingID] = true;
            registeredBuildingsDistrict[claim.buildingID] = districtNo;
        }
    }
}