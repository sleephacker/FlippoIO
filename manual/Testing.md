
# Testing your program using FlippoIO

## Creating a Test Script

In the same folder where your copy of `FlippoIO.exe` is located, create a file called `test.txt`, along with a file called `default.txt`. You’re going to put your testing script in `test.txt`, and use `default.txt` to ensure that FlippoIO runs your test-script by default.

In `default.txt`, simply add a single command:

```
include test.txt
```

You are now ready to enter the commands necessary to test your program into `test.txt`.

## Registering Your Player

Before you can test your player, FlippoIO must know its filename. Simply enter the following command in `test.txt`:

```
player MyPlayer path\to\my\player.exe
```

Replace `MyPlayer` by a name of your choice and the path by the actual path to your players executable/jar/script. You can also give your player an alias for future reference in case its name is too long, using this command:

```
alias longName “Player with a ridiculously long name”
```

This allows you to reference the player called `Player with a ridiculously long name` by its new alias: `longName`.

## Running a Match

If this is your first player, you’ll probably want to use the built-in random player as your testing opponent. This player is always registered in FlippoIO, and has the name `Built-in Random Player`, as well as an alias: `Random`.

To schedule a match in which your player plays as white, add the following command at the end of `test.txt`:

```
match MyPlayer Random
```

To schedule a match in which your player plays as black, add the following command

```
match Random MyPlayer.
```

Note: those two commands together are the equivalent of the following command:

```
duel MyPlayer Random
```

The same goes for this command:

```
competition MyPlayer Random
```

## Running Multiple Matches

If you want to run e.g. ten matches in which your program plays as white and another ten in which it plays as black, you could do so in a single line using the following command:

```
repeat 10 duel MyPlayer Random
```

## Displaying Scores

At the end of your testing, you may want to know how many points your program scored. You can do this by putting the following command at the end of your script.

```
score display
```

This will display a ranking of all registered players and their scores.

## Complete Testing Script

Your final testing script could look something like this:
```
player “My First Player” path/to/my/first/player.jar
alias player1 “My First Player”
repeat 10 duel player1 Random
score display
```
