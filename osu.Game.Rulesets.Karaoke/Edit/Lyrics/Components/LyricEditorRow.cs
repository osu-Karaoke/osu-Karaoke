﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Rulesets.Karaoke.Objects;

namespace osu.Game.Rulesets.Karaoke.Edit.Lyrics.Components
{
    public abstract class LyricEditorRow : CompositeDrawable
    {
        private const int info_part_spacing = 200;
        private const int min_height = 75;
        private const int max_height = 120;

        private readonly Lyric lyric;

        protected LyricEditorRow(Lyric lyric)
        {
            this.lyric = lyric;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.3f,
                    Colour = colours.Gray7,
                },
                new GridContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    ColumnDimensions = new[]
                    {
                        new Dimension(GridSizeMode.Absolute, info_part_spacing),
                        new Dimension(GridSizeMode.Distributed)
                    },
                    RowDimensions = new[] { new Dimension(GridSizeMode.AutoSize, minSize: min_height, maxSize: max_height) },
                    Content = new[]
                    {
                        new[]
                        {
                            CreateLyricInfo(lyric),
                            CreateContent(lyric)
                        }
                    }
                }
            };
        }

        protected abstract Drawable CreateLyricInfo(Lyric lyric);

        protected abstract Drawable CreateContent(Lyric lyric);
    }
}
