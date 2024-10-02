using PiwotOBS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;
using Websocket.Client;

namespace PiwotOBS
{
    public static class OBSDeck
    {
        public static OBSWebsocket OBS { get; private set; }
        public static event EventHandler Connected;
        public static event EventHandler<Communication.ObsDisconnectionInfo> Disconnected;
        public static bool IsConnected { get; private set; }

        // TODO Create methods for populating and searching all scenes. Add collections to store them as dicts and lists.

        static OBSDeck()
        {
            OBS = new OBSWebsocket();
        }

        public static OBSWebsocket Connect(string ip, string port, string password)
        {
            

            OBS.Connected += ConnectedHandler;
            OBS.Disconnected += OBS_Disconnected1;
            Task.Run(() =>
            {
                try
                {
                    OBS.ConnectAsync("ws://127.0.0.1:4455", "zaq1235pli");
                    //OBS.ConnectAsync($"ws://{ip}:{port}", password);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });


            return OBS;
        }

        private static void OBS_Disconnected1(object? sender, Communication.ObsDisconnectionInfo e) 
        { 
            IsConnected = false;
            Disconnected.Invoke(sender, e);
        }

        static void ConnectedHandler(object? sender, EventArgs e)
        {
            IsConnected = true;
            Connected.Invoke(sender, e);

        }

        public static Scene GetScene(string sceneName)
        {
            return Scene.GetRootScene(sceneName);

        }

        public static List<Scene> GetSceneList()
        {
            var scenes = OBS.GetSceneList();
            List<Scene> sceneList = scenes["scenes"].AsArray().Select(x => Scene.GetRootScene(x["sceneName"].GetValue<string>())).ToList();
            return sceneList;
        }

        public static JsonArray GetScenesJson()
        {
            var scenes = OBS.GetSceneList();
            var sceneNameList = scenes["scenes"].AsArray().Select(x => x["sceneName"].GetValue<string>());
            var returnSceneList = new JsonArray();
            foreach (var sceneName in sceneNameList)
            {
                var itemList = OBS.GetSceneItemList(sceneName);
                JsonObject jsonScene = new()
                {
                    { "sceneName", sceneName }
                };
                JsonArray jsonItemList = new JsonArray();
                foreach (var item in itemList)
                {
                    if ((string)item["sourceType"] == "OBS_SOURCE_TYPE_SCENE" && (bool)item["isGroup"])
                    {
                        var subItems = OBS.GetGroupSceneItemList(item["sourceName"].GetValue<string>());
                        
                        JsonArray groupItems = new JsonArray();
                        foreach (var subItem in subItems)
                        {
                            var subSsettings = OBS.GetInputSettings(subItem["sourceName"].GetValue<string>())["inputSettings"];
                            if (subSsettings != null)
                            {
                                subItem.Add("inputSettings", JsonObject.Parse(subSsettings.ToJsonString()));
                            }
                            
                            groupItems.Add(subItem);
                        }
                        item.Add("children", groupItems);
                    }
                    var settings = OBS.GetInputSettings(item["sourceName"].GetValue<string>())["inputSettings"];
                    if (settings != null)
                    {
                        item.Add("inputSettings", JsonObject.Parse(settings.ToJsonString()));
                    }
                    jsonItemList.Add(item);
                }
                jsonScene.Add("children", jsonItemList);
                returnSceneList.Add(jsonScene);
            }
            return returnSceneList;
        }
    }
}
