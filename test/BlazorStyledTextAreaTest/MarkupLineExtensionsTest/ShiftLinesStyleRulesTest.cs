using BlazorStyledTextArea;
using System.Collections.Generic;
using Xunit;

namespace BlazorStyledTextAreaTest.MarkupLineExtensionsTest;

public class ShiftLineStyleRulesAndExpirationTest
{
    private readonly List<StyleRule> styleRules = new()
    {
        StyleRule.Text("abc", "style class").OnLine(0).Named("line0"),
        StyleRule.Text("def", "style class").OnLine(1).Named("line1"),
        StyleRule.Text("ghi", "style class").OnLine(2).Named("line2"),
        StyleRule.Text("jkl", "style class").OnLine(3).Named("line3"),
        StyleRule.Text("mno", "style class").OnLine(4).Named("line4")
    };

    [Fact]
    public void NumberOfLinesNotChanged() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi\njkl\nmno")
        .And.StyleRules(styleRules)
        .When.UpdateLines()
        .Then.StyleRuleIsNamed("line0", 0)
        .And.StyleRuleIsNamed("line1", 1)
        .And.StyleRuleIsNamed("line2", 2)
        .And.StyleRuleIsNamed("line3", 3)
        .And.StyleRuleIsNamed("line4", 4)
        .And.ExpiredStyleRulesCountIs(0);

    [Fact]
    public void AllStyleRulesShiftedWhenInsertAtBeginning() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi\njkl\nmno")
        .And.StyleRules(styleRules)
        .And.UpdateLines()
        .When.InsertTextAfter("new line\nnewline\n", "")
        .And.UpdateLines()
        .Then.StyleRuleIsNamed("line0", 2)
        .And.StyleRuleIsNamed("line1", 3)
        .And.StyleRuleIsNamed("line2", 4)
        .And.StyleRuleIsNamed("line3", 5)
        .And.StyleRuleIsNamed("line4", 6)
        .And.ExpiredStyleRulesCountIs(0);

    [Fact]
    public void NoStyleRulesShiftedWhenInsertAtEnd() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi\njkl\nmno")
        .And.StyleRules(styleRules)
        .And.UpdateLines()
        .When.InsertTextAfter("\nnew line\nnewline", "abc\ndef\nghi\njkl\nmno")
        .And.UpdateLines()
        .Then.StyleRuleIsNamed("line0", 0)
        .And.StyleRuleIsNamed("line1", 1)
        .And.StyleRuleIsNamed("line2", 2)
        .And.StyleRuleIsNamed("line3", 3)
        .And.StyleRuleIsNamed("line4", 4)
        .And.ExpiredStyleRulesCountIs(0);

    [Fact]
    public void StyleRulesShiftedWhenInsertWithin() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi\njkl\nmno")
        .And.StyleRules(styleRules)
        .And.UpdateLines()
        .When.InsertTextAfter("\nnew line\nnewline", "abc\ndef\n")
        .And.UpdateLines()
        .Then.StyleRuleIsNamed("line0", 0)
        .And.StyleRuleIsNamed("line1", 1)
        .And.StyleRuleIsNamed("line2", 4)
        .And.StyleRuleIsNamed("line3", 5)
        .And.StyleRuleIsNamed("line4", 6)
        .And.ExpiredStyleRulesCountIs(0);

    [Fact]
    public void RemainingStyleRulesShiftedWhenLineRemovedFromBeginning() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi\njkl\nmno")
        .And.StyleRules(styleRules)
        .And.UpdateLines()
        .When.RemoveText("abc\ndef\n")
        .And.UpdateLines()
        .Then.StyleRuleIsNamed("line0", 0)
        .And.StyleRuleIsNamed("line1", 1)
        .And.StyleRuleIsNamed("line2", 0)
        .And.StyleRuleIsNamed("line3", 1)
        .And.StyleRuleIsNamed("line4", 2)
        .And.ExpiredStyleRulesAre("line0", "line1");

    [Fact]
    public void NoStyleRulesShiftedWhenLineRemovedFromEnd() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi\njkl\nmno")
        .And.StyleRules(styleRules)
        .And.UpdateLines()
        .When.RemoveText("\njkl\nmno")
        .And.UpdateLines()
        .Then.StyleRuleIsNamed("line0", 0)
        .And.StyleRuleIsNamed("line1", 1)
        .And.StyleRuleIsNamed("line2", 2)
        .And.StyleRuleIsNamed("line3", 3)
        .And.StyleRuleIsNamed("line4", 4)
        .And.ExpiredStyleRulesAre("line3", "line4");

    [Fact]
    public void StyleRulesShiftedWhenLinesRemovedFromWithin() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi\njkl\nmno")
        .And.StyleRules(styleRules)
        .And.UpdateLines()
        .When.RemoveText("\ndef\nghi")
        .And.UpdateLines()
        .Then.StyleRuleIsNamed("line0", 0)
        .And.StyleRuleIsNamed("line1", 1)
        .And.StyleRuleIsNamed("line2", 2)
        .And.StyleRuleIsNamed("line3", 1)
        .And.StyleRuleIsNamed("line4", 2)
        .And.ExpiredStyleRulesAre("line1", "line2");

    [Fact]
    public void StyleRulesShiftedWhenTwoLinesRemovedFromWithin() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi\njkl\nmno")
        .And.UpdateLines()
        .And.StyleRules(styleRules)
        .When.RemoveText("\ndef")
        .And.RemoveText("\njkl")
        .And.UpdateLines()
        .Then.StyleRuleIsNamed("line0", 0)
        .And.StyleRuleIsNamed("line1", 1)
        .And.StyleRuleIsNamed("line2", 2)
        .And.StyleRuleIsNamed("line3", 1)
        .And.StyleRuleIsNamed("line4", 2)
        .And.ExpiredStyleRulesAre("line1", "line2", "line3");

}
