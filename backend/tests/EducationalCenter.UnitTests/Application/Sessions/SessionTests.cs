using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using EducationalCenter.Domain.Shared;
using EducationalCenter.Domain.Sessions;
using EducationalCenter.Domain.TeacherPricing;
using EducationalCenter.Application.Sessions.Commands;

namespace EducationalCenter.UnitTests.Application.Sessions
{
  public class SessionTests
  {
    private readonly Mock<ISessionRepository> _sessionRepositoryMock;
    private readonly Mock<ITeacherPricingRepository> _pricingRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public SessionTests()
    {
      _sessionRepositoryMock = new Mock<ISessionRepository>();
      _pricingRepositoryMock = new Mock<ITeacherPricingRepository>();
      _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    private TeacherGradePrice CreatePricing(Guid teacherId, Guid gradeId, decimal price)
    {
      var pricing = TeacherGradePrice.Create(teacherId, gradeId, price);
      return pricing;
    }

    [Fact]
    public void Create_WithValidData_ShouldInitializeCorrectly()
    {
      var teacherId = Guid.NewGuid();
      var gradeId = Guid.NewGuid();
      var price = 150.00m;
      var date = DateTime.UtcNow;

      var result = Session.Create(teacherId, gradeId, date, price);

      Assert.NotEqual(Guid.Empty, result.Id);
      Assert.Equal(teacherId, result.TeacherId);
      Assert.Equal(gradeId, result.GradeId);
      Assert.Equal(price, result.UnitPrice);
      Assert.Equal(SessionStatus.Scheduled, result.Status);
    }

    [Fact]
    public void AddAttendance_WithValidData_ShouldAddToList()
    {
      var session = Session.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 100m);
      var studentId = Guid.NewGuid();

      session.AddAttendance(studentId, true, "Good performance");

      Assert.Single(session.Attendances);
      Assert.Equal(studentId, session.Attendances.First().StudentId);
      Assert.True(session.Attendances.First().IsPresent);
    }

    [Fact]
    public void AddAttendance_WhenDuplicateStudent_ShouldThrowInvalidOperationException()
    {
      var session = Session.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 100m);
      var studentId = Guid.NewGuid();
      session.AddAttendance(studentId, true);

      Assert.Throws<InvalidOperationException>(() => session.AddAttendance(studentId, false));
    }

    [Fact]
    public void Complete_WithAttendances_ShouldChangeStatusToCompleted()
    {
      var session = Session.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 100m);
      session.AddAttendance(Guid.NewGuid(), true);

      session.Complete();

      Assert.Equal(SessionStatus.Completed, session.Status);
    }

    [Fact]
    public void Complete_WithoutAttendances_ShouldThrowInvalidOperationException()
    {
      var session = Session.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 100m);

      Assert.Throws<InvalidOperationException>(() => session.Complete());
    }

    [Fact]
    public void Cancel_WhenScheduled_ShouldChangeStatusToCancelled()
    {
      var session = Session.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 100m);

      session.Cancel();

      Assert.Equal(SessionStatus.Cancelled, session.Status);
    }

    [Fact]
    public async Task Handle_CreateSessionCommand_WithValidData_ShouldSaveAndReturnId()
    {
      var teacherId = Guid.NewGuid();
      var gradeId = Guid.NewGuid();
      var studentId = Guid.NewGuid();
      var pricing = CreatePricing(teacherId, gradeId, 120m);

      _pricingRepositoryMock.Setup(x => x.GetByTeacherAndGradeAsync(teacherId, gradeId, It.IsAny<CancellationToken>()))
          .ReturnsAsync(pricing);

      var command = new CreateSessionCommand(
          teacherId,
          gradeId,
          DateTime.UtcNow,
          new List<AttendanceInput> { new AttendanceInput(studentId, true, null) }
      );

      var handler = new CreateSessionCommandHandler(
          _sessionRepositoryMock.Object,
          _pricingRepositoryMock.Object,
          _unitOfWorkMock.Object
      );

      var result = await handler.Handle(command, CancellationToken.None);

      Assert.NotEqual(Guid.Empty, result);
      _sessionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()), Times.Once);
      _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateSessionCommand_WhenPricingNotFound_ShouldThrowException()
    {
      var teacherId = Guid.NewGuid();
      var gradeId = Guid.NewGuid();

      _pricingRepositoryMock.Setup(x => x.GetByTeacherAndGradeAsync(teacherId, gradeId, It.IsAny<CancellationToken>()))
          .ReturnsAsync((TeacherGradePrice)null!);

      var command = new CreateSessionCommand(
          teacherId,
          gradeId,
          DateTime.UtcNow,
          new List<AttendanceInput>()
      );

      var handler = new CreateSessionCommandHandler(
          _sessionRepositoryMock.Object,
          _pricingRepositoryMock.Object,
          _unitOfWorkMock.Object
      );

      await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }
  }
}
