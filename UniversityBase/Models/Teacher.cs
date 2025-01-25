public  class Teacher
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public ICollection<Group> Groups { get; set; } = new List<Group>();

    public string FullName => $"{Name} {Surname}";
}