using Xunit;

namespace BlazorStyledTextAreaTest.MarkupLineExtensionsTest;

public class LinesReturnTest
{
    [Fact]
    public void NoLinesAndNoCurrentMarkupLines() => new UpdateLinesFixture()
        .Given.Text("")
        .When.UpdateLines()
        .Then.LineCountIs(1)
        .And.LineTextIs(0, "")
        .And.ChangeStartIndexIs(0)
        .And.ChangeLengthIs(1);

    [Fact]
    public void LinesButNoCurrentMarkupLines() => new UpdateLinesFixture()
        .Given.Text("abc\r\ndef\rghi\n")
        .When.UpdateLines()
        .Then.LineCountIs(4)
        .And.LineTextIs(0, "abc")
        .And.LineTextIs(1, "def")
        .And.LineTextIs(2, "ghi")
        .And.LineTextIs(3, "")
        .And.ChangeStartIndexIs(0)
        .And.ChangeLengthIs(4);

    [Fact]
    public void MarkupLinesCreatedFromTextAndNoCurrentMarkupLines() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi")
        .When.UpdateLines()
        .Then.LineCountIs(3)
        .And.LineMarkupIs(0, "abc")
        .And.LineMarkupIs(1, "def")
        .And.LineMarkupIs(2, "ghi")
        .And.ChangeStartIndexIs(0)
        .And.ChangeLengthIs(3);

    [Fact]
    public void LinesAddedToEndOfText() =>  new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi")
        .And.UpdateLines()
        .When.InsertTextAfter("\ntext\nline", "abc\ndef\nghi")
        .And.UpdateLines()
        .Then.LineCountIs(5)
        .And.LineMarkupIs(3, "text")
        .And.LineMarkupIs(4, "line")
        .And.ChangeStartIndexIs(3)
        .And.ChangeLengthIs(2);

    [Fact]
    public void LinesAddedToStartOfText() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi")
        .And.UpdateLines()
        .When.InsertTextAfter("text\nline\n", "")
        .And.UpdateLines()
        .Then.LineCountIs(5)
        .And.LineMarkupIs(0, "text")
        .And.LineMarkupIs(1, "line")
        .And.ChangeStartIndexIs(0)
        .And.ChangeLengthIs(2);

    [Fact]
    public void LinesInsertedWithinText() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi")
        .And.UpdateLines()
        .When.InsertTextAfter("text\nline\n", "abc\n")
        .And.UpdateLines()
        .Then.LineCountIs(5)
        .And.LineMarkupIs(1, "text")
        .And.LineMarkupIs(2, "line")
        .And.ChangeStartIndexIs(1)
        .And.ChangeLengthIs(2);

    [Fact]
    public void LinesRemovedFromEnd() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi")
        .And.UpdateLines()
        .When.RemoveText("\nghi")
        .And.UpdateLines()
        .Then.LineCountIs(2)
        .And.LineMarkupIs(0, "abc")
        .And.LineMarkupIs(1, "def")
        .And.ChangeStartIndexIs(2)
        .And.ChangeLengthIs(0);

    [Fact]
    public void LinesRemovedFromBeginning() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi")
        .And.UpdateLines()
        .When.RemoveText("abc\n")
        .And.UpdateLines()
        .Then.LineCountIs(2)
        .And.LineMarkupIs(0, "def")
        .And.LineMarkupIs(1, "ghi")
        .And.ChangeStartIndexIs(0)
        .And.ChangeLengthIs(1);

    [Fact]
    public void LinesRemovedFromWithin() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi")
        .And.UpdateLines()
        .When.RemoveText("def\n")
        .And.UpdateLines()
        .Then.LineCountIs(2)
        .And.LineMarkupIs(0, "abc")
        .And.LineMarkupIs(1, "ghi")
        .And.ChangeStartIndexIs(1)
        .And.ChangeLengthIs(1);

    [Fact]
    public void TwoLinesRemovedFromWithin() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi\njkl")
        .And.UpdateLines()
        .When.RemoveText("def\nghi\n")
        .And.UpdateLines()
        .Then.LineCountIs(2)
        .And.LineMarkupIs(0, "abc")
        .And.LineMarkupIs(1, "jkl")
        .And.ChangeStartIndexIs(1)
        .And.ChangeLengthIs(1);

    [Fact]
    public void AllLinesRemoved() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi\njkl")
        .And.UpdateLines()
        .When.RemoveText("abc\ndef\nghi\njkl")
        .And.UpdateLines()
        .Then.LineCountIs(1)
        .And.LineMarkupIs(0, "")
        .And.ChangeStartIndexIs(0)
        .And.ChangeLengthIs(1);

    [Fact]
    public void RemoveTwoLines() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi\njkl\nmno\npqr")
        .And.UpdateLines()
        .When.RemoveText("def\n")
        .And.RemoveText("mno\n")
        .And.UpdateLines()
        .Then.LineCountIs(4)
        .And.LineMarkupIs(0, "abc")
        .And.LineMarkupIs(1, "ghi")
        .And.LineMarkupIs(2, "jkl")
        .And.LineMarkupIs(3, "pqr")
        .And.ChangeStartIndexIs(1)
        .And.ChangeLengthIs(2);

    [Fact]
    public void SingleLineChangeAtEnd() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi")
        .And.UpdateLines()
        .When.ReplaceText("ghi", "jkl")
        .And.UpdateLines()
        .Then.LineCountIs(3)
        .And.LineMarkupIs(0, "abc")
        .And.LineMarkupIs(1, "def")
        .And.LineMarkupIs(2, "jkl")
        .And.ChangeStartIndexIs(2)
        .And.ChangeLengthIs(1);

    [Fact]
    public void SingleLineChangeAtBeginning() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi")
        .And.UpdateLines()
        .When.ReplaceText("abc", "123")
        .And.UpdateLines()
        .Then.LineCountIs(3)
        .And.LineMarkupIs(0, "123")
        .And.LineMarkupIs(1, "def")
        .And.LineMarkupIs(2, "ghi")
        .And.ChangeStartIndexIs(0)
        .And.ChangeLengthIs(1);

    [Fact]
    public void ReplaceLineChangeWithin() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi")
        .And.UpdateLines()
        .When.ReplaceText("def", "123")
        .And.UpdateLines()
        .Then.LineCountIs(3)
        .And.LineMarkupIs(0, "abc")
        .And.LineMarkupIs(1, "123")
        .And.LineMarkupIs(2, "ghi")
        .And.ChangeStartIndexIs(1)
        .And.ChangeLengthIs(1);

    [Fact]
    public void ChangeTwoLineChangesLengthIncludesFirstAndSecondChange() => new UpdateLinesFixture()
        .Given.Text("abc\ndef\nghi\njkl\nmno\npqr")
        .And.UpdateLines()
        .When.ReplaceText("def", "123")
        .And.ReplaceText("mno", "456")
        .And.UpdateLines()
        .Then.LineCountIs(6)
        .And.LineMarkupIs(0, "abc")
        .And.LineMarkupIs(1, "123")
        .And.LineMarkupIs(2, "ghi")
        .And.LineMarkupIs(3, "jkl")
        .And.LineMarkupIs(4, "456")
        .And.LineMarkupIs(5, "pqr")
        .And.ChangeStartIndexIs(1)
        .And.ChangeLengthIs(4);
}
