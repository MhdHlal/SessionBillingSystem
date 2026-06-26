using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EducationalCenter.Domain.TeacherPricing;

namespace EducationalCenter.Application.TeacherPricing.Queries
{
  public record GetTeacherPricingQuery : IRequest<IEnumerable<TeacherPricingDto>>;

  public class GetTeacherPricingQueryHandler : IRequestHandler<GetTeacherPricingQuery, IEnumerable<TeacherPricingDto>>
  {
    private readonly ITeacherPricingRepository _pricingRepository;

    public GetTeacherPricingQueryHandler(ITeacherPricingRepository pricingRepository)
    {
      _pricingRepository = pricingRepository;
    }

    public async Task<IEnumerable<TeacherPricingDto>> Handle(GetTeacherPricingQuery request, CancellationToken cancellationToken)
    {
      var pricing = await _pricingRepository.GetAllAsync(cancellationToken);

      return pricing.Select(x => new TeacherPricingDto(
          x.Id,
          x.TeacherId,
          x.Teacher != null ? x.Teacher.Name : string.Empty,
          x.GradeId,
          x.Grade != null ? x.Grade.Name : string.Empty,
          x.SessionPrice
      ));
    }
  }
}
