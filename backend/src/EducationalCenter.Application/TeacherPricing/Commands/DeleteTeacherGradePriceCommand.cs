using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EducationalCenter.Domain.TeacherPricing;
using EducationalCenter.Domain.Shared;

namespace EducationalCenter.Application.TeacherPricing.Commands
{
  public record DeleteTeacherGradePriceCommand(Guid Id) : IRequest;

  public class DeleteTeacherGradePriceCommandHandler : IRequestHandler<DeleteTeacherGradePriceCommand>
  {
    private readonly ITeacherPricingRepository _pricingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTeacherGradePriceCommandHandler(
        ITeacherPricingRepository pricingRepository,
        IUnitOfWork unitOfWork)
    {
      _pricingRepository = pricingRepository;
      _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTeacherGradePriceCommand request, CancellationToken cancellationToken)
    {
      var teacherGradePrice = await _pricingRepository.GetByIdAsync(request.Id, cancellationToken);
      if (teacherGradePrice == null)
      {
        throw new Exception("Teacher pricing not found.");
      }

      _pricingRepository.Delete(teacherGradePrice);
      await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
  }
}
