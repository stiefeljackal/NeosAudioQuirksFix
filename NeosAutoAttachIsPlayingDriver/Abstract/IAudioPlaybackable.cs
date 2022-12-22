using FrooxEngine;

namespace JworkzNeosMod.Abstract
{
    internal interface IAudioPlaybackable
    {
        ISyncMember PlaybackSource { get; }

        bool IsVideoPlayer { get; }
    }
}
