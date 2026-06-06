### AI free repository - artificial intelligence is killing creativity and our nature!

# Win-PS2EXE
Graphical front end to the module version of the PS1-to-EXE-compiler PS2EXE

Author: Markus Scholtes

Version: 1.0.1.3

Date: 2026-06-04

With [PS2EXE](https://github.com/MScholtes/PS2EXE) originally created by Ingo Karstein you can compile Powershell scripts to real Windows executables. **Win-PS2EXE** is a small graphical front end to the script.

### Features and restrictions:
* WPF application that compiles without Visual Studio or MSBuild on every Windows with .Net 4.x
* only one source file
* drag'n'drop for file names

### Screenshot:
![Screenshot](Screenshot.jpg)

### How to compile:
Run **Compile.bat**.

### How to use:
Install module **PS2EXE**.

Start **Win-PS2EXE** by typing Win-PS2EXE in a powershell console and fill in the desired fields (only *Source file* is mandatory).

Click **Compile**, a powershell window opens and your powershell script will be compiled to an executable.

### Changes:
1.0.1.3: new GUI (thanks to https://github.com/Shayne55434)
* better .Net Core compatiblity and Powershell.Core detection (thanks to https://github.com/necrose99)
* new icon

1.0.1.2: new text field for additional parameters

1.0.1.1: target folder dialog added (code by Josip Medved, https://www.medo64.com/2011/12/openfolderdialog/)

1.0.0.3: file fields no longer run out

1.0.0.2: -noConfigFile is default now
