﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Karaoke.Configuration;
using osu.Game.Rulesets.Karaoke.Utils;

namespace osu.Game.Rulesets.Karaoke.Edit.Components.Menu
{
    public abstract class EnumMenu<T> : MenuItem
    {
        private readonly Bindable<T> bindableEnum = new Bindable<T>();

        protected abstract KaraokeRulesetEditSetting Setting { get; }

        protected EnumMenu(KaraokeRulesetEditConfigManager config, string text)
            : base(text)
        {
            Items = createMenuItems();

            config.BindWith(Setting, bindableEnum);
            bindableEnum.BindValueChanged(e =>
            {
                var newSelection = e.NewValue;
                Items.OfType<ToggleMenuItem>().ForEach(x =>
                {
                    var match = x.Text.Value == GetName(newSelection);
                    x.State.Value = match;
                });
            }, true);
        }

        private ToggleMenuItem[] createMenuItems()
        {
            return ValidEnums.Select(e =>
            {
                var item = new ToggleMenuItem(GetName(e), MenuItemType.Standard, _ => UpdateSelection(e));
                return item;
            }).ToArray();
        }

        protected virtual T[] ValidEnums => EnumUtils.GetValues<T>();

        protected abstract string GetName(T selection);

        protected virtual void UpdateSelection(T selection)
        {
            bindableEnum.Value = selection;
        }
    }
}
