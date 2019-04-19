Test()
{
	MouseMove, 0, 0
}

ClickBet()
{
	MouseMove, 1170, 850
	Click
}

ReadHistory(lineNumber)
{
	Send, {PgUp}
	Send, {PgUp}
	Send, {PgUp}
	Send, {PgUp}
	Send, {PgUp}
	Send, {PgUp}
	Sleep, 200
	MouseMove, 1917, 300
	Click, down
	lineNumber -= 1
	lineNumber *= 11.2
	lineNumber += 300
	MouseMove, 1917, lineNumber
	Click, up
	Sleep, 100
	MouseMove, 520, 960
	Click
	Click
	Sleep, 50
	clipboard =
	Send,{CTRL Down}c{CTRL Up}
	Sleep, 50
	Return,  %clipboard%
}

CurrentStatus()
{
	MouseMove, 1200, 500
	Click
	Click
	Click
	Click
	Click
	Sleep, 100
	clipboard =
	Send,{CTRL Down}c{CTRL Up}
	Sleep, 100
	Return,  %clipboard%
}

GetBalance()
{
	MouseMove, 1120, 773
	Click
	Click
	Click
	Click
	Click
	Sleep, 100
	clipboard =
	Send,{CTRL Down}c{CTRL Up}
	Sleep, 100
	Return,  %clipboard%
}

GetBetAmount()
{
	MouseMove, 620, 773
	Click
	Click
	Click
	Click
	Click
	Sleep, 100
	clipboard =
	Send,{CTRL Down}c{CTRL Up}
	Sleep, 100
	Return,  %clipboard%
}

SetBetAmount(bet)
{
	MouseMove, 620, 773
	Click
	Click
	Click
	Click
	Click
	Sleep, 100
	Send, %bet%
	Sleep, 100
}

GetMultiplier()
{
	MouseMove, 620, 860
	Click
	Click
	Click
	Click
	Click
	Sleep, 100
	clipboard =
	Send,{CTRL Down}c{CTRL Up}
	Sleep, 100
	Return,  %clipboard%
}

SetMultiplier(multiplier)
{
	MouseMove, 620, 860
	Click
	Click
	Click
	Click
	Click
	Sleep, 100
	Send, %multiplier%
	Sleep, 100
}