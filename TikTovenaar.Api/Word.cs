namespace TikTovenaar.Api
{
    public class Word(int id, string value, int length)
    {
        public int ID { get; } = id;
        public string Value { get; } = value;
        public int Length { get; } = length;
    }
}
