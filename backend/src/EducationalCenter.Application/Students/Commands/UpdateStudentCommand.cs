using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EducationalCenter.Domain.Students;
using EducationalCenter.Domain.Grades;
using EducationalCenter.Domain.Shared;
using MediatR;

namespace EducationalCenter.Application.Students.Commands
{
  public sealed record UpdateStudentCommand(Guid StudentId, string Name, Guid GradeId) : IRequest<Unit>;

  public sealed class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Unit>
  {
    private readonly IStudentRepository _studentRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStudentCommandHandler(
        IStudentRepository studentRepository,
        IGradeRepository gradeRepository,
        IUnitOfWork unitOfWork)
    {
      _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
      _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
      _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Unit> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
      var student = await _studentRepository.GetByIdAsync(request.StudentId);
      if (student == null)
        throw new KeyNotFoundException($"Student with ID {request.StudentId} was not found.");

      var gradeExists = await _gradeRepository.ExistsAsync(request.GradeId);
      if (!gradeExists)
        throw new KeyNotFoundException($"The associated Grade with ID {request.GradeId} does not exist.");

      student.Update(request.Name, request.GradeId);

      _studentRepository.Update(student);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      return Unit.Value;
    }
  }
}
