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
            int invalidId = -999;

            // Act
            var player = service?.GetPlayerById(invalidId);

            // Assert
            Assert.That(player, Is.Null);
        }

        [Test]
        public void GetGeneralStats_ShouldReturnNotNull()
        {
            // Arrange
            var service = _serviceProvider?.GetRequiredService<IHeadToHeadService>();

            // Act
            var stats = service?.GetGeneralStats();

            // Assert
            Assert.That(stats, Is.Not.Null);
        }

        [Test]
        public void GetGeneralStats_ShouldReturnValidData()
        {
            // Arrange
            var service = _serviceProvider?.GetRequiredService<IHeadToHeadService>();

            // Act
            var stats = service?.GetGeneralStats();

            // Assert
            Assert.That(stats, Is.Not.Null);
            Assert.That(stats?.CountryWithBestWinRatio, Is.Not.Null.And.Not.Empty);
            Assert.That(stats?.AverageIMC, Is.GreaterThanOrEqualTo(0));
            Assert.That(stats?.MedianHeight, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void GetGeneralStats_ShouldReturnConsistentData()
        {
            // Arrange
            var service = _serviceProvider?.GetRequiredService<IHeadToHeadService>();

            // Act
            var stats1 = service?.GetGeneralStats();
            var stats2 = service?.GetGeneralStats();

            // Assert
            Assert.That(stats1?.CountryWithBestWinRatio, Is.EqualTo(stats2?.CountryWithBestWinRatio));
            Assert.That(stats1?.AverageIMC, Is.EqualTo(stats2?.AverageIMC));
            Assert.That(stats1?.MedianHeight, Is.EqualTo(stats2?.MedianHeight));
        }
    }
}
