# TerrainTools

A set of tools to help fix Valheim Terrain lag

## Installation  
Get the latest package from [Releases](https://github.com/BlueAmulet/TerrainTools/releases) and unpack it in the same folder as valheim.exe  
If you don't already have BepInEx, grab it from [Valheim Thunderstore](https://valheim.thunderstore.io/package/1F31A/BepInEx_Valheim_Full/) and place the **contents** of 'BepInEx_Valheim_Full' in the same folder as valheim.exe  
If you don't know where valheim.exe is, right click Valheim in your Steam library, go to Manage, and Browse Local Files

https://github.com/BlueAmulet/TerrainTools/releases

## Usage

TerrainTools adds a new set of commands to the game:

```
countterrain [radius=max]
	Counts nearby terrain modifications
	Groups them by level, smooth, and paint modifications
resetterrain [radius=5] [type]
	Remove nearby terrain modifications
	Valid types are: all, level, smooth, paint
debugterrain
	Enables a terrain modification visualization mode
	This mode can be laggy depending on how many modifiers are nearby
	Can also be toggled via a hotkey (default F4)
debugstrength [new strength]
	Strength of lights in visualization
debugdistance [new distance]
	Distance of visible lights in visualization
```

TerrainTools also creates a config file at BepInEx\config\TerrainTools.cfg

LightDistance: (default: 25)  
Distance of visible debug lights

LightStrength: (default: 0.5)  
Strength of debug lights

Toggle: (keybind, default: F4)  
Terrain debug toggle keybind

## Building  
You will need [Visual Studio 2019](https://visualstudio.microsoft.com/vs/community/) and .NET Framework 4.7.2  
All required libraries are bundled with this repository.  
They have been stripped and do not contain proprietary code.
