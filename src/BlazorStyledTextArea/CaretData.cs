namespace BlazorStyledTextArea;

public class CaretData
{
    public CaretData(
        int index,
        int length,
        int row, int col, 
        decimal top, decimal left,
        bool atEndOfLine,
        string wordTextToCaret,
        decimal wordTextStartTop, decimal wordTextStartLeft)
    {
        Index = index;
        Length = length;
        Row = row;
        Col = col;
        Top = top;
        Left = left;
        AtEndOfLine = atEndOfLine;
        WordTextToCaret = wordTextToCaret;
        WordTextStartTop = wordTextStartTop;
        WordTextStartLeft = wordTextStartLeft;
    }

    /// <summary>
    /// The index of the position of the caret in the text area value.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Length of text useful when caret not moved but deletion or insertion occured.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// As text is an enumerable set of lines, this is the index of that enumerable at the caret position.
    /// </summary>
    public int Row { get; }

    /// <summary>
    /// As text is an enumerable set of lines, this is the index in the Row at the caret position.
    /// </summary>
    public int Col { get; }

    /// <summary>
    /// The 'top' (y) pixel value on the page where the caret is positioned.
    /// </summary>
    public decimal Top { get; }

    /// <summary>
    /// The 'left' (x) pixel value on the page where the caret is positioned.
    /// </summary>
    public decimal Left { get; }

    /// <summary>
    /// True, if remaining line text after the caret is whitespace or none.
    /// </summary>
    public bool AtEndOfLine { get; }

    /// <summary>
    /// The text of the word leading up to the caret position.
    /// </summary>
    public string WordTextToCaret { get; }

    /// <summary>
    /// The 'top' (y) pixel value on the page where the start of the currently typed work is positioned.
    /// </summary>
    public decimal WordTextStartTop { get; }

    /// <summary>
    /// The 'left' (x) pixel value on the page where the start of the currently typed work is positioned.
    /// </summary>
    public decimal WordTextStartLeft { get; }


    public bool Equals(CaretData? other)
    {
        if (other  == null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (this is null) return false;

        return this.Index.Equals(other.Index) &&
            this.Row.Equals(other.Row) &&
            this.Col.Equals(other.Col) &&
            this.Top.Equals(other.Top) &&
            this.Left.Equals(other.Left) &&
            this.AtEndOfLine.Equals(other.AtEndOfLine) &&
            this.WordTextToCaret.Equals(other.WordTextToCaret);
    }
}
