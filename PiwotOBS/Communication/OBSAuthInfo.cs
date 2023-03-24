using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PiwotOBS.Communication
{
    public class OBSAuthInfo
    {
        [JsonPropertyName("challenge")]
        public string? Challenge { get; set; }

        [JsonPropertyName("salt")]
        public string? PasswordSalt { get; set; }

        public static OBSAuthInfo FromJson(JsonObject? data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            return JsonSerializer.Deserialize<OBSAuthInfo>(data.ToString());
        }
    }

}
