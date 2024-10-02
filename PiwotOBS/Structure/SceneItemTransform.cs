using PiwotOBS.PMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PiwotOBS.Structure
{
    public class SceneItemTransform
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

        [JsonIgnore]
        public Float2 Position { get => new Float2(positionX, positionY); }
        [JsonIgnore]
        public Float2 Size { get => new Float2(width, height); }
        [JsonIgnore]
        public Float2 Scale { get => new Float2(scaleX, scaleY); }

        public static SceneItemTransform? FromJson(JsonObject? jsonObject)
        {
            if (jsonObject == null) return null;
            return JsonSerializer.Deserialize<SceneItemTransform>(jsonObject.ToJsonString());
        }

        public static SceneItemTransform? FromJson(string jsonString)
        {
            SceneItemTransform sit = JsonSerializer.Deserialize<SceneItemTransform>(jsonString);
            Console.WriteLine(sit);
            return sit;
        }

        public JsonObject ToJson()
        {
            return JsonSerializer.SerializeToNode(this)?.AsObject() ?? new JsonObject();
        }

        public void UpdateFrom(SceneItemTransform transform)
        {
            alignment = transform.alignment;
            boundsAlignment = transform.boundsAlignment;
            boundsHeight = transform.boundsHeight;
            boundsType = transform.boundsType;
            boundsWidth = transform.boundsWidth;
            cropBottom = transform.cropBottom;
            cropLeft = transform.cropLeft;
            cropRight = transform.cropRight;
            cropTop = transform.cropTop;
            height = transform.height;
            positionX = transform.positionX;
            positionY = transform.positionY;
            rotation = transform.rotation;
            scaleX = transform.scaleX;
            scaleY = transform.scaleY;
            sourceHeight = transform.sourceHeight;
            sourceWidth = transform.sourceWidth;
            width = transform.width;
        }

        public override string ToString()
        {
            return ToJson().ToString();
        }
    }
}
