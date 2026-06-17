using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EducationalCenter.Domain.Students;
using EducationalCenter.Domain.Shared;
using MediatR;

namespace EducationalCenter.Application.Students.Commands
{
  public sealed record DeleteStudentCommand(Guid StudentId) : IRequest<Unit>;

  public sealed class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, Unit>
  {
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteStudentCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
    {
      _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
      _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Unit> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
      var student = await _studentRepository.GetByIdAsync(request.StudentId);
      if (student == null)
        throw new KeyNotFoundException($"Student with ID {request.StudentId} was not found.");

      _studentRepository.Delete(student);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      return Unit.Value;
    }
  }
}
