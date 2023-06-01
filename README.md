Compiled with Microsoft Visual Studio Community 2022 (64-bit) - Current Version 17.2.1

Solution consist of 3 projects:
- BattleshipConsole
- BattleshipWPF
- GameModel

GameModel delivers game engine which is separated from the UI tier.
WPF and Console applications provide implemented interfaces responsible for displaying board and interaction with user.
WPF version provides also possibility to edit game configuration like number and size of ships, map size and ships creation rules.
'Debug Mode' option shows all created (even not hit) ships grayed, allowing to check if game is working correctly. 

As user (in WPF version) may set his own rules, it may happen that ships placement on map may be difficult or impossible.
Ship creation is done by running parallel tasks. Result is taken from the first finished one. If no task is finished in 6 seconds all tasks are interrupted.

GameModel library allows to customize ships placement algorithm (by deriving from AbstractGameCreator class), however WPF and Console applications use  
provided DefaultGameCreator.

Unit tests cover GameModel module. 
