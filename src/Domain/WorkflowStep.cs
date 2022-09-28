namespace Domain;

public sealed record WorkflowStep
{
    public int Order { get; }
    public Role? Role { get; }
    public User? User { get; }

    internal WorkflowStep(int order, User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        Order = order;
        User = user;
    }

    internal WorkflowStep(int order, Role role)
    {
        ArgumentNullException.ThrowIfNull(role);

        Order = order;
        Role = role;
    }

    internal WorkflowStep(WorkflowStep workflowStep)
    {
        ArgumentNullException.ThrowIfNull(workflowStep);

        Order = workflowStep.Order;
        Role = workflowStep.Role;
        User = workflowStep.User;
    }

    public bool IsCanApprove(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return User == user || Role == user.Role;
    }
}
