using System;
using System.Threading;
using System.Threading.Tasks;
using EducationalCenter.Domain.Students;
using EducationalCenter.Domain.Grades;
using EducationalCenter.Domain.Shared;
using MediatR;

namespace EducationalCenter.Application.Students.Commands
{
  public sealed record CreateStudentCommand(string Name, Guid GradeId) : IRequest<Guid>;

  public sealed class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Guid>
  {
    private readonly IStudentRepository _studentRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStudentCommandHandler(
        IStudentRepository studentRepository,
        IGradeRepository gradeRepository,
        IUnitOfWork unitOfWork)
    {
      _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
      _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
      _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Guid> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
      // التحقق من إلزامية وجود المرحلة الدراسية أولاً (US-004)
      var gradeExists = await _gradeRepository.ExistsAsync(request.GradeId);
      if (!gradeExists)
        throw new KeyNotFoundException($"The associated Grade with ID {request.GradeId} does not exist.");

      // إنشاء الكيان الحامي لقواعد العمل
      var student = Student.Create(request.Name, request.GradeId);

      await _studentRepository.AddAsync(student);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      return student.StudentId;
    }
  }
}
