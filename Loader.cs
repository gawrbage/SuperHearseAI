using System;
using ICities;
using ColossalFramework;
using UnityEngine;
using ColossalFramework.Plugins;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace SuperHearseAI
{
    public class TestMod : IUserMod
    {
        public string Name { get { return "Super Hearse AI: Redefining Deathcare"; } }

        public string Description { get { return "(Optimizes hearse routes & only serves buildings within the district)"; } }
    }


    public class Loader : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            DeathRegistry.Initialize();

            AIOverwriter.OverwriteCitizenAI<ResidentAI, C_ResidentAI>();
            AIOverwriter.OverwriteVehicleAI<HearseAI, C_HearseAI>();
        }
    }
}
