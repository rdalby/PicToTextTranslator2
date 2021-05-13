using System.Collections.Generic;
using System.Threading.Tasks;

namespace TextToPicTranslator.Services
{
    public interface IDataStoreT<T>
    {
        // Translator dependency service that allows the context to be moved between a model and a viewmodel
        // ultimately allowing for the proper data to be passed to the UI
        Task<string> GetDetectedLanguageAsync();
        Task<string> GetToScriptAsync();
        Task<string> GetFromScriptAsync();
        Task<string> GetTextToTranslateAsync();
        Task<string> GetTransliteratedTextAsync();
        Task<string> GetTransliteratedTextFAsync();
        Task<string> GetIsFromPicAsync();
        Task<IEnumerable<T>> GetLanguagesAsync(bool forceRefresh = false);
        Task<string> TranslateSelected(string text, string to, string from);
    }
}
