﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Karaoke.Edit.Generator.Notes;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Utils;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Edit.Compose;

namespace osu.Game.Rulesets.Karaoke.Edit.Notes
{
    public class NoteManager : Component
    {
        [Resolved]
        private EditorBeatmap beatmap { get; set; }

        [Resolved]
        private IEditorChangeHandler changeHandler { get; set; }

        [Resolved]
        private IPlacementHandler placementHandler { get; set; }

        public void AutoGenerateNotes(Lyric[] lyrics)
        {
            changeHandler.BeginChange();

            // clear exist notes if from those
            var matchedNotes = beatmap.HitObjects.OfType<Note>().Where(x => lyrics.Contains(x.ParentLyric)).ToArray();
            beatmap.RemoveRange(matchedNotes);

            // todo : should get the config from setting.
            var config = new NoteGeneratorConfig();
            var generator = new NoteGenerator(config);

            foreach (var lyric in lyrics)
            {
                var notes = generator.CreateNotes(lyric);
                beatmap.AddRange(notes);
            }

            changeHandler.EndChange();
        }

        public void ChangeDisplay(Note note, bool display)
        {
            changeHandler.BeginChange();

            changeDisplay(note, display);

            changeHandler.EndChange();
        }

        public void ChangeDisplay(List<Note> notes, bool display)
        {
            changeHandler.BeginChange();

            foreach (var note in notes)
            {
                changeDisplay(note, display);
            }

            changeHandler.EndChange();
        }

        private void changeDisplay(Note note, bool display)
        {
            note.Display = display;

            // Move to center if note is not display
            if (!note.Display)
                note.Tone = new Tone();
        }

        public void SplitNote(Note note, float percentage = 0.5f)
        {
            var (firstNote, secondNote) = NotesUtils.SplitNote(note, 0.5);
            beatmap?.Add(firstNote);
            beatmap?.Add(secondNote);
            beatmap?.Remove(note);
        }

        public void CombineNote(List<Note> notes)
        {
            // todo : might use NotesUtils.CombineNote(notes);

            // Select at least two object.
            if (notes.Count < 2)
                return;

            changeHandler.BeginChange();

            // Recover end time
            var firstObject = notes.FirstOrDefault();
            if (firstObject != null)
                firstObject.Duration = notes.Sum(x => x.Duration);

            changeHandler.EndChange();

            // Delete objects
            var deleteObjects = notes.Skip(1).ToList();

            foreach (var deleteObject in deleteObjects)
            {
                placementHandler.Delete(deleteObject);
            }
        }
    }
}
