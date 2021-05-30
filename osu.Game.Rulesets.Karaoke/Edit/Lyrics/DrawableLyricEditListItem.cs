﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.Karaoke.Edit.Lyrics.CaretPosition;
using osu.Game.Rulesets.Karaoke.Edit.Lyrics.LyricRows;
using osu.Game.Rulesets.Karaoke.Edit.Lyrics.Overlays;
using osu.Game.Rulesets.Karaoke.Objects;

namespace osu.Game.Rulesets.Karaoke.Edit.Lyrics
{
    public class DrawableLyricEditListItem : OsuRearrangeableListItem<Lyric>
    {
        private Box draggingBackground;
        private Box selectedBackground;
        private Box hoverBackground;
        private FillFlowContainer content;

        private readonly Bindable<Mode> bindableMode = new Bindable<Mode>();
        private readonly Bindable<ICaretPosition> bindableHoverCaretPosition = new Bindable<ICaretPosition>();
        private readonly Bindable<ICaretPosition> bindableCaretPosition = new Bindable<ICaretPosition>();

        public DrawableLyricEditListItem(Lyric item)
            : base(item)
        {
            bindableMode.BindValueChanged(e =>
            {
                // Only draggable in edit mode.
                ShowDragHandle.Value = e.NewValue == Mode.EditMode;

                // should remove overlay when switch mode.
                removeOverlay();
            }, true);

            bindableHoverCaretPosition.BindValueChanged(e =>
            {
                if (e.NewValue == null)
                {
                    hoverBackground.Hide();
                    return;
                }

                if (e.NewValue.Lyric != Model)
                {
                    hoverBackground.Hide();
                    return;
                }

                // show selected background.
                hoverBackground.Show();
            });

            bindableCaretPosition.BindValueChanged(e =>
            {
                if (e.NewValue == null)
                {
                    selectedBackground.Hide();
                    return;
                }

                if (e.NewValue.Lyric != Model)
                {
                    removeOverlay();
                    selectedBackground.Hide();
                    return;
                }

                // show selected background.
                selectedBackground.Show();

                // show not create again if contains same overlay.
                var existOverlay = getOverlay();
                if (existOverlay != null)
                    return;

                // show extra overlay if hover to current lyric.
                var editOverlay = createOverlay(bindableMode.Value, Model);
                if (editOverlay == null)
                    return;

                editOverlay.RelativeSizeAxes = Axes.X;
                content.Add(editOverlay);
                editOverlay.Show();
            });

            static EditOverlay createOverlay(Mode mode, Lyric lyric)
            {
                switch (mode)
                {
                    case Mode.EditNoteMode:
                        return new EditNoteOverlay(lyric);

                    case Mode.TimeTagEditMode:
                        return new EditTimeTagOverlay(lyric);

                    default:
                        return null;
                }
            }

            void removeOverlay()
            {
                var existOverlay = getOverlay();
                if (existOverlay == null)
                    return;

                // todo : might remove component until overlay effect end.
                content.Remove(existOverlay);
            }
        }

        private EditOverlay getOverlay()
        {
            return content?.Children.OfType<EditOverlay>().FirstOrDefault();
        }

        public float OverlayHeight => getOverlay()?.ContentHeight ?? 0;

        protected override Drawable CreateContent()
        {
            return new Container
            {
                Masking = true,
                CornerRadius = 5,
                AutoSizeAxes = Axes.Y,
                RelativeSizeAxes = Axes.X,
                Children = new Drawable[]
                {
                    draggingBackground = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0
                    },
                    hoverBackground = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0
                    },
                    selectedBackground = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0
                    },
                    content = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new EditLyricRow(Model)
                            {
                                RelativeSizeAxes = Axes.X
                            }
                        }
                    }
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours, ILyricEditorState state)
        {
            draggingBackground.Colour = colours.YellowDarker;
            selectedBackground.Colour = colours.PinkDark;
            hoverBackground.Colour = colours.PinkLight;

            bindableMode.BindTo(state.BindableMode);
            bindableHoverCaretPosition.BindTo(state.BindableHoverCaretPosition);
            bindableCaretPosition.BindTo(state.BindableCaretPosition);
        }

        protected override bool OnDragStart(DragStartEvent e)
        {
            if (!base.OnDragStart(e))
                return false;

            draggingBackground.Show();
            return true;
        }

        protected override void OnDragEnd(DragEndEvent e)
        {
            draggingBackground.Hide();
            base.OnDragEnd(e);
        }
    }
}
