namespace BlazorStyledTextArea;

public struct RowCol
{
    public int Col { get; }
    public int Row { get; }

    public RowCol(int col, int row)
    {
        Col = col;
        Row = row;
    }
}
