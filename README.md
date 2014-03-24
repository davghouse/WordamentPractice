wordament-practice
==================

Wordament-based desktop app for some single-player practice. I think you need .NET 4.5 for this to run. 

Latest release [here](link). I am working on this as an introduction to C#/.NET/WPF/XAML.

![one](/Screenshots/1.PNG)
![two](/Screenshots/2.PNG)
![three](/Screenshots/3.PNG)

Use
---
* Right now I'm entering boards from Wordament and playing on them when I want to practice. Pressing Start without all the tiles strings entered will randomly fill the remaining with single letters. Pressing Start with full tile strings will guess at any point values that haven't been filled in. These guesses are only correct all the time (I think) for single-letter non-special/high-value tiles.

To-do
-----
* Good (common word dense) board generation sounds like a fun related project.
* Rotation maybe. 
* Just like the solver I'm using the TWL dictionary for Scrabble so I don't have 16 letters words. Getting those would be good. 
* Tabbing with space and enter.
* Sounds.
* Only tabbing to point values corresponding to special tiles might be better than tabbing through all point values. Wouldn't get high-value single-letter tiles though.
