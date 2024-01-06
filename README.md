# DnDApp

.NET WPF app to move / copy files and folders to a single location using Drag-and-Drop

## Requirements

* net6.0-windows

## Default behaviour

The default behaviour is based on Windows Explorer.

If the source files and folders are on the same drive, they are moved, otherwise they are copied.

### Exemple

`C:\test.txt` will be **moved** to `C:\Documents\test.txt`

whereas

`C:\test.txt` will be **copied** to `D:\test.txt`

## Smart Copy

When you enable Smart Copy, you will be asked to select a **source** folder.

This feature will change the files' destination according to the relative path from the source folder.

This makes sorting through your hard drive much easier and faster.

### Example

If you have the following configuration
```
source: C:\Downloads
target: D:\Documents
```
`C:\Downloads\New Folder\test.txt` will be moved to `D:\Documents\New Folder\test.txt` instead of just `D:\Documents\test.txt`.

## Modifiers

Depending on the modifer keys that you press when dropping files, the default behaviour will change depending on those keys.

* Alt: Change ***target*** to dropped folder (drop payload must be a single folder)
* Alt + Shift: Change ***source*** to dropped folder (and enable smart copy if it's disabled)
* Shift: **Move** Files / Folder
* Ctrl: **Copy** Files / Folder

***Note**: The Shift modifier has the precedance over Ctrl which means that if you press Shift and Ctrl, only Shift will be taken into account*
