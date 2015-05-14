# Xamarin Code Comfort Addins

I'm a long time Jetbrains user, and there are some features of the Jetbrains IDE that I sorely miss in Xamarin Studio.

As such, I wrote a simple addin that adds those features I miss, plus adds features useful for Xamarin forms developers:

  * List class members via hotkey,
  * Filter and jump straight to a class member,
  * Hotkey for previous/next member,
  * Filterable file history,
  * Jump to ViewModel from Xaml/Code behind,
  * Jump to CodeBehind/xaml from ViewModel,
  * More forms stuff coming soon...
  

##Installation
Either build this in xamarin, or install the package via the package manager.

##Usage
All commands are available from the edit menu. Hotkeys can be reassigned in settings to suit your comfort needs. Out the box.

###History
As you open documents, your history list will become more populated. It contains up to 30 recent documents. Pressing **APPLE/WINDOWS+SHIFT+H** will bring up your history window. Use up/down arrow or type to filter the list. Enter/click to select.

###Members
Press **CTRL+F12** to bring up the members menu. All members in the current document are listed in order. Type to filter them, up/down to change selected and enter/click to select.
Selecting will take you straight to that member.

Press **CTRL+UP** or **CTRL+DOWN** to navigate between members in a file.

###Xaml addins
Press **APPLE/WINDOWS+F9** to switch between Xaml/View model files. 

Press **APPLE/WINDOWS+F10** to switch between Xaml code behind/View model files. 
This functionality expects that you follow the following naming convention:

  * Your View Model files are named XXXVM.cs or XXXViewModel.cs
  * Your Xaml files are named XXX.xaml,
  * Your codebehind files are named XXX.xaml.cs,
  * All files are located in the same folder.
  

##Motivation
  * I don't like touching the mouse while I work,
  * I don't like typing lots of characters to navigate,
  * I don't want to think about nor spend time navigating between where I'm writing code.

### But I can already do some of this stuff in Xamarin
Yes, that is true, especially recent files; but for me it just isn't as quick as it is in other IDEs. For example filtering the history allows me to get to the documents I'm actively working on, with very few keypresses and no need for the mouse. The Xamarin search box is great; but if you work on a big project, it comes up with so many options, so you need more keypresses to filter out what you don't want.

##State of the addin
This is my first add in with Xamarin studio and it certainly shows. It's functional enough to do what I want. 

TODO
  
  * The pattern matching for history and members has a few glitches; but I'll eventually replace that, with the expected xamarin implementation. 
  * The popups are also pig-ugly and will be updated shortly.
  * I can't guarantee this won't crash. It's a 0.1, work in progress. Use at your own risk!


##Thanks
Thanks to Frank A. Krueger who wrote the C# Interactive Addin, which formed the basis of my project and helped me get started making monodevelop addins.

##Contribute/Contact
Please feel free to get in touch if you want to collaborate.