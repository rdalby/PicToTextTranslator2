using System;
using TextToPicTranslator.Services;
using TextToPicTranslator.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TextToPicTranslator
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            // sets the Dependency Service on the OCR and Translate Services
            // which is needed to send the data back to the view models/views
            DependencyService.Register<Translator>();
            DependencyService.Register<OCR>();

            // sets the mainpage as the appshell which is what is used to make cross-platform
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
