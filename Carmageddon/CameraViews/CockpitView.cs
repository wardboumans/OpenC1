﻿using System;
using System.Collections.Generic;
using System.Text;
using Carmageddon.Parsers;
using PlatformEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NFSEngine;
using StillDesign.PhysX;
using Carmageddon.HUD;
using System.Diagnostics;
using System.IO;

namespace Carmageddon.CameraViews
{
    class CockpitView : BaseHUDItem, ICameraView
    {
        CockpitFile _cockpitFile;
        SimpleCamera _camera;
        CActorHierarchy _actors;
        Vehicle _vehicle;

        public CockpitView(Vehicle vehicle, string cockpitFile)
        {
            _vehicle = vehicle;
            if (!File.Exists(cockpitFile))
            {
                cockpitFile = Path.GetDirectoryName(cockpitFile) + "\\blkeagle.txt";
            }
            _cockpitFile = new CockpitFile(cockpitFile);
            _camera = new SimpleCamera();
            _camera.FieldOfView = MathHelper.ToRadians(55.55f);
            _camera.AspectRatio = Engine.AspectRatio;

            DatFile modelsFile = new DatFile(GameVars.BasePath + "data\\models\\" + vehicle.Config.BonnetModelFile);
            ActFile actFile = new ActFile(GameVars.BasePath + "data\\actors\\" + vehicle.Config.BonnetActorFile, modelsFile.Models);
            _actors = actFile.Hierarchy;
            _actors.ResolveTransforms(false, null);

            //move head back
            _vehicle.Config.DriverHeadPosition.Z += 0.11f;

            foreach (var x in _cockpitFile.LeftHands)
            {
                x.Position1 += new Vector2(-20, 0);
                x.Position2 += new Vector2(-20, 0);
                x.Position1 /= new Vector2(640, 480);
                x.Position2 /= new Vector2(640, 480);
            }
            foreach (var x in _cockpitFile.RightHands)
            {
                x.Position1 += new Vector2(-20, 0);
                x.Position2 += new Vector2(-20, 0);
                x.Position1 /= new Vector2(640, 480);
                x.Position2 /= new Vector2(640, 480);
            }
            _cockpitFile.CenterHands.Position1 += new Vector2(-20, 0);
            _cockpitFile.CenterHands.Position2 += new Vector2(-20, 0);
            _cockpitFile.CenterHands.Position1 /= new Vector2(640, 480);
            _cockpitFile.CenterHands.Position2 /= new Vector2(640, 480);

        }

        #region ICameraView Members

        public bool Selectable
        { 
            get { return true; }
        }

        public override void Update()
        {
            Matrix m = Matrix.CreateRotationX(-0.08f) * _vehicle.Chassis.Actor.GlobalOrientation;
            Vector3 forward = m.Forward;
            _camera.Orientation = forward;

            _camera.Up = m.Up;
                        
            _camera.Position = _vehicle.GetBodyBottom() + Vector3.Transform(_vehicle.Config.DriverHeadPosition, _vehicle.Chassis.Actor.GlobalOrientation) + new Vector3(0, 0.018f, 0);

            Engine.Camera = _camera;
        }

        public override void Render()
        {
            Rectangle src = new Rectangle(32, 20, 640, 480);
            Rectangle rect = new Rectangle(0, 0, 800, 600);
            Engine.SpriteBatch.Draw(_cockpitFile.Forward, rect, src, Color.White);

            float steerRatio = _vehicle.Chassis.SteerRatio;
            
            CockpitHandFrame frame = null;
            if (steerRatio < -0.2)
            {
                if (steerRatio < -0.8f)
                {
                    int hands = Math.Min(2, _cockpitFile.RightHands.Count - 1);
                    frame = _cockpitFile.RightHands[hands];
                }
                else if (steerRatio < -0.5f)
                    frame = _cockpitFile.RightHands[1];
                else if (steerRatio < -0.2f)
                    frame = _cockpitFile.RightHands[0];
                
            }
            else if (steerRatio > 0.2f)
            {
                if (steerRatio > 0.8f)
                {
                    int hands = Math.Min(2, _cockpitFile.LeftHands.Count - 1);
                    frame = _cockpitFile.LeftHands[hands];
                }
                else if (steerRatio > 0.5f)
                    frame = _cockpitFile.LeftHands[1];
                else if (steerRatio > 0.2)
                    frame = _cockpitFile.LeftHands[0];
            }
            else
            {
                frame = _cockpitFile.CenterHands;
            }

            if (frame.Texture1 != null)
                Engine.SpriteBatch.Draw(frame.Texture1, ScaleVec2(frame.Position1), Color.White);
            if (frame.Texture2 != null)
                Engine.SpriteBatch.Draw(frame.Texture2, ScaleVec2(frame.Position2), Color.White);
            
            _actors.Render(Matrix.CreateFromQuaternion(_vehicle.Chassis.Actor.GlobalOrientationQuat) * Matrix.CreateTranslation(_vehicle.GetBodyBottom()), null);
        }

        public void Activate()
        {
            Engine.Camera = _camera;
            _camera.Position = _vehicle.GetBodyBottom() + Vector3.Transform(_vehicle.Config.DriverHeadPosition, _vehicle.Chassis.Actor.GlobalOrientation);
            _camera.Update(); 
        }

        public void Deactivate()
        {
            
        }

        #endregion

        }
    }
