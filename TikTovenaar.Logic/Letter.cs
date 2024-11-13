namespace TikTovenaar.Logic
{
    public class Letter(char? value)
    {
        public char? Value { get; } = value;
        public char? Received { get; set; }
        public bool HasGuessed { get => Received != null; }
        public bool IsCorrect { get => Value.Equals(Received); }
    }
}
