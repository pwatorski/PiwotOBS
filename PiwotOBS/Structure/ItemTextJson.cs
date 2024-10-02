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

    public partial class ItemText
    {
        [JsonPropertyName("textInputSettings")]
        public ItemTextSettings ItemTextSettings { get; set; }
        public string UndoSname { get; set; }

        [JsonConstructor]
        public ItemText(string sourceName) : base(sourceName)
        {
            ItemTextSettings = JsonSerializer.Deserialize<ItemTextSettings>(OBSDeck.OBS.GetInputSettings(sourceName).ToJsonString(), Misc.JsonOptions) ?? new ItemTextSettings();
        }

        public void SetText(string text)
        {
            JsonObject settings = new JsonObject()
            {
                {"text", text}
            };
            OBSDeck.OBS.SetInputSettings(Name, settings);
            ItemTextSettings.text = text;
        }
    }

}
