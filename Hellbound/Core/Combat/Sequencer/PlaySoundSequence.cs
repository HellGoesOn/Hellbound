﻿using Microsoft.Xna.Framework.Audio;

namespace Casull.Core.Combat.Sequencer
{
    public class PlaySoundSequence : ISequenceAction
    {
        public string sound;
        public float volume;
        public float pitch;
        public SoundEffectInstance snd;

        public PlaySoundSequence(string sound, float volume = -1, float pitch = -1)
        {
            this.sound = sound;
            this.volume = volume;
            this.pitch = pitch;
        }

        public void Update(List<Unit> actors, Battle battle)
        {

        }

        public bool IsFinished()
        {
            snd = SoundEngine.PlaySound(sound, volume, pitch);
            return true;
        }
    }
}
