namespace BlazorStyledTextArea;

public struct Coords
{
    public decimal X {  get; }   
    public decimal Y {  get; }

    public Coords(decimal x, decimal y)
    {
        X = x;
        Y = y;
    }
}
