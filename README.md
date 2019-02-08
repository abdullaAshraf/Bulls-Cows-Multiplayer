# Bulls-Cows-Multiplayer
Android turn based brain teaser game with Unity.
# Project Status
This project is currently in development,the main gameplay mechanics is done like lobby teams select, choosing names and numbers for players, turn management, turn timer, guess helpers, notifications system, number and guess validation and calculate answers, also 5 different AIs with varying difficulty is implemented. 
## Screen shots
![alt text](https://imgur.com/fjNEUTp.png)
![alt text](https://imgur.com/zlZjdB1.png)
![alt text](https://imgur.com/hahljz8.png)
# Reflection
This is a personal project based on an old desktop game I tried to implement using java SE and custom forms.

The game idea is similar to other known games like bulls and cows or master mind but with the modifications to allow multiple players -in different teams- to try to guess the enemy number before getting eliminated which more fun and competition to the game, in addition to a story mode with speical boss fights, local network and online modes, battle royale mode and a lot of intersting boosts to help you.
# Implementation
The game starts with the main menu scene which navigate to the lobby making allowing player to host local network games,pick AI players and assign real local players or network players to the right teams, or join a game lobby which another player already created, the network connection in this game is based on simple `TCP` protocol and connection sockets.
The game coding is doen in `C#` as `Unity` scripts, and the game is implemeted in a way to allow for easier future updates and adding new modes.
some of the graphics are from [freepik](https://www.freepik.com) free packs and others where made using `Photoshop` and `Illustrator`.
