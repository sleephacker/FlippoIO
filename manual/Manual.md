# FlippoIO General Operation
## Executing Commands
FlippoIO operates by reading commands to execute from one or more files. These commands can be used to register players, to change settings, or to start matches between players. The syntax of these commands should be familiar to anyone: each command is followed by a space-separated list of arguments. To accommodate filenames that include spaces, arguments that are enclosed in double quotes (`"`) are interpreted as a single argument. A typical command will look something like this:

```
command argument1 “argument with spaces” argument3
```

For a complete list of commands see: https://github.com/sleephacker/FlippoIO/blob/master/manual/Commands.md.
If no command-line arguments are passed to FlippoIO, the program will try to open and execute a file called `default.txt`. A different file can be specified in the first command-line argument to FlippoIO, like this:

```
FlippoIO my script.txt
```

Note that filenames that contain spaces don't have to be enclosed in double quotes because FlippoIO only takes one argument.

## Viewing Matches and Player Logs
FlippoIO saves all logs to a per match folder that has the same name as the corresponding match. E.g. the player logs and match log of `Match #1` will be saved in the folder `logs\Match #1`. In each folder there will be a file called `match.txt` in which the match can be viewed. Depending on whether each player wrote to Standard Error, the files `white.txt` and `black.txt` can also be found in each folder and contain the Standard Error output of the white and black player during that match.

## Running Java and Python Players
If your Java and Python have been installed properly, no additional effort is required to run Java or Python players. Files with the .jar extension will by default be run using the following command: `java -jar path\to\your\player.jar`. For files with the `.py` extension, the following command is used by default: `py path\to\your\player.py`.

This default behaviour can be modified to accommodate faulty installations using the `set JavaCmd` and `set PythonCmd` commands. For more information on this, see: https://github.com/sleephacker/FlippoIO/blob/master/manual/Settings.md.

Languages that do not produce `.exe` files, other than Java and Python, are not supported at this time.
