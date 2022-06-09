namespace BlazorStyledTextArea;

public struct Element
{
    public Element(string id, string text)
    {
        Id = id;
        Text = text;
    }

    /// <summary>
    /// the Id that was specified in the StyleRule.WithId method
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The text of the element
    /// </summary>
    public string Text { get; }
}
