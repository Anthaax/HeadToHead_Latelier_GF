using Microsoft.Extensions.DependencyInjection;
using HeadToHead_Latelier.Common.Interfaces.Services;
using HeadToHead_Latelier.API.Services;
using HeadToHead_Latelier.Common.Interfaces.Readers;
using HeadToHead_Latelier.Reader;

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
            services.AddScoped<IHeadToHeadReader, HeadToHeadReader>();
            _serviceProvider = services.BuildServiceProvider();
        }

        [TearDown]
        public void TearDown()
        {
            // Nettoyer les ressources
            (_serviceProvider as IDisposable)?.Dispose();
        }

        [Test]
        public void GetPlayer_ShouldNotReturnNull()
        {
            // Arrange
            var service = _serviceProvider?.GetRequiredService<IHeadToHeadService>();

            // Act
            var players = service?.GetPlayers();

            // Assert
            Assert.That(players, Is.Not.Null);
        }

        [Test]
        public void GetPlayers_ShouldReturnPlayersInCorrectOrder()
        {
            // Arrange
            var service = _serviceProvider?.GetRequiredService<IHeadToHeadService>();
            // Act
            var players = service?.GetPlayers();
            // Assert
            Assert.That(players, Is.Not.Null);
            var ranks = players.Select(p => p.Data.Rank).ToList();
            var sortedRanks = ranks.OrderBy(r => r).ToList();
            Assert.That(ranks, Is.EqualTo(sortedRanks));
        }

        [Test]
        public void GetPlayerById_WithValidId_ShouldReturnPlayer()
        {
            // Arrange
            var service = _serviceProvider?.GetRequiredService<IHeadToHeadService>();
            var players = service?.GetPlayers()?.ToList();
            var firstPlayer = players?.FirstOrDefault();

            // Act
            var player = service?.GetPlayerById(firstPlayer?.Id ?? 0);

            // Assert
            Assert.That(player, Is.Not.Null);
            Assert.That(player?.Id, Is.EqualTo(firstPlayer?.Id));
        }

        [Test]
        public void GetPlayerById_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var service = _serviceProvider?.GetRequiredService<IHeadToHeadService>();
            int invalidId = -999; // Un Id qui n'existe probablement pas

            // Act
            var player = service?.GetPlayerById(invalidId);

            // Assert
            Assert.That(player, Is.Null);
        }
    }
}
