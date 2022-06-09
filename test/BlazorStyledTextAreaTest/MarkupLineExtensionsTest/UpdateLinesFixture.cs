using BlazorStyledTextArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BlazorStyledTextAreaTest.MarkupLineExtensionsTest;

internal class UpdateLinesFixture
{
    private List<StyleRule> styleRules = new();
    private List<MarkupLine> markupLines = new();
    private string? text;
    private ChangedLines? changedLines;

    internal UpdateLinesFixture Given => this;
    internal UpdateLinesFixture When => this;
    internal UpdateLinesFixture Then => this;
    internal UpdateLinesFixture And => this;

    internal UpdateLinesFixture MarkupLines(List<MarkupLine> v)
    {
        markupLines = v;
        return this;
    }

    internal UpdateLinesFixture StyleRules(List<StyleRule> v)
    {
        styleRules = v;
        return this;
    }

    internal UpdateLinesFixture Text(string v)
    {
        text = v;
        return this;
    }

    internal UpdateLinesFixture UpdateLines()
    {
        changedLines = markupLines.UpdateLines(text!, styleRules).Result;
        return this;
    }

    internal UpdateLinesFixture InsertTextAfter(string insertedText, string after)
    {
        if (text!.Length != after.Length)
        {
            text = text.Insert(after.Length, insertedText);
        }
        else
        {
            text += insertedText;
        }

        return this;
    }

    internal UpdateLinesFixture LineCountIs(int expect)
    {
        Assert.Equal(expect, changedLines!.Lines.Count);
        Assert.Equal(expect, markupLines.Count);
        return this;
    }

    internal UpdateLinesFixture ExpiredStyleRulesCountIs(int expect)
    {
        Assert.Equal(expect, changedLines!.ExpiredStyleRules.Count);
        return this;
    }

    internal UpdateLinesFixture ExpiredStyleRulesAre(params string[] expect)
    {
        Assert.Equal(expect.OrderBy(x=>x), changedLines!.ExpiredStyleRules.Select(x=>x.Name).OrderBy(x=>x));
        ExpiredStyleRulesCountIs(expect.Length);
        return this;
    }

    internal UpdateLinesFixture LineTextIs(int index, string expect)
    {
        Assert.Equal(expect, changedLines!.Lines[index]);
        return this;
    }

    internal UpdateLinesFixture LineMarkupIs(int index, string expect)
    {
        Assert.Equal(expect, markupLines[index].Markup.ToString());
        return this;
    }

    internal UpdateLinesFixture StyleRuleIsNamed(string name, int expect)
    {
        Assert.Equal(expect, styleRules.Single(x=>x.Name == name).LineIndex);
        return this;
    }

    internal UpdateLinesFixture RemoveText(string textToRemove)
    {
        return ReplaceText(textToRemove, "");
    }

    internal UpdateLinesFixture ReplaceText(string find, string replaceWith)
    {
        text = text!.Replace(find, replaceWith);
        return this;
    }

    internal UpdateLinesFixture ChangeStartIndexIs(int expect)
    {
        Assert.Equal(expect, changedLines!.StartIndex);
        return this;
    }

    internal UpdateLinesFixture ChangeLengthIs(int expect)
    {
        Assert.Equal(expect, changedLines!.Length);
        return this;
    }
}
