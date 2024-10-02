using System.Net.Sockets;
using System.Net;
using System;
using System.Collections;
using System.Speech.Synthesis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text.Json.Nodes;

namespace WebTest
{
    class Server
    {
        public void Main()
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8765);

            server.Start();
            Console.WriteLine("Server has started on 127.0.0.1:8765.{0}Waiting for a connection…", Environment.NewLine);

            TcpClient client = server.AcceptTcpClient();

            Console.WriteLine("A client connected.");
            NetworkStream stream = client.GetStream();

            //enter to an infinite cycle to be able to handle every change in stream
            while (true)
            {
                while (!stream.DataAvailable) 
                    Thread.Sleep(100);

                byte[] bytes = new byte[client.Available];

                stream.Read(bytes, 0, bytes.Length);
                string text = System.Text.Encoding.UTF8.GetString(bytes);
                JsonObject? request = JsonObject.Parse(text)?.AsObject();
                if (request == null)
                    return;
                Console.WriteLine($"{request.ToJsonString(Misc.JsonOptions)}");
                var synthesizer = new SpeechSynthesizer();
                synthesizer.SetOutputToDefaultAudioDevice();
                var builder = new PromptBuilder();
                builder.StartVoice(new CultureInfo("pl-PL"));
                builder.AppendText(text);
                builder.EndVoice();
                synthesizer.Speak(builder);
            }
        }
    }
}