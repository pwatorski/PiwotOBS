using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PiwotOBS.Structure
{

    public partial class SceneItem
    {
        [JsonPropertyName("inputKind")]
        public string InputKind { get; set; }
        [JsonPropertyName("isGroup")]
        public bool? IsGroup { get; set; } = false;
        [JsonPropertyName("sceneItemBlendMode")]
        public string SceneItemBlendMode { get; set; }
        [JsonPropertyName("sceneItemEnabled")]
        public bool OBSEnabled { get; set; }
        [JsonPropertyName("sceneItemId")]
        public int SceneItemId { get; set; }
        [JsonPropertyName("sceneItemIndex")]
        public int SceneItemIndex { get; set; }
        [JsonPropertyName("sceneItemLocked")]
        public bool Locked { get; set; }
        [JsonPropertyName("sceneItemTransform")]
        internal SceneItemTransform? Transform { get; set; }
        [JsonPropertyName("sourceName")]
        public string SourceName { get; set; }
        [JsonPropertyName("sourceType")]
        public string SourceType { get; set; }

        public bool IsScene { get=>!(IsGroup??false) && SourceType== "OBS_SOURCE_TYPE_SCENE"; }

        public string Name { get => SourceName; }

        public static SceneItem? FromOBSJson(JsonObject jsonObject)
        {
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            
            if((string)jsonObject["sourceType"] == "OBS_SOURCE_TYPE_SCENE")
            {
                if ((bool)jsonObject["isGroup"])
                {
                    var group = JsonSerializer.Deserialize<Group>(jsonObject.ToJsonString(), jsonSerializerOptions);
                    group.BuildOBSChildren();
                    return group;
                }
                var scene = JsonSerializer.Deserialize<Scene>(jsonObject.ToJsonString(), jsonSerializerOptions);
                scene.BuildOBSChildren();
                return scene;
            }

            var obj = JsonSerializer.Deserialize<SceneItem>(jsonObject.ToJsonString(), jsonSerializerOptions);
            obj?.Init();
            obj.Transform = SceneItemTransform.FromJson(jsonObject["sceneItemTransform"]?.AsObject());
            return obj;
        }

        public static SceneItem? FromJson(JsonObject jsonObject)
        {
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            if ((string)jsonObject["sourceType"] == "OBS_SOURCE_TYPE_SCENE")
            {
                var x = jsonObject["items"].AsArray().Select((x) => x.AsObject());
                jsonObject.Remove("items");
                if ((bool)jsonObject["isGroup"])
                {
                    var group = JsonSerializer.Deserialize<Group>(jsonObject.ToJsonString(), jsonSerializerOptions);
                    group.BuildChildren(x);
                    return group;
                }
                var scene = JsonSerializer.Deserialize<Scene>(jsonObject.ToJsonString(), jsonSerializerOptions);
                scene.BuildChildren(x);
                return scene;
            }

            var obj = JsonSerializer.Deserialize<SceneItem>(jsonObject.ToJsonString(), jsonSerializerOptions);
            obj?.Init();
            obj.Transform = SceneItemTransform.FromJson(jsonObject["sceneItemTransform"]?.AsObject());
            return obj;
        }


        public virtual JsonObject ToOBSJson()
        {
            JsonObject json = new JsonObject
            {
                { "inputKind", InputKind },
                { "isGroup", IsGroup },
                { "sceneItemBlendMode", SceneItemBlendMode },
                { "sceneItemEnabled", OBSEnabled },
                { "sceneItemId", SceneItemId },
                { "sceneItemIndex", SceneItemIndex },
                { "sceneItemLocked", Locked },
                { "sceneItemTransform", Transform?.ToJson() },
                { "sourceName", SourceName },
                { "sourceType", SourceType }
            };
            return json;
        }

        public virtual JsonObject ToJson()
        {
            JsonObject json = ToOBSJson();
            return json;
        }

        public virtual void Save(string directory, string? fileName=null)
        {
            fileName ??= Path.GetFileNameWithoutExtension(Name);
            if(!fileName.EndsWith(".json"))
            {
                fileName = Path.GetFileNameWithoutExtension(fileName) + ".json";
            }
            string filepath = Path.Combine(directory, fileName);
            JsonSerializerOptions serializerOptions = new() { WriteIndented = true };
            using StreamWriter sw = new (filepath, false, System.Text.Encoding.UTF8);
            sw.Write(ToJson().ToJsonString(serializerOptions));
        }

        public static SceneItem? Load(string filepath)
        {
            using StreamReader sw = new(filepath, System.Text.Encoding.UTF8);
            var node = JsonNode.Parse(sw.ReadToEnd());
            return node == null ? throw new Exception("Empty load json.") : FromJson(node.AsObject());
        }

        public virtual JsonObject GetCurrentOBSJson()
        {
            if (Parent == null)
            {
                throw new InvalidOperationException($"\"GetCurrentOBSJson\" operation is not supported for root scenes: \"{Name}\"!");
            }
            var json = OBSDeck.OBS.GetSceneItemTransform(SceneName, SceneItemId);
            return json;
        }

        public virtual bool GetCurrentOBSEnabled()
        {
            bool enabled = OBSDeck.OBS.GetSceneItemEnabled(SceneName, SceneItemId);
            return enabled;
        }
    }
}
