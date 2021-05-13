using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using TextToPicTranslator.ViewModels;

namespace TextToPicTranslator.Views
{
    // most of the functions are not in fact used in this 'behind' code but rather in the ViewModel
    // OCRViewModel holds the redirection for the OCR service tasks. That ultimately process 
    // the code. The use of data bindings are on the XAML side instead of declared back here due to ultimately 
    // being cleaner/ the ease of reading
    public partial class OCRPage : ContentPage
    {
        // preps the _viewModel object
        OCRViewModel _viewModel;
        public OCRPage()
        {
            // initializes the view but also sets the data binding context to the ocr view model
            InitializeComponent();
            BindingContext = _viewModel = new OCRViewModel();
        }

        // When it appears sets the base/ocr view models to fire off their "tasks" 
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        // the clipboard button is not set to be a command but rather just a basic button click.
        // I would have preferred to have it be uniform like the other button commands but unfortunately
        // the clipboard function does not work reliably as a command but does as a 'clicked' function
        private async void ClipboardButton_Clicked(object sender, EventArgs e)
        {
            // sets your device clipboard to the result text and notifies the user that it was set
            await Clipboard.SetTextAsync(ResultLabel.Text);
            await DisplayAlert("text copied", "Your text is copied to your clipboard", "ok");
        }
    }
}