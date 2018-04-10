Wordament Practice
==================

Wordament-based desktop app for single-player practice.

Latest release [here](https://github.com/davghouse/WordamentPractice/releases/tag/v2.1.2).

![Solving](/Screenshots/Solving.png)
![Showing](/Screenshots/Showing.png)
![Wonderments](/Screenshots/Wonderments.png)

Implementation
--------------
For details about how a board's solution is built, see [my solver library](https://github.com/davghouse/Daves.WordamentSolver), whose NuGet package I reference in this project.

I'm using this project to learn some WPF and MVVM. Through data binding the solution updates instantly when any changes to the board are made.
I think I've achieved pretty good MVVM-ness, but the PracticeView.xaml.cs probably has some handling that could be refactored into a view model.

When the game is started, the board generation quality factor controls how much work is done to generate a decent board.
It's configurable through the Settings menu and the default is 6.
In the default case, (6 * the number of empty tiles) boards are randomly generated (excluding tiles with Z, Q, X, and J) and solved.
The board with the most words is chosen to play on.
This is about as naive as it gets, but the solver is fast enough to make it a viable strategy, yielding boards of comparable density to those I see on Wordament.

The found path for a word is displayed when selecting an item in the found list, the best path for a word is displayed when selecting an item in the solution list.
All words for a path are granted when a path is traversed. None of this really matters unless you have a crazy board like the last one pictured.

To-do
-----
* Rotation
* Sounds
* Animated word highlighting (I tried, but couldn't get it good enough)
* More responsive path building (Wordament's seems to understand the tile you're coming from, I just use circles to cut **all** the corners off)
