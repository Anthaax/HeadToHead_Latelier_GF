using Microsoft.Extensions.DependencyInjection;
using HeadToHead_Latelier.Common.Data;
using HeadToHead_Latelier.Common.Interfaces.Readers;
using HeadToHead_Latelier.Common.Interfaces.Writers;
using HeadToHead_Latelier.Reader;
using HeadToHead_Latelier.Tests.Helpers;

namespace HeadToHead_Latelier.Tests
{
    public class HeadToHeadWriterTests
    {
        private IServiceProvider? _serviceProvider;
        private IHeadToHeadWriter? _writer;
        private IHeadToHeadReader? _reader;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddScoped<IHeadToHeadReader, HeadToHeadReader>();
            services.AddScoped<ICacheManager>(provider => provider.GetRequiredService<IHeadToHeadReader>() as ICacheManager ?? throw new InvalidOperationException("IHeadToHeadReader must implement ICacheManager"));
            services.AddScoped<IHeadToHeadWriter, HeadToHeadWriter>();
            _serviceProvider = services.BuildServiceProvider();

            _writer = _serviceProvider?.GetRequiredService<IHeadToHeadWriter>();
            _reader = _serviceProvider?.GetRequiredService<IHeadToHeadReader>();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up resources
            (_serviceProvider as IDisposable)?.Dispose();
        }

        [Test]
        public void InsertPlayer_WithRandomValidPlayerData_ShouldReturnPlayerWithGeneratedId()
        {
            // Arrange
            var newPlayer = PlayerTestDataGenerator.GenerateRandomPlayer();
            var originalFirstname = newPlayer.Firstname;
            var originalLastname = newPlayer.Lastname;
            var originalCountryCode = newPlayer.Country.Code;
            var originalRank = newPlayer.Data.Rank;

            // Act
            var insertedPlayer = _writer?.InsertPlayer(newPlayer);

            // Assert
            Assert.That(insertedPlayer, Is.Not.Null);
            Assert.That(insertedPlayer?.Id, Is.GreaterThan(0));
            Assert.That(insertedPlayer?.Firstname, Is.EqualTo(originalFirstname));
            Assert.That(insertedPlayer?.Lastname, Is.EqualTo(originalLastname));
            Assert.That(insertedPlayer?.Country.Code, Is.EqualTo(originalCountryCode));
            Assert.That(insertedPlayer?.Data.Rank, Is.EqualTo(originalRank));
        }

        [Test]
        public void InsertPlayer_ShouldResetCacheAndReflectNewPlayer()
        {
            // Arrange
            var initialPlayers = _reader?.GetPlayersByRank().ToList();
            var initialCount = initialPlayers?.Count ?? 0;

            var newPlayer = PlayerTestDataGenerator.GenerateRandomPlayer();
            var newPlayerFirstname = newPlayer.Firstname;
            var newPlayerLastname = newPlayer.Lastname;

            // Act
            _writer?.InsertPlayer(newPlayer);
            var playersAfterInsert = _reader?.GetPlayersByRank().ToList();
            var countAfterInsert = playersAfterInsert?.Count ?? 0;

            // Assert
            Assert.That(countAfterInsert, Is.EqualTo(initialCount + 1));

            var insertedPlayer = playersAfterInsert?.FirstOrDefault(p => 
                p.Firstname == newPlayerFirstname && p.Lastname == newPlayerLastname);
            Assert.That(insertedPlayer, Is.Not.Null);
        }

        [Test]
        public void InsertPlayer_MultipleRandomInserts_ShouldGenerateSequentialIds()
        {
            // Arrange
            var firstPlayer = PlayerTestDataGenerator.GenerateRandomPlayer();
            var secondPlayer = PlayerTestDataGenerator.GenerateRandomPlayer();

            // Act
            var insertedFirst = _writer?.InsertPlayer(firstPlayer);
            var insertedSecond = _writer?.InsertPlayer(secondPlayer);

            // Assert
            Assert.That(insertedFirst?.Id, Is.GreaterThan(0));
            Assert.That(insertedSecond?.Id, Is.GreaterThan(0));
            Assert.That(insertedFirst?.Id, Is.Not.EqualTo(insertedSecond?.Id));
            Assert.That(insertedSecond?.Id, Is.EqualTo(insertedFirst?.Id + 1));
        }

        [Test]
        public void InsertPlayer_ShouldBeRetrievableByGetPlayerById()
        {
            // Arrange
            var newPlayer = PlayerTestDataGenerator.GenerateRandomPlayer();
            var originalFirstname = newPlayer.Firstname;
            var originalLastname = newPlayer.Lastname;
            var originalRank = newPlayer.Data.Rank;

            // Act
            var insertedPlayer = _writer?.InsertPlayer(newPlayer);
            var retrievedPlayer = _reader?.GetPlayerById(insertedPlayer?.Id ?? 0);

            // Assert
            Assert.That(retrievedPlayer, Is.Not.Null);
            Assert.That(retrievedPlayer?.Firstname, Is.EqualTo(originalFirstname));
            Assert.That(retrievedPlayer?.Lastname, Is.EqualTo(originalLastname));
            Assert.That(retrievedPlayer?.Data.Rank, Is.EqualTo(originalRank));
            Assert.That(retrievedPlayer?.Id, Is.EqualTo(insertedPlayer?.Id));
        }

        [Test]
        public void InsertPlayer_MultipleDifferentPlayers_ShouldAllBeInsertedAndRetrievable()
        {
            // Arrange
            var players = PlayerTestDataGenerator.GenerateRandomPlayers(3);
            var insertedPlayers = new List<Player>();

            // Act
            foreach (var player in players)
            {
                var inserted = _writer?.InsertPlayer(player);
                if (inserted != null)
                {
                    insertedPlayers.Add(inserted);
                }
            }

            // Assert
            Assert.That(insertedPlayers.Count, Is.EqualTo(3));

            var ids = insertedPlayers.Select(p => p.Id).ToList();
            Assert.That(ids, Is.Unique);
            Assert.That(ids, Is.All.GreaterThan(0));

            foreach (var player in insertedPlayers)
            {
                var retrieved = _reader?.GetPlayerById(player.Id);
                Assert.That(retrieved, Is.Not.Null);
                Assert.That(retrieved?.Firstname, Is.EqualTo(player.Firstname));
                Assert.That(retrieved?.Lastname, Is.EqualTo(player.Lastname));
            }
        }


        [Test]
        public void InsertPlayer_MaleAndFemaleRandomPlayers_ShouldPreserveGender()
        {
            // Arrange
            var malePlayer = PlayerTestDataGenerator.GenerateRandomMalePlayer();
            var femalePlayer = PlayerTestDataGenerator.GenerateRandomFemalePlayer();

            // Act
            var insertedMale = _writer?.InsertPlayer(malePlayer);
            var insertedFemale = _writer?.InsertPlayer(femalePlayer);

            // Assert
            Assert.That(insertedMale?.Sex, Is.EqualTo("M"));
            Assert.That(insertedFemale?.Sex, Is.EqualTo("F"));

            var retrievedMale = _reader?.GetPlayerById(insertedMale?.Id ?? 0);
            var retrievedFemale = _reader?.GetPlayerById(insertedFemale?.Id ?? 0);

            Assert.That(retrievedMale?.Sex, Is.EqualTo("M"));
            Assert.That(retrievedFemale?.Sex, Is.EqualTo("F"));
        }
    }
}
