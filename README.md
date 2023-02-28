# SharpDune
C# port of the excellent [OpenDUNE](https://github.com/OpenDUNE/OpenDUNE).

In order to run the original game data files need to be present in a folder named `data`.

Running on Windows:
1. Edit paths (`datadir`, `savedir`) in sharpdune.ini
2. Select the `Debug` or `Release` configuration
3. Start SharpDune with the `SharpDune` profile

Running on Linux (WSL 2/WSLg):
1. Edit paths (`datadir`, `savedir`) in sharpdune.ini
2. Select the `DebugWSL` or `ReleaseWSL` configuration
3. Start SharpDune with the `WSL` profile

For video [SDL2](https://www.libsdl.org) and [SDL2 Image](https://www.libsdl.org/projects/SDL_image) are used.

For audio WinMM is used on Windows (sound and music) and PulseAudio or ALSA on Linux (sound).

IDE: latest Visual Studio Community 2022 Preview.

## Screenshots

Intro:

![](Images/intro.png)

Main menu:

![](Images/mainmenu.png)

Mentat:

![](Images/mentat.png)

In-game 1:

![](Images/ingame1.png)

In-game 2:

![](Images/ingame2.png)

Outro:

![](Images/outro.png)
