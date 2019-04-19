using AutoHotkey.Interop;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TRONbet.AutoBet.Moon
{
    class Program
    {
        private static readonly List<MoonResults> _history = new List<MoonResults>();
        private static IAppSettings _appSettings;
        private static IAhkFunctions _ahkFunctions;
              
        private static int CurrentBetCount = 0;
        private static int CurrentResetCount = 0;        
        private static DateTime LastCanBet = DateTime.MinValue;

        static async Task Main(string[] args)
        {
            Console.Title = "TronBet - Auto Bet Moon";

            SetUp();

            if (_appSettings.Mode == Enums.Modes.Test)
            {
                Console.Title = "[Test Mode] TronBet - Auto Bet Moon";
                Console.WriteLine($"Test Mode");
                Console.WriteLine("Executing 'Test' function in functions.ahk file");
                Console.WriteLine("Press any key to execute function (will run 2 seconds after button press)");
                Console.ReadKey();
                Console.Write("Running...");
                Thread.Sleep(2000);
                _ahkFunctions.Test();
                Console.Write("Done.");
            }
            else
            {
                if (_appSettings.Mode == Enums.Modes.Bet)
                {
                    Console.Title = "[Bet Mode] TronBet - Auto Bet Moon";
                    Console.WriteLine($"Currently set to 'Bet' mode bets WILL be placed");
                }
                else
                {
                    Console.Title = "[Watch Mode] TronBet - Auto Bet Moon";
                    Console.WriteLine($"Currently set to 'Watch' mode bets WONT be placed");
                }

                Console.WriteLine($"Multiplier set to '{_appSettings.Multiplier}'");
                Console.WriteLine($"Max number of successes set to '{_appSettings.MaxNumberOfWinners}'");
                Console.WriteLine($"Max number of successes in how many records set to '{_appSettings.MaxNumberOfWinnersInHowManyRecords}'");
                Console.WriteLine($"Max number of resets set to '{_appSettings.MaxNumberOfResets}'");
                Console.WriteLine($"Bets set to:");

                foreach (var item in _appSettings.Bets)
                    Console.WriteLine($"{item}");

                Console.WriteLine($"If this is all correct press any key");
                Console.ReadKey();
                Console.WriteLine($"Now press any key then quickly click on TronBet Moon page.");
                Console.ReadKey();
                Console.WriteLine("Starting in 2 seconds...");
                Thread.Sleep(2000);

                Console.WriteLine("Waitting till end of current round");
                await WaitTilCanBet(17);

                Console.Write($"Setting mutiplier to {_appSettings.Multiplier}x...");
                _ahkFunctions.SetMultiplier(_appSettings.Multiplier);
                Console.Write("Done.");
                Console.WriteLine("");

                // Read full history of moon
                Console.Write("Loading history...");
                for (var i = 50; i >= 1; i--)
                {
                    _history.Add(new MoonResults()
                    {
                        Multiplier = _ahkFunctions.ReadHistory(i),
                        Timestamp = DateTime.Now
                    });
                }

                Console.Write("Done.");
                Console.WriteLine("");

                var didBet = false;

                while (1 == 1)
                {
                    await WaitTilCanBet(7);

                    var lastMoon = _ahkFunctions.ReadHistory(1);

                    _history.Add(new MoonResults()
                    {
                        Multiplier = lastMoon,
                        Timestamp = DateTime.Now
                    });

                    if (didBet)
                    {
                        if (DidWin())
                        {
                            Console.Write("Won.");
                            CurrentBetCount = 0;
                            CurrentResetCount = 0;
                        }
                        else
                            Console.Write("Lost.");
                    }

                    Console.WriteLine("");

                    if (ShouldBet())
                    {
                        await PlaceBet();
                        didBet = true;
                    }
                    else
                    {
                        didBet = false;
                        if (CurrentBetCount >= _appSettings.Bets.Count()
                            && CurrentResetCount >= _appSettings.MaxNumberOfResets)
                        {
                            await WaitTilNextWin();
                        }
                    }
                }
            }
        }

        #region Setup

        private static void SetUp()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            _appSettings = serviceProvider.GetService<IAppSettings>();
            _ahkFunctions = serviceProvider.GetService<IAhkFunctions>();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                  .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IAppSettings, AppSettings>();
            services.AddSingleton<IAhkFunctions, AhkFunctions>();
        }

        #endregion

        /// <summary>
        /// Waits until the next 'win' before returning
        /// a win is a value equal to or more than the set 
        /// multiplier in the configuration
        /// </summary>
        private static async Task WaitTilNextWin()
        {
            Console.Write("Waitting for a winner...");
            while (1 == 1)
            {
                await WaitTilCanBet(7);

                var lastMoon = _ahkFunctions.ReadHistory(1);

                if (lastMoon >= _appSettings.Multiplier)
                {
                    LastCanBet = DateTime.MinValue;
                    CurrentBetCount = 0;
                    CurrentResetCount = 0;
                    Console.Write("Winner.");
                    break;
                }

                // After checking if won so we don't add it 2x
                _history.Add(new MoonResults()
                {
                    Multiplier = lastMoon,
                    Timestamp = DateTime.Now
                });               
            }
        }

        /// <summary>
        /// Waits until the current round is over and can send new bet
        /// </summary>
        /// <param name="minTimeLeftToBet">Min value left on the clock to allow a bet</param>
        private static async Task WaitTilCanBet(decimal? minTimeLeftToBet = null)
        {
            var secondsSinceLastCanBet = (DateTime.Now - LastCanBet).TotalSeconds;
            if (secondsSinceLastCanBet < 20)            
                await Task.Delay((20 - (int)secondsSinceLastCanBet) * 1000);            

            while (1 == 1)
            {
                var timeLeft = _ahkFunctions.GetTimeLeftToBet();

                if (!minTimeLeftToBet.HasValue || timeLeft > minTimeLeftToBet)
                {
                    LastCanBet = DateTime.Now;
                    break;
                }

                await Task.Delay(1000);
            }
        }        

        /// <summary>
        /// Checks to see if the last game was a winner
        /// </summary>
        /// <returns>If [True] did win else [False] lost</returns>
        private static bool DidWin() => _history.Last().Multiplier >= _appSettings.Multiplier;

        /// <summary>
        /// Works out if it should bet this round
        /// </summary>
        /// <returns>If [True] it should bet this round else [False] it should not</returns>
        private static bool ShouldBet()
        {
            // If there are 5 or more successful bets in the last 50 spins we don't bet
            if (TotalSuccesses(_appSettings.MaxNumberOfWinnersInHowManyRecords) > _appSettings.MaxNumberOfWinners)
                return false;

            // If the current bet is maxed out we will be resetting on the net bet
            // so we need to check to see if we have reached our limit for resets
            if (CurrentBetCount >= _appSettings.Bets.Count()
                && CurrentResetCount >= _appSettings.MaxNumberOfResets)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Places a bet on Tron Moon to the value of the current bet
        /// </summary>
        private static async Task PlaceBet()
        {
            if (CurrentBetCount >= _appSettings.Bets.Count())
            {                
                CurrentBetCount = 0;
                CurrentResetCount++;

                Console.WriteLine($"Reset bet count - {CurrentResetCount}");
            }

            var betAmount = _appSettings.Bets[CurrentBetCount];

            // Make sure we wait for win or loss amount to go into our TRX balance
            await Task.Delay(2000);

            // Check we have the trx left to bet with
            if (_ahkFunctions.GetBalance() < betAmount)
                throw new Exception("Run out of TRX!");

            // Check it was set correctly 
            if (_ahkFunctions.GetMultiplier() != _appSettings.Multiplier)
                throw new Exception("Failed to set multiplier correctly");

            // Set the bet
            _ahkFunctions.SetBetAmount(betAmount);
            await Task.Delay(500);
            // Check it was set correctly 
            if (_ahkFunctions.GetBetAmount() != betAmount)
                throw new Exception("Failed to set bet correctly");

            if (_appSettings.Mode == Enums.Modes.Bet)
            {
                _ahkFunctions.ClickBet();
                Console.Write($"Placed bet worth {betAmount} TRX...");
            }
            else
            {
                Console.Write($"Would place bet worth {betAmount} TRX...");
            }

            CurrentBetCount++;

            return;
        }

        /// <summary>
        /// Gets the total number of successful spins / moons from the history
        /// </summary>
        /// <param name="multiplier">Min multiplier to look for</param>
        /// <param name="numberToLookBack">Number of records to look back if null will use all history</param>
        /// <returns>Total number of successful spins in the list</returns>
        private static int TotalSuccesses(int? numberToLookBack = null) => numberToLookBack is null ? _history.Count(x => x.Multiplier >= _appSettings.Multiplier)
                : _history.TakeLast(numberToLookBack.Value).Count(x => x.Multiplier >= _appSettings.Multiplier);        
    }
}
