using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;

namespace PiwotOBS.Communication
{
    

    static internal class Messenger
    {
        static internal JsonObject BuildMessage(MessageTypes opCode, string? messageType, JsonObject? additionalFields, out string messageId)
        {
            messageId = Guid.NewGuid().ToString();
            JsonObject payload = new JsonObject()
            {
                { "op", (int)opCode }
            };

            JsonObject data = new JsonObject();

            switch (opCode)
            {
                case MessageTypes.Request:
                    data.Add("requestType", messageType);
                    data.Add("requestId", messageId);
                    data.Add("requestData", additionalFields);
                    additionalFields = null;
                    break;
                case MessageTypes.RequestBatch:
                    data.Add("requestId", messageId);
                    break;

            }
            if (additionalFields != null)
            {
                data = additionalFields;
            }
            payload.Add("d", data);
            return payload;
        }
    }


}
