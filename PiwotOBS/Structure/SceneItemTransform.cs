using PiwotOBS.PMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PiwotOBS.Structure
{
    internal class Sceneitemtransform
    {
        public int alignment { get; set; }
        public int boundsAlignment { get; set; }
        public float boundsHeight { get; set; }
        public string boundsType { get; set; }
        public float boundsWidth { get; set; }
        public int cropBottom { get; set; }
        public int cropLeft { get; set; }
        public int cropRight { get; set; }
        public int cropTop { get; set; }
        public float height { get; set; }
        public float positionX { get; set; }
        public float positionY { get; set; }
        public float rotation { get; set; }
        public float scaleX { get; set; }
        public float scaleY { get; set; }
        public float sourceHeight { get; set; }
        public float sourceWidth { get; set; }
        public float width { get; set; }

        public Float2 Position { get => new Float2(positionX, positionY); }
        public Float2 Size { get => new Float2(width, height); }
        public Float2 Scale { get => new Float2(scaleX, scaleY); }

        public static Sceneitemtransform? FromJson(JsonObject? jsonObject)
        {
            if (jsonObject == null) return null;
            return JsonSerializer.Deserialize<Sceneitemtransform>(jsonObject.ToJsonString());
        }

        public JsonObject ToJson()
        {
            return JsonSerializer.SerializeToNode(this).AsObject();
        }
    }
}
