﻿using HydroTech.Autopilots;
using HydroTech.Managers;
using UnityEngine;
using static HydroTech.Utils.GUIUtils;

namespace HydroTech.Panels
{
    public class PanelLanding : PanelAP
    {
        #region Static properties
        private static APLanding LA => HydroFlightManager.Instance.LandingAutopilot;
        #endregion

        #region Fields
        public PanelLandingInfo panelInfo = new PanelLandingInfo();
        protected string altKeepText;
        protected string maxThrText;
        protected string stdsText;
        protected float tempAltKeep;
        protected bool tempBurnRetro;
        protected bool tempEngines;
        protected bool tempTouchdown;
        protected bool tempUseTrueAlt;
        protected bool tempVabPod;
        #endregion

        #region Properties
        private bool TempEngines
        {
            get { return this.tempEngines; }
            set
            {
                if (value != this.tempEngines) { ResetHeight(); }
                this.tempEngines = value;
            }
        }

        private bool TempTouchdown
        {
            get { return this.tempTouchdown; }
            set
            {
                if (value != this.tempTouchdown) { ResetHeight(); }
                this.tempTouchdown = value;
            }
        }

        protected override bool Settings
        {
            set
            {
                if (value != this.settings)
                {
                    if (value) //Start settings
                    {
                        this.tempBurnRetro = LA.burnRetro;
                        this.TempEngines = LA.Engines;
                        this.TempTouchdown = LA.touchdown;
                        this.tempVabPod = LA.vabPod;
                        this.stdsText = LA.safeTouchDownSpeed.ToString("#0.0");
                        this.maxThrText = (LA.maxThrottle * 100).ToString("#0.0");
                        this.tempUseTrueAlt = LA.useTrueAlt;
                        this.tempAltKeep = LA.altKeep;
                        this.altKeepText = this.tempAltKeep.ToString("#0.0");
                    }
                    else //Apply settings
                    {
                        LA.burnRetro = this.tempBurnRetro;
                        LA.Engines = this.TempEngines;
                        LA.touchdown = this.TempTouchdown;
                        LA.vabPod = this.tempVabPod;
                        LA.useTrueAlt = this.tempUseTrueAlt;
                        LA.altKeep = this.tempAltKeep;
                        float tryParse;
                        if (float.TryParse(this.stdsText, out tryParse)) { LA.safeTouchDownSpeed = tryParse; }
                        if (float.TryParse(this.maxThrText, out tryParse) && tryParse > 0 && tryParse <= 100) { LA.maxThrottle = tryParse / 100f; }
                    }
                }
                base.Settings = value;
            }
        }

        public override bool Visible
        {
            set
            {
                base.Visible = value;
                this.panelInfo.Visible = value;
            }
        }

        protected override bool Engaged
        {
            get { return LA.Engaged; }
            set { LA.Engaged = value; }
        }

        public override string Title => "Landing Autopilot";
        #endregion

        #region Constructor
        public PanelLanding() : base(new Rect(548, 80, 200, 184), GetID<PanelLanding>()) { }
        #endregion

        #region Overrides
        public override void OnFlightStart()
        {
            this.panelInfo.OnFlightStart();
        }

        public override void OnUpdate()
        {
            this.panelInfo.OnUpdate();
        }

        protected override void Window(int id)
        {
            GUI.DragWindow(this.drag);

            if (this.Settings) { DrawSettings(); }
            else
            {
                GUILayout.Label($"Pod orientation: {(LA.vabPod ? "Up" : "Horizon")}");
                GUILayout.Label($"Touchdown speed: {LA.safeTouchDownSpeed:#0.0}m/s");
                GUILayout.Label($"Use engines: {(LA.Engines ? "true" : "false")}");
                if (LA.Engines)
                {
                    GUILayout.Label($"Max throttle: {LA.maxThrottle * 100:#0.0}");
                }
                if (LA.touchdown)
                {
                    GUILayout.Label("Perform touchdown");
                    if (GUILayout.Button($"Hover at {LA.altKeep:#0.00}m {(LA.useTrueAlt ? "AGL" : "ASL")}"))
                    {
                        LA.touchdown = false;
                    }
                }
                else
                {
                    GUILayout.Label($"Hover at {LA.altKeep:#0.00}m {(LA.useTrueAlt ? "AGL" : "ASL")}");
                    GUILayout.Label($"Max allowed horizontal speed: {LA.AltKeepTrue * 0.01f:#0.0}m/s");
                    if (GUILayout.Button("Switch True/ASL"))
                    {
                        if (LA.useTrueAlt)
                        {
                            LA.altKeep = LA.AltKeepASL;
                            LA.useTrueAlt = false;
                        }
                        else
                        {
                            LA.altKeep = LA.AltKeepTrue;
                            LA.useTrueAlt = true;
                        }
                    }
                    if (GUILayout.Button("Land"))
                    {
                        LA.touchdown = true;
                        ResetHeight();
                    }
                }
                if (GUILayout.Button("Change settings"))
                {
                    this.Settings = true;
                }
                this.panelInfo.Visible = GUILayout.Toggle(this.panelInfo.Visible, "Advanced info");
                if (EngageButton(this.Engaged)) { this.Engaged = !this.Engaged; }
                GUILayout.Label("Status: " + LA.StatusString);
                GUILayout.Label(LA.WarningString, ColouredLabel(LA.WarningColor));
            }
        }

        protected override void DrawSettings()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Pod orientation: ");
            if (GUILayout.Button(this.tempVabPod ? "Up" : "Horizon"))
            {
                this.tempVabPod = !this.tempVabPod;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Touchdown speed");
            this.stdsText = GUILayout.TextField(this.stdsText);
            GUILayout.Label("m/s");
            GUILayout.EndHorizontal();
            this.TempEngines = GUILayout.Toggle(this.TempEngines, "Use engines");
            if (this.TempEngines)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Max throttle");
                this.maxThrText = GUILayout.TextField(this.maxThrText);
                GUILayout.EndHorizontal();
            }
            this.TempTouchdown = !GUILayout.Toggle(!this.TempTouchdown, "Hover at");
            if (!this.TempTouchdown)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("True Alt", this.tempUseTrueAlt ? ButtonStyle(Color.green) : Skin.button))
                {
                    if (!this.tempUseTrueAlt)
                    {
                        this.tempAltKeep -= LA.TerrainHeight;
                        if (this.tempAltKeep < APLanding.finalDescentHeight) { this.tempAltKeep = APLanding.finalDescentHeight; }
                        this.altKeepText = this.tempAltKeep.ToString("#0.0");
                    }
                    this.tempUseTrueAlt = true;
                }
                if (GUILayout.Button("ASL Alt", this.tempUseTrueAlt ? Skin.button : ButtonStyle(Color.green)))
                {
                    if (this.tempUseTrueAlt)
                    {
                        this.tempAltKeep += LA.TerrainHeight;
                        this.altKeepText = this.tempAltKeep.ToString("#0.0");
                    }
                    this.tempUseTrueAlt = false;
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                this.altKeepText = GUILayout.TextField(this.altKeepText);
                GUILayout.Label("m");
                GUILayout.EndHorizontal();
                float temp;
                if (this.tempUseTrueAlt)
                {
                    if (float.TryParse(this.altKeepText, out temp) && temp >= APLanding.finalDescentHeight)
                    {
                        this.tempAltKeep = temp;
                        float tempAltKeepAsl = this.tempAltKeep + LA.TerrainHeight;
                        GUILayout.Label($"ASL alt: {tempAltKeepAsl:#0.0}m");
                        GUILayout.Label($"Max allowed horizontal speed: {0.01f * this.tempAltKeep:#0.0}m/s");
                    }
                    else { GUILayout.Label("Invalid altitude", ColouredLabel(Color.red)); }
                }
                else
                {
                    if (float.TryParse(this.altKeepText, out temp))
                    {
                        this.tempAltKeep = temp;
                        float tempAltKeepTrue = Mathf.Max(this.tempAltKeep - LA.TerrainHeight, APLanding.finalDescentHeight);
                        GUILayout.Label($"True alt: {tempAltKeepTrue:#0.0}m");
                        GUILayout.Label($"Max allowed horizontal speed: {0.01f * tempAltKeepTrue:#0.0}m/s");
                    }
                    else { GUILayout.Label("Invalid altitude", ColouredLabel(Color.red)); }
                }
            }

            base.DrawSettings();
        }
        #endregion
    }
}