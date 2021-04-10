﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Globalization;
using osu.Framework.Bindables;
using osu.Framework.Configuration.Tracking;
using osu.Framework.Graphics.Sprites;
using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;
using osu.Game.Rulesets.Karaoke.Bindables;
using osu.Game.Rulesets.Karaoke.UI;

namespace osu.Game.Rulesets.Karaoke.Configuration
{
    public class KaraokeRulesetConfigManager : RulesetConfigManager<KaraokeRulesetSetting>
    {
        public KaraokeRulesetConfigManager(SettingsStore settings, RulesetInfo ruleset, int? variant = null)
            : base(settings, ruleset, variant)
        {
        }

        protected override void InitialiseDefaults()
        {
            base.InitialiseDefaults();

            // Visual
            SetDefault(KaraokeRulesetSetting.ScrollTime, 5000.0, 1000.0, 10000.0, 100.0);
            SetDefault(KaraokeRulesetSetting.ScrollDirection, KaraokeScrollingDirection.Left);
            SetDefault(KaraokeRulesetSetting.DisplayAlternativeText, false);
            SetDefault(KaraokeRulesetSetting.ShowCursor, false);

            // Translate
            SetDefault(KaraokeRulesetSetting.UseTranslate, true);
            SetDefault(KaraokeRulesetSetting.PreferLanguage, "en-US");

            SetDefault(KaraokeRulesetSetting.DisplayRuby, true);
            SetDefault(KaraokeRulesetSetting.DisplayRomaji, true);

            // Pitch
            SetDefault(KaraokeRulesetSetting.OverridePitchAtGameplay, false);
            SetDefault(KaraokeRulesetSetting.Pitch, 0, -10, 10);
            SetDefault(KaraokeRulesetSetting.OverrideVocalPitchAtGameplay, false);
            SetDefault(KaraokeRulesetSetting.VocalPitch, 0, -10, 10);
            SetDefault(KaraokeRulesetSetting.OverrideSaitenPitchAtGameplay, false);
            SetDefault(KaraokeRulesetSetting.SaitenPitch, 0, -10, 10);

            // Playback
            SetDefault(KaraokeRulesetSetting.OverridePlaybackSpeedAtGameplay, false);
            SetDefault(KaraokeRulesetSetting.PlaybackSpeed, 0, -10, 10);

            // Practice
            SetDefault(KaraokeRulesetSetting.PracticePreemptTime, 3000.0, 0.0, 5000.0, 100.0);

            // Device
            SetDefault(KaraokeRulesetSetting.MicrophoneDevice, "");

            // Font
            SetDefault(KaraokeRulesetSetting.MainFont, FontUsage.Default, 20f);
            SetDefault(KaraokeRulesetSetting.RubyFont, FontUsage.Default, 8f, 48f);
            SetDefault(KaraokeRulesetSetting.RomajiFont, FontUsage.Default, 8f, 48f);
            SetDefault(KaraokeRulesetSetting.ForceUseDefaultFont, false);
            SetDefault(KaraokeRulesetSetting.TranslateFont, FontUsage.Default, 10f, 48f);
            SetDefault(KaraokeRulesetSetting.ForceUseDefaultTranslateFont, false);
            SetDefault(KaraokeRulesetSetting.NoteFont, FontUsage.Default, 10f, 32f);
            SetDefault(KaraokeRulesetSetting.ForceUseDefaultNoteFont, false);
        }

        protected override void AddBindable<TBindable>(KaraokeRulesetSetting lookup, Bindable<TBindable> bindable)
        {
            switch (lookup)
            {
                case KaraokeRulesetSetting.PreferLanguage:
                    // todo : need to hve a default value here because it will cause error if object is null while saving.
                    base.AddBindable(lookup, new BindableCultureInfo(new CultureInfo("en-US")));
                    break;

                case KaraokeRulesetSetting.MainFont:
                case KaraokeRulesetSetting.RubyFont:
                case KaraokeRulesetSetting.RomajiFont:
                case KaraokeRulesetSetting.TranslateFont:
                case KaraokeRulesetSetting.NoteFont:
                    base.AddBindable(lookup, new BindableFontUsage(FontUsage.Default));
                    break;

                default:
                    base.AddBindable(lookup, bindable);
                    break;
            }
        }

        protected BindableFontUsage SetDefault(KaraokeRulesetSetting setting, FontUsage fontUsage, float? minFontSize = null, float? maxFontSize = null)
        {
            base.SetDefault(setting, fontUsage);

            // Should not use base.setDefault's value because it will return Bindable<FontUsage>, not BindableFontUsage
            var bindable = GetOriginalBindable<FontUsage>(setting);
            if (!(bindable is BindableFontUsage bindableFontUsage))
                throw new InvalidCastException(nameof(bindable));

            // Assign size restriction in here.
            if (minFontSize.HasValue) bindableFontUsage.MinFontSize = minFontSize.Value;
            if (maxFontSize.HasValue) bindableFontUsage.MaxFontSize = maxFontSize.Value;

            return bindableFontUsage;
        }

        public override TrackedSettings CreateTrackedSettings() => new TrackedSettings
        {
            new TrackedSetting<double>(KaraokeRulesetSetting.ScrollTime, v => new SettingDescription(v, "Scroll Time", $"{v}ms"))
        };
    }

    public enum KaraokeRulesetSetting
    {
        // Visual
        ScrollTime,
        ScrollDirection,
        DisplayAlternativeText,
        ShowCursor,

        // Translate
        UseTranslate,
        PreferLanguage,

        // Ruby/Romaji
        DisplayRuby,
        DisplayRomaji,

        // Pitch
        OverridePitchAtGameplay,
        Pitch,
        OverrideVocalPitchAtGameplay,
        VocalPitch,
        OverrideSaitenPitchAtGameplay,
        SaitenPitch,

        // Playback
        OverridePlaybackSpeedAtGameplay,
        PlaybackSpeed,

        // Practice
        PracticePreemptTime,

        // Device
        MicrophoneDevice,

        // Font
        MainFont,
        RubyFont,
        RomajiFont,
        ForceUseDefaultFont,
        TranslateFont,
        ForceUseDefaultTranslateFont,
        NoteFont,
        ForceUseDefaultNoteFont,
    }
}
