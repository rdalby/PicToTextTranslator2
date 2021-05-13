# Pic To Text Translator

## Rachel Dalby

## supported devices ##
Android
IOS
Windows

## Structure ##

I used a MVVM structure for better adaptability and as that is the perferred why for the Xamarin designs to be
There are 4 projects within the solution that are "linked" to eachother and main project is programmed on the back end with C#.
The front UI features are prodominately created/managed with XAML through data bindings. The other 3 projects in the in the solution
form the links to the desired system you want to build the project to. UWP for windows, IOS for apple, and Android for android.

4 Projects in solution:
-Main project with Xamarin C#
-Android project
-IOS project
-UWP project

*Side note it is downloaded as TextToPicTranslator because I ended up with a corrupt file somewhere and I had to basically start fresh
but a new project and it wouldn't let me use the same name even in a different directory.


## Main Project Elements ##
Top level
- App.xaml
- AppShell.xaml
- AssemblyInfo.cs

Dependencies
- Assemblies
-- Windows.Foundation.UniversalApiContract
- Packages
-- Microsoft.Azure.CognitiveServices.Vision.ComputerVision
-- Microsoft.CSharp
-- NETStandard.Library
-- Newtonsoft.Json
-- SixLabors.ImageSharp
-- Xam.Plugin.COnnectivity
-- Xam.Plugin.Media
-- Xamarin.Essentials
-- Xamarin.Forms


UI/Views
- AboutPage.xaml
- OCRPage.xaml
- TranslatorPage.xaml 


ViewModels
- AboutViewModel.cs
- BaseViewModel.cs
- OCRViewModel.cs
- TranslatorViewModel.cs 


Model
- Language.cs
- Shared.cs
- Text.cs

Services
- IDataStoreO.cs
- IDataStoreT.cs
- OCR.cs
- Sharpen.cs
- Translator.cs




## Top Level ##

### App ###
Sets some basic stylings on button
The code behind registers the dependency services for the Translator and OCR services.

### AppShell ###
This is really important for the multi-platform I set some platform specific values and it really is the main style for the application. It creates the menu items and routes them to where they need to go. Sets the Primary properties
for items.

Really only initialize from the back end.

### AssemblyInfo ###
Sets it to compile and save time for hot reloads and such


## Dependencies ##
Each of the packages have been referenced in the above and have been listed at the top.  The Xam plugin packages are needed for the OCR section to get and process that, as well as the Translate section for checking internet.
The Newtosoft Json library is needed to serialize and deserialize the API calls. SixLabors ImageSharp is going to be needed in the future for image sharpening. The Microsoft CSharp, NetStandard, Windows Foundation Universal API Contract,
Xamarin Essentials and Xamarin Forms are all needed for base functions and purposes.



## UI ##

Sidebar menu for View/Page selection

3 main UI Views
- About View
- Text from Pic View
- Translator View


I wanted a streamlined easy to access/know what you are looking at type of APP. I chose simplilicty for the user. I didn't want to chance it with the
user and end up having them break something that I didn't anticipate. I have the all the data that displays linked through data bindings to allow
for a more "seamless" approach. This also allows for elements to be adapted later to be used across didn't pages/views and maintain the values without
try to pass multiple values through and risk breaking.

The color scheme is semi analogous to keep it looking clean. The buttons are rounded to give it a more subtle and not harsh feel. Apps to me feel old/
outdated if they are really boxey. I added frame elements for the translation, transliteration, and result sections respectively to emphasize where
you would look for the responses.

All of my data bound entries and labels are scaleable so that you can input as much text as you want in them. I have not texted past a page of words but
I know that a full pages does work.

### About View ###
The About view just gives a brief How to and what each of the views offer/ what to do with them.

There is only the initialize code and that it inherits the contenpage interface behind this view in the .cs file.

### OCR View ###
Originally the OCR view had a translate button at the bottom to allow the user to push that result to the translate view. I removed that (commented out) for
now to focus more on the other elements. For now it just has the copy button on the bottom after you pull in a picture either by taking one or picking one from
their storage on the device. This allows for the text to be copied from the label. The button is required at this point because with xamarin you cannot copied from
a label at this time. In order to get something where you could copy from and it not be editable would be to create a custom element and I did not have the time to
do that.

If something went wrong with the OCR process a badrequest response gets sent to the Result box. If nothing gets pulled from the image then a "no text or to blurry"
comment gets put in the results.

The code behinds sets the data binding context to the OCR ViewModel, onapearing calls the base onappearing tasks which grab current data bindings, and the OCRViewModel
Onappearing function to clear out the values that shouldn't be there. This also contains the clipboard button on click because it does not always function properly as a
command.

### Translate View ###
The Translate view has 4 frames on the bottom for Original text, Translated Text, Orginal Transliteration (orginal text transliterated for a native speaker of the "To"
language), and the Translated Transliteration (translated text transliterated for a native speaker of the "from" language). To get to that point there is the editor
element for the text that needs to be translated, 2 dropdown pickers that were set to have a selectedindex for "from" to be "Detect" and "to" to be "English", and a tranlate
button. The 2 pickers are populated by the language lists that are fired off to 'get' and 'load' when some first loads the Translate page. (of note Klingon is valid tranlation
language).

The code behinds sets the data binding context to the Translate ViewModel, onapearing calls the base onappearing tasks which grab current data bindings, and the TranslateViewModel
Onappearing function to clear out the values that shouldn't be there.


## ViewModels ##
Each of the ViewModels don't really do a whole lot in terms of manipulating any data they are really
designed more to grab the manipulated data and return it to the view. Most of the functions/methods
in my ViewModels actually send the data to a service to be worked on.

Also they call the "propertychanged" values so that all the values sync up across the classes.


### AboutViewModel ###
Sets the Title value and is the base for anything that I may want to add later. It inherits the properties of the BaseViewModel.

### BaseViewModel ###
This is the base that the other ViewModels actually build off of. It sets the dependency services and
contains the SetProperty method that gets called when the the set methods get called for any data bound
variable. This is based off of the INotifyPropertyChanged function that is required for successful updating
of the Views/ variables to sync across ViewModels/services.

### OCRViewModel ###
This one sets and gets all the data bound values for the OCRPage view it inherits from the BaseViewModel. Clears the values OnAppearing to make sure that no stagnant data is left.

This is what calls all the process functions for the photos. It routes the photo picker and the photo taker tasks to their prospective functions that fire off the tasks the OCR service class.

### TranslateViewModel ###
Contains the Get/Set functions for the data bound propeties on the Translate View.  It inherits the properties of the BaseViewModel.

Creates the command for the Translate button to fire off the task that will send
data to the translate service for processing.

The OnAppearing method clears out the values when you leave the page so that the stagnant
data does not persist

Most of the functions get and set values for the data bindings.




## Models ##

### Language ###
This sets the Language class and the LiterateLanguage class. These are used to make observable collections and lists to be processed by the
ViewModels for UI elements, for the dictionaries to compare to, and for any other reference.

### Shared ###
This model serves are more of a bridge between the viewModels and functions and contains the get/set functions to assist with making sure they are
accurately data bound and passed with the current values.

### Text ###
The structure used for the the OCR section. This is really expandable for any additional features I want to include later to enhance the OCR calls.



## Services ##

### IDataStoreO ###
This is one of the 2 declared dependency services declared in the BaseViewModel. This sets the functions will end up being used in The OCR service.
This allows for better data binding calls and make sure that all the views/viewmodels/services/etc all have the same values matching up.
It is mostly just a list of "required" tasks that the OCR service must have implemented to meet the requirements of the interface.

### IDataStoreT ###
This is the other one of the 2 declared dependency services declared in the BaseViewModel. This sets the functions will end up being used in The
Translator service. This allows for better data binding calls and make sure that all the views/viewmodels/services/etc all have the same values matching up.
It is mostly just a list of "required" tasks that the Translator service must have implemented to meet the requirements of the interface.


### OCR ###
This is where all the OCR process lies. It is set to inherit from both the OCRViewModel and the IDataStoreO with the Text model. The interface tasks set
the values for the Text model that can be then passed to the OCR ViewModel or to other methods/tasks.

I am using multiple API service calls, I created services using the Microsof Azure ComputerVision API through AZURE. The subscription key and endpoint are currently
hardcoded. Ideally with additional future changes these would be made variables that would referenced from a secured database. For this project purposes I did leave them
hard coded.

This service calls the device specific functions to either open the camera and store the picture taken, or to select a picture from the users device storage. To do this
I use the Xam.Plugin.Media Nuget that assist in making the calls without me having to write the device specific code. Also used the Xam.Plugin.Connectivity Nuget to ensure
that the user is in fact connected to the internet as they will need internet access in order to process the API requests.

Once it gets the file it sends it back to the Viewmodel so that the busy/loading indicators can be fired off. Once that is done the file gets sent back to the OCR Service to
be processed. The file gets sent to the TextPictureAsync task which calls the Azure API. Now the first call it makes in an attempt to get a result is the ReadInStreamWithHttpMessagesAsync
call. This one is good to get handwritten letters and fonts that are not "standard". If no response from this one then it goes on to be processed by a second API call to the
RecognizePrintedTextInStreamAsync call. This one is better at recognizing common fonts and for certain box text in pictures. If neither call gets a response then it sends a to blurry/no text
string back to the ViewModel with the Files imagesource. The viewmodel then sends the imagesource and result back to the OCRPage to be displayed through data bound values.

There were some samples online using WPF and the Azure API calls that I did reference for context on how the calls should be formatted. Though this did turn out to take a lot of time even
still a lot of the documentation is for the older version of the API calls, and parsing didn't work properly so adaptions needed to be made to get the calls to function how I thought they should
as well as plenty of trial and error for the results.

### Sharpen ###
I had to set alias for the Image object because 2 of my libraries have definitions for Image. This will be needed for the Sharpen
functionality, that is currently not finished. It contains sample code right now for future processes.

### Translator ###
This is where all the Translation processes lies. It is set to inherit from both the TranslateViewModel and the IDataStoreT with the Language model. The interface tasks set
the values for the Language model that can be then passed to the Translate ViewModel or to other methods/tasks.

I am again using multiple API service calls, I created services using the Microsof Azure Cognitive services translator API through AZURE. The subscription key and endpoint are currently
hardcoded. Ideally with additional future changes these would be made variables that would referenced from a secured database. For this project purposes I did leave them
hard coded.

Originally the scope was just to put in the translation portion but as I was translating I found that I did not know how to pronounce some of the languages I translated into. I researched and
found that transliterate api calls that are apart of the same translator service. The trasnliteration calls were actually quite complex in that I was not able to deserialize the json using the
Newtonsoft Json deserializers. The translator language call was relatively easy to deserialize and put together for a dictionary and list to compare the language code and names to for later. That was nothing
compared to the the json that was sent in with the transliteration call to make that dictionary and list.

This service sets the values for the language lists for the UI picker dropdown and for the transliteration right off the bat. You need to process the languages right away for them to be able to be populated
in the pickers. The transliteration needs to be fired off right off the bat so that only 1 call needs to be made instead of each time someone tries to find off a tranlation. This also reduces load times. It
also sets the values from the IDataStoreT interface.

When the TranslateSelected task is called it first checks to make sure you have a proper selected language and compares to the dictionary, if not says it could not process it. If it is set to detect the
Dectect language function that allows for someone to send an "unknown" language in to be translated. This calls the detect service of the translator API and parses the result out if the the confidence
score is over 50%. If the confidence score is to low does set it. So long as everything is good it goes on to use the translate portion of the translator api and parses the result out to be sent back to the
TranslateViewModel to be set to the TranslatPage/View. But before that process is complete the transliterate function references the dictionary to confirm if the values can even be transliterated to each other.
Also this has to be ran either after or in the TranslateText task because it needs the translated text to transliterate as well as the original. If all is good they get processed by the transliterate service of the translator api.




## Sub Projects Elements ##
Android
- needed to make modifications to the AndroidManifest to get the permissions for doing camera, reading storage, writing storage. Then creating the provider section and the xml folder in the resources directory. All of these are very
specific for the android project to run. The MainActivity portion of this project This links back directly to the main project and converts the elements in there to an android build.

IOS
- beyond specifing the permissions that needed to be given for the most part this has just stayed pretty basic. I could not perform the testing I wanted to with this as I don't have a mac to deploy off of but I was able to test with an
emulated device and it did what was expected.

UWP
- modied permissions and verified that it accepts the load function of the applications xaml.

