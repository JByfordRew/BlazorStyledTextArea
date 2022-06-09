namespace BlazorStyledTextArea;

public static class MarkupLineExtensions
{
    public static async Task<ChangedLines> UpdateLines(
        this List<MarkupLine> markupLines, string withText, List<StyleRule> styleRules)
    {
        var lines = GetLines(withText);
        var linesCountDiff = lines.Count - markupLines.Count;
        var firstChangeIndex = GetFirstChangeIndex(markupLines, lines);
        AdjustMarkupLinesCountToMatchTextLinesCount(markupLines, lines, firstChangeIndex);
        var lastChangeIndex = GetLastChangeIndex(markupLines, lines, firstChangeIndex);

        ShiftStyleRulesWithSpecificLineNumbers(styleRules, lastChangeIndex, lines.Count, linesCountDiff);

        var expiredStyleRules = await UpdateChangedLinesAddedOrAffectedDueToRemoval(markupLines, styleRules, lines, firstChangeIndex, lastChangeIndex);

        expiredStyleRules.AddRange(styleRules.Where(x => x.LineIndex.HasValue && x.LineIndex.Value >= lines.Count));

        var changes = new ChangedLines(lines, firstChangeIndex, lastChangeIndex - firstChangeIndex + 1, expiredStyleRules);

        return changes;
    }

    private static List<string> GetLines(string withText)
    {
        return (withText ?? "").Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
    }
    
    private static async Task<List<StyleRule>> UpdateChangedLinesAddedOrAffectedDueToRemoval(
        List<MarkupLine> markupLines, List<StyleRule> styleRules, List<string> lines, 
        int firstChangeOffset, int lastChangeIndex)
    {
        var expiredStyleRules = new List<StyleRule>();

        for (var i = firstChangeOffset; i <= lastChangeIndex + 1; i++)
        {
            if (i < markupLines.Count)
            {
                expiredStyleRules.AddRange(await markupLines[i].Update(lines[i], styleRules));
            }
        }

        return expiredStyleRules.Distinct().ToList();
    }

    internal static async Task UpdateLinesAffectedByNewlyAddedStyleRules(
        this List<MarkupLine> markupLines, List<StyleRule> styleRules)
    {
        styleRules.Where(x => x.IsNew && x.LineIndex.HasValue)
            .Select(x=>x.LineIndex!.Value).ToList()
            .ForEach(async x => await markupLines[x]
                .Update(markupLines[x].Text, styleRules));

        await Task.CompletedTask;
    }

    private static int GetFirstChangeIndex(
        List<MarkupLine> markupLines, List<string> lines)
    {
        var minLineIndex = Math.Min(markupLines.Count, lines.Count) - 1;

        var index = 0;

        while (index <= minLineIndex && markupLines[index].Text.Equals(lines[index])) 
        { 
            index++; 
        }

        return index;
    }

    private static int GetLastChangeIndex(
        List<MarkupLine> markupLines, List<string> lines, int firstChangeIndex)
    {
        var index = lines.Count - 1;

        while (index > firstChangeIndex && markupLines[index].Text.Equals(lines[index])) 
        {
            index--;
        }

        return index;
    }

    private static void AdjustMarkupLinesCountToMatchTextLinesCount(
        List<MarkupLine> markupLines, List<string> lines, int firstChangeOffset)
    {
        while (markupLines.Count < lines.Count)
        {
            markupLines.Insert(firstChangeOffset, MarkupLine.Placeholder(markupLines));
        }

        while (markupLines.Count > lines.Count)
        {
            markupLines.RemoveAt(firstChangeOffset);
        }
    }

    private static void ShiftStyleRulesWithSpecificLineNumbers(
        List<StyleRule> styleRules, int lastChangeIndex, int linesCount, int linesCountDiff)
    {
        var isLastChangeBeforeTheLastLine = lastChangeIndex < linesCount - 1;

        if (linesCountDiff != 0 && isLastChangeBeforeTheLastLine)
        {
            styleRules.ForEach(s =>
            {
                if (s.LineIndex.HasValue)
                {
                    var lineNumber = s.LineIndex.Value + linesCountDiff;
                    if (lineNumber >= lastChangeIndex)
                    {
                        s.LineIndex += linesCountDiff;
                    }
                }
            });
        }
    }
}
