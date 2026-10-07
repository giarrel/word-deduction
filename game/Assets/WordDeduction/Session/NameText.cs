using System;

namespace WordDeduction
{
    /// <summary>Unicode 17.0.0 extended graphemes, independent of the runtime's Unicode tables.</summary>
    public static partial class NameText
    {
        /// <summary>Returns the first extended grapheme, or empty for empty input. Invalid UTF-16 throws ArgumentException.</summary>
        public static string FirstElement(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            EnsureWellFormed(text);
            return text.Length == 0 ? "" : text.Substring(0, NextBoundary(text, 0));
        }

        internal static int ElementCount(string text)
        {
            int count = 0;
            for (int offset = 0; offset < text.Length; count++) offset = NextBoundary(text, offset);
            return count;
        }

        internal static bool HasVisibleBase(string text)
        {
            bool visible = false;
            for (int offset = 0; offset < text.Length;)
                visible |= (Properties(ReadScalar(text, ref offset)) & 128) == 0;
            return visible;
        }

        internal static void EnsureWellFormed(string text)
        {
            for (int offset = 0; offset < text.Length;) ReadScalar(text, ref offset);
        }

        // Values match the generated property table. UAX #29 revision 47, GB1–GB999:
        // https://www.unicode.org/reports/tr29/tr29-47.html#Grapheme_Cluster_Boundary_Rules
        private enum Break { Other, CR, LF, Control, Extend, ZWJ, Regional, Prepend, SpacingMark, L, V, T, LV, LVT }

        private static int NextBoundary(string text, int start)
        {
            int offset = start, previous = Properties(ReadScalar(text, ref offset));
            int regionalRun = (Break)(previous & 15) == Break.Regional ? 1 : 0;
            bool pictographRun = (previous & 16) != 0, joinedPictograph = false;
            int conjunct = (previous & 96) == 32 ? 1 : 0; // 1: consonant; 2: consonant then a linker.
            while (offset < text.Length)
            {
                int boundary = offset, current = Properties(ReadScalar(text, ref offset));
                var left = (Break)(previous & 15);
                var right = (Break)(current & 15);
                bool join;
                if (left == Break.CR && right == Break.LF) join = true; // GB3
                else if (Control(left) || Control(right)) join = false; // GB4, GB5
                else if (left == Break.L && (right == Break.L || right == Break.V || right == Break.LV || right == Break.LVT)) join = true; // GB6
                else if ((left == Break.LV || left == Break.V) && (right == Break.V || right == Break.T)) join = true; // GB7
                else if ((left == Break.LVT || left == Break.T) && right == Break.T) join = true; // GB8
                else if (right == Break.Extend || right == Break.ZWJ || right == Break.SpacingMark || left == Break.Prepend) join = true; // GB9, GB9a, GB9b
                else if ((current & 96) == 32 && conjunct == 2) join = true; // GB9c
                else if ((current & 16) != 0 && left == Break.ZWJ && joinedPictograph) join = true; // GB11
                else join = left == Break.Regional && right == Break.Regional && regionalRun % 2 == 1; // GB12, GB13, GB999
                if (!join) return boundary;

                // These states describe the left context at the next possible boundary.
                regionalRun = right == Break.Regional ? regionalRun + 1 : 0;
                joinedPictograph = right == Break.ZWJ && pictographRun;
                if ((current & 16) != 0) pictographRun = true;
                else if (right != Break.Extend) pictographRun = false;
                int indic = current & 96;
                if (indic == 32) conjunct = 1;
                else if (indic == 96 && conjunct != 0) conjunct = 2;
                else if (indic != 64 && indic != 96) conjunct = 0;
                previous = current;
            }
            return text.Length; // GB2; the caller supplies GB1 at start of text.
        }

        private static bool Control(Break value) => value == Break.CR || value == Break.LF || value == Break.Control;

        private static int ReadScalar(string text, ref int offset)
        {
            char first = text[offset++];
            if (!char.IsSurrogate(first)) return first;
            if (!char.IsHighSurrogate(first) || offset >= text.Length || !char.IsLowSurrogate(text[offset]))
                throw new ArgumentException("Text contains an unpaired surrogate.", nameof(text));
            return char.ConvertToUtf32(first, text[offset++]);
        }

        private static int Properties(int scalar)
        {
            int low = 0, high = PropertyRanges.Length / 3 - 1;
            while (low <= high)
            {
                int middle = (low + high) / 2, index = middle * 3;
                if (scalar < PropertyRanges[index]) high = middle - 1;
                else if (scalar > PropertyRanges[index + 1]) low = middle + 1;
                else return PropertyRanges[index + 2];
            }
            return 0;
        }
    }
}
