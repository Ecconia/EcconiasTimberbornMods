# Ecconias mods for Timberborn .zip

In this repository you will find all mods that I have created for Timberborn.

Hint: There won't be many...

## Structure:

In the solution folder (the root folder), there is a folder named `Timberborn` expected, that leads to the `Timberborn_Data/Managed` folder.\
In my case this folder will be a symbolic link to the latest game files. You can copy your game DLL dependencies to that folder if you like. 

Each mod has its own project in the solution.

To use the mods, build the projects or solution and copy the DLL and PDB files into a mod folder in the BepInEx plugins folder.

### Issues:

As I am using Rider on Linux, there are a few structural issues:\
- No build script or similar to package mod for upload or direct usage. (As it should work on Win & Unix).
- No good NuGet integration. (Rider is horrible with this, I have to integrate it manually and the NuGet file messes it up).
