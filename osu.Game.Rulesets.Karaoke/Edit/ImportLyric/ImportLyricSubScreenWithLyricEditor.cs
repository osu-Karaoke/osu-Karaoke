﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Karaoke.Edit.Lyrics;

namespace osu.Game.Rulesets.Karaoke.Edit.ImportLyric
{
    public abstract class ImportLyricSubScreenWithLyricEditor : ImportLyricSubScreenWithTopNavigation
    {
        protected LyricEditor LyricEditor { get; private set; }

        [Cached]
        protected readonly LyricManager LyricManager;

        protected ImportLyricSubScreenWithLyricEditor()
        {
            AddInternal(LyricManager = new LyricManager());
        }

        protected override Drawable CreateContent()
            => LyricEditor = new ImportLyricEditor
            {
                RelativeSizeAxes = Axes.Both,
            };

        private class ImportLyricEditor : LyricEditor
        {
            [Resolved]
            private ImportLyricSubScreenStack screenStack { get; set; }

            public override void NavigateToFix(LyricEditorMode mode)
            {
                switch (mode)
                {
                    case LyricEditorMode.Typing:
                        screenStack.Pop(ImportLyricStep.EditLyric);
                        break;

                    case LyricEditorMode.Language:
                        screenStack.Pop(ImportLyricStep.AssignLanguage);
                        break;

                    case LyricEditorMode.AdjustTimeTag:
                        screenStack.Pop(ImportLyricStep.GenerateTimeTag);
                        break;

                    default:
                        throw new IndexOutOfRangeException("Oops, seems some navigation to fix case has been missing in lyric editor.");
                }
            }
        }
    }
}
