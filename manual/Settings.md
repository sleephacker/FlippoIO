
# List of FlippoIO Settings

FlippoIO allows various settings to be changed. All settings below are strongly typed, and are parsed using `type.parse()` in C#, where `type` is the type specified between parentheses.

## `LogDebufInfo (bool)`

Indicates whether or not to display debugging info. Set to `false` by default.

## `SuspendPlayers (bool)`

Indicates whether or not to freeze players when it’s not their turn. Must be set to true for the most accurate measurement of time usage. Note that setting this to `true` also causes significant overhead. Set to `false` by default.

## `AsyncRead (bool)`

Indicates whether or not to read player output asynchronously. Must be set to `true` to properly detect timeouts. Set to `true` by default.

## `KillPrograms (bool)`

Indicates whether or not to kill player processes after sending `Quit` to their Standard Input. Set to `true` by default.

## `NameMatchByPlayers (bool)`

Indicates whether or not to name matches by the names of the white and black player followed by a number. If set to false, matches will be named using just a number. Set to `true` by default.

## `TimeLimit (int)`

Specifies the time limit for players. Set to `5000` by default.

## `ReadTimeMargin (int)`

Specifies the extra time margin that is used to wait for a player timeout. Note that this doesn’t give players more time, but exists to prevent the thread waiting for a timeout from becoming active too soon which could interfere with the availability of the CPU during a player’s turn. Set to `200` by default.

## `MilliSecondsBeforeKill (int)`

Specifies the waiting period between sending `Quit` and killing a player process. Can be used to allow a player to write its last words to its Standard Error. Only applies when `KillPrograms` is set to true. Set to `0` by default.

## `ThreadCount (int)`

Specifies the number of matches to execute concurrently. Set to `Environment.ProcessorCount` by default.

## `JavaCmd (String)`

Can be used to specify a replacement for the java command. Files with the .jar extension are started using the following command:
```
<JavaCmd> -jar path\to\file.jar
```
Set to `java` by default.

## `PythonCmd (String)`

Can be used to specify a replacement for the py command. Files with the .py extension are started using the following command:
```
PythonCmd path\to\file.py
```
Set to `py` by default.

## `WhiteChar (char)`

Specifies the character to use to represent white pieces in match logs. Set to `X` by default

## `BlackChar (char)`

Specifies the character to use to represent black pieces in match logs. Set to `O` by default

## `EmptyChar (char)`

Specifies the character to use to represent empty tiles in match logs. Set to `.` by default
