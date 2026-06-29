using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EducationalCenter.Domain.Sessions;
using EducationalCenter.Domain.Shared;

namespace EducationalCenter.Application.Sessions.Commands
{
  public record CompleteSessionCommand(Guid Id) : IRequest;

  public class CompleteSessionCommandHandler : IRequestHandler<CompleteSessionCommand>
  {
    private readonly ISessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteSessionCommandHandler(ISessionRepository sessionRepository, IUnitOfWork unitOfWork)
    {
      _sessionRepository = sessionRepository;
      _unitOfWork = unitOfWork;
    }

    public async Task Handle(CompleteSessionCommand request, CancellationToken cancellationToken)
    {
      var session = await _sessionRepository.GetByIdAsync(request.Id, cancellationToken);
      if (session == null)
      {
        throw new Exception("Session not found.");
      }

      session.Complete();

      _sessionRepository.Update(session);
      await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
  }
}
