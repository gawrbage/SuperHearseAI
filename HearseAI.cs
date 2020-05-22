using System;
using ICities;
using ColossalFramework;
using UnityEngine;

namespace SuperHearseAI
{
    public class C_HearseAI : HearseAI
    {
        private static DeathClaim[] currentClaim;
        private static byte[] districtNo = new byte[ushort.MaxValue+1];

        public override bool ArriveAtDestination(ushort vehicleID, ref Vehicle vehicleData)
        {
            try
            {
                if (currentClaim[vehicleID] != null)
                {
                    DeathRegistry.RestClaim(currentClaim[vehicleID]);
                }
            } catch (Exception E) { }

            try
            {
                currentClaim[vehicleID] = null;
            } catch (Exception E) { }

            return base.ArriveAtDestination(vehicleID, ref vehicleData);
        }

        public override void CreateVehicle(ushort vehicleID, ref Vehicle data)
        {
            districtNo[vehicleID] = Singleton<DistrictManager>.instance.GetDistrict(Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_sourceBuilding].m_position);

            if (currentClaim == null) currentClaim = new DeathClaim[ushort.MaxValue+1];

            base.CreateVehicle(vehicleID, ref data);
        }

        public override void ReleaseVehicle(ushort vehicleID, ref Vehicle data)
        {
            //check if currentclaim is null
            try
            {
                if (currentClaim[vehicleID] != null)
                {
                    DeathRegistry.SubmitDeathClaim(districtNo[vehicleID], currentClaim[vehicleID]);
                }
            }
            catch (Exception E) { }

            base.ReleaseVehicle(vehicleID, ref data);
        }

        public override void StartTransfer(ushort vehicleID, ref Vehicle data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if ((data.m_flags & Vehicle.Flags.TransferToTarget) != 0)
            {
                base.StartTransfer(vehicleID, ref data, material, offer);
                return;
            }
            
            Debug.Log("Starting Transfer");
            try
            {
                districtNo[vehicleID] = Singleton<DistrictManager>.instance.GetDistrict(Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_sourceBuilding].m_position);
                currentClaim[vehicleID] = DeathRegistry.GetDeathClaim(districtNo[vehicleID], data.GetLastFramePosition());
            } catch (Exception E) { return; }
            
            if (currentClaim[vehicleID] == null) return;

            Debug.Log("Successful!");

            TransferManager.TransferOffer betterOffer = new TransferManager.TransferOffer();

            betterOffer.Active = true;
            betterOffer.Amount = 0;
            betterOffer.Building = currentClaim[vehicleID].buildingID;
            betterOffer.Citizen = currentClaim[vehicleID].citizenID;

            base.StartTransfer(vehicleID, ref data, material, betterOffer);
        }
    }
}