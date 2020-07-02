﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osu.Framework.Testing;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Online.API;
using osu.Game.Rulesets.Karaoke.Tests.Beatmaps;
using osu.Game.Scoring;
using osu.Game.Screens;
using osu.Game.Screens.Play;
using osu.Game.Screens.Ranking;
using osu.Game.Screens.Ranking.Statistics;
using osu.Game.Tests;
using osu.Game.Tests.Visual;
using osuTK;
using osuTK.Input;

namespace osu.Game.Rulesets.Karaoke.Tests.Ranking
{
    [TestFixture]
    public class TestSceneResultsScreen : OsuManualInputManagerTestScene
    {
        protected override IBeatmap CreateBeatmap(RulesetInfo ruleset) => new TestKaraokeBeatmap(ruleset);

        private TestResultsScreen createResultsScreen() => new TestResultsScreen(new TestScoreInfo(new KaraokeRuleset().RulesetInfo));

        private UnrankedSoloResultsScreen createUnrankedSoloResultsScreen() => new UnrankedSoloResultsScreen(new TestScoreInfo(new KaraokeRuleset().RulesetInfo));

        [Test]
        public void TestResultsWithoutPlayer()
        {
            TestResultsScreen screen = null;
            OsuScreenStack stack;

            AddStep("load results", () =>
            {
                Child = stack = new OsuScreenStack
                {
                    RelativeSizeAxes = Axes.Both
                };

                stack.Push(screen = createResultsScreen());
            });
            AddUntilStep("wait for loaded", () => screen.IsLoaded);
            AddAssert("retry overlay not present", () => screen.RetryOverlay == null);
        }

        [Test]
        public void TestResultsWithPlayer()
        {
            TestResultsScreen screen = null;

            AddStep("load results", () => Child = new TestResultsContainer(screen = createResultsScreen()));
            AddUntilStep("wait for loaded", () => screen.IsLoaded);
            AddAssert("retry overlay present", () => screen.RetryOverlay != null);
        }

        [Test]
        public void TestResultsForUnranked()
        {
            UnrankedSoloResultsScreen screen = null;

            AddStep("load results", () => Child = new TestResultsContainer(screen = createUnrankedSoloResultsScreen()));
            AddUntilStep("wait for loaded", () => screen.IsLoaded);
            AddAssert("retry overlay present", () => screen.RetryOverlay != null);
        }

        [Test]
        public void TestShowHideStatisticsViaOutsideClick()
        {
            TestResultsScreen screen = null;

            AddStep("load results", () => Child = new TestResultsContainer(screen = createResultsScreen()));
            AddUntilStep("wait for loaded", () => screen.IsLoaded);

            AddStep("click expanded panel", () =>
            {
                var expandedPanel = this.ChildrenOfType<ScorePanel>().Single(p => p.State == PanelState.Expanded);
                InputManager.MoveMouseTo(expandedPanel);
                InputManager.Click(MouseButton.Left);
            });

            AddAssert("statistics shown", () => this.ChildrenOfType<StatisticsPanel>().Single().State.Value == Visibility.Visible);

            AddUntilStep("expanded panel at the left of the screen", () =>
            {
                var expandedPanel = this.ChildrenOfType<ScorePanel>().Single(p => p.State == PanelState.Expanded);
                return expandedPanel.ScreenSpaceDrawQuad.TopLeft.X - screen.ScreenSpaceDrawQuad.TopLeft.X < 150;
            });

            AddStep("click to right of panel", () =>
            {
                var expandedPanel = this.ChildrenOfType<ScorePanel>().Single(p => p.State == PanelState.Expanded);
                InputManager.MoveMouseTo(expandedPanel.ScreenSpaceDrawQuad.TopRight + new Vector2(100, 0));
                InputManager.Click(MouseButton.Left);
            });

            AddAssert("statistics hidden", () => this.ChildrenOfType<StatisticsPanel>().Single().State.Value == Visibility.Hidden);

            AddUntilStep("expanded panel in centre of screen", () =>
            {
                var expandedPanel = this.ChildrenOfType<ScorePanel>().Single(p => p.State == PanelState.Expanded);
                return Precision.AlmostEquals(expandedPanel.ScreenSpaceDrawQuad.Centre.X, screen.ScreenSpaceDrawQuad.Centre.X, 1);
            });
        }

        [Test]
        public void TestShowHideStatistics()
        {
            TestResultsScreen screen = null;

            AddStep("load results", () => Child = new TestResultsContainer(screen = createResultsScreen()));
            AddUntilStep("wait for loaded", () => screen.IsLoaded);

            AddStep("click expanded panel", () =>
            {
                var expandedPanel = this.ChildrenOfType<ScorePanel>().Single(p => p.State == PanelState.Expanded);
                InputManager.MoveMouseTo(expandedPanel);
                InputManager.Click(MouseButton.Left);
            });

            AddAssert("statistics shown", () => this.ChildrenOfType<StatisticsPanel>().Single().State.Value == Visibility.Visible);

            AddUntilStep("expanded panel at the left of the screen", () =>
            {
                var expandedPanel = this.ChildrenOfType<ScorePanel>().Single(p => p.State == PanelState.Expanded);
                return expandedPanel.ScreenSpaceDrawQuad.TopLeft.X - screen.ScreenSpaceDrawQuad.TopLeft.X < 150;
            });

            AddStep("click expanded panel", () =>
            {
                var expandedPanel = this.ChildrenOfType<ScorePanel>().Single(p => p.State == PanelState.Expanded);
                InputManager.MoveMouseTo(expandedPanel);
                InputManager.Click(MouseButton.Left);
            });

            AddAssert("statistics hidden", () => this.ChildrenOfType<StatisticsPanel>().Single().State.Value == Visibility.Hidden);

            AddUntilStep("expanded panel in centre of screen", () =>
            {
                var expandedPanel = this.ChildrenOfType<ScorePanel>().Single(p => p.State == PanelState.Expanded);
                return Precision.AlmostEquals(expandedPanel.ScreenSpaceDrawQuad.Centre.X, screen.ScreenSpaceDrawQuad.Centre.X, 1);
            });
        }

        [Test]
        public void TestShowStatisticsAndClickOtherPanel()
        {
            TestResultsScreen screen = null;

            AddStep("load results", () => Child = new TestResultsContainer(screen = createResultsScreen()));
            AddUntilStep("wait for loaded", () => screen.IsLoaded);

            ScorePanel expandedPanel = null;
            ScorePanel contractedPanel = null;

            AddStep("click expanded panel then contracted panel", () =>
            {
                expandedPanel = this.ChildrenOfType<ScorePanel>().Single(p => p.State == PanelState.Expanded);
                InputManager.MoveMouseTo(expandedPanel);
                InputManager.Click(MouseButton.Left);

                contractedPanel = this.ChildrenOfType<ScorePanel>().First(p => p.State == PanelState.Contracted && p.ScreenSpaceDrawQuad.TopLeft.X > screen.ScreenSpaceDrawQuad.TopLeft.X);
                InputManager.MoveMouseTo(contractedPanel);
                InputManager.Click(MouseButton.Left);
            });

            AddAssert("statistics shown", () => this.ChildrenOfType<StatisticsPanel>().Single().State.Value == Visibility.Visible);

            AddAssert("contracted panel still contracted", () => contractedPanel.State == PanelState.Contracted);
            AddAssert("expanded panel still expanded", () => expandedPanel.State == PanelState.Expanded);
        }

        [Test]
        public void TestFetchScoresAfterShowingStatistics()
        {
            DelayedFetchResultsScreen screen = null;

            AddStep("load results", () => Child = new TestResultsContainer(screen = new DelayedFetchResultsScreen(new TestScoreInfo(new KaraokeRuleset().RulesetInfo), 3000)));
            AddUntilStep("wait for loaded", () => screen.IsLoaded);
            AddStep("click expanded panel", () =>
            {
                var expandedPanel = this.ChildrenOfType<ScorePanel>().Single(p => p.State == PanelState.Expanded);
                InputManager.MoveMouseTo(expandedPanel);
                InputManager.Click(MouseButton.Left);
            });

            AddUntilStep("wait for fetch", () => screen.FetchCompleted);
            AddAssert("expanded panel still on screen", () => this.ChildrenOfType<ScorePanel>().Single(p => p.State == PanelState.Expanded).ScreenSpaceDrawQuad.TopLeft.X > 0);
        }

        private class TestResultsContainer : Container
        {
            [Cached(typeof(Player))]
            private readonly Player player = new TestPlayer();

            public TestResultsContainer(IScreen screen)
            {
                RelativeSizeAxes = Axes.Both;
                OsuScreenStack stack;

                InternalChild = stack = new OsuScreenStack
                {
                    RelativeSizeAxes = Axes.Both,
                };

                stack.Push(screen);
            }
        }

        private class TestResultsScreen : ResultsScreen
        {
            public HotkeyRetryOverlay RetryOverlay;

            public TestResultsScreen(ScoreInfo score)
                : base(score)
            {
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                RetryOverlay = InternalChildren.OfType<HotkeyRetryOverlay>().SingleOrDefault();
            }

            protected override APIRequest FetchScores(Action<IEnumerable<ScoreInfo>> scoresCallback)
            {
                var scores = new List<ScoreInfo>();

                for (int i = 0; i < 20; i++)
                {
                    var score = new TestScoreInfo(new KaraokeRuleset().RulesetInfo);
                    score.TotalScore += 10 - i;
                    scores.Add(score);
                }

                scoresCallback?.Invoke(scores);

                return null;
            }
        }

        private class DelayedFetchResultsScreen : TestResultsScreen
        {
            public bool FetchCompleted { get; private set; }

            private readonly double delay;

            public DelayedFetchResultsScreen(ScoreInfo score, double delay)
                : base(score)
            {
                this.delay = delay;
            }

            protected override APIRequest FetchScores(Action<IEnumerable<ScoreInfo>> scoresCallback)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(delay));

                    var scores = new List<ScoreInfo>();

                    for (int i = 0; i < 20; i++)
                    {
                        var score = new TestScoreInfo(new KaraokeRuleset().RulesetInfo);
                        score.TotalScore += 10 - i;
                        scores.Add(score);
                    }

                    scoresCallback?.Invoke(scores);

                    Schedule(() => FetchCompleted = true);
                });

                return null;
            }
        }

        private class UnrankedSoloResultsScreen : SoloResultsScreen
        {
            public HotkeyRetryOverlay RetryOverlay;

            public UnrankedSoloResultsScreen(ScoreInfo score)
                : base(score)
            {
                Score.Beatmap.OnlineBeatmapID = 0;
                Score.Beatmap.Status = BeatmapSetOnlineStatus.Pending;
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                RetryOverlay = InternalChildren.OfType<HotkeyRetryOverlay>().SingleOrDefault();
            }
        }
    }
}
