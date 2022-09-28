namespace Domain;

public sealed class Workflow
{
    private readonly List<WorkflowStep> _steps;
    private readonly List<WorkflowLog> _workflowLogs;

    internal Workflow()
    {
        _steps = new List<WorkflowStep>();
        _workflowLogs = new List<WorkflowLog>();
        CurrentStepNumber = 0;
        IsCompleted = false;
    }

    public int CurrentStepNumber { get; private set; }
    public bool IsCompleted { get; private set; }
    public IReadOnlyCollection<WorkflowLog> Logs => _workflowLogs;
    public IReadOnlyCollection<WorkflowStep> Steps => _steps;

    public void AddStep(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        CheckCompleted();

        _steps.Add(new WorkflowStep(_steps.Count, user));
    }

    public void AddStep(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);
        CheckCompleted();

        _steps.Add(new WorkflowStep(_steps.Count, role));
    }

    internal ApplicantStatus Approve(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        CheckCompleted();
        CheckCurrentStep(user);

        _workflowLogs.Add(new WorkflowLog());

        if (CurrentStepNumber == _steps.Count)
        {
            IsCompleted = true;
            return ApplicantStatus.Approved;
        }

        return ApplicantStatus.InProgress;
    }

    internal ApplicantStatus Rejected(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        CheckCompleted();
        CheckCurrentStep(user);

        _workflowLogs.Add(new WorkflowLog());
        IsCompleted = true;

        return ApplicantStatus.Rejected;
    }

    private void CheckCurrentStep(User user)
    {
        var step = _steps.SingleOrDefault(step => step.Order == CurrentStepNumber);
        if (step is null)
        {
            throw new InvalidOperationException($"Invalid workflow, step {CurrentStepNumber} not found");
        }

        if (!step.IsCanApprove(user))
        {
            throw new MethodAccessException("The user does not have access to this action");
        }

        CurrentStepNumber++;
    }

    private void CheckCompleted()
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException("Workflow completed");
        }
    }
}
