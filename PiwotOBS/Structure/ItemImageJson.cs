using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PiwotOBS.Structure
{

    public partial class ItemImage
    {
        [JsonPropertyName("InputSettings")]
        public ItemImageSettings ItemTextSettings { get; set; }
        public string UndoSname { get; set; }

        [JsonConstructor]
        public ItemImage(string sourceName) : base(sourceName)
        {
            ItemTextSettings = JsonSerializer.Deserialize<ItemImageSettings>(OBSDeck.OBS.GetInputSettings(sourceName).ToJsonString(), Misc.JsonOptions) ?? new ItemImageSettings();
        }

        public void SetSource(string sourcePath)
        {
            JsonObject settings = new JsonObject()
            {
                {"file", sourcePath}
            };
            OBSDeck.OBS.SetInputSettings(Name, settings);
            ItemTextSettings.file = sourcePath;
        }
    }

}
