using System;
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
            get => Transform?.Position ?? Float2.Zero;
            protected set
            {
                if (Transform != null)
                {
                    Transform.positionX = value.X;
                    Transform.positionX = value.Y;
                }
            }
        }
        public Float2 CurPosition { get => curPosition ?? OBSPosition; protected set { curPosition = value; } }
        protected Float2? curPosition = null;

        public Float2 OBSScale
        {
            get => Transform?.Scale ?? Float2.Zero;
            protected set
            {
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
        public float OBSRotation { get => Transform?.rotation ?? 0; protected set { if (Transform != null) Transform.rotation = value; } }
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

        protected JsonObject GetCurrentTransformJson()
        {
            JsonObject transform = new JsonObject()
            {
                //{ "height", CurSize.Y},
                //{ "width", CurSize.X},
                { "positionX", CurPosition.X},
                { "positionY", CurPosition.Y},
                { "scaleX", CurScale.X},
                { "scaleY", CurScale.Y},
                { "rotation", CurRotation }
            };
            return transform;
        }

        protected string GetSceneName()
        {
            if (Parent == null)
            {
                return Name;
            }

            if (Parent?.IsScene ?? false)
            {
                return Parent.Name;
            }

            return Parent?.GetSceneName() ?? string.Empty;
        }

        public void TransformObject(
            Float2? newPos = null,
            Float2? deltaPos = null,
            Float2? newScale = null,
            Float2? deltaScale = null,
            Float2? relativeScale = null,
            float? newRotation = null,
            float deltaRotation = 0)
        {
            if (deltaPos == null)
            {
                deltaPos = Float2.Zero;
            }



            if (newScale == null)
            {
                if (relativeScale != null)
                {
                    CurScale = OBSScale * relativeScale;
                }
                else if (deltaScale != null)
                {
                    CurScale += deltaScale;
                }
            }
            else
            {
                CurScale = newScale;
            }

            CurPosition = newPos ?? CurPosition + deltaPos;
            CurRotation = newRotation ?? CurRotation + deltaRotation;
            CurSize = OBSSize * CurScale / OBSScale;
            OBSDeck.OBS.SetSceneItemTransform(SceneName, SceneItemId, GetCurrentTransformJson());
        }


        public void SetRelativeScale(float relativeScaleX = 1, float relativeScaleY = 1)
        {
            TransformObject(relativeScale: new Float2(relativeScaleX, relativeScaleY));
        }

        public void SetRelativeScale(Float2 relativeScale)
        {
            TransformObject(relativeScale: relativeScale);
        }

        public void SetSize(float? sizeX = null, float? sizeY = null)
        {
            SetRelativeScale(new Float2(sizeX ?? OBSSize.X, sizeY ?? OBSSize.Y));
        }

        public void SetSize(Float2 size)
        {
            SetRelativeScale(size / OBSSize);
        }

        public void SetRelativePosition(float deltaX = 0, float deltaY = 0)
        {
            TransformObject(newPos: OBSPosition + new Float2(deltaX, deltaY));
        }

        public void SetRelativePosition(Float2 delta)
        {
            TransformObject(newPos: OBSPosition + delta);
        }

    }
}
