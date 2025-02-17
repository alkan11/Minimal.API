namespace Minimal.API
{
    public record Person(string FullName);
    public sealed class PeopleService
    {
        private readonly List<Person> _people = new()
        {
            new Person("Alkan"),
            new Person("Berkem")
        };

        public IEnumerable<Person> Search(string searchTerm)
        {
            return _people.Where(x => x.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

    }
}
