// See https://aka.ms/new-console-template for more information

using PiwotOBS;
using PiwotOBS.PMath;
using PiwotOBS.Structure;
using System.Data.SqlTypes;
using System.Diagnostics;
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
//    float maxVolume = 32767;
//    int peakValue = 0;
//    int bytesPerSample = 2;
//    for (int index = 0; index < args.BytesRecorded; index += bytesPerSample)
//    {
//        int value = BitConverter.ToInt16(args.Buffer, index);
//        peakValue = Math.Max(peakValue, value);
//    }

//    Console.WriteLine("L=" + GetBars(peakValue / maxVolume));
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

//Scene scene2 = Scene.GetRootScene("SAFARI");
//scene2.Save("");
//var jumpscare = scene.FindItem("Foxyer_0706_bez_ta.png");
var scene = (Scene)Scene.Load("SAFARI.json");
var jumpscare = scene.FindItem("Foxyer_0706_bez_ta.png");
var XD_FACE = scene.FindItem("XD_FACE_AVATAR");
Animator animator = new Animator();
VoiceMeter voiceMeter = new VoiceMeter(volumeRecordLength: 4);
voiceMeter.Start();

float maxRotation = 60f;
float rotationSetCut = 0.7f;
float rotationResetCut = 0.4f;
float curDirection = 0;
Thread thread = new Thread(() =>
{
    while (true) 
    { 
        var c = Console.ReadKey(true).Key;

        switch (c)
        {
            default:
                break;
        }
        Console.WriteLine($"Key=[{c}]");
    }
});
thread.Start();
float DecideDirection(float val, float curDirection, float rotationSetCut = 0.7f, float rotationResetCut = 0.4f)
{
    if (val > rotationSetCut && (curDirection == 0 || Rand.Int(10) == 0))
    {

        return Rand.Int(2) * 2 - 1;
    }
    if (val < rotationResetCut)
    {
        return 0;
    }
    return curDirection;
}

float GetTargetRotation(float val, float curDirection, float maxRotation=60)
{
    curDirection = DecideDirection(val, curDirection);
    return maxRotation * curDirection * (1f - val);
}



ProceduralAnimation proceduralAnimation = new ProceduralAnimation(XD_FACE, (float T, SceneItem x) =>
{
    var vol = voiceMeter.CurVolume * 1.2f + 0.1f;
    var volSqrt = (float)(Math.Sqrt(voiceMeter.CurVolume) * 1.2f + 0.1f);
    var scale = new Float2(
            (float)Math.Abs(0.5 - volSqrt) + 0.5f,
            (float)volSqrt) * XD_FACE.OBSScale;
    var rotation = GetTargetRotation(vol, curDirection);
    return new AnimationTransform(
        XD_FACE, 
        scale: Float2.Larp(XD_FACE.CurScale, scale, 0.5f),
        rotation: Arit.Larp(XD_FACE.CurRotation, rotation, 0.5f));
});
animator.RegisterAnimation(proceduralAnimation);
animator.Run();
//var x = obs.GetSceneItemList("SAFARI");
//SceneItem si = SceneItem.FromJson(x[0]);

//var a = new JsonArray(x.ToArray());

//using (StreamWriter sw = new StreamWriter("XD.json"))
//    sw.WriteLine(a.ToString());

//obs.SetSceneItemTransform("SAFARI", 97, new JsonObject() { { "scaleX", 1 } });
Stopwatch sw = new Stopwatch();
sw.Start();

while (OBSDeck.OBS.IsConnected)
{
    //Console.WriteLine($"step {step}");
    Thread.Sleep(50);
    step += 1;
    var t = sw.Elapsed.TotalSeconds;
    //jumpscare.SetRelativePosition((float)(t%100)*50, (float)Math.Sin(t) * 100);
    //jumpscare.SetRelativeScale(1, 0.1f + voiceMeter.VolumeRecordAvg);
    //jumpscare.SetSize(null, (0.1f + voiceMeter.VolumeRecordAvg) * 500);
    //OBSDeck.OBS.SetSceneItemEnabled("SAFARI", 97, step % 2 == 0);
}