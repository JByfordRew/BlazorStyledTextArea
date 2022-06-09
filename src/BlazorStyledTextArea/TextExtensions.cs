namespace BlazorStyledTextArea;

public static class TextExtensions
{
    public static bool IsMarkup(this string text, int atIndex)
    {
        var indexOfClosingChevron = text.IndexOf('>', atIndex);
        return indexOfClosingChevron > atIndex
            && ((indexOfClosingChevron < text.IndexOf('<', atIndex))
                || text.IndexOf('<', atIndex) == -1);
    }

    public static int IndexOfTextWithMarkup(this string text, int indexOf)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        var i = 0;
        var textIndex = 0;

        while (textIndex < indexOf && i < text.Length)
        {
            if (text[i] == '<')
            {
                i = text.IndexOf('>', i);
            }
            else if (text[i] == '&')
            {
                i = text.IndexOf(';', i);
                textIndex++;
            }
            else
            {
                textIndex++;
            }

            i++;
        }

        return i;
    }

    public static bool IsWord(this string text, int atIndex, string words)
    {
        var charBefore = atIndex == 0 ? ' ' : text[atIndex - 1];
        var charAfter = atIndex + words.Length == text.Length ? ' ' : text[atIndex + words.Length];
        var isCharBeforeWordBoundary = !char.IsLetterOrDigit(charBefore);
        var isCharAfterWordBoundary = !char.IsLetterOrDigit(charAfter);
        return isCharBeforeWordBoundary && isCharAfterWordBoundary;
    }

    internal static bool IsAtEndOfLine(this string text, int caretPosition)
    {
        caretPosition = Math.Min(caretPosition, text.Length);

        var i = text.IndexOf('\n', caretPosition);
        if (i == -1)
        {
            i = text.Length;
        }
        var remainingText = text[caretPosition..i];
        return remainingText.Trim().Length == 0;
    }

    internal static string LastWordTextToCaret(this string text, int caretPosition)
    {
        caretPosition = Math.Min(caretPosition, text.Length);

        int i = text[..caretPosition].ToList()
            .FindLastIndex(x => !(char.IsLetterOrDigit(x) || x == '-')) + 1;
        i = Math.Max(0, i);
        var word = text[i..caretPosition];
        return word.Trim();
    }

    public static string RemoveMarkupBetween(this string markup, int startIndex, int endIndex)
    {
        if (string.IsNullOrWhiteSpace(markup))
        {
            return markup;
        }

        startIndex = markup.IndexOfTextWithMarkup(startIndex);
        endIndex = markup.IndexOfTextWithMarkup(endIndex);

        startIndex = Math.Min(markup.Length - 1, startIndex);

        var closingTags = "";
        var openingTags = "";

        var startInHtml = markup.LastIndexOf('>', startIndex) > -1;
        if (startInHtml)
        {
            var depth = 0;
            var i = markup.IndexOf('<', startIndex);
            while (i > -1)
            {
                if (i != -1)
                {
                    depth += markup[i + 1] == '/' ? -1 : 1;

                    if (depth < 0 && markup[i + 1] == '/')
                    {
                        closingTags += markup.Substring(i, markup.IndexOf('>', i) - i + 1);
                    }
                }

                i = markup.IndexOf('<', i + 1);
            }
        }

        var endInHtml = markup.IndexOf("</", endIndex) > -1;
        if (endInHtml)
        {
            var depth = 0;
            var i = markup.LastIndexOf('<', Math.Max(0, endIndex - 1));
            while (i > -1)
            {
                if (i != -1)
                {
                    depth += markup[i + 1] == '/' ? 1 : -1;

                    if (depth < 0 && markup[i + 1] != '/')
                    {
                        var openingTag = markup.Substring(i, markup.IndexOf('>', i) - i + 1);
                        
                        if (openingTag.Contains("data-v="))
                        {
                            var textEnd = markup.IndexOf('<', endIndex);
                            var textPartial = markup[endIndex..textEnd];
                            openingTag = ReplaceDataVAttribute(openingTag, textPartial);
                        }

                        openingTags = openingTag + openingTags;
                        depth++;
                    }
                }

                i = i > 0 ? markup.LastIndexOf('<', i-1) : -1;
            }
        }

        var startMarkup = markup[..startIndex];

        var tagClosingChevron = markup.LastIndexOf('>', startIndex);
        var tagOpeningChevron = markup.LastIndexOf('<', startIndex);
        if (tagClosingChevron > tagOpeningChevron && markup[tagOpeningChevron + 1] != '/') {
            var tagComplete = markup.Substring(tagOpeningChevron, tagClosingChevron - tagOpeningChevron + 1);
            if (tagComplete.Contains("data-v="))
            {
                var textStart = markup.LastIndexOf('>', startIndex) + 1;
                var textPartial = markup[textStart..startIndex];
                var tagPartial = ReplaceDataVAttribute(tagComplete, textPartial);
                startMarkup = startMarkup.Remove(tagOpeningChevron, tagComplete.Length);
                startMarkup = startMarkup.Insert(tagOpeningChevron, tagPartial);
            }
        }

        var endMarkup = markup[endIndex..];

        var text = StripMarkup(markup, startIndex, endIndex);

        markup = startMarkup + closingTags + text + openingTags + endMarkup;

        return markup;
    }

    private static string ReplaceDataVAttribute(string openingTag, string textPartial)
    {
        var attr = "data-v";
        var quoteChar = openingTag.Contains($"{attr}='") ? '\'' : '"';
        var start = openingTag.IndexOf(attr) + attr.Length + 2;
        var end = openingTag.IndexOf(quoteChar, start);
        openingTag = openingTag.Remove(start, end - start);
        openingTag = openingTag.Insert(start, textPartial);
        return openingTag;
    }

    private static string StripMarkup(string markup, int startIndex, int endIndex)
    {
        startIndex = Math.Max(0, startIndex);
        var length = Math.Max(0, endIndex - startIndex);
        var unwrap = markup.Substring(startIndex, length);
        var parts = unwrap.Split('>').ToArray();
        unwrap = string.Join("", parts.Select(x => x[..Math.Max(0, !x.Contains('<') ? x.Length : x.IndexOf('<'))]));
        return unwrap;
    }
}
