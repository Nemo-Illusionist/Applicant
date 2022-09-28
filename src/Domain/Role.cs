namespace Domain;

public sealed record Role
{
    private Role(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    private Role()
    {
    }

    public Guid Id { get; private init; }
    public string Name { get; private init; }

    public static Role GetHr() => new(Guid.Parse("5D24AFB6-0B3B-40CA-BBE9-72C7A5E29835"), "HR");
    public static Role GetSpecialist() => new(Guid.Parse("FE28E93E-DF64-4F52-8131-8031DD756AAF"), "Specialist");
    public static Role GetChief() => new(Guid.Parse("44589979-D182-4137-BD5C-34F21E961A6E"), "Chief");
}
