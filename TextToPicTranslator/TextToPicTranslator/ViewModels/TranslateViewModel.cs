using System;
using Xamarin.Forms;
using TextToPicTranslator.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;


namespace TextToPicTranslator.ViewModels
{
    public class TranslateViewModel : BaseViewModel
    {
        public TranslateViewModel()
        {
            // designates the title for the Translate menu through Data binding
            Title = "Translate";

            // Creates a new collection of languages based on the Language model that will be used to fill the langauge
            // pickers from the UI
            Languages = new ObservableCollection<Language>();

            // this command is used on the refresh of the page to populated and load the pickers on the UI
            // without this command the pickers will not populate and you will have empty picker lists
            LoadLanguagesCommand = new Command(async () => await ExecuteLoadLanguagesCommand());

            // this sets the Translate Command to fire off the translate task
            TranslateCommand = new Command(async() => await Translate());

            // this basically makes sure all the properties that may be changed for the UI are updated to their current values
            // after the translate button is selected. This is what makes sure the translated values/ dectected language make it to the UI
            this.PropertyChanged +=
                       (_, __) => TranslateCommand.ChangeCanExecute();
        }

        // This method clears out the results everytime you go to the View
        public void OnAppearing()
        {
            IsBusy = true;
            TextToTranslate = null;
            TranslatedText = null;
            TransliteratedText = null;
            TransliteratedTextF = null;
            ToLanguageSelected = null;
            FromLanguageSelected = null;
            ToScript = null;
            FromScript = null;
            DetectedLanguage = null;
        }

        // used as part of setting the commands/ collection for the UI used in the main method
        public Command LoadLanguagesCommand { get; }

        public Command TranslateCommand { get; }

        public ObservableCollection<Language> Languages { get; }

        // This is the task that the LoadLanguagesCommand fires off
        async Task ExecuteLoadLanguagesCommand()
        {
            IsBusy = true;

            // tries to get the languages and load them in. If there is an issue writes to the debugger
            try
            {
                // clears the values in Languages first
                Languages.Clear();
                // calls the task to get the langauges/populate them based on the translator model
                var languages = await DataStoreT.GetLanguagesAsync(true);

                // For all the languages in the returned var langauges it addes them 1 by one to the Language List
                foreach (var language in languages)
                {
                    Languages.Add(language);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ***** PERFORM TRANSLATION ON BUTTON CLICK
        public async Task Translate()
        {
            //clears out values on screen
            TranslatedText = null;
            TransliteratedText = null;
            TransliteratedTextF = null;

            // local strings to send to methods - don't really need to do this work extra step but it helps for my clarity
            string textToTranslate = TextToTranslate;
            string fromLanguage = "";
            string toLanguage = "";

            // if from language is null substitutes with Detect however if to language is null then does not and will end up exiting later
            if (FromLanguageSelected == null)
            {
                fromLanguage = "Detect";
            }
            else
            {
                fromLanguage = FromLanguageSelected.Lang;
            }

            if (ToLanguageSelected != null)
            {
                if (FromLanguageSelected.Lang != "Detect")
                {
                    toLanguage = ToLanguageSelected.Lang;
                }
            }

            
            // This sends the the text to translate, the language select for the to and the from to the translate task
            // it returns the new translated text to be displayed on the UI
            TranslatedText = await DataStoreT.TranslateSelected(textToTranslate, toLanguage, fromLanguage);

            // gets the Detected Language from the task off of the translater service model for the UI
            DetectedLanguage = await DataStoreT.GetDetectedLanguageAsync();
            
            //The next four get values for the transliterate data binding from the service model
            TransliteratedText = await DataStoreT.GetTransliteratedTextAsync();
            TransliteratedTextF = await DataStoreT.GetTransliteratedTextFAsync();
            ToScript = await DataStoreT.GetToScriptAsync();
            FromScript = await DataStoreT.GetFromScriptAsync();


        }


        // The following code sets all the Translator data bindings

        // This sets the FromLanguageSelected data binding
        private Language _FromLanguageSelected;
        public Language FromLanguageSelected
        {
            get => _FromLanguageSelected;
            set => SetProperty(ref _FromLanguageSelected, value);
        }

        // This sets the ToLanguageSelected data binding
        private Language _ToLanguageSelected;
        public Language ToLanguageSelected
        {
            get => _ToLanguageSelected;
            set
            {
                SetProperty(ref _ToLanguageSelected, value);
            }
        }


        // This sets and grabs the Shared Values that are shared by the multiple viewmodels/views
        public Shared _SharedValues;
        public Shared SharedValues {
            get => _SharedValues;
            set => SetProperty(ref _SharedValues, value);
        } 


        // This sets the DetectedLanguage data binding
        private string _DetectedLanguage;
        public string DetectedLanguage
        {
            get => _DetectedLanguage;
            set => SetProperty(ref _DetectedLanguage, value);

        }


        // This sets the languagedropdown data binding stuff
        public ObservableCollection<Language> _LangDropDown;
        public ObservableCollection<Language> LanguageDropDown
        {
            get => _LangDropDown;
            set => SetProperty(ref _LangDropDown, value);
        }

        // This sets the TextToTranslate data binding
        private string _TextToTranslate;
        public string TextToTranslate
        {
            get => _TextToTranslate;
            set => SetProperty(ref _TextToTranslate, value);
        }

        // This sets the TranslatedText data binding
        private string _TranslatedText;
        public string TranslatedText
        {
            get => _TranslatedText;
            set => SetProperty(ref _TranslatedText, value);
        }

        // This sets the Transliterated Text data binding
        private string _TransliteratedText;
        public string TransliteratedText
        {
            get => _TransliteratedText;
            set => SetProperty(ref _TransliteratedText, value);
        }

        // This sets the Transliterated Text data binding
        private string _TransliteratedTextF;
        public string TransliteratedTextF
        {
            get => _TransliteratedTextF;
            set => SetProperty(ref _TransliteratedTextF, value);
        }


        // This sets the TranslatedText data binding
        private string _FromScript;
        public string FromScript
        {
            get => _FromScript;
            set => SetProperty(ref _FromScript, value);
        }

        // This sets the TranslatedText data binding
        private string _ToScript;
        public string ToScript
        {
            get => _ToScript;
            set => SetProperty(ref _ToScript, value);
        }
    }
}
