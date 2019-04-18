using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TRONbet.AutoBet.Moon
{
    interface IAppSettings
    {        
        Enums.Modes Mode { get; }
        decimal Multiplier { get; }
        int MaxNumberOfResets { get; }
        List<int> Bets { get; }
        int MaxNumberOfWinners { get; }
        int MaxNumberOfWinnersInHowManyRecords { get; }
    }

    class AppSettings : IAppSettings
    {
        public Enums.Modes Mode { get; private set; }
        public decimal Multiplier { get; private set; }
        public int MaxNumberOfResets { get; private set; }
        public List<int> Bets { get; private set; } = new List<int>();
        public int MaxNumberOfWinners { get; private set; }
        public int MaxNumberOfWinnersInHowManyRecords { get; private set; }

        public AppSettings(IConfiguration configuration)
        {
            if (Enum.TryParse(configuration["Mode"], out Enums.Modes mode))
                Mode = mode;
            else
                throw new Exception("Failed to load mode - must be a 'Bet', 'Watch', or 'Test'");

            if (decimal.TryParse(configuration["Multiplier"], out var multiplier))
                Multiplier = multiplier;
            else
                throw new Exception("Failed to load multiplier - must be a decimal");

            if (int.TryParse(configuration["MaxNumberOfResets"], out var maxNumberOfResets))
                MaxNumberOfResets = maxNumberOfResets;
            else
                throw new Exception("Failed to load max number of resets - must be a int");

            if (int.TryParse(configuration["MaxNumberOfWinners"], out var maxNumberOfWinners))
                MaxNumberOfWinners = maxNumberOfWinners;
            else
                throw new Exception("Failed to load max number of winners - must be a int");

            if (int.TryParse(configuration["MaxNumberOfWinnersInHowManyRecords"], out var maxNumberOfWinnersInHowManyRecords))
                MaxNumberOfWinnersInHowManyRecords = maxNumberOfWinnersInHowManyRecords;
            else
                throw new Exception("Failed to load max number of winners in how many records - must be a int");

            var bets = configuration.GetSection("Bets").GetChildren().ToArray().Select(x => x.Value).ToArray();

            foreach (var sBet in bets)
            {
                if (int.TryParse(sBet, out var bet))
                    Bets.Add(bet);
                else
                    throw new Exception("Failed to load bets - must be a int array");
            }
        }
    }
}
