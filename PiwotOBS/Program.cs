// See https://aka.ms/new-console-template for more information

using PiwotOBS;
using PiwotOBS.Structure;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;


//var waveIn = new NAudio.Wave.WaveInEvent
//{
//    DeviceNumber = 0, // customize this to select your microphone device
//    WaveFormat = new NAudio.Wave.WaveFormat(rate: 44100, bits: 16, channels: 1),
//    BufferMilliseconds = 50
//};

//static string GetBars(double fraction, int barCount = 35)
//{
//    int barsOn = (int)(barCount * fraction);
//    int barsOff = barCount - barsOn;
//    return new string('#', barsOn) + new string('-', barsOff);
//}
//static void ShowPeakMono(object? sender, NAudio.Wave.WaveInEventArgs args)
//{
//    float maxValue = 32767;
//    int peakValue = 0;
//    int bytesPerSample = 2;
//    for (int index = 0; index < args.BytesRecorded; index += bytesPerSample)
//    {
//        int value = BitConverter.ToInt16(args.Buffer, index);
//        peakValue = Math.Max(peakValue, value);
//    }

//    Console.WriteLine("L=" + GetBars(peakValue / maxValue));
//}

//waveIn.DataAvailable += ShowPeakMono;
//waveIn.StartRecording();

OBSDeck.Connected += new EventHandler(ConnectedHandler);
OBSDeck.Initialize("192.168.0.80", "4455", "zaq1235pli");


void ConnectedHandler(object? sender, EventArgs e)
{
    Console.WriteLine("CONNECTED! :D");
}




int step = 0;
Thread.Sleep(3000);

Scene scene = Scene.GetRootScene("SAFARI");
scene.Save("");
scene = (Scene)Scene.Load("SAFARI.json");
Console.WriteLine(scene.GetTreeString());
//var x = obs.GetSceneItemList("SAFARI");
//SceneItem si = SceneItem.FromJson(x[0]);

//var a = new JsonArray(x.ToArray());

//using (StreamWriter sw = new StreamWriter("XD.json"))
//    sw.WriteLine(a.ToString());

//obs.SetSceneItemTransform("SAFARI", 97, new JsonObject() { { "scaleX", 1 } });

while (OBSDeck.OBS.IsConnected)
{
    Console.WriteLine($"step {step}");
    Thread.Sleep(1000);
    step += 1;
    OBSDeck.OBS.SetSceneItemEnabled("SAFARI", 97, step % 2 == 0);
}