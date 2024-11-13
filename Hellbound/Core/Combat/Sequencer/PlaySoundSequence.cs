namespace HellTrail.Core.Combat.Sequencer
{
    public class PlaySoundSequence : ISequenceAction
    {
        public string sound;
        public float volume;

        public PlaySoundSequence(string sound, float volume = -1)
        {
            this.sound = sound;
            this.volume = volume;
        }

        public void Update(List<Unit> actors, Battle battle)
        {

        }

        public bool IsFinished()
        {
            SoundEngine.PlaySound(sound, volume);
            return true;
        }
    }
}
