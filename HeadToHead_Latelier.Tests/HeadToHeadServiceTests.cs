using Microsoft.Extensions.DependencyInjection;
using HeadToHead_Latelier.Common.Interfaces.Services;
using HeadToHead_Latelier.API.Services;

namespace HeadToHead_Latelier.Tests
{
    public class HeadToHeadServiceTests
    {
        private IServiceProvider? _serviceProvider;

        [SetUp]
        public void Setup()
        {
            // Initialiser le conteneur IoC pour les tests
            var services = new ServiceCollection();
            services.AddScoped<IHeadToHeadService, HeadToHeadService>();
            _serviceProvider = services.BuildServiceProvider();
        }

        [TearDown]
        public void TearDown()
        {
            // Nettoyer les ressources
            (_serviceProvider as IDisposable)?.Dispose();
        }

        [Test]
        public void GetPlayer()
        {
            // Arrange
            var service = _serviceProvider?.GetRequiredService<IHeadToHeadService>();

            // Act
            var players = service?.GetPlayers();

            // Assert
            Assert.That(players, Is.Not.Null);
        }
    }
}
