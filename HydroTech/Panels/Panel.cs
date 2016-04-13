﻿using UnityEngine;

namespace HydroTech.Panels
{
    public abstract class Panel
    {
        #region Fields
        protected Rect window;
        protected readonly Rect drag;
        private readonly int id;
        private readonly string title;
        #endregion

        #region Properties
        protected virtual string MinimizedTitle
        {
            get { return string.Empty; }
        }

        protected virtual bool Minimized
        {
            get { return false; }
        }

        public virtual bool Visible { get; set; }
        #endregion

        #region Constructors
        protected Panel(Rect window, int id, string title)
        {
            this.window = window;
            this.drag = new Rect(0, 0, this.window.width, 30);
            this.id = id;
            this.title = title;
        }
        #endregion

        #region Methods
        public void ResetHeight()
        {
            this.window.height = 0;
        }

        internal void DrawGUI()
        {
            this.window = KSPUtil.ClampRectToScreen(GUILayout.Window(this.id, this.window, Window, this.Minimized ? this.MinimizedTitle : this.title));
        }
        #endregion

        #region Abstract Methods
        protected abstract void Window(int id);
        #endregion

        #region Virtual methods
        public virtual void OnFlightStart()
        {
            this.Visible = false;
        }

        public virtual void OnUpdate() { }
        #endregion
    }
}