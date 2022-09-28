namespace Domain;

public sealed record User
{
    public Guid Id { get; private init; }
    public Role Role { get; private init; }

    public User(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);

        Id = Guid.NewGuid();
        Role = role;
    }
}
