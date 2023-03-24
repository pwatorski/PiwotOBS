using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Websocket.Client;
using System.Security.Cryptography;
using System.Threading;
using System.Runtime.InteropServices.JavaScript;
using System.Collections.Concurrent;
using PiwotOBS.Communication;

using System.Text.Json;
using System.Text.Json.Nodes;

namespace PiwotOBS
{
    partial class OBSWebsocket
    {
        WebsocketClient? client;
        public bool IsConnected => client?.IsRunning ?? false;

        public string? connectionPassword { get; private set; }
        private const int SUPPORTED_RPC_VERSION = 1;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<JsonObject>> responseHandlers= new ConcurrentDictionary<string, TaskCompletionSource<JsonObject>>();

        void ConnectAsync(string ip, string port, string password)
        {
            client = new WebsocketClient(new Uri($"{ip}:{port}"));
            client.IsReconnectionEnabled = false;
            client.ReconnectTimeout = null;
            client.ErrorReconnectTimeout = null;
            client.MessageReceived.Subscribe(m => Task.Run(() => OnRecieveMessage(this, m)));
            client.DisconnectionHappened.Subscribe(d => Task.Run(() => OnDisconnect(this, d)));

            connectionPassword = password;

            Console.WriteLine("START OR FAIL");
            client.StartOrFail();
        }

        public void ConnectAsync(string url, string password)
        {

            if (client != null && client.IsRunning)
            {
                Disconnect();
            }

            client = new WebsocketClient(new Uri(url));
            client.IsReconnectionEnabled = false;
            client.ReconnectTimeout = null;
            client.ErrorReconnectTimeout = null;
            client.MessageReceived.Subscribe(m => Task.Run(() => OnRecieveMessage(this, m)));
            client.DisconnectionHappened.Subscribe(d => Task.Run(() => OnWebsocketDisconnect(this, d)));

            connectionPassword = password;
            client.StartOrFail();
        }

        /// <summary>
        /// Disconnect this instance from the server
        /// </summary>
        public void Disconnect()
        {
            connectionPassword = null;
            if (client != null)
            {
                // Attempt to both close and dispose the existing connection
                try
                {
                    client.Stop(WebSocketCloseStatus.NormalClosure, "User requested disconnect");
                    ((IDisposable)client).Dispose();
                }
                catch { }
                client = null;
            }
        }

        public bool EventLogEnabled { get; set; } = false;

        protected void ProcessEventType(string eventType, JsonObject body)
        {
            if (EventLogEnabled)
            {
                body = (JsonObject)body["eventData"];

                Console.WriteLine(body);
            }
        }

        /// <summary>
        /// Triggered when connected successfully to an OBS-websocket server
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Triggered when disconnected from an OBS-websocket server
        /// </summary>
        public event EventHandler<ObsDisconnectionInfo> Disconnected;

        // This callback handles a websocket disconnection
        private void OnWebsocketDisconnect(object sender, DisconnectionInfo d)
        {
            
            if (d == null || d.CloseStatus == null)
            {
                Disconnected?.Invoke(sender, new ObsDisconnectionInfo(ObsCloseCodes.UnknownReason, null, d));
            }
            else
            {
                Disconnected?.Invoke(sender, new ObsDisconnectionInfo((ObsCloseCodes)d.CloseStatus, d.CloseStatusDescription, d));
            }
        }

        private void OnDisconnect(OBSWebsocket oBSWebsocket, DisconnectionInfo d)
        {
            Console.WriteLine(d.ToString());
        }

        private void OnRecieveMessage(OBSWebsocket oBSWebsocket, ResponseMessage m)
        {
            if (m.MessageType != WebSocketMessageType.Text)
            {
                return;
            }

            ServerMessage? msg = JsonSerializer.Deserialize<ServerMessage>(m.Text);
            if (msg == null)
            {
                Console.WriteLine("ERROR! Null response message!");
                return;
            }
            JsonObject? body = msg.Data;


            if (body == null)
            {
                Console.WriteLine("ERROR! Null message body!");
                return;
            }
            switch (msg.OperationCode)
            {
                case MessageTypes.Hello:
                    // First message received after connection, this may ask us for authentication
                    Authenticate(body);
                    break;
                case MessageTypes.Identified:
                    Task.Run(() => Connected?.Invoke(this, EventArgs.Empty));
                    break;


                case MessageTypes.RequestResponse:
                case MessageTypes.RequestBatchResponse:
                    // Handle response to previous request
                    if (body.ContainsKey("requestId"))
                    {
                        // Handle a request :
                        // Find the response handler based on
                        // its associated message ID

                        string msgID = (string?)body["requestId"] ?? throw new NullReferenceException(nameof(body));

                        if (responseHandlers.TryRemove(msgID, out TaskCompletionSource<JsonObject>? handler))
                        {
                            // Set the response body as Result and notify the request sender
                            handler.SetResult(body);
                        }
                    }
                    break;
                case MessageTypes.Event:
                    // Handle events
                    string eventType = (string?)body["eventType"]??throw new NullReferenceException(nameof(body));
                    Task.Run(() => { ProcessEventType(eventType, body); });
                    break;

                    
                default:
                    Console.WriteLine("UNHANDLED");
                    break;
            }
        }


        private void Authenticate(JsonObject payload)
        {
            Console.WriteLine("Authenticating...");
            if (client==null || !client.IsStarted)
            {
                return;
            }

            OBSAuthInfo? authInfo = null;
            if (payload.ContainsKey("authentication"))
            {
                // Authentication required
                authInfo = OBSAuthInfo.FromJson(payload["authentication"].AsObject());
            }
            if (authInfo == null) throw new NullReferenceException(nameof(authInfo));
            Identify(connectionPassword, authInfo);

            connectionPassword = null;
        }

        protected void Identify(string? password, OBSAuthInfo? authInfo = null)
        {
            var requestFields = new JsonObject
            {
                { "rpcVersion", SUPPORTED_RPC_VERSION }
            };

            if (authInfo != null)
            {
                // Authorization required

                string secret = HashEncode(password + authInfo.PasswordSalt);
                string authResponse = HashEncode(secret + authInfo.Challenge);
                requestFields.Add("authentication", authResponse);
            }

            SendRequest(MessageTypes.Identify, null, requestFields, false);
        }

        protected string HashEncode(string input)
        {
            using var sha256 = SHA256.Create();

            byte[] textBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = sha256.ComputeHash(textBytes);

            return Convert.ToBase64String(hash);
        }

        public JsonObject? SendRequest(string requestType, JsonObject? additionalFields = null)
        {
            return SendRequest(MessageTypes.Request, requestType, additionalFields, true);
        }

        internal JsonObject? SendRequest(MessageTypes operationCode, string? requestType, JsonObject? additionalFields = null, bool waitForReply = true)
        {
            if (client == null)
            {
                throw new NullReferenceException("Websocket is not initialized");
            }

            // Prepare the asynchronous response handler
            var tcs = new TaskCompletionSource<JsonObject>();
            JsonObject? message = null;
            do
            {
                // Generate a random message id
                message = Messenger.BuildMessage(operationCode, requestType, additionalFields, out string messageId);
                if (!waitForReply || responseHandlers.TryAdd(messageId, tcs))
                {
                    break;
                }
                // Message id already exists, retry with a new one.
            } while (true && false);
            // Send the message 
            client.Send(message.ToJsonString());
            if (!waitForReply)
            {
                return null;
            }

            // Wait for a response (received and notified by the websocket response handler)
            tcs.Task.Wait(1000);

            if (tcs.Task.IsCanceled)
            {
                //throw new ErrorResponseException("Request canceled", 0);
                Console.WriteLine("Request canceled");

            }

            // Throw an exception if the server returned an error.
            // An error occurs if authentication fails or one if the request body is invalid.
            var result = tcs.Task.Result;
            if (result == null)
            {
                Console.WriteLine("Request ERROR");
                return null;
            }
            if ((bool)(result["requestStatus"]?["result"]??false))//???????
            {
                JsonObject? status = result["requestStatus"] as JsonObject;
                if(status is null)
                {
                    Console.WriteLine("Request ERROR");
                    return null;
                }
                //throw new ErrorResponseException($"ErrorCode: {status["code"]}{(status.ContainsKey("comment") ? $", Comment: {status["comment"]}" : "")}", (int)status["code"]);
                var code = (int)status["code"];
                if (code != 100)
                {
                    Console.WriteLine($"ErrorCode: {code}{status}");
                    return result;
                }

            }

            if (result?.ContainsKey("responseData")??false) // ResponseData i  s optional
                return result["responseData"].AsObject();

            return new JsonObject();
        }

    }
}
