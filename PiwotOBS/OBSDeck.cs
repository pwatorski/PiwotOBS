using PiwotOBS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Websocket.Client;

namespace PiwotOBS
{
    public static class OBSDeck
    {
        public static OBSWebsocket OBS { get; private set; }
        public static event EventHandler Connected;

        // TODO Create methods for populating and searching all scenes. Add collections to store them as dicts and lists.
        public static OBSWebsocket Initialize(string ip, string port, string password)
        {
            OBS = new OBSWebsocket();

            OBS.Connected += new EventHandler(ConnectedHandler);
            Task.Run(() =>
            {
                try
                {
                    //OBS.ConnectAsync("ws://192.168.0.80:4455", "zaq1235pli");
                    OBS.ConnectAsync($"ws://{ip}:{port}", password);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });


            return OBS;
        }

        static void ConnectedHandler(object? sender, EventArgs e)
        {
            Connected.Invoke(sender, e);
        }

        public static Scene GetScene(string sceneName)
        {
            return Scene.GetRootScene(sceneName);

        }
    }
}
