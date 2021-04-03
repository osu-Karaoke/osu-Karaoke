﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.Containers;
using osu.Game.Screens;

namespace osu.Game.Rulesets.Karaoke.Screens.Config
{
    public class KaraokeConfigScreen : OsuScreen
    {
        private readonly KaraokeConfigWaveContainer waves;
        private readonly KaraokeSettingsOverlay settingsOverlay;
        private readonly Header header;

        public KaraokeConfigScreen()
        {
            var backgroundColour = Color4Extensions.FromHex(@"3e3a44");

            InternalChild = waves = new KaraokeConfigWaveContainer
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = backgroundColour,
                    },
                    settingsOverlay = new KaraokeSettingsOverlay(),
                    header = new Header(),
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            // todo : should move into better place.
            header.Tabs.Items = settingsOverlay.SectionsContainer.Children;
            settingsOverlay.SectionsContainer.SelectedSection.ValueChanged += section =>
            {
                header.Tabs.Current.Value = section.NewValue;
            };

            header.Tabs.Current.ValueChanged += term =>
            {
                if (settingsOverlay.SectionsContainer.SelectedSection.Value == term.NewValue)
                    return;

                settingsOverlay.SectionsContainer.ScrollTo(term.NewValue);
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            waves.Show();
        }

        private class KaraokeConfigWaveContainer : WaveContainer
        {
            protected override bool StartHidden => true;

            public KaraokeConfigWaveContainer()
            {
                FirstWaveColour = Color4Extensions.FromHex(@"654d8c");
                SecondWaveColour = Color4Extensions.FromHex(@"554075");
                ThirdWaveColour = Color4Extensions.FromHex(@"44325e");
                FourthWaveColour = Color4Extensions.FromHex(@"392850");
            }
        }
    }
}
