﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Karaoke.Skinning
{
    public class LegacyNotePiece : LegacyKaraokeColumnElement
    {
        private readonly IBindable<ScrollingDirection> direction = new Bindable<ScrollingDirection>();

        private Container directionContainer;
        private Sprite noteSprite;

        public LegacyNotePiece()
        {
            RelativeSizeAxes = Axes.Y;
            AutoSizeAxes = Axes.X;
        }

        [BackgroundDependencyLoader]
        private void load(ISkinSource skin, IScrollingInfo scrollingInfo)
        {
            InternalChild = directionContainer = new Container
            {
                Origin = Anchor.CentreLeft,
                RelativeSizeAxes = Axes.Y,
                AutoSizeAxes = Axes.X,
                Child = noteSprite = new Sprite { Texture = GetTexture(skin) }
            };

            direction.BindTo(scrollingInfo.Direction);
            direction.BindValueChanged(OnDirectionChanged, true);
        }

        protected override void Update()
        {
            base.Update();

            if (noteSprite.Texture != null)
            {
                var scale = DrawHeight / noteSprite.Texture.DisplayHeight;
                noteSprite.Scale = new Vector2(scale);
            }
        }

        protected virtual void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> direction)
        {
            if (direction.NewValue == ScrollingDirection.Up)
            {
                directionContainer.Anchor = Anchor.CentreLeft;
                directionContainer.Scale = new Vector2(-1, 1);
            }
            else
            {
                directionContainer.Anchor = Anchor.CentreRight;
                directionContainer.Scale = Vector2.One;
            }
        }

        protected virtual Texture GetTexture(ISkinSource skin) => GetTextureFromLookup(skin, LegacyKaraokeSkinConfigurationLookups.NoteImage);

        protected Texture GetTextureFromLookup(ISkin skin, LegacyKaraokeSkinConfigurationLookups lookup)
        {
            // TODO : Implementation
            string noteImage = GetKaraokeSkinConfig<string>(skin, lookup)?.Value
                               ?? "karaoke-note";

            return skin.GetTexture(noteImage);
        }
    }
}