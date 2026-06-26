using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EducationalCenter.Domain.TeacherPricing;

namespace EducationalCenter.Application.TeacherPricing.Queries
{
  public record GetTeacherPricingByIdQuery(Guid Id) : IRequest<TeacherPricingDto?>;

  public class GetTeacherPricingByIdQueryHandler : IRequestHandler<GetTeacherPricingByIdQuery, TeacherPricingDto?>
  {
    private readonly ITeacherPricingRepository _pricingRepository;

    public GetTeacherPricingByIdQueryHandler(ITeacherPricingRepository pricingRepository)
    {
      _pricingRepository = pricingRepository;
    }

    public async Task<TeacherPricingDto?> Handle(GetTeacherPricingByIdQuery request, CancellationToken cancellationToken)
    {
      var pricing = await _pricingRepository.GetByIdAsync(request.Id, cancellationToken);
      if (pricing == null)
      {
        return null;
      }

      return new TeacherPricingDto(
          pricing.Id,
          pricing.TeacherId,
          pricing.Teacher != null ? pricing.Teacher.Name : string.Empty,
          pricing.GradeId,
          pricing.Grade != null ? pricing.Grade.Name : string.Empty,
          pricing.SessionPrice
      );
    }
  }
}
