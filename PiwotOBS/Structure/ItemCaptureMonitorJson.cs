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

    public partial class ItemCaptureMonitor
    {
        [JsonPropertyName("InputSettings")]
        public ItemCaptureMonitorSettings ItemCaptureMonitorSettings { get; set; }

        [JsonConstructor]
        public ItemCaptureMonitor(string sourceName) : base(sourceName)
        {
            ItemCaptureMonitorSettings = JsonSerializer.Deserialize<ItemCaptureMonitorSettings>(OBSDeck.OBS.GetInputSettings(sourceName).ToJsonString(), Misc.JsonOptions) ?? new ItemCaptureMonitorSettings();
        }

    }

}
