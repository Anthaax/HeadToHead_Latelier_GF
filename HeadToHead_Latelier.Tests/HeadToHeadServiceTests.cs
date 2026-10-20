using Microsoft.Extensions.DependencyInjection;
using HeadToHead_Latelier.Common.Interfaces.Services;
using HeadToHead_Latelier.API.Services;
using HeadToHead_Latelier.Common.Interfaces.Readers;
using HeadToHead_Latelier.Reader;
using HeadToHead_Latelier.Common.Interfaces.Writers;
using HeadToHead_Latelier.Tests.Helpers;

namespace HeadToHead_Latelier.Tests
{
    public class HeadToHeadServiceTests
    {
        private IServiceProvider? _serviceProvider;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddScoped<IHeadToHeadReader, HeadToHeadReader>();
            services.AddScoped(provider => provider.GetRequiredService<IHeadToHeadReader>() as ICacheManager ?? throw new InvalidOperationException("IHeadToHeadReader must implement ICacheManager"));
            services.AddScoped<IHeadToHeadWriter, HeadToHeadWriter>();
            services.AddScoped<IHeadToHeadService, HeadToHeadService>();
            _serviceProvider = services.BuildServiceProvider();
        }

        [TearDown]
        public void TearDown()
        {
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

        [Test]
        public void InsertPlayer_WithRandomPlayerViaService_ShouldReturnPlayerWithGeneratedId()
        {
            // Arrange
            var service = _serviceProvider?.GetRequiredService<IHeadToHeadService>();
            var newPlayer = PlayerTestDataGenerator.GenerateRandomPlayer();
            var originalFirstname = newPlayer.Firstname;
            var originalLastname = newPlayer.Lastname;
            var originalCountryCode = newPlayer.Country.Code;

            // Act
            var insertedPlayer = service?.InsertPlayer(newPlayer);

            // Assert
            Assert.That(insertedPlayer, Is.Not.Null);
            Assert.That(insertedPlayer?.Id, Is.GreaterThan(0));
            Assert.That(insertedPlayer?.Firstname, Is.EqualTo(originalFirstname));
            Assert.That(insertedPlayer?.Lastname, Is.EqualTo(originalLastname));
            Assert.That(insertedPlayer?.Country.Code, Is.EqualTo(originalCountryCode));
        }

        [Test]
        public void InsertPlayer_ShouldBeRetrievableViService()
        {
            // Arrange
            var service = _serviceProvider?.GetRequiredService<IHeadToHeadService>();
            var newPlayer = PlayerTestDataGenerator.GenerateRandomPlayer();
            var originalFirstname = newPlayer.Firstname;
            var originalLastname = newPlayer.Lastname;

            // Act
            var insertedPlayer = service?.InsertPlayer(newPlayer);
            var retrievedPlayer = service?.GetPlayerById(insertedPlayer?.Id ?? 0);

            // Assert
            Assert.That(retrievedPlayer, Is.Not.Null);
            Assert.That(retrievedPlayer?.Firstname, Is.EqualTo(originalFirstname));
            Assert.That(retrievedPlayer?.Lastname, Is.EqualTo(originalLastname));
            Assert.That(retrievedPlayer?.Id, Is.EqualTo(insertedPlayer?.Id));
        }

        [Test]
        public void InsertPlayer_MultiplePlayersViaService_ShouldBeReflectedInGetPlayers()
        {
            // Arrange
            var service = _serviceProvider?.GetRequiredService<IHeadToHeadService>();
            var initialPlayers = service?.GetPlayers().ToList();
            var initialCount = initialPlayers?.Count ?? 0;

            var player1 = PlayerTestDataGenerator.GenerateRandomPlayer();
            var player2 = PlayerTestDataGenerator.GenerateRandomPlayer();

            // Act
            var inserted1 = service?.InsertPlayer(player1);
            var inserted2 = service?.InsertPlayer(player2);
            var playersAfterInsert = service?.GetPlayers().ToList();
            var countAfterInsert = playersAfterInsert?.Count ?? 0;

            // Assert
            Assert.That(countAfterInsert, Is.EqualTo(initialCount + 2));

            var foundPlayer1 = playersAfterInsert?.FirstOrDefault(p => 
                p.Firstname == player1.Firstname && p.Lastname == player1.Lastname);
            var foundPlayer2 = playersAfterInsert?.FirstOrDefault(p => 
                p.Firstname == player2.Firstname && p.Lastname == player2.Lastname);

            Assert.That(foundPlayer1, Is.Not.Null);
            Assert.That(foundPlayer2, Is.Not.Null);
        }

        [Test]
        public void InsertPlayer_SequentialIdsViaService_ShouldBeGenerated()
        {
            // Arrange
            var service = _serviceProvider?.GetRequiredService<IHeadToHeadService>();
            var player1 = PlayerTestDataGenerator.GenerateRandomPlayer();
            var player2 = PlayerTestDataGenerator.GenerateRandomPlayer();

            // Act
            var inserted1 = service?.InsertPlayer(player1);
            var inserted2 = service?.InsertPlayer(player2);

            // Assert
            Assert.That(inserted1?.Id, Is.GreaterThan(0));
            Assert.That(inserted2?.Id, Is.GreaterThan(0));
            Assert.That(inserted2?.Id, Is.EqualTo(inserted1?.Id + 1));
        }

        [Test]
        public void InsertPlayer_ShouldNotAffectGetGeneralStatsValidity()
        {
            // Arrange
            var service = _serviceProvider?.GetRequiredService<IHeadToHeadService>();
            var statsBeforeInsert = service?.GetGeneralStats();
            var newPlayer = PlayerTestDataGenerator.GenerateRandomPlayer();

            // Act
            service?.InsertPlayer(newPlayer);
            var statsAfterInsert = service?.GetGeneralStats();

            // Assert
            Assert.That(statsAfterInsert, Is.Not.Null);
            Assert.That(statsAfterInsert?.CountryWithBestWinRatio, Is.Not.Null.And.Not.Empty);
            Assert.That(statsAfterInsert?.AverageIMC, Is.GreaterThanOrEqualTo(0));
            Assert.That(statsAfterInsert?.MedianHeight, Is.GreaterThanOrEqualTo(0));
        }
    }
}
