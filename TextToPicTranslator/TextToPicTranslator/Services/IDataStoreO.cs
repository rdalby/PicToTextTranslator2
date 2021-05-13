using Plugin.Media.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TextToPicTranslator.Services
{
    public interface IDataStoreO<T>
    {
        // OCR dependency service that allows the context to be moved between a model and a viewmodel
        // ultimately allowing for the proper data to be passed to the UI
        Task<string> TranslateSelected(string text, string to, string from);

        Task<MediaFile> PicToTake();

        Task<MediaFile> PicToPick();

        Task<ImageSource> GetImageSourceAsync();
        Task<string> GetTextToTranslateAsync();
        Task<MediaFile> ProcessStream(MediaFile file);
        Task<string> GetIsFromPicAsync();
    }
}
