using NeosModLoader;
using HarmonyLib;
using FrooxEngine;
using JworkzNeosMod.Events;
using JworkzNeosMod.Abstract;
using JworkzNeosMod.Extensions;
using JworkzNeosMod.Wrappers;

namespace JworkzNeosMod
{
    public class JworkzAudioQuirksFix : NeosMod
    {
        public const int DEFAULT_APPLY_DELAY_UPDATES = 6;

        public override string Name => nameof(JworkzAudioQuirksFix);
        public override string Author => "Stiefel Jackal";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/stiefeljackal/NeosAutoAttachIsPlayingDriver";

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> KEY_ENABLE =
            new ModConfigurationKey<bool>("enabled", $"Enables the {nameof(JworkzAudioQuirksFix)} mod", () => true);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<int> KEY_APPLY_DELAY_UPDATES =
            new ModConfigurationKey<int>("applyUpdates", $"Number of updates to wait until the mod attempts to drive the AudioOutput's enabled state", () => DEFAULT_APPLY_DELAY_UPDATES);

        private static ModConfiguration Config;

        private Harmony harmony;

        public bool IsEnabled { get; private set; }

        private static WorldManager WorldManager => Engine.Current.WorldManager;

        public override void DefineConfiguration(ModConfigurationDefinitionBuilder builder)
        {
            builder.Version(Version)
                .AutoSave(false);
        }

        public override void OnEngineInit()
        {
            harmony = new Harmony($"jworkz.sjackal.{Name}");
            Config = GetConfiguration();
            Config.OnThisConfigurationChanged += OnConfigurationChanged;
            Engine.Current.OnReady += OnCurrentNeosEngineReady;

            harmony.PatchAll();
        }

        private void RefreshMod()
        {
            var isEnabled = Config.GetValue(KEY_ENABLE);
            ToggleHarmonyPatchState(isEnabled);
        }

        private void ToggleHarmonyPatchState(bool isEnabled)
        {
            if (isEnabled == IsEnabled) { return; }

            IsEnabled = isEnabled;

            if (!IsEnabled)
            {
                TurnOffMod();
            }
            else
            {
                TurnOnMod();
            }
        }

        private void TurnOffMod()
        {
            var audOutputComp = ComponentEventPublisher.RegisterComponentEventPublisher<AudioOutput>();
            audOutputComp.OnAttachComplete -= OnAudioOutputComponentAttachComplete;

            WorldManager.Worlds.Do(OnWorldRemoved);
            WorldManager.WorldAdded -= OnWorldAdded;
            WorldManager.WorldRemoved -= OnWorldRemoved;
        }

        private void TurnOnMod()
        {
            var audOutputComp = ComponentEventPublisher.RegisterComponentEventPublisher<AudioOutput>();
            audOutputComp.OnAttachComplete += OnAudioOutputComponentAttachComplete;

            WorldManager.Worlds.Do(OnWorldAdded);
            WorldManager.WorldAdded += OnWorldAdded;
            WorldManager.WorldRemoved += OnWorldRemoved;
        }

        private static void OnAudioOutputComponentAttachComplete(object sender, ComponentOnAttachCompleteEventArgs e)
        {
            var slot = e.Slot;
            var localUser = WorldManager.FocusedWorld.LocalUser;

            if (!slot.World.HostUser.IsLocalUser && !localUser.IsAllocatingUserOf(slot)) { return; }

            var updatesToWait = Config.GetValue(KEY_APPLY_DELAY_UPDATES);
            slot.RunInUpdates(updatesToWait, () =>
            {
                var outputable = new NeosMediaSlotWrapper(slot);
                IAudioPlaybackable playbackable = new NeosAudioPlayerWrapper(slot);

                if (playbackable.PlaybackSource == null)
                {
                    playbackable = new NeosVideoPlayerWrapper(slot);
                }

                if (!outputable.Slot.IsPersistent || !outputable.HasAudioSource || playbackable.PlaybackSource == null) { return; }

                outputable.DriveEnabledField(playbackable);
            });
        }

        private void OnConfigurationChanged(ConfigurationChangedEvent @event) => RefreshMod();


        private void OnCurrentNeosEngineReady() => RefreshMod();

        private void OnWorldAdded(World world) => world.ComponentAdded += OnComponentAdded;

        private void OnWorldRemoved(World world) => world.ComponentAdded -= OnComponentAdded;

        private void OnComponentAdded(Slot _, Component component) =>
            ComponentEventPublisher.RaiseOnAttachCompleteEvent(component);
    }
}
