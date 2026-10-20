using HeadToHead_Latelier.Common.Data;

namespace HeadToHead_Latelier.Tests.Helpers
{
    /// <summary>
    /// Helper class to generate random test data
    /// </summary>
    public static class PlayerTestDataGenerator
    {
        private static readonly Random _random = new Random();

        private static readonly string[] FirstNamesM = 
        { 
            "Liam", "Noah", "Oliver", "Ethan", "Ava", "Sophia", "Isabella", "Mia",
            "Lucas", "Benjamin", "Henry", "Alexander", "Mason", "Michael", "Ethan",
            "Carlos", "Rafael", "Juan", "Marco", "Diego", "Antonio", "Paulo",
            "Roger", "Novak", "Andy", "Tommy", "Andre", "Bjorn", "Ivan"
        };

        private static readonly string[] FirstNamesF = 
        { 
            "Naomi", "Emma", "Olivia", "Ava", "Isabella", "Sophia", "Charlotte", "Amelia",
            "Mia", "Harper", "Evelyn", "Abigail", "Madison", "Elizabeth", "Sofia",
            "Serena", "Venus", "Angelique", "Maria", "Justine", "Caroline", "Ana"
        };

        private static readonly string[] LastNames = 
        { 
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller",
            "Davis", "Rodriguez", "Martinez", "Lopez", "Gonzalez", "Hernandez",
            "Thompson", "Anderson", "Taylor", "Thomas", "Moore", "Jackson", "Martin",
            "Tsitsipas", "Sinner", "Alcaraz", "Rune", "Osaka", "Sabalenka", "Swiatek"
        };

        private static readonly string[] Countries = 
        { 
            "USA", "ESP", "ITA", "GRE", "GER", "FRA", "GBR", "AUS", "CAN", "CHN",
            "JPN", "AUS", "SRB", "SUI", "NED", "SVK", "POL", "CZE", "DNK", "SWE",
            "BEL", "ARG", "BRA", "MEX", "IND", "RUS", "UKR", "TUR", "ISR", "EGY"
        };

        private static readonly Dictionary<string, string> CountryPictures = new()
        {
            { "USA", "https://tenisu.latelier.co/resources/USA.png" },
            { "ESP", "https://tenisu.latelier.co/resources/Espagne.png" },
            { "ITA", "https://tenisu.latelier.co/resources/Italie.png" },
            { "GRE", "https://tenisu.latelier.co/resources/Grece.png" },
            { "GER", "https://tenisu.latelier.co/resources/Allemagne.png" },
            { "FRA", "https://tenisu.latelier.co/resources/France.png" },
            { "GBR", "https://tenisu.latelier.co/resources/RoyaumeUni.png" },
            { "AUS", "https://tenisu.latelier.co/resources/Australie.png" },
            { "CAN", "https://tenisu.latelier.co/resources/Canada.png" },
            { "CHN", "https://tenisu.latelier.co/resources/Chine.png" },
            { "JPN", "https://tenisu.latelier.co/resources/Japon.png" },
            { "SRB", "https://tenisu.latelier.co/resources/Serbie.png" },
            { "SUI", "https://tenisu.latelier.co/resources/Suisse.png" },
            { "NED", "https://tenisu.latelier.co/resources/PaysBas.png" },
            { "SVK", "https://tenisu.latelier.co/resources/Slovaquie.png" },
            { "POL", "https://tenisu.latelier.co/resources/Pologne.png" },
            { "CZE", "https://tenisu.latelier.co/resources/Republique Tcheque.png" },
            { "DNK", "https://tenisu.latelier.co/resources/Danemark.png" },
            { "SWE", "https://tenisu.latelier.co/resources/Suede.png" },
            { "ARG", "https://tenisu.latelier.co/resources/Argentine.png" },
            { "BRA", "https://tenisu.latelier.co/resources/Bresil.png" }
        };

        /// <summary>
        /// Generates a random player with valid data
        /// </summary>
        public static Player GenerateRandomPlayer(string? sex = null)
        {
            // Determine gender randomly if not provided
            sex ??= _random.Next(2) == 0 ? "M" : "F";

            // Select first name based on gender
            var firstname = sex == "M" 
                ? FirstNamesM[_random.Next(FirstNamesM.Length)] 
                : FirstNamesF[_random.Next(FirstNamesF.Length)];

            var lastname = LastNames[_random.Next(LastNames.Length)];
            var shortname = $"{firstname[0]}.{lastname[..3].ToUpper()}";

            // Select a random country
            var countryCode = Countries[_random.Next(Countries.Length)];
            var countryPicture = CountryPictures.TryGetValue(countryCode, out var picture)
                ? picture
                : "https://tenisu.latelier.co/resources/default.png";

            // Generate realistic random data
            var rank = _random.Next(1, 200);
            var points = _random.Next(500, 4000);
            var weight = _random.Next(60000, 95000); // In grams (60-95 kg)
            var height = _random.Next(160, 210); // In cm
            var age = _random.Next(18, 45);
            var last = Enumerable.Range(0, 5).Select(_ => _random.Next(2)).ToArray(); // Last 5 results

            return new Player
            {
                Firstname = firstname,
                Lastname = lastname,
                Shortname = shortname,
                Sex = sex,
                Country = new Country
                {
                    Code = countryCode,
                    Picture = countryPicture
                },
                Picture = $"https://tenisu.latelier.co/resources/{firstname}.png",
                Data = new PlayerData
                {
                    Rank = rank,
                    Points = points,
                    Weight = weight,
                    Height = height,
                    Age = age,
                    Last = last
                }
            };
        }

        /// <summary>
        /// Generates a list of random players
        /// </summary>
        public static List<Player> GenerateRandomPlayers(int count)
        {
            var players = new List<Player>();
            for (int i = 0; i < count; i++)
            {
                players.Add(GenerateRandomPlayer());
            }
            return players;
        }

        /// <summary>
        /// Generates a random male player
        /// </summary>
        public static Player GenerateRandomMalePlayer()
        {
            return GenerateRandomPlayer("M");
        }

        /// <summary>
        /// Generates a random female player
        /// </summary>
        public static Player GenerateRandomFemalePlayer()
        {
            return GenerateRandomPlayer("F");
        }
    }
}
