﻿using HydroTech.Utils;
using UnityEngine;

namespace HydroTech.Panels
{
    public abstract class Panel
    {
        #region Fields
        protected Rect window, drag;
        private readonly int id;
        private readonly string title;
        #endregion

        #region Properties
        public virtual bool Visible { get; set; }
        #endregion

        #region Abstract properties
        public abstract string Title { get; }
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
        public void ResetHeight() => this.window.height = 0;

        internal void Draw() => this.window = GUIUtils.ClampedWindow(this.id, this.window, Window, this.title);
        #endregion

        #region Abstract Methods
        protected abstract void Window(int id);
        #endregion

        #region Virtual methods
        public virtual void OnFlightStart() { }

        public virtual void OnUpdate() { }
        #endregion
    }
}