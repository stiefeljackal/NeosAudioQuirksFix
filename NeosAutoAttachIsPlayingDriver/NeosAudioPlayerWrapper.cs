using FrooxEngine;
using JworkzNeosMod.Abstract;

namespace JworkzNeosMod
{
    internal class NeosAudioPlayerWrapper : IAudioPlaybackable
    {
        internal const int AUDIO_CLIP_PLAYER_PLAYBACK_INDEX = 3;

        public ISyncMember PlaybackSource { get; private set; }

        public bool IsVideoPlayer => false;

        internal NeosAudioPlayerWrapper(Slot slot)
        {
            var audioOutputComponent = slot.GetComponent<AudioClipPlayer>();
            PlaybackSource = audioOutputComponent?.GetSyncMember(AUDIO_CLIP_PLAYER_PLAYBACK_INDEX);
        }
    }
}