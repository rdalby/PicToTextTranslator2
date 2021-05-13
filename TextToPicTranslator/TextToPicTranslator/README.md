# Pic to Text Translator #
## Rachel Dalby ##

## Description ##
This application's purpose is to pull the text from an image allow the user to copy that text for later use. Then you can either paste the text
that was pulled from the image out and translate it or you can chose whatever text you would like to translate. It does include transliteration
for both the original text and the translated text.

You can choose from 90 different languages to translate to and from. You may need to install language packs on your devices for some of the
results to work properly and show the proper character. The transliteration feature is available to a subset of the languages and some are
one way meaning that it may not be able to transliterate all languages.

There are additional features and improvements in the works to make it even better including but not limited to user specific prior transaction
storage, image sharpening for even greater picture results, Speech recognition, and being Read service to allow the user to hear how a translation
would sound.

## Purpose ##
This started out as something I just wanted to be able to pull Japanese text mostly for the kanji to pull out of a picture. I noticed a need for this
while working on things in my Japanese class. There was always some sort of Kanji that I did not understand the meaning to and it did not have the
hiragana (Japanese alphabet) with it so that I could look it up.

The vast majority of the time these Kanji were in pictures so I could not copy and paste them into a translator either. All the other OCR applications
I could find did not work with Japanese text properly. So the idea came up for this final project.

It has since expanded from just English, Japanese, and Chinese (for the Kanji) languages to be pulled out of an image but for 90 languages to be pulled.
Then translated and even trasnliterated. I am hoping to design this well enough to be useful for traveling abroad, foreign language courses, and for everyday
use. A good OCR processor can be used to pull recipes, handwritten notes, etc to be put wherever you want them.

## Project URLS ##
Youtube Video
https://youtu.be/I47Fp6CwQ_Q

GitHub project page
https://github.com/rdalby/TextToPicTranslator

*Sidenote project name is reversed due to some issues with having to recreate the entire project after a file was corrupted and could not use the same name.



## Basic how to run instructions ##

- Download .zip file
- Extract .zip file (recommended filepath C:\Source)
- open .sln file or import project into VS
- create emulator / plug in phone/ or allow development on pc
- Build solution
- Select appropriate project type for device/emulator
- Run the project on emulator
- Take a picture or pick one from storage
- Copy your text over
- Try out the translate function
- look into the transliteration


## Test cases ##
OCR
- An image with multiple languages in it
- An image you have taken of writing
- An image with a lot of text
- Copy the text that was pulled out
** when using images do not have the text take up the 'whole' image allow the text to be taken from slightly further back

Translate
- Same language translations - return original
- English text to Japanese - should see transliteration under English and Japanese results sections
- Japanese to English - should see no transliterations under English and Japanese results sections


## Possible Android Error On Build ##
If you receive an error that states "Failed to create JaveTypeInfo for class........"
You need to save the program in a shorter file path. The issue is that the filepath to the solution or project is just to long
Try saving in C:\Source\TextToPicTranslator for instance instead of where you currently have it saved. Then reopen and build the project
the error should be gone by that point.


# what you need to if creating your own - High Level #
Visual Studio 2019 - community edition is fine also had EDU versions that will work
Install the packages for Xamarin, UWP, Android, and IOS
Install Visual Studio emulator
Create a free Azure account - don't really need to for this step unless if you want to tweak the endpoints of 2 api's
Install Nugets
-xam.plugin.camera
-xam.plugin.connectivity
-Microsoft.Azure.CognitiveServices.Vision.ComputerVision
-Microsoft.CSharp
-NewtonSoft.Json
-SixLabors.ImageSharp
-Xamarin.Essentials
-Xamarin.Forms

permissions need to be added for both Android and UWP for camera and external files - IOS does not require an additional step as the nugets take care of it
- see documentation from the xam.plugin.camera and xam.plugin.connectivity nugets they have very detailed in specific instructions on how to set these up
- https://github.com/jamesmontemagno/MediaPlugin#android-required-setup

set up emulator or phone connected via USB (need to make sure USB connectivity is installed with your Visual Studio installation if going this route)
- Visual studio has fantastic walk through instructions for setting up an emulator if you have not done it highly recommend. They even reference Google's
- emulator on their main page if you prefer to use that you can. But I did use the VS android emulator with Hyper-V
- https://visualstudio.microsoft.com/vs/msft-android-emulator/
- https://docs.microsoft.com/en-us/xamarin/android/get-started/installation/android-emulator/
** you do need windows pro to use Hyper-V

If you have an android phone you can enable that to use enabling developer options on your phone prior to. I have provided Samsungs instructions below for reference.
- https://www.samsung.com/uk/support/mobile-devices/how-do-i-turn-on-the-developer-options-menu-on-my-samsung-galaxy-device/

Setting up Windows computer for use instructions below
- https://docs.microsoft.com/en-us/windows/apps/get-started/get-set-up

Setting up Mac to deploy to IOS device
- https://docs.microsoft.com/en-us/xamarin/ios/get-started/installation/windows/connecting-to-mac/





# My Development Setup Documentation #


## What was used ##
- Environment
- Nuget packages
- APIs
- Deployed options


## Environment ##

### Visual Studio setup Installation Options  ###
My Visual Studio Installation Setup:
Version Enterprise 2019 16.9.4
Workloads installed
- .Net desktop development
- Universal Windows Platform development
- Mobile development with .Net

Individual Componets: (all of these may not be needed but it is my current setup reduced obvious ones but kept full list in case issues)
.Net
- .Net 5.0 Runtime
- .Net Core 2.1 Runtime (LTS)
- .Net Core 3.1 Runtime (LTS)
- .Net Framework 4 targeting pack
- .Net Framework 4.5 targeting pack
- .Net Framework 4.5.1 targeting pack
- .Net Framework 4.5.2 targeting pack
- .Net Framework 4.6 targeting pack
- .Net Framework 4.6.1 targeting pack
- .Net Framework 4.7.2 targeting pack
- .Net Framework 4.8 SDK
- .Net Framework 4.8 targeting pack
- .Net Native
- .Net SDK

Cloud, database, and server
- CLR data types for SQL Server
- Connectivity and publishing tools
- SQL server Express 2016 LocalDB
- Web Deploy

Code Tools
- Class Designer
- Code Clone
- Code Map
- Developer Analytics tools
- DGML editor
- Git for Windows
- GitHub extension for Visual Studio
- Live Dependency Validation
- NuGet package Manager
- Text Template Transformation

Complilers, build tools, and runtimes
- .Net Compiler Platform SDK
- C# and Visual Basic Roslyn compilers
- MSBuild


Debugging and testing
- .Net profiing tools
- Debugger for GitHub Codespaces
- IntelliTrace
- Just-In-Time debugger


Development activities
- C# and Visual Basic
- IntelliCode
- Live Share
- Live Unit Testing
- Xamarin
- Xamarin Profiler
- Xamarin Remoted Simulator


Emulators
- Google ANdroid Emulator (API Level 25) (local install)
- Intel Hardware Accelerated Execution Manager (HAXM)(local install)


Games and Graphics
- Image and 3D model editors


SDKs, libraries, and frameworks
- Android SDK Setup (API level 25)(local install for Mobile development with C++)
- Android SDK setup (API level 30)
- Entity Framework 6 tools
- OpenJDK (Microsoft distribution)
- TypeScript 4.1 SDK
- USB Device Connectivity
- Windows 10 SDK (10.0.18362.0)
- Windows 10 SDK (10.0.19041.0)
- Windows Universal C Runtime




### Operating System ###
Windows 10 Pro Version 20H2
Windows Features modified
- Hyper-V enabled
- Windows HyperVision Platform enabled



## Nuget Packages ##
Xam.Plugin.Connectivity
Xam.Plugin.Media
Microsoft.Azure.CognitiveServices.Vision.ComputerVision
Microsoft.CSharp
NewtonSoft.Json
SixLabors.ImageSharp
Xamarin.Essentials
Xamarin.Forms

## APIs ##
Microsoft Azure Computer Vision API
Microsoft Azure Congitive Services Translation API

Created a service for both of those to have an endpoint and subscription key to reference in the app
Used the Free service levels that allow 2 million translations a month and about 5,000 OCR calls a month

Xam.Plugin.Media and Xam.Plugin.Connectivity are also both technically APIs

## Deployed Options Tested ##

Android project
- Samsung S20 directly plugged in
- Emulated Pixel 2 Pie 9.0 - API 28


UWP project
- Local Machine with developer options enabled


IOS project
- Emulated a mac to pair to my Windows VS
- used "simulator" with Mac
- deployed to physical ipad from emulated Mac