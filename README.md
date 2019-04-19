# Caution!
You use this application at your own risk! Remember the house always wins!

# Check check and check again
The application is way OTT on checking the values before it bets. It will on each bet check you have the required amount of TRX, the multiplier is the correct value, it then sets the bet amount and checks the bet amount - only after all this will it bet. Better safe than sorry I say!

# How to use
This application will bet on moon to your settings. It uses [Auto Hot Key](https://www.autohotkey.com/) to interact with your computer. You cannot use the computer for anything else while this is running, it is design so you can leave your computer and not miss a bet!

The current setup is for a 23" 1080p monitior using chrome with a favorites bar e.g. the most common full PC setup. If your PC is not like this you will need to set the application into `Test` mode and modifiy the `functions.ahk` changing all `MouseMove` commands to fit your screen - Please submit your changes stating your screen size and resolution to help others!

The application will not automatically sign for your bets so you need to bet once yourself setting how long you wish it to automatically sign for - this is a nice fail safe to stop betting if you forget to come back to your PC!

Within the `dist` folder you will find the published version of the application there are three important files:
1. `appsettings.json` file which you should edit after reading below
1. `functions.ahk` contains all the AutoHotKey functions you will edit this if you have a different setup to the one stated above
1. `Tronbet.AutoBet.Moon.exe` this will run the application.

## Settings
`appsettings.json` contains the values used to bet make sure you understand each one before running the application!

#### Mode
There are three modes:
1. `Test` used when setting up the application for your screen. It will run the 'Test' function within the functions.ahk file
1. `Watch` will do all but actually place the bet, this is good while you are testing or just want to see what it does
1. `Bet` This will place bets

#### Multiplier
The Multiplier to stop at

#### MaxNumberOfResets
The number of times it will loop over your `Bets` before stopping and waitting for a 'winner' (a moon equal to or over your multiplier value)

#### Bets
An array of the amount of TRX and order you wish to bet in for example, if I have
```
"Bets": [
  "10",
  "12",
  "14",
  "16",
  "18"
]
```
The first bet will be 10 TRX, second is 12 TRX and so on until 18 TRX, it will then (depending on the value set in `MaxNumberOfResets`) reset back to 10 TRX or if `MaxNumberOfResets` is reached it will wait until the next winner.

#### MaxNumberOfWinners and MaxNumberOfWinnersInHowManyRecords
The maximum number of winners (moons equal to or over your set multiplier) in X number of moons where X is equal to the value set in  `MaxNumberOfWinnersInHowManyRecords` for example: 

If I only want to bet if there are less than 11 'winners' in the last 50 moons I would set `MaxNumberOfWinners` to 10 and `MaxNumberOfWinnersInHowManyRecords` to 50

## Feedback
Any feedback or suggestions are welcome submit an issue or create a pull request with the change.

### Won big? tips are welcome - TD4991Vc6fAPW4CpGEqAWQkHnZd346Cmbp

# Links
- [AutoHotkey.Interop](https://github.com/amazing-andrew/AutoHotkey.Interop) .NET wrapper for AutoHotKey
- [AutoHotKey](https://www.autohotkey.com/)
- [Dotnet Core](https://github.com/dotnet/core)
