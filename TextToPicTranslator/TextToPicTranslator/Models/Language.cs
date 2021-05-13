using System;

namespace TextToPicTranslator.Models
{
    public class Language
    {
        // This model is used for the Translate IDataStoreT for the language lists to be pulled down
        // Using a broken out model allows for ease of moving the values between viewmodels/views/services
        public string Id { get; set; }
        public string Lang { get; set; }
        public string Code { get; set; }

    }

    public class LiterateLanguage
    {
        // This model is used for the Translate IDataStoreT for the language lists to be pulled down
        // Using a broken out model allows for ease of moving the values between viewmodels/views/services
        public string Id { get; set; }
        public string FName { get; set; }
        public string FScript { get; set; }
        public string ToScriptCd { get; set; }
        public string ToScriptName { get; set; }


    }
}