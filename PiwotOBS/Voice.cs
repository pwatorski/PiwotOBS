using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotOBS
{
    public class VoiceMeter
    {
        NAudio.Wave.WaveInEvent waveIn;
        static readonly float maxVolume = 32767;
        public float CurVolume { get; protected set; } = 0;
        public float[] VolumeRecord;
        public List<float> VolumeHistory;
        public List<float> AvgVolumeHistory;
        public float VolumeRecordAvg { get; protected set; } = 0;
        int volumeRecordPoint = 0;
        int volumeHistoryLenght;
        public bool Enabled { get; protected set; } = false;
        public event EventHandler<Tuple<float, float>> VolumeUpdate;
        public VoiceMeter(int deviceNumber=0, int bufferMilliseconds=50, int volumeRecordLength = 10, int volumeHistoryLenght = 100)
        {
            waveIn = new NAudio.Wave.WaveInEvent
            {
                DeviceNumber = deviceNumber,
                WaveFormat = new NAudio.Wave.WaveFormat(rate: 44100, bits: 16, channels: 1),
                BufferMilliseconds = 50
            };
            waveIn.DataAvailable += LogSound;
            VolumeRecord = new float[volumeRecordLength];
            VolumeHistory = new List<float>() { 0 };
            AvgVolumeHistory = new List<float>() { 0 };
            this.volumeHistoryLenght = volumeHistoryLenght;
        }

        public void Start()
        {
            waveIn.StartRecording();
            Enabled = true;
        }
        public void Stop()
        {
            waveIn.StopRecording();
            Enabled = false;
        }

        static string GetBars(double fraction, int barCount = 35)
        {
            int barsOn = (int)(barCount * fraction);
            int barsOff = barCount - barsOn;
            return new string('#', barsOn) + new string('-', barsOff);
        }
        void LogSound(object? sender, NAudio.Wave.WaveInEventArgs args)
        {
            int peakValue = 0;
            int bytesPerSample = 2;
            for (int index = 0; index < args.BytesRecorded; index += bytesPerSample)
            {
                int value = BitConverter.ToInt16(args.Buffer, index);
                peakValue = Math.Max(peakValue, value);
            }
            CurVolume = peakValue / maxVolume;
            VolumeRecord[volumeRecordPoint] = CurVolume;
            volumeRecordPoint = (volumeRecordPoint + 1) % VolumeRecord.Length;
            VolumeRecordAvg = VolumeRecord.Sum() / VolumeRecord.Length;
            VolumeHistory.Add(CurVolume);
            AvgVolumeHistory.Add(VolumeRecordAvg);
            if (VolumeHistory.Count > volumeHistoryLenght)
            {
                AvgVolumeHistory.RemoveAt(0);
                VolumeHistory.RemoveAt(0);
            }
            VolumeUpdate.Invoke(this, new Tuple<float, float>(CurVolume, VolumeRecordAvg));
        }
    }
}
