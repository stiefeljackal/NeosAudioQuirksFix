using FrooxEngine;
using JworkzNeosMod.Abstract;

namespace JworkzNeosMod
{
    internal class NeosMediaSlotWrapper : IAudioOutputable
    {
        public IWorldElement Slot { get; private set; }

        public bool IsEnabledFieldDriven => _audioOutput.EnabledField.IsDriven;

        public AudioOutput _audioOutput;

        public bool HasAudioSource  => _audioOutput?.Source?.ReferenceID != null;

        public NeosMediaSlotWrapper(Slot slot)
        {
            Slot = slot;
            _audioOutput = slot.GetComponent<AudioOutput>();
        }

        public void DriveEnabledField(IAudioPlaybackable playbackable)
        {
            if (IsEnabledFieldDriven) { return; }

            var isPlayingDriverComp = _audioOutput.Slot.GetComponentOrAttach<IsPlayingDriver>();

            var boolFieldDrive = isPlayingDriverComp.Targets.Add();
            boolFieldDrive.TrySet(_audioOutput.EnabledField);

            isPlayingDriverComp.Playback.TrySet(playbackable.PlaybackSource);
        }
    }
}
