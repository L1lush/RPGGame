using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoSomething
{
    internal class MusicPlayer
    {
        private string path;
        private string fullPath;
        private WaveOutEvent? outputDevice;
        private AudioFileReader? audioFile;

        public MusicPlayer(string path)
        {
            this.path = path;
            this.fullPath = Path.GetFullPath(path);

            if (outputDevice == null)
            {
                audioFile = new AudioFileReader(fullPath);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
            }
        }

        public void Play()
        {
            try
            {
                Task.Run(() =>
                {
                    outputDevice.Play();
                    outputDevice.PlaybackStopped += (s, e) =>
                    {
                        audioFile.Position = 0;
                        outputDevice.Play();
                    };
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error playing audio: " + ex.Message);
            }
        }

        public void Stop()
        {
            outputDevice?.Stop();
        }

        public void Pause()
        {
            outputDevice?.Pause();
        }

        public void SetMasterVolume(int volume)
        {
            outputDevice.Volume = volume / 100f;
        }
        public int GetMasterVolume()
        {
            return (int)(outputDevice.Volume * 100f);
        }

        public void Setvolume(int volume)
        {
            audioFile.Volume = volume / 100f;
        }
        public int Getvolume()
        {
            return (int)(audioFile.Volume * 100);
        }
    }
}
