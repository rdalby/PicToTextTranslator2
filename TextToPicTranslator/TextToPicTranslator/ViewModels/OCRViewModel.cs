using System;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using System.Threading.Tasks;


namespace TextToPicTranslator.ViewModels
{

    public class OCRViewModel : BaseViewModel
    {
        public OCRViewModel()
        {
            // data bound title
            Title = "Pic to Text";

            // Setting the commands to their prospective methods
            CameraCommand = new Command(async () => await PicTaker());
            PickCommand = new Command(async () => await PicPicker());
            TranslateCommand = new Command(async () => await Translate());

        }

        // This method clears out the results everytime you go to the View
        public void OnAppearing()
        {
            IsBusy = true;
            ImageSource = null;
            ResultT = null;
        }

        // Setting the commands for the 3 command buttons on the page
        public Command CameraCommand { get; }
        public Command PickCommand { get; }
        public Command TranslateCommand { get; }


        // The following is the Get/Set code for the UI Data Bindings
        private string _ResultT;
        public string ResultT
        {
            get => _ResultT;
            set
            {
                SetProperty(ref _ResultT, value);
            }
        }

        private Boolean _InRunning;
        public Boolean InRunning
        {
            get => _InRunning;
            set
            {
                SetProperty(ref _InRunning, value);
            }
        }

        private Boolean _InVisible;
        public Boolean InVisible
        {
            get => _InVisible;
            set
            {
                SetProperty(ref _InVisible, value);
            }
        }

        private string _ResultLabel;
        public string ResultLabel
        {
            get => _ResultLabel;
            set
            {
                SetProperty(ref _ResultLabel, value);
            }
        }

        private ImageSource _ImageSource;
        public ImageSource ImageSource {
            get => _ImageSource;
            set
            {
                SetProperty(ref _ImageSource, value);
            }
        }

        // The next 2 methods are for the user to either pick an image or take one
        public async Task PicPicker()
        {
            // This method returns the image chosen from the user's gallery
            MediaFile file = await DataStoreO.PicToPick();
            // Display Indicator shows a loading and while processing it 
            InVisible = true;
            InRunning = true;
            // grabs the MediaFile and processes it
            _ = await DataStoreO.ProcessStream(file);

            // This pulls the ImageSource property value that is shared across multiple classes
            ImageSource = await DataStoreO.GetImageSourceAsync();
            // DisplayIndicator goes away now that processing is over
            InRunning = false;
            InVisible = false;

            // Pulls the ResultT property value that is shared across multiple classes
            ResultT = await DataStoreO.GetTextToTranslateAsync();
        }
        public async Task PicTaker()
        {
            // This method returns the image taken from the camera
            MediaFile file = await DataStoreO.PicToTake();
            // Display Indicator shows a loading and while processing it 
            InVisible = true;
            InRunning = true;
            // grabs the MediaFile and processes it
            _ = await DataStoreO.ProcessStream(file);

            // This pulls the ImageSource property value that is shared across multiple classes
            ImageSource = await DataStoreO.GetImageSourceAsync();
            // DisplayIndicator goes away now that processing is over
            ImageSource = await DataStoreO.GetImageSourceAsync();
            InRunning = false;
            InVisible = false;

            // Pulls the ResultT property value that is shared across multiple classes
            ResultT = await DataStoreO.GetTextToTranslateAsync();
        }

        // The start of transferring the text to the translate page/methods This is not currently 
        // being used but will remain here for future modifications/improvements
        public async Task Translate()
        {
           
           // string textToTranslate = TextToTranslate;

           // string fromLanguage = FromLanguageSelected.Lang;

           // string toLanguage = ToLanguageSelected.Lang;

           // ResultT = await DataStoreO.TranslateSelected(ResultT, ResultT, ResultT);
        }



    }
}
