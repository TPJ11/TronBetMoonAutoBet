using AutoHotkey.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TRONbet.AutoBet.Moon
{
    interface IAhkFunctions
    {
        /// <summary>
        /// Test function only call when in test mode
        /// </summary>
        void Test();

        /// <summary>
        /// Gets the current time left to bet
        /// </summary>
        /// <returns>Amount of time left to bet, if -1 not currently able to bet</returns>
        decimal GetTimeLeftToBet();

        /// <summary>
        /// Clicks the 'Bet' button on TronBet Moon
        /// </summary>
        void ClickBet();

        /// <summary>
        /// Gets the given line number mutiplier in the history table
        /// </summary>
        /// <param name="lineNumber">Line number to read</param>
        /// <returns>Multiplier</returns>
        decimal ReadHistory(int lineNumber);

        /// <summary>
        /// Gets the current TRX balance in wallet
        /// </summary>
        /// <returns>Current TRX balance</returns>
        decimal GetBalance();

        /// <summary>
        /// Gets the currently set bet amount
        /// </summary>
        /// <returns>Current set bet amount</returns>
        decimal GetBetAmount();

        /// <summary>
        /// Sets the bet amount
        /// </summary>
        /// <param name="bet">Amount to set bet to</param>
        void SetBetAmount(decimal bet);

        /// <summary>
        /// Gets the currently set multiplier
        /// </summary>
        /// <returns>Currently set mutiplier</returns>
        decimal GetMultiplier();

        /// <summary>
        /// Sets the multiplier
        /// </summary>
        /// <param name="multiplier">The multiplier to set</param>
        void SetMultiplier(decimal multiplier);
    }

    class AhkFunctions : IAhkFunctions
    {
        private readonly AutoHotkeyEngine _ahk;

        public AhkFunctions()
        {
            _ahk = AutoHotkeyEngine.Instance;
            _ahk.LoadFile("functions.ahk");
        }

        public void Test() => _ahk.ExecFunction("Test");

        public decimal GetTimeLeftToBet()
        {
            var currentStatus = _ahk.ExecFunction("CurrentStatus");
            if (!string.IsNullOrWhiteSpace(currentStatus))
            {
                currentStatus = currentStatus.Trim();
                if (currentStatus.ToCharArray().Last() == 's')
                {
                    var sTimeLeft = currentStatus.Substring(0, currentStatus.Count() - 1);
                    if (decimal.TryParse(sTimeLeft, out var timeLeft))
                    {
                        return timeLeft;
                    }
                }
            }

            return -1;
        }

        public void ClickBet() => _ahk.ExecFunction("ClickBet");

        public decimal ReadHistory(int lineNumber)
        {
            var number = _ahk.ExecFunction("ReadHistory", lineNumber.ToString());

            if (string.IsNullOrWhiteSpace(number))
                throw new Exception($"Unable to read result for line number #{lineNumber}");

            number = number.Substring(0, number.Length - 1);

            if (decimal.TryParse(number, out var result))
                return result;

            throw new Exception($"Unable to convert result for line number #{lineNumber} to a number");
        }

        public decimal GetBalance()
        {
            var sBalance = _ahk.ExecFunction("GetBalance");

            if (decimal.TryParse(sBalance, out var balance))
                return balance;

            return -1;
        }

       public decimal GetBetAmount()
        {
            var sBetAmount = _ahk.ExecFunction("GetBetAmount");

            if (decimal.TryParse(sBetAmount, out var bet))
                return bet;

            return -1;
        }

       public void SetBetAmount(decimal bet) => _ahk.ExecFunction("SetBetAmount", bet.ToString());

        public decimal GetMultiplier()
        {
            var sBetAmount = _ahk.ExecFunction("GetMultiplier");

            if (decimal.TryParse(sBetAmount, out var bet))
                return bet;

            return -1;
        }

        public void SetMultiplier(decimal multiplier) => _ahk.ExecFunction("SetMultiplier", multiplier.ToString());
    }
}
