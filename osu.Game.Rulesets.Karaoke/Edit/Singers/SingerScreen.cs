﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays;
using osu.Game.Rulesets.Karaoke.Edit.Components;
using osu.Game.Rulesets.Karaoke.Edit.Lyrics;
using osu.Game.Rulesets.Karaoke.Edit.Singers.Detail;

namespace osu.Game.Rulesets.Karaoke.Edit.Singers
{
    public class SingerScreen : EditorSubScreen
    {
        [Cached]
        protected readonly OverlayColourProvider ColourProvider;

        [Cached]
        protected readonly SingerManager SingerManager;

        [Cached]
        protected readonly LyricManager LyricManager;

        [Cached]
        private readonly EditSingerDialog editSingerDialog;

        public SingerScreen()
        {
            ColourProvider = new OverlayColourProvider(OverlayColourScheme.Purple);
            Content.Add(SingerManager = new SingerManager());
            Content.Add(LyricManager = new LyricManager());
            Content.Add(editSingerDialog = new EditSingerDialog
            {
                Depth = -1,
            });
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            Add(new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(50),
                Child = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 10,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            Colour = colours.GreySeafoamDark,
                            RelativeSizeAxes = Axes.Both,
                        },
                        new FixedSectionsContainer<Drawable>
                        {
                            FixedHeader = new SingerScreenHeader(),
                            RelativeSizeAxes = Axes.Both,
                            Children = new[]
                            {
                                new SingerEditSection
                                {
                                    RelativeSizeAxes = Axes.Both,
                                },
                            }
                        },
                    }
                }
            });
        }

        internal class FixedSectionsContainer<T> : SectionsContainer<T> where T : Drawable
        {
            private readonly Container<T> content;

            protected override Container<T> Content => content;

            public FixedSectionsContainer()
            {
                AddInternal(content = new Container<T>
                {
                    Masking = true,
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Top = 55 }
                });
            }
        }

        internal class SingerScreenHeader : OverlayHeader
        {
            protected override OverlayTitle CreateTitle() => new TranslateScreenTitle();

            private class TranslateScreenTitle : OverlayTitle
            {
                public TranslateScreenTitle()
                {
                    Title = "singer";
                    Description = "create singer of your beatmap";
                    IconTexture = "Icons/Hexacons/social";
                }
            }
        }
    }
}
