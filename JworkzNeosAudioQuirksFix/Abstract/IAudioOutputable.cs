using FrooxEngine;

namespace JworkzNeosMod.Abstract
{
    internal interface IAudioOutputable
    {
        IWorldElement Slot { get; }

        bool IsEnabledFieldDriven { get; }

        bool HasAudioSource { get; }

        void DriveEnabledField(IAudioPlaybackable playbackable);
    }
}
