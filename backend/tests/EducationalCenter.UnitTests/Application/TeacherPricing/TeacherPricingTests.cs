using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using EducationalCenter.Domain.Shared;
using EducationalCenter.Domain.Grades;
using EducationalCenter.Domain.Teachers;
using EducationalCenter.Domain.TeacherPricing;
using EducationalCenter.Application.TeacherPricing.Commands;

namespace EducationalCenter.UnitTests.Application.TeacherPricing
{
  public class TeacherPricingTests
  {
    private readonly Mock<ITeacherPricingRepository> _pricingRepositoryMock;
    private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
    private readonly Mock<IGradeRepository> _gradeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public TeacherPricingTests()
    {
      _pricingRepositoryMock = new Mock<ITeacherPricingRepository>();
      _teacherRepositoryMock = new Mock<ITeacherRepository>();
      _gradeRepositoryMock = new Mock<IGradeRepository>();
      _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    // Helper methods using official static factories and reflection to set IDs for isolation
    private Teacher CreateTeacher(Guid id, string name)
    {
      var teacher = Teacher.Create(name);
      SetProperty(teacher, "TeacherId", id);
      return teacher;
    }

    private Grade CreateGrade(Guid id, string name)
    {
      var grade = Grade.Create(name);
      SetProperty(grade, "GradeId", id);
      return grade;
    }

    private static void SetProperty(object obj, string propertyName, object value)
    {
      var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
      if (property != null && property.CanWrite)
      {
        property.SetValue(obj, value);
        return;
      }

      var field = obj.GetType().GetField($"<{propertyName}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
      if (field != null)
      {
        field.SetValue(obj, value);
      }
    }

    // ====================================================================
    // Domain Logic Tests
    // ====================================================================

    [Fact]
    public void Create_WithValidData_ShouldInitializeCorrectly()
    {
      var teacherId = Guid.NewGuid();
      var gradeId = Guid.NewGuid();
      var price = 150.00m;

      var result = TeacherGradePrice.Create(teacherId, gradeId, price);

      Assert.NotEqual(Guid.Empty, result.Id);
      Assert.Equal(teacherId, result.TeacherId);
      Assert.Equal(gradeId, result.GradeId);
      Assert.Equal(price, result.SessionPrice);
      Assert.True(result.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void Create_WithInvalidPrice_ShouldThrowArgumentException(decimal invalidPrice)
    {
      var teacherId = Guid.NewGuid();
      var gradeId = Guid.NewGuid();

      var exception = Assert.Throws<ArgumentException>(() =>
          TeacherGradePrice.Create(teacherId, gradeId, invalidPrice));

      Assert.Contains("Session price must be greater than zero.", exception.Message);
    }

    [Fact]
    public void UpdatePrice_WithValidPrice_ShouldUpdateValueAndSetUpdatedAt()
    {
      var pricing = TeacherGradePrice.Create(Guid.NewGuid(), Guid.NewGuid(), 100m);
      var newPrice = 200m;

      pricing.UpdatePrice(newPrice);

      Assert.Equal(newPrice, pricing.SessionPrice);
      Assert.NotNull(pricing.UpdatedAt);
    }

    // ====================================================================
    // Command Handlers Tests
    // ====================================================================

    [Fact]
    public async Task Handle_CreateCommand_WithValidRequest_ShouldSaveAndReturnId()
    {
      var teacherId = Guid.NewGuid();
      var gradeId = Guid.NewGuid();
      var command = new CreateTeacherGradePriceCommand(teacherId, gradeId, 150m);

      var mockTeacher = CreateTeacher(teacherId, "Teacher Name");
      var mockGrade = CreateGrade(gradeId, "Grade Name");

      _teacherRepositoryMock.Setup(x => x.GetByIdAsync(teacherId))
          .ReturnsAsync(mockTeacher);

      _gradeRepositoryMock.Setup(x => x.GetByIdAsync(gradeId))
          .ReturnsAsync(mockGrade);

      _pricingRepositoryMock.Setup(x => x.ExistsAsync(teacherId, gradeId, It.IsAny<CancellationToken>()))
          .ReturnsAsync(false);

      var handler = new CreateTeacherGradePriceCommandHandler(
          _pricingRepositoryMock.Object,
          _teacherRepositoryMock.Object,
          _gradeRepositoryMock.Object,
          _unitOfWorkMock.Object);

      var result = await handler.Handle(command, CancellationToken.None);

      Assert.NotEqual(Guid.Empty, result);
      _pricingRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TeacherGradePrice>(), It.IsAny<CancellationToken>()), Times.Once);
      _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateCommand_WhenTeacherNotFound_ShouldThrowException()
    {
      var command = new CreateTeacherGradePriceCommand(Guid.NewGuid(), Guid.NewGuid(), 150m);

      _teacherRepositoryMock.Setup(x => x.GetByIdAsync(command.TeacherId))
          .ReturnsAsync((Teacher)null!);

      var handler = new CreateTeacherGradePriceCommandHandler(
          _pricingRepositoryMock.Object,
          _teacherRepositoryMock.Object,
          _gradeRepositoryMock.Object,
          _unitOfWorkMock.Object);

      var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
      Assert.Equal("Teacher not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_CreateCommand_WhenPricingAlreadyExists_ShouldThrowException()
    {
      var teacherId = Guid.NewGuid();
      var gradeId = Guid.NewGuid();
      var command = new CreateTeacherGradePriceCommand(teacherId, gradeId, 150m);

      var mockTeacher = CreateTeacher(teacherId, "Teacher Name");
      var mockGrade = CreateGrade(gradeId, "Grade Name");

      _teacherRepositoryMock.Setup(x => x.GetByIdAsync(teacherId))
          .ReturnsAsync(mockTeacher);

      _gradeRepositoryMock.Setup(x => x.GetByIdAsync(gradeId))
          .ReturnsAsync(mockGrade);

      _pricingRepositoryMock.Setup(x => x.ExistsAsync(teacherId, gradeId, It.IsAny<CancellationToken>()))
          .ReturnsAsync(true);

      var handler = new CreateTeacherGradePriceCommandHandler(
          _pricingRepositoryMock.Object,
          _teacherRepositoryMock.Object,
          _gradeRepositoryMock.Object,
          _unitOfWorkMock.Object);

      var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
      Assert.Equal("Pricing for this teacher and grade already exists.", exception.Message);
    }
  }
}
