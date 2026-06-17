using Microsoft.Extensions.DependencyInjection;

namespace EducationalCenter.Application
{
  public static class DependencyInjection
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
      // تسجيل MediatR والبحث التلقائي عن جميع الـ Handlers داخل هذا المشروع (Assembly)
      services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

      return services;
    }
  }
}
