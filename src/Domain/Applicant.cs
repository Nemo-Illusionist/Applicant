namespace Domain;

public sealed class Applicant
{
    public User Author { get; private init; } = null!;
    public Workflow Workflow { get; private init; } = null!;
    public ApplicantDocument Document { get; private init; } = null!;
    public ApplicantStatus Status { get; private set; }

    public Applicant(User author, ApplicantDocument document)
    {
        ArgumentNullException.ThrowIfNull(author);
        ArgumentNullException.ThrowIfNull(document);

        Author = author;
        Workflow = new Workflow();
        Document = document;
        Status = ApplicantStatus.InProgress;
    }

    public Applicant(User author, ApplicantDocument document, Workflow workflowTemplate)
        : this(author, document)
    {
        ArgumentNullException.ThrowIfNull(workflowTemplate);

        Workflow = new Workflow(workflowTemplate);
    }

    private Applicant()
    {
    }

    public void Approve(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        Status = Workflow.Approve(user);
    }

    public void Rejected(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        Status = Workflow.Rejected(user);
    }
}
