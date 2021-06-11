﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Edit.Checks.Components;
using osu.Game.Rulesets.Karaoke.Edit.Checker;
using osu.Game.Rulesets.Karaoke.Edit.Checks.Components;
using osu.Game.Rulesets.Karaoke.Edit.Components.Containers;
using osu.Game.Rulesets.Karaoke.Graphics.Shapes;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Utils;
using osu.Game.Screens.Edit;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Karaoke.Edit.Lyrics.Extends.TimeTags
{
    public class TimeTagIssueSection : Section
    {
        protected override string Title => "Invalid time-tag";

        private BindableDictionary<Lyric, Issue[]> bindableReports;

        private TimeTagIssueTable table;

        [BackgroundDependencyLoader]
        private void load(OsuColour colour, LyricCheckerManager lyricCheckerManager)
        {
            Children = new[]
            {
                // todo : should all invalid tag number
                table = new TimeTagIssueTable(),
            };

            bindableReports = lyricCheckerManager.BindableReports.GetBoundCopy();
            bindableReports.BindCollectionChanged((a, b) =>
            {
                // todo : might have filter in here.
                var issues = bindableReports.Values.SelectMany(x => x);
                table.Issues = issues.OfType<TimeTagIssue>();
            }, true);
        }

        public class TimeTagIssueTable : TableContainer
        {
            protected const float ROW_HEIGHT = 25;

            public const int TEXT_SIZE = 14;

            [Resolved]
            private EditorClock clock { get; set; }

            [Resolved]
            private OsuColour colour { get; set; }

            private Bindable<TimeTagIssue> selectedIssue;

            protected readonly FillFlowContainer<RowBackground> BackgroundFlow;

            public TimeTagIssueTable()
            {
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;

                RowSize = new Dimension(GridSizeMode.Absolute, ROW_HEIGHT);

                AddInternal(BackgroundFlow = new FillFlowContainer<RowBackground>
                {
                    RelativeSizeAxes = Axes.Both,
                    Depth = 1f,
                    Margin = new MarginPadding { Top = ROW_HEIGHT }
                });
            }

            public IEnumerable<TimeTagIssue> Issues
            {
                set
                {
                    Content = null;
                    BackgroundFlow.Clear();

                    if (value == null)
                        return;

                    foreach (var issue in value)
                    {
                        BackgroundFlow.Add(new RowBackground(issue)
                        {
                            Action = () =>
                            {
                                selectedIssue.Value = issue;

                                if (issue.Time != null)
                                {
                                    // seek to target time-tag time if time-tag has time.
                                    clock.Seek(issue.Time.Value);
                                }

                                // todo : select target time-tag.
                            },
                        });
                    }

                    Columns = createHeaders();
                    Content = value.Select((g, i) =>
                    {
                        var lyric = g.HitObjects.FirstOrDefault() as Lyric;

                        var rows = new List<Drawable[]>();

                        foreach (var (invalidReason, timeTags) in g.InvalidTimeTags)
                        {
                            foreach (var timeTag in timeTags)
                            {
                                rows.Add(createContent(lyric, timeTag, invalidReason));
                            }
                        }

                        return rows;
                    }).SelectMany(x => x).ToArray().ToRectangular();
                }
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                // todo : might get from outside.
                // selectedIssue = verify.SelectedIssue.GetBoundCopy();

                selectedIssue = new Bindable<TimeTagIssue>();
                selectedIssue.BindValueChanged(issue =>
                {
                    foreach (var b in BackgroundFlow) b.Selected = b.Item == issue.NewValue;
                }, true);
            }

            private TableColumn[] createHeaders()
            {
                var columns = new List<TableColumn>
                {
                    new TableColumn(string.Empty, Anchor.CentreLeft, new Dimension(GridSizeMode.AutoSize, minSize: 30)),
                    new TableColumn("Lyric", Anchor.CentreLeft, new Dimension(GridSizeMode.AutoSize, minSize: 40)),
                    new TableColumn("Position", Anchor.CentreLeft, new Dimension(GridSizeMode.AutoSize, minSize: 60)),
                    new TableColumn("Message", Anchor.CentreLeft),
                };

                return columns.ToArray();
            }

            private Drawable[] createContent(Lyric lyric, TimeTag timeTag, TimeTagInvalid invalid) => new Drawable[]
            {
                new RightTriangle
                {
                    Origin = Anchor.Centre,
                    Size = new Vector2(10),
                    Colour = getInvalidColour(invalid),
                    Margin = new MarginPadding { Left = 10 },
                },
                new OsuSpriteText
                {
                    Text = $"#{lyric.Order}",
                    Font = OsuFont.GetFont(size: TEXT_SIZE, weight: FontWeight.Bold),
                    Margin = new MarginPadding { Right = 10 },
                },
                new OsuSpriteText
                {
                    Text = TimeTagUtils.FormattedString(timeTag),
                    Font = OsuFont.GetFont(size: TEXT_SIZE, weight: FontWeight.Bold),
                    Margin = new MarginPadding { Right = 10 },
                },
                new OsuSpriteText
                {
                    Text = getInvalidReason(invalid),
                    Truncate = true,
                    RelativeSizeAxes = Axes.X,
                    Font = OsuFont.GetFont(size: TEXT_SIZE, weight: FontWeight.Medium)
                },
            };

            private Color4 getInvalidColour(TimeTagInvalid invalid)
            {
                switch (invalid)
                {
                    case TimeTagInvalid.OutOfRange:
                        return colour.Red;

                    case TimeTagInvalid.Overlapping:
                        return colour.Red;

                    case TimeTagInvalid.EmptyTime:
                        return colour.Yellow;

                    default:
                        throw new IndexOutOfRangeException(nameof(invalid));
                }
            }

            private string getInvalidReason(TimeTagInvalid invalid)
            {
                switch (invalid)
                {
                    case TimeTagInvalid.OutOfRange:
                        return "Time-tag out of range.";

                    case TimeTagInvalid.Overlapping:
                        return "Time-tag overlapping.";

                    case TimeTagInvalid.EmptyTime:
                        return "Time-tag has no time.";

                    default:
                        throw new IndexOutOfRangeException(nameof(invalid));
                }
            }

            protected override Drawable CreateHeader(int index, TableColumn column) => new HeaderText(column?.Header ?? string.Empty);

            private class HeaderText : OsuSpriteText
            {
                public HeaderText(string text)
                {
                    Text = text.ToUpper();
                    Font = OsuFont.GetFont(size: 12, weight: FontWeight.Bold);
                }
            }

            public class RowBackground : OsuClickableContainer
            {
                public readonly object Item;

                private const int fade_duration = 100;

                private readonly Box hoveredBackground;

                [Resolved]
                private EditorClock clock { get; set; }

                public RowBackground(object item)
                {
                    Item = item;

                    RelativeSizeAxes = Axes.X;
                    Height = 25;

                    AlwaysPresent = true;

                    CornerRadius = 3;
                    Masking = true;

                    Children = new Drawable[]
                    {
                        hoveredBackground = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = 0,
                        },
                    };

                    // todo delete
                    Action = () =>
                    {
                    };
                }

                private Color4 colourHover;
                private Color4 colourSelected;

                [BackgroundDependencyLoader]
                private void load(LyricEditorColourProvider colourProvider)
                {
                    hoveredBackground.Colour = colourHover = colourProvider.Background1(LyricEditorMode.EditTimeTag);
                    colourSelected = colourProvider.Colour3(LyricEditorMode.EditTimeTag);
                }

                private bool selected;

                public bool Selected
                {
                    get => selected;
                    set
                    {
                        if (value == selected)
                            return;

                        selected = value;
                        updateState();
                    }
                }

                protected override bool OnHover(HoverEvent e)
                {
                    updateState();
                    return base.OnHover(e);
                }

                protected override void OnHoverLost(HoverLostEvent e)
                {
                    updateState();
                    base.OnHoverLost(e);
                }

                private void updateState()
                {
                    hoveredBackground.FadeColour(selected ? colourSelected : colourHover, 450, Easing.OutQuint);

                    if (selected || IsHovered)
                        hoveredBackground.FadeIn(fade_duration, Easing.OutQuint);
                    else
                        hoveredBackground.FadeOut(fade_duration, Easing.OutQuint);
                }
            }
        }
    }
}