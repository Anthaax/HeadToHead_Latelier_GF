using Microsoft.Extensions.DependencyInjection;
using HeadToHead_Latelier.Common.Interfaces.Readers;
using HeadToHead_Latelier.Reader;
using HeadToHead_Latelier.Common.Data;

namespace HeadToHead_Latelier.Tests
{
    public class HeadToHeadReaderTests
    {
        private IServiceProvider? _serviceProvider;
        private IHeadToHeadReader? _reader;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddScoped<IHeadToHeadReader, HeadToHeadReader>();
            _serviceProvider = services.BuildServiceProvider();
            _reader = _serviceProvider?.GetRequiredService<IHeadToHeadReader>();
        }

        [TearDown]
        public void TearDown()
        {
            (_serviceProvider as IDisposable)?.Dispose();
        }

        #region GetPlayersByRank Tests

        [Test]
        public void GetPlayersByRank_ShouldReturnNotNull()
        {
            // Arrange
            // Act
            var players = _reader?.GetPlayersByRank();

            // Assert
            Assert.That(players, Is.Not.Null);
        }

        [Test]
        public void GetPlayersByRank_ShouldReturnPlayersOrderedByRank()
        {
            // Arrange
            // Act
            var players = _reader?.GetPlayersByRank().ToList();

            // Assert
            Assert.That(players, Is.Not.Null);
            Assert.That(players?.Count, Is.GreaterThan(0));

            for (int i = 1; i < players?.Count; i++)
            {
                Assert.That(players[i].Data.Rank, Is.GreaterThanOrEqualTo(players[i - 1].Data.Rank),
                    "Players aren't ordered by rank");
            }
        }

        [Test]
        public void GetPlayersByRank_ShouldCacheResults()
        {
            // Arrange
            // Act
            var players1 = _reader?.GetPlayersByRank();
            var players2 = _reader?.GetPlayersByRank();

            // Assert
            Assert.That(ReferenceEquals(players1, players2), Is.True,
                "Result aren't cached");
        }

        #endregion

        #region GetPlayerById Tests

        [Test]
        public void GetPlayerById_WithValidId_ShouldReturnPlayer()
        {
            // Arrange
            var players = _reader?.GetPlayersByRank().ToList();
            var expectedId = players?[0].Id ?? 1;

            // Act
            var player = _reader?.GetPlayerById(expectedId);

            // Assert
            Assert.That(player, Is.Not.Null);
            Assert.That(player?.Id, Is.EqualTo(expectedId));
        }

        [Test]
        public void GetPlayerById_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var invalidId = 999999;

            // Act
            var player = _reader?.GetPlayerById(invalidId);

            // Assert
            Assert.That(player, Is.Null);
        }

        [Test]
        public void GetPlayerById_ShouldReturnCorrectPlayerData()
        {
            // Arrange
            var players = _reader?.GetPlayersByRank().First();
            var expectedId = players?.Id ?? 1;

            // Act
            var player = _reader?.GetPlayerById(expectedId);

            // Assert
            Assert.That(player?.Firstname, Is.EqualTo(players?.Firstname));
            Assert.That(player?.Lastname, Is.EqualTo(players?.Lastname));
            Assert.That(player?.Country.Code, Is.EqualTo(players?.Country.Code));
        }

        #endregion

        #region GetCountryWithTheBestWinRatio Tests

        [Test]
        public void GetCountryWithTheBestWinRatio_ShouldMatchTopCountryFromList()
        {
            // Arrange
            var countriesWithRatio = GetCountriesWithWinRatio(_reader?.GetPlayersByRank() ?? new List<Player>()).ToList();
            var topCountry = countriesWithRatio?.FirstOrDefault();

            // Act
            var bestCountry = _reader?.GetCountryWithTheBestWinRatio();
            var bestCountryRatio = countriesWithRatio?.FirstOrDefault(c => c.CountryCode == bestCountry);

            // Assert
            Assert.That(bestCountryRatio?.WinRatio, Is.EqualTo(topCountry?.WinRatio),
                "The best country's win ratio doesn't match the top country's win ratio");
        }

        [Test]
        public void GetCountryWithTheBestWinRatio_ShouldHaveHighestWinRatio()
        {
            // Arrange
            var countriesWithRatio = GetCountriesWithWinRatio(_reader?.GetPlayersByRank() ?? new List<Player>()).ToList();

            // Act
            var bestCountry = _reader?.GetCountryWithTheBestWinRatio();
            var bestCountryRatio = countriesWithRatio?.FirstOrDefault(c => c.CountryCode == bestCountry).WinRatio ?? 0;

            // Assert
            foreach (var country in countriesWithRatio ?? new List<(string, double)>())
            {
                Assert.That(bestCountryRatio, Is.GreaterThanOrEqualTo(country.WinRatio),
                    $"The win ratio of {bestCountry} is not the highest");
            }
        }

        #endregion

        #region GetAverageIMC Tests

        [Test]
        public void GetAverageIMC_ShouldReturnPositiveValue()
        {
            // Arrange
            // Act
            var averageIMC = _reader?.GetAverageIMC();

            // Assert
            Assert.That(averageIMC, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void GetAverageIMC_ShouldExcludeZeroHeights()
        {
            // Arrange
            // Act
            var averageIMC = _reader?.GetAverageIMC();
            var players = _reader?.GetPlayersByRank().ToList();

            // Assert
            Assert.That(averageIMC, Is.Not.NaN);
            Assert.That(double.IsInfinity((double)(averageIMC ?? 0)), Is.False,
                "The IMC contains infinite values");
        }

        #endregion

        #region GetMedianHeight Tests

        [Test]
        public void GetMedianHeight_ShouldReturnPositiveValue()
        {
            // Arrange
            // Act
            var medianHeight = _reader?.GetMedianHeight();

            // Assert
            Assert.That(medianHeight, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void GetMedianHeight_ShouldBeWithinPlayerHeights()
        {
            // Arrange
            var players = _reader?.GetPlayersByRank().ToList();
            var heights = players?.Select(p => p.Data.Height).OrderBy(h => h).ToList();

            // Act
            var medianHeight = _reader?.GetMedianHeight();

            // Assert
            Assert.That(medianHeight, Is.GreaterThanOrEqualTo(heights?.FirstOrDefault() ?? 0));
            Assert.That(medianHeight, Is.LessThanOrEqualTo(heights?.LastOrDefault() ?? int.MaxValue));
        }

        [Test]
        public void GetMedianHeight_WithOddCountOfPlayers_ShouldReturnMiddleValue()
        {
            // Arrange
            var players = _reader?.GetPlayersByRank().ToList();
            var heights = players?.Select(p => p.Data.Height).OrderBy(h => h).ToList();

            // Act
            var medianHeight = _reader?.GetMedianHeight();

            // Assert
            Assert.That(heights, Is.Not.Null);
            if (heights.Count % 2 == 1)
            {
                var middleIndex = heights.Count / 2;
                Assert.That(medianHeight, Is.EqualTo(heights[middleIndex]));
            }
        }

        [Test]
        public void GetMedianHeight_WithEvenCountOfPlayers_ShouldReturnAverage()
        {
            // Arrange
            var players = _reader?.GetPlayersByRank().ToList();
            var heights = players?.Select(p => p.Data.Height).OrderBy(h => h).ToList();

            // Act
            var medianHeight = _reader?.GetMedianHeight();

            // Assert
            Assert.That(heights, Is.Not.Null);
            if (heights.Count > 0 && heights.Count % 2 == 0)
            {
                var middleIndex = heights.Count / 2;
                var expectedMedian = (heights[middleIndex - 1] + heights[middleIndex]) / 2.0;
                Assert.That(medianHeight, Is.EqualTo(expectedMedian));
            }
        }

        #endregion

        #region Private
        private IEnumerable<(string CountryCode, double WinRatio)> GetCountriesWithWinRatio(IEnumerable<Player> players)
        {
            return players
                .Where(p => p.Data.Last.Length > 0)
                .GroupBy(p => p.Country.Code)
                .Select(group =>
                {
                    var totalMatches = group.Sum(p => p.Data.Last.Length);
                    var winCount = group.Sum(p => p.Data.Last.Count(result => result == 1));
                    var winRatio = totalMatches > 0 ? (double)winCount / totalMatches : 0;
                    return (group.Key, winRatio);
                })
                .OrderByDescending(x => x.winRatio);
        }
        #endregion

    }
}
