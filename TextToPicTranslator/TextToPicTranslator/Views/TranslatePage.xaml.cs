using Xamarin.Forms;
using TextToPicTranslator.ViewModels;

namespace TextToPicTranslator.Views
{
    // most of the functions are not in fact used in this 'behind' code but rather in the ViewModel
    // TranslateViewModel holds the redirection for the Translator service tasks. That ultimately process 
    // the code. The use of data bindings are on the XAML side instead of declared back here due to ultimately 
    // being cleaner/ the ease of reading
    public partial class TranslatePage : ContentPage
    {
        // preps the _viewModel object
        TranslateViewModel _viewModel;
        public TranslatePage()
        {
            // initializes the view but also sets the data binding context to the translate view model
            InitializeComponent();
            BindingContext = _viewModel = new TranslateViewModel();
        }

        // When it appears sets the base/translate view models to fire off their "tasks" 
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}