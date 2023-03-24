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
    public class Sceneitemtransform
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

        static Sceneitemtransform? FromJson(JsonObject jsonObject)
        {
            return JsonSerializer.Deserialize<Sceneitemtransform>(jsonObject.ToJsonString());
        }

        public JsonObject ToJson()
        {
            return JsonSerializer.SerializeToNode(this).AsObject();
        }
    }
}
