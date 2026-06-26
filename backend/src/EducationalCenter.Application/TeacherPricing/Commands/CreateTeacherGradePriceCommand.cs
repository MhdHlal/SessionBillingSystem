using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EducationalCenter.Domain.Grades;
using EducationalCenter.Domain.Teachers;
using EducationalCenter.Domain.TeacherPricing;
using EducationalCenter.Domain.Shared;

namespace EducationalCenter.Application.TeacherPricing.Commands
{
  public record CreateTeacherGradePriceCommand(Guid TeacherId, Guid GradeId, decimal SessionPrice) : IRequest<Guid>;

  public class CreateTeacherGradePriceCommandHandler : IRequestHandler<CreateTeacherGradePriceCommand, Guid>
  {
    private readonly ITeacherPricingRepository _pricingRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTeacherGradePriceCommandHandler(
        ITeacherPricingRepository pricingRepository,
        ITeacherRepository teacherRepository,
        IGradeRepository gradeRepository,
        IUnitOfWork unitOfWork)
    {
      _pricingRepository = pricingRepository;
      _teacherRepository = teacherRepository;
      _gradeRepository = gradeRepository;
      _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTeacherGradePriceCommand request, CancellationToken cancellationToken)
    {
      var teacher = await _teacherRepository.GetByIdAsync(request.TeacherId);
      if (teacher == null)
      {
        throw new Exception("Teacher not found.");
      }

      var grade = await _gradeRepository.GetByIdAsync(request.GradeId);
      if (grade == null)
      {
        throw new Exception("Grade not found.");
      }

      var exists = await _pricingRepository.ExistsAsync(request.TeacherId, request.GradeId, cancellationToken);
      if (exists)
      {
        throw new Exception("Pricing for this teacher and grade already exists.");
      }

      var teacherGradePrice = TeacherGradePrice.Create(request.TeacherId, request.GradeId, request.SessionPrice);

      await _pricingRepository.AddAsync(teacherGradePrice, cancellationToken);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      return teacherGradePrice.Id;
    }
  }
}
