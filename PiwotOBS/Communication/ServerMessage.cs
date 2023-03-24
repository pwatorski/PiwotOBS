﻿

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PiwotOBS.Communication
{
    /// <summary>
    /// Message received from the server
    /// </summary>
    internal class ServerMessage
    {
        /// <summary>
        /// Server Message's operation code
        /// </summary>
        [JsonPropertyName("op")]
        public MessageTypes OperationCode { set; get; }

        /// <summary>
        /// Server Data
        /// </summary>
        [JsonPropertyName("d")]
        public JsonObject Data { get; set; }
    }
}
