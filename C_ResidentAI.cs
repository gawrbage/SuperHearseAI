using UnityEngine;
using ICities;
using ColossalFramework;

namespace SuperHearseAI
{
    public class C_ResidentAI : ResidentAI
    {
        public override void SimulationStep(uint citizenID, ref Citizen data)
        {
            base.SimulationStep(citizenID, ref data);

            /* TODO:
            remove lock statements as they may not be useful
            also refactor code
            */

            switch (data.CurrentLocation)
            {
                case Citizen.Location.Home:
                    if (data.m_homeBuilding == 0) { return; }
                    break;

                case Citizen.Location.Moving:
                    if (data.m_vehicle == 0) { return; }
                    break;

                case Citizen.Location.Visit:
                    if (data.m_visitBuilding == 0) { return; }
                    break;

                case Citizen.Location.Work:
                    if (data.m_workBuilding == 0) { return; }
                    break;

                default: return;
            }

            ushort citizenBuilding = 0;

            switch (data.CurrentLocation)
            {
                case Citizen.Location.Home:
                    citizenBuilding = data.m_homeBuilding;
                    break;

                case Citizen.Location.Visit:
                    citizenBuilding = data.m_visitBuilding;
                    break;

                case Citizen.Location.Work:
                    citizenBuilding = data.m_workBuilding;
                    break;

                default: break;
            }

            Vector3 p = Singleton<BuildingManager>.instance.m_buildings.m_buffer[citizenBuilding].m_position;
            byte currentDistrict = Singleton<DistrictManager>.instance.GetDistrict(p);

            if (data.Dead && (DeathRegistry.IsRegistered(citizenBuilding) == false))
            {
                DeathRegistry.claims[citizenBuilding] = new DeathClaim();
                DeathClaim claim = DeathRegistry.claims[citizenBuilding];
                claim.buildingID = citizenBuilding;
                claim.citizenID = citizenID;
                claim.pos = p;
                claim.location = data.CurrentLocation;

                //Debug.Log("Died At : " + Singleton<BuildingManager>.instance.m_buildings.m_buffer[citizenBuilding].Info.name);

                DeathRegistry.SubmitDeathClaim(currentDistrict, claim);

                return;
            }

            if (data.Dead && DeathRegistry.IsRegistered(citizenBuilding) && DeathRegistry.GetBuildingDistrict(citizenBuilding) != currentDistrict)
            {
                DeathClaim claim = DeathRegistry.claims[citizenBuilding];
                if (!claim.vehicleArriving)
                {
                    DeathRegistry.RecallDeathClaim(DeathRegistry.GetBuildingDistrict(claim.buildingID), claim);

                    DeathRegistry.claims[citizenBuilding] = new DeathClaim();
                    claim = DeathRegistry.claims[citizenBuilding];
                    claim.buildingID = citizenBuilding;
                    claim.citizenID = citizenID;
                    claim.pos = p;
                    claim.location = data.CurrentLocation;

                    DeathRegistry.SubmitDeathClaim(currentDistrict, claim);

                    return;
                }
            }
        }
    }
}