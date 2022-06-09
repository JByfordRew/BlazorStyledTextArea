using System.Net;
using System.Text;

namespace BlazorStyledTextArea;

internal static class StyleParser
{
    internal static async Task<string> ApplyMatchedStyles(this string line, List<StyleRule> styles, int lineIndex)
    {
        line = WebUtility.HtmlEncode(line);

        styles.Where(x => x.LineIndex == lineIndex).ToList().ForEach(x => x.IsMatched = false); ;

        var linesStyles = MatchingStyleRulesForLines(styles, line, lineIndex);
        var globalStyles = GlobalStyleRules(styles);

        var matchedStyles = linesStyles.Concat(globalStyles).OrderByDescending(x => x.MatchText.Length).ToList();
        
        var tasks = matchedStyles.Select(async style =>
        {
            var appliedStyle = await ApplyMatchedStyles(line, style);
            style.IsMatched = !style.LineIndex.HasValue || !line.Equals(appliedStyle);
            line = appliedStyle;
        });

        await Task.WhenAll(tasks);

        return line;
    }

    private static async Task<string> ApplyMatchedStyles(string line, StyleRule style)
    {
        var find = style.MatchText ?? line; 
        var ignoreCase = true;
        var length = find.Length;
        var styleClassNames = string.Join(' ', style.StyleClassNames);
        var interactiveClass = string.IsNullOrWhiteSpace(style.Id) ? "" : "x p ";
        var id = string.IsNullOrWhiteSpace(style.Id) ? "" : $"id='{style.Id}' onclick='window.blazorStyledTextArea.clicked(this)' onmousemove='window.blazorStyledTextArea.mousemoved(this)' ";
        string createMarkup(string found) => $"<span {id}class='{interactiveClass}{styleClassNames}'{style.Attributes.Replace(StyleRule.MatchMarker, found)}>{found}</span>";
        string createHtml(string found) => $"<span {id}class='{interactiveClass}{styleClassNames}'>" + style.WrapHtml.Replace(StyleRule.MatchMarker, $"{found}") + "</span>";
        Func<string,string> create = string.IsNullOrWhiteSpace(style.WrapHtml) ? createMarkup : createHtml;
        var replace = create(find);
        var sizeDiff = replace.Length - find.Length;

        var sb = new StringBuilder(line);

        var instance = 0;
        var offset = 0;
        var index = line.IndexOf(find, StringComparison.InvariantCultureIgnoreCase);

        while (index > -1 && index < line.Length)
        {
            if (!line.IsMarkup(index))
            {
                if ((style.MatchInstance == null || style.MatchInstance == instance)
                    && (!style.WordsOnly || line.IsWord(index, find))
                    && (!style.StartOfLine || line[..index].Trim().Length == 0 ))
                {
                    if (ignoreCase)
                    {
                        var foundInstance = line.Substring(index, find.Length);
                        replace = create(foundInstance);
                    }

                    sb.Remove(index + offset, length).Insert(index + offset, replace);

                    offset += sizeDiff;
                }

                instance++;
            }

            index = line.IndexOf(find, index + find.Length, StringComparison.InvariantCultureIgnoreCase);
        }


        return await Task.FromResult(sb.ToString());
    }

    private static IEnumerable<StyleRule> MatchingStyleRulesForLines(
        List<StyleRule> styleRules, string line, int lineIndex) => styleRules
            .OrderByDescending(x => (x.MatchText ?? line).Length)
            .Where(x => x.LineIndex == lineIndex
                    && line.IndexOf(x.MatchText ?? line, StringComparison.InvariantCultureIgnoreCase) > -1);

    private static IEnumerable<StyleRule> GlobalStyleRules(List<StyleRule> styleRules) =>
            styleRules.Where(x => x.LineIndex == null);
}
