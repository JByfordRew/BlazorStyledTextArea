using Xunit;
using BlazorStyledTextArea;

namespace BlazorStyledTextAreaTest.TextExtensionsTest;

public class RemoveMarkupTest
{
    [Theory]
    [InlineData(null, 0, 0, null)]
    [InlineData("", 0, 0, "")]
    [InlineData("just some text", 5, 9, "just some text")]
    [InlineData("<span>remove all markup</span>", 0, 17, "remove all markup<span></span>")]
    [InlineData("remove <span>all</span> markup", 7, 10, "remove all<span></span> markup")]
    [InlineData("leave <span>markup</span> unaltered", 0, 5, "leave <span>markup</span> unaltered")]
    [InlineData("remove <span>all</span> instances <span>of</span> markup", 0, 29, "remove all instances of markup")]
    [InlineData("remove <span>partial</span> markup", 0, 10, "remove par<span>tial</span> markup")]
    [InlineData("remove <span>partial</span> markup", 10, 21, "remove <span>par</span>tial markup")]
    [InlineData("remove <span>only this text</span> markup", 12, 16, "remove <span>only </span>this<span> text</span> markup")]
    [InlineData("update <span data-v='partial'>partial</span> data-v attribute", 0, 10, "update par<span data-v='tial'>tial</span> data-v attribute")]
    [InlineData("update <span data-v='partial'>partial</span> data-v attribute", 10, 30, "update <span data-v='par'>par</span>tial data-v attribute")]
    [InlineData("update <span data-v='partial'>partial</span> data-v attribute", 10, 12, "update <span data-v='par'>par</span>ti<span data-v='al'>al</span> data-v attribute")]
    [InlineData("update <span data-v=\"partial\">partial</span> data-v attribute", 10, 12, "update <span data-v=\"par\">par</span>ti<span data-v=\"al\">al</span> data-v attribute")]
    [InlineData("nested <a> one <b> two </b> one </a> nested", 14, 15, "nested <a> one <b> t</b></a>w<a><b>o </b> one </a> nested")]
    [InlineData("nested <a> one <b> two </b> one </a> nested", 9, 15, "nested <a> o</a>ne  tw<a><b>o </b> one </a> nested")]
    [InlineData("nested <a> one <b> two </b> one </a> nested", 0, 15, "nested  one  tw<a><b>o </b> one </a> nested")]
    [InlineData("nested <a> one <b> two </b> one </a> nested", 15, 29, "nested <a> one <b> tw</b></a>o  one  nested")]
    [InlineData("<span class=\"b\">'bold'</span>", 2, 4, "<span class=\"b\">'b</span>ol<span class=\"b\">d'</span>")]
    [InlineData("<a>abc</a> <b>def</b>", 0, 4, "abc <b>def</b>")]
    [InlineData("<span class=\"b\">'bold'</span> <span class=\"o code\">'rounded'</span> fff", 10, 13, "<span class=\"b\">'bold'</span> <span class=\"o code\">'ro</span>und<span class=\"o code\">ed'</span> fff")]
    [InlineData("<a>abc</a> <b>def</b>", 4, 8, "<a>abc</a> def")]
    [InlineData("<span class=\"o code\">StyleRule.Words(\"<span class=\"b u\">words to style</span>\",\"bold underline\")</span>, is the", 41, 44, "<span class=\"o code\">StyleRule.Words(\"<span class=\"b u\">words to style</span>\",\"bold un</span>der<span class=\"o code\">line\")</span>, is the")]
    [InlineData("<span class=\"r\" data-v=\"● \">* </span>", 0, 1, "*<span class=\"r\" data-v=\" \"> </span>")]
    public void ShouldRemoveMarkupBetweenIndexes(
        string markup, int startIndex, int endIndex, string expected)
    {
        Assert.Equal(expected, markup.RemoveMarkupBetween(startIndex, endIndex));
    }
}

