using FrooxEngine;
using JworkzNeosMod.Abstract;

namespace JworkzNeosMod
{
    internal class NeosVideoPlayerWrapper : IAudioPlaybackable
    {
        internal const int VIDEO_PROVIDER_PLAYBACK_INDEX = 3;

        public ISyncMember PlaybackSource { get; private set; }

        public bool IsVideoPlayer => true;

        internal NeosVideoPlayerWrapper(Slot slot)
        {
            var videoProviderComponent = slot.GetComponent<VideoTextureProvider>();
            PlaybackSource = videoProviderComponent?.GetSyncMember(VIDEO_PROVIDER_PLAYBACK_INDEX);
        }
    }
}