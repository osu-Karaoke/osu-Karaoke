﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Karaoke.Beatmaps.Formats;
using osu.Game.Screens.Edit;
using System.IO;
using System.Linq;

namespace osu.Game.Rulesets.Karaoke.Edit.Import
{
    public class ImportManager : Component
    {
        public static string[] LyricFotmatExtensions { get; } = { ".lrc", ".kar" };
        public static string[] NicokaraSkinFotmatExtensions { get; } = { ".nkmproj" };

        private const string backup_lrc_name = "backup.lrc";

        [Resolved]
        private EditorBeatmap editorBeatmap { get; set; }

        [Resolved]
        private IBindable<WorkingBeatmap> beatmap { get; set; }

        public void ImportLrcFile(FileInfo info)
        {
            if (!info.Exists)
                throw new FileNotFoundException("Lyric file does not found!");

            var isFormatMatch = LyricFotmatExtensions.Contains(info.Extension);
            if (!isFormatMatch)
                throw new FileLoadException("Only .lrc or .kar karaoke file is supported now");

            var set = beatmap.Value.BeatmapSetInfo;
            var oldFile = set.Files?.FirstOrDefault(f => f.Filename == backup_lrc_name);

            using (var stream = info.OpenRead())
            {
                // todo : make a backup if has new lyric file.
                /*
                if (oldFile != null)
                    beatmaps.ReplaceFile(set, oldFile, stream, backup_lrc_name);
                else
                    beatmaps.AddFile(set, stream, backup_lrc_name);
                */

                // Import and replace all the file.
                using (var reader = new IO.LineBufferedReader(stream))
                {
                    var decoder = new LrcDecoder();
                    var lrcBeatmap = decoder.Decode(reader);

                    // todo : remove all notes and lyric
                    // or just clear all beatmap because not really sure is singer should be removed also?

                    // then re-add the lyric.
                }
            }
        }

        public void ImportNicokaraSkinFile(FileInfo info)
        {
            if (!info.Exists)
                throw new FileNotFoundException("Nicokara file does not found!");

            var isFormatMatch = NicokaraSkinFotmatExtensions.Contains(info.Extension);
            if (isFormatMatch)
                throw new FileLoadException("Nicokara's skin extension should be .nkmproj");
        }
    }
}