using Microsoft.Extensions.DependencyInjection;

namespace HeadToHead_Latelier.Common.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Initialise les services HeadToHead dans le conteneur IoC
        /// </summary>
        /// <param name="services">Le service collection</param>
        /// <returns>Le service collection pour permettre le chaînage</returns>
        public static IServiceCollection AddHeadToHeadServices(this IServiceCollection services)
        {
            return services;
        }
    }
}
