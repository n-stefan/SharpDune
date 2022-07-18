# SharpDune
C# port of the excellent [OpenDUNE](https://github.com/OpenDUNE/OpenDUNE).

In order to run the original game data files need to be present in a folder named `data`.

Running on Windows:
1. Edit paths (`datadir`, `savedir`) in sharpdune.ini
2. Start SharpDune with the `SharpDune` profile

Running on Linux (WSL):
1. Install and start an X Server (like VcXsrv)
2. Edit the environment variable `DISPLAY` in launchSettings.json
3. Define the constant `LINUX` in SharpDune.csproj
4. Edit paths (`datadir`, `savedir`) in sharpdune.ini
5. Start SharpDune with the `WSL` profile

For video [SDL2](https://www.libsdl.org) and [SDL2 Image](https://www.libsdl.org/projects/SDL_image) are used.

For audio (MIDI and WaveOut) winmm.dll is used. On WSL audio isn't supported currently.

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
