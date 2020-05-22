using ICities;
using ColossalFramework;
using UnityEngine;
using ColossalFramework.Plugins;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace SuperHearseAI
{
    public static class AIOverwriter
    {
        public static void OverwriteCitizenAI<T_OldAI, T_NewAI>() where T_OldAI : CitizenAI where T_NewAI : CitizenAI
        {
            for (uint i = 0; i < PrefabCollection<CitizenInfo>.LoadedCount(); i++)
            {
                CitizenInfo info;
                info = PrefabCollection<CitizenInfo>.GetLoaded(i);
                if (info == null) continue;
                //Debug.Log(info.gameObject.name);

                T_OldAI oldAI = info.gameObject.GetComponent<T_OldAI>();
                if (oldAI == null) continue;
                if (!oldAI.GetType().Equals(typeof(T_OldAI))) continue;


                T_NewAI newAI = info.gameObject.AddComponent<T_NewAI>();

                ShallowCopy(oldAI, newAI);

                oldAI.ReleaseAI();

                info.m_citizenAI = newAI;

                UnityEngine.Object.Destroy(oldAI);

                newAI.InitializeAI();
            }
        }

        public static void OverwriteVehicleAI<T_OldAI, T_NewAI>() where T_OldAI : VehicleAI where T_NewAI : VehicleAI
        {
            for (uint i = 0; i < PrefabCollection<VehicleInfo>.LoadedCount(); i++)
            {
                VehicleInfo info;
                info = PrefabCollection<VehicleInfo>.GetLoaded(i);
                if (info == null) continue;
                //Debug.Log(info.gameObject.name);

                T_OldAI oldAI = info.gameObject.GetComponent<T_OldAI>();
                if (oldAI == null) continue;
                if (!oldAI.GetType().Equals(typeof(T_OldAI))) continue;


                T_NewAI newAI = info.gameObject.AddComponent<T_NewAI>();

                ShallowCopy(oldAI, newAI);

                oldAI.ReleaseAI();

                info.m_vehicleAI = newAI;

                UnityEngine.Object.Destroy(oldAI);

                newAI.InitializeAI();
                Debug.Log("VEHICLE AI REPLACED");
            }
        }

        private static void ShallowCopy(object source, object destination)
        {
            var sourceFields = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).ToDictionary(f => f.Name, f => f);
            var destinationFields = destination.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).ToDictionary(f => f.Name, f => f);

            foreach (var sourceField in sourceFields)
            {
                FieldInfo destinationField;
                if (destinationFields.TryGetValue(sourceField.Key, out destinationField))
                {
                    destinationField.SetValue(destination, sourceField.Value.GetValue(source));
                }
            }
        }
    }
}