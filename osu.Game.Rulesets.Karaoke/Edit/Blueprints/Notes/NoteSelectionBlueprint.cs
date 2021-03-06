﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Karaoke.Edit.Blueprints.Notes.Components;
using osu.Game.Rulesets.Karaoke.Edit.Notes;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Karaoke.Edit.Blueprints.Notes
{
    public class NoteSelectionBlueprint : KaraokeSelectionBlueprint<Note>
    {
        public new DrawableNote DrawableObject => (DrawableNote)base.DrawableObject;

        [Resolved]
        private NoteManager noteManager { get; set; }

        public override MenuItem[] ContextMenuItems => new MenuItem[]
        {
            new OsuMenuItem(HitObject.Display ? "Hide" : "Show", HitObject.Display ? MenuItemType.Destructive : MenuItemType.Standard, () => noteManager.ChangeDisplay(HitObject, !HitObject.Display)),
            new OsuMenuItem("Split", MenuItemType.Destructive, () => noteManager.SplitNote(HitObject)),
        };

        protected override void Update()
        {
            base.Update();

            Size = DrawableObject.DrawSize;
            Position = Parent.ToLocalSpace(DrawableObject.ToScreenSpace(Vector2.Zero));
        }

        public NoteSelectionBlueprint(Note note)
            : base(note)
        {
            AddInternal(new EditBodyPiece
            {
                RelativeSizeAxes = Axes.Both
            });
        }
    }
}
