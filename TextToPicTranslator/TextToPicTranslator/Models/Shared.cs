using Xamarin.Forms;
using TextToPicTranslator.ViewModels;
using Plugin.Media.Abstractions;

namespace TextToPicTranslator.Models
{
    public class Shared : BaseViewModel
    {
        // This model is used to break out some of the shared elements to be able to be sync'd used
        // between viewmodels/views/services

        // shares the detected language so that it can be moved across viewmodels
        private string _DetectedLanguage;
        public string DetectedLang
        {
            get => _DetectedLanguage;
            set => SetProperty(ref _DetectedLanguage, value);

        }

        // shares the Transliterated Text so that it can be moved across viewmodels if needed
        private string _TransliteratedText;
        public string TransliteratedText
        {
            get => _TransliteratedText;
            set => SetProperty(ref _TransliteratedText, value);

        }

        // shares the Transliterated from Text so that it can be moved across viewmodels if needed
        private string _TransliteratedTextF;
        public string TransliteratedTextF
        {
            get => _TransliteratedTextF;
            set => SetProperty(ref _TransliteratedTextF, value);

        }


        // shares the From Script Text so that it can be moved across viewmodels if needed
        private string _FromScript;
        public string FromScript
        {
            get => _FromScript;
            set => SetProperty(ref _FromScript, value);

        }


        // shares the To Script Text so that it can be moved across viewmodels if needed
        private string _ToScript;
        public string ToScript
        {
            get => _ToScript;
            set => SetProperty(ref _ToScript, value);

        }


        // shares the text to translate so that it can be moved from the OCR to Translate page
        private string _TToTranslate;
        public string TToTranslate
        {
            get => _TToTranslate;
            set => SetProperty(ref _TToTranslate, value);

        }

        // shares the image source so that it can be returned to the page view properly
        private ImageSource _PImageSource;
        public ImageSource PImageSource
        {
            get => _PImageSource;
            set => SetProperty(ref _PImageSource, value);

        }

        // shares the image source so that it can be returned to the page view properly
        private MediaFile _FileSource;
        public MediaFile FileSource
        {
            get => _FileSource;
            set => SetProperty(ref _FileSource, value);

        }


        // shares the is from a pic value for if it should clear out the translate text or not
        private string _IsFromPic;
        public string IsFromPic
        {
            get => _IsFromPic;
            set => SetProperty(ref _IsFromPic, value);

        }

    }
}