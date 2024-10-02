using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
            get => new Float2(Transform?.Position);
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
            get => new Float2(Transform?.Scale);
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
            get => new Float2(Transform?.Size);
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

        public bool CurEnabled { get; protected set; }

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

        public SceneItemTransform GetCurrentOBSTransform()
        {
            var objectJson = GetCurrentOBSJson()?.AsObject() ?? throw new Exception($"No transform information for \"{Name}\"");

            return SceneItemTransform.FromJson(objectJson) ?? throw new Exception($"Could not deserialize transform for \"{Name}\"");
        }
        public void UpdateFromOBS()
        {
            OBSEnabled = CurEnabled = GetCurrentOBSEnabled();
            var OBSTransform = GetCurrentOBSTransform() ?? throw new Exception($"Could not prepare item \"{Name}\" transform!");
            if (Transform == null) throw new Exception($"Item \"{Name}\" has no transform!");
            Transform.UpdateFrom(OBSTransform);
            ResetCurToOBS();
        }

        public void ResetCurToOBS()
        {
            curScale = OBSScale;
            curPosition = OBSPosition;
            curSize = OBSSize;
            CurRotation = OBSRotation;
        }

        public void ResetOBSToCur()
        {
            TransformObject(newPos:CurPosition, newScale:CurScale, newRotation:CurRotation);
        }

        public void OverrideFromSave(string savePath)
        {
            using StreamReader sr = new StreamReader(savePath, encoding:Encoding.UTF8);
            var loadedObject = FromJson(sr.ReadToEnd());

            if (loadedObject != null && loadedObject.Transform != null)
                Transform?.UpdateFrom(loadedObject.Transform);
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

        public void Enable()
        {
            CurEnabled = true;
            OBSDeck.OBS.SetSceneItemEnabled(SceneName, SceneItemId, true);
        }

        public void Disable()
        {
            CurEnabled = false;
            OBSDeck.OBS.SetSceneItemEnabled(SceneName, SceneItemId, false);
        }

        public void MoveToBottom()
        {
            OBSDeck.OBS.SetSceneItemIndex(SceneName, SceneItemId, 0);
        }
        public void MoveToTop()
        {
            int topPos = ((Container?)Parent)?.Items.Count - 1 ?? 0;
            OBSDeck.OBS.SetSceneItemIndex(SceneName, SceneItemId, topPos);
        }

    }
}
