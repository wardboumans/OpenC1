﻿using System;
using System.Collections.Generic;
using System.Text;
using Carmageddon.CameraViews;
using PlatformEngine;
using Microsoft.Xna.Framework.Input;

namespace Carmageddon.EditModes
{
    class NormalMode : GameMode
    {
        int _currentView = 0;
        List<ICameraView> _views = new List<ICameraView>();

        public NormalMode()
        {
            _views.Add(new ChaseView(Race.Current.PlayerVehicle));
            _views.Add(new CockpitView(Race.Current.PlayerVehicle, GameVars.BasePath + @"data\64x48x8\cars\" + Race.Current.PlayerVehicle.Config.FileName));
            _views.Add(new FlyView(Race.Current.PlayerVehicle));
            _views.Add(new RaceCompletedView(Race.Current.PlayerVehicle));
            _views[_currentView].Activate();
        }

        public override void Activate()
        {
            _views[_currentView].Activate();
        }

        public override void Update()
        {
            if (Engine.Input.WasPressed(Keys.C))
            {
                _views[_currentView].Deactivate();
                _currentView = (_currentView + 1) % _views.Count;
                _views[_currentView].Activate();
            }

            _views[_currentView].Update();
        }

        public override void Render()
        {
            _views[_currentView].Render();
        }
    }
}
