using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EducationalCenter.Domain.TeacherPricing;
using EducationalCenter.Domain.Shared;

namespace EducationalCenter.Application.TeacherPricing.Commands
{
  public record UpdateTeacherGradePriceCommand(Guid Id, decimal NewPrice) : IRequest;

  public class UpdateTeacherGradePriceCommandHandler : IRequestHandler<UpdateTeacherGradePriceCommand>
  {
    private readonly ITeacherPricingRepository _pricingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTeacherGradePriceCommandHandler(
        ITeacherPricingRepository pricingRepository,
        IUnitOfWork unitOfWork)
    {
      _pricingRepository = pricingRepository;
      _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateTeacherGradePriceCommand request, CancellationToken cancellationToken)
    {
      var teacherGradePrice = await _pricingRepository.GetByIdAsync(request.Id, cancellationToken);
      if (teacherGradePrice == null)
      {
        throw new Exception("Teacher pricing not found.");
      }

      teacherGradePrice.UpdatePrice(request.NewPrice);

      _pricingRepository.Update(teacherGradePrice);
      await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
  }
}
