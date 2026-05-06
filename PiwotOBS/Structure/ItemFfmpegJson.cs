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

    public partial class ItemFfmpeg
    {
        [JsonPropertyName("InputSettings")]
        public ItemFfmpegSettings ItemFfmpegSettings { get; set; }
        public string UndoSname { get; set; }

        [JsonConstructor]
        public ItemFfmpeg(string sourceName) : base(sourceName)
        {
            ItemFfmpegSettings = JsonSerializer.Deserialize<ItemFfmpegSettings>(OBSDeck.OBS.GetInputSettings(sourceName).ToJsonString(), Misc.JsonOptions) ?? new ItemFfmpegSettings();
        }
    }

}
