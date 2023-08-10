using AngleSharp.Dom;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Console.Helpers
{
    public static class AudioPlayer
    {
        public static float Percent
        {
            get  
            {
                if (waveOutEvent == null)
                    return 0f;

                var currentPosition = waveOutEvent.GetPositionTimeSpan();
                float percentage = 0f;
                if(duration.TotalMilliseconds > 0)
                    percentage = (float)(currentPosition.TotalMilliseconds / (float)duration.TotalMilliseconds) * 100f;

                return percentage;
            }
        }

        private static MediaFoundationReader? foundationReader;
        private static WaveOutEvent? waveOutEvent;
        private static bool prev = false;
        private static TimeSpan duration;
        private static string source;

        static AudioPlayer()
        {
            var mediaControls = new MediaControls();
            mediaControls.PlayPausePressed += (sender, e) =>
            {
                if (waveOutEvent == null)
                    return;

                if(waveOutEvent.PlaybackState == PlaybackState.Playing)
                    waveOutEvent.Pause();
                else if (waveOutEvent.PlaybackState == PlaybackState.Paused)
                    waveOutEvent.Play();
            };
            mediaControls.StopPressed += (sender, e) =>
            {
                if (waveOutEvent == null)
                    return;

                if (waveOutEvent.PlaybackState != PlaybackState.Stopped)
                    waveOutEvent?.Stop();
            };
            mediaControls.NextTrackPressed += (sender, e) =>
            {
                if (waveOutEvent == null)
                    return;
               
                if (waveOutEvent.PlaybackState != PlaybackState.Stopped)
                    waveOutEvent?.Stop();
            };
            mediaControls.PrevTrackPressed += (sender, e) =>
            {
                if (waveOutEvent == null)
                    return;

                if(waveOutEvent.GetPositionTimeSpan().TotalSeconds > 2)
                {
                    waveOutEvent.Stop();
                    foundationReader = new MediaFoundationReader(source);
                    waveOutEvent.Init(foundationReader);
                    waveOutEvent.Play();
                    return;
                }

                prev = true;
                if (waveOutEvent.PlaybackState != PlaybackState.Stopped)
                    waveOutEvent?.Stop();
            };
            mediaControls.VolumeUpPressed += (sender, e) =>
            {
                if (waveOutEvent == null)
                    return;
                
                var volume = waveOutEvent.Volume;
                volume += 0.05f;
                if(volume > 1)
                    volume = 1;
                waveOutEvent.Volume = volume;
            };
            mediaControls.VolumeDownPressed += (sender, e) =>
            {
                if (waveOutEvent == null)
                    return;
                
                var volume = waveOutEvent.Volume;
                
                volume -= 0.025f;

                if (volume < 0)
                    volume = 0;

                waveOutEvent.Volume = volume;
            };
            mediaControls.Start();
        }

        public static void Play(string streamSource, TimeSpan? _duration)
        {
            duration = _duration ?? TimeSpan.Zero;
            source = streamSource;

            prev = false;
            foundationReader = new MediaFoundationReader(streamSource);
            waveOutEvent = new WaveOutEvent();

            waveOutEvent.Init(foundationReader);
            waveOutEvent.Play();
        }

        public static PlayEvent Status() 
        {
            if (waveOutEvent == null)
                return PlayEvent.Next;

            if (waveOutEvent.PlaybackState == PlaybackState.Playing || waveOutEvent.PlaybackState == PlaybackState.Paused)
                return PlayEvent.None;

            return prev ? PlayEvent.Prev : PlayEvent.Next;
        }

    }

    public enum PlayEvent
    {
        Next,
        Prev,
        None
    }
}
