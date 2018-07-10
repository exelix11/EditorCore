# EditorCore
This is a generic 3D level editor, it can be extended to support different games, right now it's only
a proof of concept, it requires more developement to adapt to different game engines.
The only projects needed are EditorCore and ModelViewerCore, the others are related to the extensions.

BfresLib,BnTxx,ByamlExt and SarcExt can be reused for other Switch games as they handle common file formats.

## What's done
  - 3D level editor, with search, drag, raycast, and undo
  - Interfaces for level files and objects
  - Plugin structure to load different extensions at once (but only one GameExtension)
  - Paths rendering
  - Super mario odyssey plugin to edit levels.
  - Mario Kart 8 Deluxe plugin to edit courses.

## What's missing
  - A native/GPU-accelerated 3D renderer, the current one uses WPF and is kinda laggy
  - Undo is not fully implemented
  - Probably other stuff
  
## Screenshots

![MK8Level](http://i66.tinypic.com/m8h2lc.jpg)
![OdysseyLevel](http://i64.tinypic.com/24fy9a0.jpg)

## Controls
There are two camera modes, choose the one you like from the settings.

Hotkey | action
|---|---|
Space | Move camera to selected object
Ctrl + drag object | Drag an object in the 3d view
Alt while dragging | Snap the object every 100 units
Shift while dragging | Snap the object every 50 units
\+ | Add a new object
D | Duplicate selection
Del | Delete selection
H | Hide selection from view
C | Edit the links of the selected object
B (while editing a links list) | Go back to the previous list
Q | Switch camera mode

## Building
This repo contains just the editor and its libs, to use it you also need a game module : \
[Super Mario Odyssey module](https://github.com/exelix11/OdysseyEditor) \
[Mario Kart 8 Deluxe module](https://github.com/exelix11/MK8DEditor)

To build a game module add it to the solution, you might get errors with references, restore nuget packages and check they have the right path in each project.
Remember to change the build path to the Ext folder in the bin directory or else the editor won't load the dll automatically. The dll name should end with Ext.dll 

## Credits
This editor contains code or libraries from:
- [KillzXGaming's BFRES C# code ](https://github.com/KillzXGaming/Smash-Forge)
- [gdkchan's BnTxx ](https://github.com/gdkchan/BnTxx)
- [Gericom's EveryFileExplorer](https://github.com/Gericom/EveryFileExplorer)
- [masterf0x's RedCarpet](https://github.com/masterf0x/RedCarpet)
