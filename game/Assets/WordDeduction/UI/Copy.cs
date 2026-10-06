using System;
using System.Collections.Generic;
using System.Globalization;
namespace WordDeduction.UI
{
    public static class Copy
    {
        static readonly Dictionary<string, string[]> Text = new Dictionary<string, string[]> {
            {"title", new[]{"Good company.\nHidden words.","Gute Runde.\nGeheime Wörter."}},
            {"subtitle", new[]{"One phone. Everyone is in on it.","Ein Handy. Alle sind dabei."}},
            {"group", new[]{"Your group","Eure Gruppe"}},
            {"count", new[]{"{0} playing","{0} spielen mit"}},
            {"countOne", new[]{"1 playing","1 spielt mit"}},
            {"emptyTitle", new[]{"Who's joining?","Wer ist dabei?"}},
            {"emptyHint", new[]{"Add your first name below.","Tragt unten den ersten Namen ein."}},
            {"name", new[]{"Player name","Name eingeben"}},
            {"add", new[]{"Add player","Person hinzufügen"}},
            {"editHint", new[]{"Tap a name to edit. Pause anyone sitting out.","Name antippen zum Ändern. Wer aussetzt, pausiert."}},
            {"pause", new[]{"Pause","Pause"}}, {"join", new[]{"Join","Mitspielen"}},
            {"edit", new[]{"Edit {0}","{0} bearbeiten"}}, {"save", new[]{"Save","Speichern"}},
            {"cancel", new[]{"Cancel","Abbrechen"}}, {"remove", new[]{"Remove","Entfernen"}},
            {"quick", new[]{"Quick","Schnell"}}, {"classic", new[]{"Classic","Klassisch"}},
            {"quickDescription", new[]{"One clue round. One vote. Everyone stays in.","Eine Hinweisrunde. Eine Abstimmung. Alle bleiben dabei."}},
            {"classicDescription", new[]{"Find the outsiders, one elimination at a time.","Entlarvt die anderen – Runde für Runde."}},
            {"play", new[]{"Let's play","Los geht's"}},
            {"needed", new[]{"Add {0} more to play this mode.","Für diesen Modus fehlen noch {0} Personen."}},
            {"neededOne", new[]{"Add one more to play this mode.","Für diesen Modus fehlt noch eine Person."}},
            {"ready", new[]{"Your group is saved. Ready when you are.","Eure Gruppe ist gespeichert. Ihr könnt loslegen."}},
            {"undo", new[]{"Undo removal","Entfernen rückgängig"}},
            {"Removed", new[]{"Player removed.","Person entfernt."}},
            {"InvalidName", new[]{"Use 1–24 characters, without line breaks.","Bitte 1–24 Zeichen ohne Zeilenumbrüche verwenden."}},
            {"GroupFull", new[]{"40 names are saved. Remove someone to add another.","40 Namen sind gespeichert. Entferne jemanden, um weitere hinzuzufügen."}},
            {"ActiveFull", new[]{"20 people are playing. Pause someone first.","20 Personen spielen mit. Pausiere zuerst jemanden."}},
            {"AddedPaused", new[]{"Saved and paused: 20 people are already playing.","Gespeichert und pausiert: 20 Personen spielen bereits mit."}},
            {"SaveFailed", new[]{"Couldn't save. Nothing changed. Free some space and try again.","Speichern nicht möglich. Nichts wurde geändert. Schaffe Speicherplatz und versuche es erneut."}},
            {"NothingToUndo", new[]{"There is no removal to undo.","Es gibt nichts rückgängig zu machen."}},
            {"PlayerNotFound", new[]{"That player is no longer in the group.","Diese Person ist nicht mehr in der Gruppe."}},
            {"InvalidSetting", new[]{"That setting isn't available.","Diese Einstellung ist nicht verfügbar."}},
            {"StorageBlocked", new[]{"Your saved group needs attention before you can continue.","Bitte prüfe den Speicherhinweis, bevor es weitergeht."}},
            {"RecoveredBackup", new[]{"The latest save was damaged. We restored the previous saved group. Please check the names.","Der letzte Speicherstand war beschädigt. Die vorherige Gruppe wurde wiederhergestellt. Prüfe bitte die Namen."}},
            {"DamagedData", new[]{"The saved group couldn't be recovered. Keep the damaged files and start a new group?", "Die gespeicherte Gruppe konnte nicht wiederhergestellt werden. Beschädigte Dateien behalten und eine neue Gruppe anfangen?"}},
            {"NewerVersion", new[]{"This save comes from a newer app version. Update the app to keep playing. Your files are untouched.","Dieser Speicherstand stammt aus einer neueren App-Version. Aktualisiere die App zum Weiterspielen. Die Dateien bleiben erhalten."}},
            {"ReadFailed", new[]{"Your saved group isn't accessible. Close and reopen the app to retry. Your files are untouched.","Auf die gespeicherte Gruppe kann nicht zugegriffen werden. Schließe und öffne die App erneut. Die Dateien bleiben erhalten."}},
            {"startFresh", new[]{"Keep files & start fresh","Dateien behalten & neu anfangen"}},
            {"english", new[]{"English","Englisch"}}, {"german", new[]{"German","Deutsch"}}
        };
        public static string Get(Language language, string key, params object[] arguments)
        {
            if (!Text.TryGetValue(key, out var values)) throw new ArgumentException("Missing UI copy: " + key);
            return string.Format(language == Language.German ? CultureInfo.GetCultureInfo("de-DE") : CultureInfo.GetCultureInfo("en-US"), values[language == Language.German ? 1 : 0], arguments);
        }
    }
}
