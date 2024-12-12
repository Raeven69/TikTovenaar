namespace TikTovenaar.Api
{
    public class PartialUser(int id, string name)
    {
        public int ID { get; } = id;
        public string Name { get; } = name;
    }
}