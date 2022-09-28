using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;

namespace Domain.Tests;

public class WorkflowTests
{
    private readonly Fixture _fixture;

    public WorkflowTests()
    {
        var fixture = new Fixture();
        fixture.Customize(new AutoMoqCustomization());
        fixture.Customizations.Add(new ElementsBuilder<Role>(Role.GetHr(), Role.GetChief(), Role.GetSpecialist()));
        fixture.Customize<Workflow>(customizationComposer => customizationComposer.FromFactory(() => new Workflow()));

        _fixture = fixture;
    }

    [Test]
    public void NewWorkflowTest()
    {
        // Act
        var workflow = _fixture.Create<Workflow>();

        // Assert
        workflow.Logs.Should().BeEmpty();
        workflow.Steps.Should().BeEmpty();
        workflow.IsCompleted.Should().BeFalse();
        workflow.CurrentStepNumber.Should().Be(0);
    }

    [Test]
    public void AddWorkflowStepByUserTest()
    {
        // Arrange
        var workflow = _fixture.Create<Workflow>();
        var user = _fixture.Create<User>();

        // Act
        workflow.AddStep(user);

        // Assert
        workflow.Steps.Should().HaveCount(1);
        var step = workflow.Steps.First();
        step.User.Should().Be(user);
        step.Order.Should().Be(0);
        step.Role.Should().BeNull();
    }

    [Test]
    public void AddWorkflowStepByRoleTest()
    {
        // Arrange
        var workflow = _fixture.Create<Workflow>();
        var role = _fixture.Create<Role>();

        // Act
        workflow.AddStep(role);

        // Assert
        workflow.Steps.Should().HaveCount(1);
        var step = workflow.Steps.First();
        step.User.Should().BeNull();
        step.Order.Should().Be(0);
        step.Role.Should().Be(role);
    }

    [Test]
    public void AddWorkflowStep_ThrowArgumentNullExceptionTest()
    {
        // Arrange
        var workflow = _fixture.Create<Workflow>();

        // Act
        var addStepByUser = () => workflow.AddStep((User)null!);
        var addStepByRole = () => workflow.AddStep((Role)null!);

        // Assert
        addStepByUser.Should().Throw<ArgumentNullException>();
        addStepByRole.Should().Throw<ArgumentNullException>();
        workflow.Steps.Should().BeEmpty();
    }

    [Test]
    public void AddWorkflowStep_ThrowInvalidOperationExceptionTest()
    {
        // Arrange
        var workflow = _fixture.Create<Workflow>();
        var user = _fixture.Create<User>();
        workflow.AddStep(user);
        workflow.Approve(user);

        // Act
        var addStepByUser = () => workflow.AddStep(_fixture.Create<User>());
        var addStepByRole = () => workflow.AddStep(_fixture.Create<Role>());

        // Assert
        addStepByUser.Should().Throw<InvalidOperationException>("Workflow completed");
        addStepByRole.Should().Throw<InvalidOperationException>("Workflow completed");
        workflow.Steps.Should().HaveCount(1);
    }

    [Test]
    public void ApproveTest()
    {
        // Arrange
        var workflow = _fixture.Create<Workflow>();
        var user = _fixture.Create<User>();
        workflow.AddStep(user);
        workflow.AddStep(_fixture.Create<User>());

        // Act
        var status = workflow.Approve(user);

        // Assert
        workflow.Logs.Should().HaveCount(1);
        workflow.CurrentStepNumber.Should().Be(1);
        workflow.IsCompleted.Should().BeFalse();
        status.Should().Be(ApplicantStatus.InProgress);
    }

    [Test]
    public void RejectedTest()
    {
        // Arrange
        var workflow = _fixture.Create<Workflow>();
        var user = _fixture.Create<User>();
        workflow.AddStep(user);
        workflow.AddStep(_fixture.Create<User>());

        // Act
        var status = workflow.Rejected(user);

        // Assert
        workflow.Logs.Should().HaveCount(1);
        workflow.CurrentStepNumber.Should().Be(1);
        workflow.IsCompleted.Should().BeTrue();
        status.Should().Be(ApplicantStatus.Rejected);
    }

    [Test]
    public void ApproveCompletedTest()
    {
        // Arrange
        var workflow = _fixture.Create<Workflow>();
        var user = _fixture.Create<User>();
        workflow.AddStep(user);

        // Act
        var status = workflow.Approve(user);

        // Assert
        workflow.Logs.Should().HaveCount(1);
        workflow.CurrentStepNumber.Should().Be(1);
        workflow.IsCompleted.Should().BeTrue();
        status.Should().Be(ApplicantStatus.Approved);
    }

    [Test]
    public void ApproveOrRejectedArgumentNullTest()
    {
        // Arrange
        var workflow = _fixture.Create<Workflow>();
        var user = _fixture.Create<User>();
        workflow.AddStep(user);

        // Act
        var approve = () => workflow.Approve(null!);
        var rejected = () => workflow.Rejected(null!);

        // Assert
        approve.Should().Throw<ArgumentNullException>();
        rejected.Should().Throw<ArgumentNullException>();
        workflow.IsCompleted.Should().BeFalse();
        workflow.CurrentStepNumber.Should().Be(0);
    }

    [Test]
    public void ApproveOrRejectedUserDoesNotHaveAccessTest()
    {
        // Arrange
        var workflow = _fixture.Create<Workflow>();
        var user = _fixture.Create<User>();
        workflow.AddStep(user);

        // Act
        var approve = () => workflow.Approve(_fixture.Create<User>());
        var rejected = () => workflow.Rejected(_fixture.Create<User>());

        // Assert
        approve.Should().Throw<MethodAccessException>("The user does not have access to this action");
        rejected.Should().Throw<MethodAccessException>("The user does not have access to this action");
        workflow.IsCompleted.Should().BeFalse();
        workflow.CurrentStepNumber.Should().Be(0);
    }

    [Test]
    public void ApproveOrRejectedStepsNotFoundTest()
    {
        // Arrange
        var workflow = _fixture.Create<Workflow>();
        var user = _fixture.Create<User>();

        // Act
        var approve = () => workflow.Approve(user);
        var rejected = () => workflow.Rejected(user);

        // Assert
        approve.Should()
            .Throw<InvalidOperationException>($"Invalid workflow, step {workflow.CurrentStepNumber} not found");
        rejected.Should()
            .Throw<InvalidOperationException>($"Invalid workflow, step {workflow.CurrentStepNumber} not found");
        workflow.IsCompleted.Should().BeFalse();
        workflow.CurrentStepNumber.Should().Be(0);
    }
}
