﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers.Markdown;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Rulesets.Karaoke.Edit.Components.Containers;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Karaoke.Edit.Lyrics.Extends.Components
{
    public abstract class EditModeSection<T> : Section where T : Enum
    {
        protected sealed override string Title => "Edit mode";

        [Cached]
        private readonly OverlayColourProvider overlayColourProvider;

        [Resolved]
        private OsuColour colour { get; set; }

        private readonly EditModeButton[] buttons;
        private readonly OsuMarkdownContainer description;

        protected EditModeSection()
        {
            overlayColourProvider = new OverlayColourProvider(CreateColourScheme());

            Children = new Drawable[]
            {
                new GridContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    RowDimensions = new[]
                    {
                        new Dimension(GridSizeMode.AutoSize)
                    },
                    Content = new[]
                    {
                        buttons = CreateSelections().Select(x => new EditModeButton(x.Key, x.Value)
                        {
                            Padding = new MarginPadding { Horizontal = 5 },
                            Action = UpdateEditMode,
                        }).ToArray(),
                    }
                },
                description = new OsuMarkdownContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                }
            };

            // should wait until derived class BDL ready.
            Schedule(() =>
            {
                UpdateEditMode(DefaultMode());
            });
        }

        protected virtual void UpdateEditMode(T mode)
        {
            // update button style.
            foreach (var child in buttons)
            {
                var highLight = Equals(child.Mode, mode);
                child.Alpha = highLight ? 0.8f : 0.4f;
                child.BackgroundColour = GetColour(colour, child.Mode, highLight);

                if (!highLight)
                    continue;

                // update description text.
                var item = child.Item;
                description.Text = item.Description.Value.ToString();
            }
        }

        protected abstract OverlayColourScheme CreateColourScheme();

        protected abstract T DefaultMode();

        protected abstract Dictionary<T, EditModeSelectionItem> CreateSelections();

        protected abstract Color4 GetColour(OsuColour colour, T mode, bool active);

        private class EditModeButton : OsuButton
        {
            public new Action<T> Action;

            public T Mode { get; }

            public EditModeSelectionItem Item { get; }

            public EditModeButton(T mode, EditModeSelectionItem item)
            {
                Mode = mode;
                Item = item;

                RelativeSizeAxes = Axes.X;
                Content.CornerRadius = 15;

                item.Text.BindValueChanged(e =>
                {
                    Text = e.NewValue;
                }, true);

                item.Alert.BindValueChanged(e =>
                {
                    // todo : show / hide alert.
                }, true);

                base.Action = () => Action.Invoke(mode);
            }
        }

        protected class EditModeSelectionItem
        {
            /// <summary>
            /// The text which this <see cref="EditModeButton"/> displays.
            /// </summary>
            public readonly Bindable<LocalisableString> Text = new Bindable<LocalisableString>(string.Empty);

            /// <summary>
            /// The description which this <see cref="EditModeButton"/> displays.
            /// </summary>
            public readonly Bindable<LocalisableString> Description = new Bindable<LocalisableString>(string.Empty);

            /// <summary>
            /// The alert number which this <see cref="EditModeButton"/> displays.
            /// </summary>
            public readonly Bindable<int> Alert = new Bindable<int>();

            public EditModeSelectionItem(LocalisableString text, LocalisableString description)
            {
                Text.Value = text;
                Description.Value = description;
            }
        }
    }
}
