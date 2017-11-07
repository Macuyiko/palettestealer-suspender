# palettestealer-suspender

A tool to prevent color problems in old Directdraw games

PalettestealerSuspender is a launcher tool which allows you to play Directdraw games in fullscreen on Windows Vista, 7, 8 and 10 without color problems.

*Legacy note*: this program was pretty popular around 2010-2012, but now has fallen out of use, since most games have been patched to prevent this issue. The source code is provided on this repository for reference. The tool itself is not actively maintained any more.

See [this post](http://blog.macuyiko.com/post/2009/solving-color-problem-red-grass-purple-water-in-age-of-empires-2-age-of-kings-the-conquerors-and-others-too-on-vista-and-windows-7.html) for a full description of the problem and solutions, and how this tool came to be.

## Works with:

*   Worms Armageddon
*   Age Of Empires (and Rise Of Rome)
*   Age Of Empires 2: Age Of Kings (and The Conquerors) (note that the HD version and UPatch also fix the color issue)
*   Starcraft (note that Blizzard fixed the color issue in a recent patch)
*   Diablo 1
*   Diable 2 (note that Blizzard fixed the color issue in a recent patch)
*   Star Wars: Galactic Battlegrounds
*   Fallout 1
*   Fallout 2

## Usage (GUI)

After opening the program automatically places itself in your task tray and loads your saved settings. Settings are saved in "save.xml"; you may delete this file and start the program again to start with a blank slate if you desire to do so. Only one instance of the program can run at once.

Right clicking the task tray icon brings up a menu with the following options:

* Games: this menu will subdivide itself into a list of games. When starting the program for the first time, no games will be present. For each game added, a (un)checkable entry will appear, together with an option to start a game manually.
* Configuration...: this menu item brings up the configuration screen, where you can add games and manage settings. Double clicking the icon has the same effect.
* Quit: exits the program completely.

The configuration screen (brought up by choosing "Configuration..." in the right-click menu or double clicking the task tray icon) allows for the following:

* A list of games: you can add game executables here using the "Browse..." and "Add" buttons. Double clicking removes entries from the list. Checking an item denotes that you want to monitor this particular game. More about this function later. Every change you make to this list will also be reflected in the right-click menu under "Games".
* A list of palette stealing processes: which should require no tampering in most cases. Palette stealing processes are automatically detected and added to the list while the configuration screen is active. If the program isn't working correctly for you, you might want to leave the configuration screen open while running the game to let new entries appear in this list. By checking and unchecking items you can choose which processes you want to suspend. Note that it is impossible to check or uncheck some processes.
* The method to suspend and resume palette stealing programs. In most cases, the recommended option should work well enough. Try the other options in case the program isn't working.
* Buttons. "Save and Hide" saves your settings to save.xml and hides the program back to the task tray. "Quit" completely exits the program and has the same effect as picking "Quit" from the right-click menu.

There are two ways to play your games. The first one is to "manually start" your games with the right-click menu. PalettestealerSuspender will automatically suspend all palette stealing programs and resume them when exiting the game.

The other way is to let PalettestealerSuspender monitor your list of running processes to see if a game has been started. To allow PalettestealerSuspender to monitor a particular game, check its entry in the configuration screen or through the right-click menu itself. This option provides the most flexibility, as you can just configure a list of games you want to monitor in the configuration screen, make sure they are checked, and then just hide PalettestealerSuspender and forget all about it. It will automatically react when you start a game, while still allowing to "manually start" games from the right-click menu as well.

## Usage (Console)

The program also contains a console mode, so it can for example be used in batch scripts. Following command line parameters are currently available:

    PalettestealerSuspender.exe /nogui /game <GAME_PATH> [/wait] [/restore]

* `/nogui`: must be set to use the program in console mode, otherwise the GUI window will open and other parameters will be ignored.
* `/game`: must be set, <GAME_PATH> contains the full path the the game's executable. Double quotes (") may be used to delimit complex paths (see example below).
* `/wait`: optional. Don't start the game automatically but wait until the game is started manually.
* `/restore`: optional. Use this in case of a lockout/frozen system to try to restore it to a non-suspended state. Other parameters will be ignored.

Example:

    PalettestealerSuspender.exe /nogui /game "C:\Games\Age Of Empires 2\age2_x1\age2_x1.exe"

## License and Disclaimer

This work is licensed under a Creative Commons Attribution-Share Alike 2.0 Belgium License as stated at http://creativecommons.org/licenses/by-sa/2.0/be/.

THIS SOFTWARE IS PROVIDED "AS IS" AND ANY EXPRESSED OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE REGENTS OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
