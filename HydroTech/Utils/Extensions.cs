﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HydroTech.Utils
{
    internal static class Extensions
    {
        #region VesselExtensions
        //From https://github.com/MuMech/MechJeb2/blob/master/MechJeb2/VesselExtensions.cs#L47-L66
        private static float lastFixedTime;
        private static readonly Dictionary<Guid, HydroJebModule> jebCache = new Dictionary<Guid, HydroJebModule>();

        public static HydroJebModule GetMasterJeb(this Vessel vessel)
        {
            if (lastFixedTime != Time.fixedTime)
            {
                jebCache.Clear();
                lastFixedTime = Time.fixedTime;
            }

            Guid vesselKey = vessel.id;
            HydroJebModule jeb;
            if (!jebCache.TryGetValue(vesselKey, out jeb))
            {
                jeb = vessel.FindPartModulesImplementing<HydroJebModule>().FirstOrDefault();
                if (jeb != null) { jebCache.Add(vesselKey, jeb); }
            }
            return jeb;
        }
        #endregion
    }
}