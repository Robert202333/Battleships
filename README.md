Compiled with Microsoft Visual Studio Community 2022 (64-bit) - Current Version 17.2.1

Solution consist of 3 projects:
- BattleshipConsole
- BattleshipWPF
- GameModel

GameModel delivers game engine but is separated from the UI tier.
WPF and Console applications provide implmented interfaces responsible for displaying board and interaction with user.
WPF version provides also possibility to edit game configuration like number and size of ships, map size and ship creation rules.
'Debug Mode' option shows all created ships grayed, allowing to check if game is working correctly. 

As user (in WPF version) may create his own rules, it may happen that ship placement on map may be difficult or impossible.
Ship creation is done by running parallel tasks. Result is taken from the first fineshed one. If no task is finished in 6 seconds all tasks are interrupted.


