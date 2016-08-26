using System;
using System.Collections.Generic;

namespace SerahToolkit_SharpGL.FF8_Core
{
    public class SmSoundPlay
    {
        /*static Song song;
        static SoundEffect SE;
        static SoundEffectInstance SEI;*/

        public static void Play(sbyte Volume, UInt16 ID)
        {
            /*song = Singleton.contentManager.Load<Song>("Music/" + Singleton.Songs[ID]);
            MediaPlayer.Volume = ((float)Volume / 127.0f);
            DebugOutputter.Output("Playing Song:", ID + " Volume:", MediaPlayer.Volume);
            MediaPlayer.Play(song);*/
            return;
        }

        public static void SoundEffect(sbyte volume, UInt16 ID)
        {
            /*SE = Singleton.contentManager.Load<SoundEffect>(Sounds[ID]);
            SEI = SE.CreateInstance();
            SEI.Volume = volume / 127.0f;
            DebugOutputter.Output("Playing:", ID + " Volume:", SEI.Volume * 127);
            SEI.Play();*/
            return;
        }

        public static void Stop()
        {
            //MediaPlayer.Stop();
            return;
        }

        public static void StopSound()
        {
            //SEI.Stop();
            return;
        }
    }
}
