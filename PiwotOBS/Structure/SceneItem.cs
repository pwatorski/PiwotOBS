﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NAudio.SoundFont;
using PiwotOBS.PMath;

namespace PiwotOBS.Structure
{

    public partial class SceneItem
    {
        public Float2 OBSPosition 
        { 
            get=>Transform?.Position??Float2.Zero; 
            protected set { 
                if (Transform != null) 
                { 
                    Transform.positionX = value.X; 
                    Transform.positionX = value.Y; 
                } 
            } 
        }
        public Float2 CurPosition { get => curPosition??OBSPosition; protected set { curPosition = value; } }
        protected Float2? curPosition = null;

        public Float2 OBSScale
        { 
            get=>Transform?.Scale??Float2.Zero; 
            protected set { 
                if (Transform != null) 
                { 
                    Transform.scaleX = value.X; 
                    Transform.scaleY = value.Y; 
                }
            } 
        }
        public Float2 CurScale { get => curScale ?? OBSScale; protected set { curScale = value; } }
        protected Float2? curScale = null;
        public Float2 OBSSize
        {
            get => Transform?.Size ?? Float2.Zero;
            protected set
            {
                if (Transform != null)
                {
                    Transform.width = value.X;
                    Transform.height = value.Y;
                }
            }
        }
        public Float2 CurSize { get => curSize ?? OBSSize; protected set { curSize = value; } }
        protected Float2? curSize = null;
        public float OBSRotation { get=> Transform?.rotation??0; protected set { if (Transform != null) Transform.rotation=value; } }
        public float CurRotation { get; protected set; }
        public SceneItem? Parent { get; protected set; }

        public string SceneName { get; protected set; } = String.Empty;

        internal void ClearParent()
        {
            Parent = null;
        }

        internal void SetParent(SceneItem parent) 
        {
            Parent = parent;
            SceneName = GetSceneName();

        }

        protected void Init()
        {
            //CurPosition = OBSPosition;
            //CurRotation = OBSRotation;
            //CurPosition = OBSPosition;
        }

        protected string GetSceneName()
        {
            if(Parent == null)
            {
                return Name;
            }

            if(Parent?.IsScene??false)
            {
                return Parent.Name;
            }

            return Parent?.GetSceneName()??string.Empty;
        }

        public void TransformObject(
            Float2? newPos = null,
            Float2? deltaPos = null,
            Float2? newScale = null,
            Float2? deltaScale = null,
            Float2? relativeScale = null,
            float? newRotation=null,
            float deltaRotation=0)
        {
            if(deltaPos == null)
            {
                deltaPos = Float2.Zero;
            }
            if(newPos == null)
            {
                newPos = deltaPos + CurPosition;
            }

            if(newScale == null)
            {
                if (relativeScale != null)
                {
                    newScale = OBSScale * relativeScale;
                }
                else if (deltaScale != null)
                {
                    newScale = deltaScale + CurScale;
                }
            }

            if(newRotation == null)
            {
                newRotation = CurRotation + deltaRotation;
            }

            
        }
            

        public void ScaleObjectRelative(float relativeScaleX=1, float relativeScaleY=1)
        {

        }
    }
}
