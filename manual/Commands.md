# List of FlippoIO Commands
## `set`
The command set takes two arguments, the name of a setting and a value, and can be used to change a setting. Note that the arguments are case sensitive.

Example usage (sets time limit to 10 seconds):
```
set TimeLimit 10000
```

For a full list of settings, see: https://github.com/sleephacker/FlippoIO/blob/master/manual/Settings.md.

Note that changing the settings requires FlippoIO to wait for all scheduled matches to complete and to abort all threads that were used during those matches. This process can take a lot of time depending of various settings. As a rule of thumb, the number of threads used is usually equal to `ThreadCount * (AsyncRead ? 4 : 3)`. These threads only become active once the first match or the first match since the last change of settings is started. It is therefore recommended to change all desired settings before starting any matches.

## `player`
This command takes two arguments, the name of a player and the path to its executable, and can be used to register a player.

Example usage:
```
player “Player One” C:\folder\player.exe
player “Player Two” player.py
player player3 “D:\folder that contains my player\java player.jar”
```

## `alias`
This command takes two arguments, an alias and the name of a player to apply that alias to. It can be used to give a player a different or shorter name for easier readability of later commands, without actually registering that player with a different name.

Example usage:
```
alias 1 “Player Number One”
alias 2 “Player 2”
alias 3 player_3
```

## `score`
Can be used to create and display scoreboards. This command takes one argument, which can either be display or clear. If display is passed as the argument, a ranking of all players and their scores so far will be displayed. If clear is passed as the argument, the score of every player will be reset to zero.

## `match`
This command can be used to schedule a match between two players, and takes the names or aliases of those players as its two arguments. The first argument specifies the white player, and the second specifies the black player.

Example usage:
```
match playerA playerB`
```

## `duel`
This command can be used to schedule two matches at once, and takes two player names or aliases as its arguments. In the second match the players will switch sides (white plays as black and vice versa) to create a fair competition between the two players.

Example usage:
```
duel player1 player2
```

## `competition`
This command takes at least two players as its arguments, and can be used to schedule a fair competition between the specified players. All players will play against every other player, both as white and as black.

Example usage:
```
competition player1 “player two” “Player 3” player4
```

## `repeat`
This command can be used to repeat any of the three match scheduling commands above. It takes a number of repetitions as an argument, followed by a command to repeat.

Example usage:
```
repeat 100 match Player OtherPlayer
```

## `include`
This command can be used to execute all commands of the file specified in its first and only argument.

Example usage:
```
include “path\to\settings file.txt”
include players.txt
```

Note that there are no checks in place to prevent an infinite loop of file inclusion, so have fun!
