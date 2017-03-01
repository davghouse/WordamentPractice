Dave's Wordament Practice
==================

(English) Wordament-based desktop app for single-player practice. I think you need .NET 4.5 for this to run. 

Latest release [here](https://github.com/davghouse/Daves.WordamentPractice/releases/tag/v1.0.0). I'm working on this as an introduction to C#/.NET/WPF/XAML. In retrospect this was pretty useless because I used WPF in the same way I'd been using WinForms. I just handled events in the code-behind and didn't get experience with MVVM, data binding, triggers, templates... XAML in general.

![one](/Screenshots/1.PNG)
![two](/Screenshots/2.PNG)
![three](/Screenshots/3.PNG)

Use
---
* Right now I'm entering boards from Wordament and playing on them when I want to practice. Pressing Start without all the tile strings entered will randomly fill the remaining empty tiles with single letters. Pressing Start with full tile strings will guess at any point values that haven't been filled in. These guesses are only correct all the time (I think) for basic single-letter tiles. (See [here](https://github.com/davghouse/wordament-solver#limitations) for more information on limitations.)

To-do
-----
* Good (common-word-dense) board generation
* Rotation
* A dictionary with 16-letter words
* Tabbing with space and enter
* Sounds

