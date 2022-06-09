using Xunit;
using BlazorStyledTextArea;

namespace BlazorStyledTextAreaTest.TextExtensionsTest;

public class IndexOfTextInMarkupTest
{
    [Theory]
    [InlineData(null, 0, 0)]
    [InlineData("", 0, 0)]
    [InlineData("all text", 4, 0)]
    [InlineData("<span>text</span>", 2, 6)]
    [InlineData("<span>text</span>text", 5, 13)]
    [InlineData("text<span>text</span>", 2, 0)]
    [InlineData("text<span>text</span>", 5, 6)]
    [InlineData("text&lt;text", 7, 3)]
    [InlineData("&lt;text", 3, 3)]
    [InlineData("text", 3, 0)]
    [InlineData("text", 4, 0)]
    [InlineData("<a><b>text", 2, 6)]
    [InlineData("a&lt;&gt;&lt;&gt;bc", 6, 12)]
    [InlineData("a&lt;&gt;&lt;&gt;bc", 2, 3)]
    public void ShouldAddMarkupOffset(string markup, int indexOf, int markupOffset)
    {
        Assert.Equal(indexOf + markupOffset, markup.IndexOfTextWithMarkup(indexOf));
    }
}

