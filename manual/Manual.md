# FlippoIO General Operation
## Executing Commands
FlippoIO operates by reading commands to execute from one or more files. These commands can be used to register players, to change settings, or to start matches between players. The syntax of these commands should be familiar to anyone: each command is followed by a space-separated list of arguments. To accommodate filenames that include spaces, arguments that are enclosed in double quotes (`"`) are interpreted as a single argument. A typical command will look something like this:

```
command argument1 “argument with spaces” argument3
```

See the complete list of commands [here](https://github.com/sleephacker/FlippoIO/blob/master/manual/Commands.md).
If no command-line arguments are passed to FlippoIO, the program will try to open and execute a file called `default.txt`. A different file can be specified in the first command-line argument to FlippoIO, like this:

```
FlippoIO my script.txt
```

Note that filenames that contain spaces don't have to be enclosed in double quotes because FlippoIO only takes one argument.

## Viewing Matches and Player Logs
Matches and player logs can be viewed using the [match viewer](INSERT_LINK_HERE). FlippoIO saves all logs to a per match folder that has the same name as the corresponding match (e.g: the player logs and match logs of `Match #1` will be saved in the folder `logs\Match #1`). If enabled in the settings, each folder may contain a file called `match.txt` in which the match can be viewed. By default there will be a `match` file which can be opened using the [match viewer](INSERT_LINK_HERE). Depending on settings and whether each player wrote to Standard Error, the files `white.txt` and `black.txt` can also be found in each folder and contain the Standard Error output of the white and black player during that match.

## Running Java, Python and JavaScript Players
If your Java and Python have been installed properly, no additional effort is required to run Java or Python players. 

Files with the `.jar` extension will by default be run using the following command: `java -jar path\to\your\player.jar`. For files with the `.py` extension, the following command is used by default: `py path\to\your\player.py`. 

This default behaviour can be modified to accommodate faulty installations using the `set JavaCmd` and `set PythonCmd` commands. For more information on this, see the [list of settings](https://github.com/sleephacker/FlippoIO/blob/master/manual/Settings.md).

## Running JavaScript Players
JavaScript players are run using [Cscript](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/cscript), which comes standard with Windows. You can change the path to Cscript using the command `set JavaScriptCmd`, but on normal Windows installations this shouldn't be necessary.

However, in order for your script to remain compatible with both Cscript and V8 JavaScript Engine (which is used by the CodeCup backend), you have to add the following lines at the start of your program:
```JavaScript
if(typeof print !== "function")
	print = function(s) { WSH.Echo(s) };
if(typeof readline !== "function")
	readline = function() { return WSH.StdIn.ReadLine() };
```
These lines define the `print()` and `readline()` functions (which are used in V8 scripts) as their Cscript equivalent, but only if they are not defined yet. When running under V8 these lines do nothing, but when running under Cscript they ensure `print()` and `readline()` can be used later on in the script.

## Other Languages
All compiled languages are supported since they all produce executable files. Languages that do not produce `.exe` files, other than Java, Python and JavaScript, are not supported at this time.