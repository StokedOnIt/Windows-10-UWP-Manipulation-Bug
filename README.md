# Windows-10-UWP-Manipulation-Bug

e.Position incorrect and ManipulationStarting stops firing

This app displays a bug in the Windows 10 UWP app Manipulation

Happens on a Surface Pro 1 with Windows 10 version 1809 for sure. Not sure about other devices.

Compile and start the app in Visual Studio 2017 Version 15.9.5.

Manipulation works great at first.

Then double tap and try to manipulate quickly multiple times
by tapping 2 fingers on the screen and sliding during the pause.

After the Manipulation Center point is wrong in the upper left

This is due to the e.Position coming from the Manipulation being incorrect after.

In addition the Event ManipulationStarting does not fire afterward as well.
