using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextToPicTranslator.Models;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json; // Install Newtonsoft.Json with NuGet
using System.Net;
using System.IO;
using TextToPicTranslator.ViewModels;


namespace TextToPicTranslator.Services
{
    public class Translator : TranslateViewModel, IDataStoreT<Language>
    {
        // sets some readonly objects for use later
        private List<Language> languages;
        private List<LiterateLanguage> lilanguages;
        readonly Shared sharedValues = new Shared();
        public Translator()
        {
            // starts to populate the language list with "Detect" this is not in the list of 
            // languages that will be returned by the API so need to add this option in
            // creates this option in the beginning so if something fails later they still have the detect options
            languages = new List<Language>()
            {
                new Language { Id = Guid.NewGuid().ToString(), Lang = "Detect"  },
            };

            // on run this goes through the task of getting the Languages for translation
            // then to populate the language list to be passed to the UI from the GetLanguagesAsync
            // task. That task will return empty if these are not ran on "load"
            GetLanguagesForTranslate();
            GetTransliterateLanguages();
            PopulateLanguageMenus();

        }

        // these are setters tasks that push the values to the viewmodel and down to the UI from there
        public async Task<string> GetDetectedLanguageAsync()
        {
            return await Task.FromResult(sharedValues.DetectedLang);
        }
        public async Task<string> GetToScriptAsync()
        {
            return await Task.FromResult(sharedValues.ToScript);
        }
        public async Task<string> GetFromScriptAsync()
        {
            return await Task.FromResult(sharedValues.FromScript);
        }
        public async Task<IEnumerable<Language>> GetLanguagesAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(languages);
        }

        public async Task<string> GetTextToTranslateAsync()
        {
            return await Task.FromResult(sharedValues.TToTranslate);
        }

        public async Task<string> GetTransliteratedTextAsync()
        {
            return await Task.FromResult(sharedValues.TransliteratedText);
        }

        public async Task<string> GetTransliteratedTextFAsync()
        {
            return await Task.FromResult(sharedValues.TransliteratedTextF);
        }


        public async Task<string> GetIsFromPicAsync()
        {
            return await Task.FromResult(sharedValues.IsFromPic);
        }

        // This is the key needed for the Translator piece of the API. Ideally this will be moved to more of a database option
        // This shouldn't stay in the code itself to avoid getting the Key value taken from others
        public const string COGNITIVE_SERVICES_KEY = "";
        // Endpoints for Translator and Bing Spell Check - not really using the Spell Check option right now but including for future
        // use and to be able to add on for additional features without having to relook up endpoint. 
        // Again ideally these would be saved in a database and the endpoint chosen by "type" but for now hardcoded
        public static readonly string TEXT_TRANSLATION_API_ENDPOINT = "https://api.cognitive.microsofttranslator.com/{0}?api-version=3.0";
        const string BING_SPELL_CHECK_API_ENDPOINT = "https://eastus.api.cognitive.microsoft.com/bing/v7.0/spellcheck/";

        // An array of language codes
        private string[] languageCodes;

        // Dictionaries to map language codes from friendly name (sorted case-insensitively on language name)
        private SortedDictionary<string, string> languageCodesAndTitles =
            new SortedDictionary<string, string>(Comparer<string>.Create((a, b) => string.Compare(a, b, true)));
        private SortedDictionary<string, string> lilanguageCodesAndTitles =
            new SortedDictionary<string, string>(Comparer<string>.Create((a, b) => string.Compare(a, b, true)));


        // This grabs the languages that the API supports for traslations
        public void GetLanguagesForTranslate()
        {
            // uri is the base url that needs to be used in the webrequest... languages does not include scope so it will bring in 
            // supported languages for translate AND transliteration filling a seperate list for the transliteration stuff
            string uri = String.Format(TEXT_TRANSLATION_API_ENDPOINT, "languages") + "&scope=translation";
            WebRequest WebRequest = WebRequest.Create(uri);
            WebRequest.Headers.Add("Accept-Language", "en");
            WebResponse response = null;

            // Reads then pulls apart the JSON response from the web request
            response = WebRequest.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream(), UnicodeEncoding.UTF8))
            {
                Console.Write(reader);
                try
                {
                    // deserializes the reader to a specific format
                    var result = JsonConvert.DeserializeObject <Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(reader.ReadToEnd());
 
                    // grabs the languauges from the 'translation' section of the array
                    var lang = result["translation"];


                    // Pulls all the codes and language names and puts them in string array so that they can be used for verifying/checking later
                    languageCodes = lang.Keys.ToArray();
                    foreach (var kv in lang)
                    {
                        // two dictionaries for reverse saves time later
                        languageCodesAndTitles.Add(kv.Value["name"], kv.Key);
                        lilanguageCodesAndTitles.Add(kv.Key, kv.Value["name"]);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        // This method is triggered by the TranslateSelected task to transliterate text
        public void GetTransliterateLanguages()
        {
            // intializes the lilanguages list and creates teh llangitem item for later
            lilanguages = new List<LiterateLanguage>();
            LiterateLanguage llangItem;

            //// uri is the base url that needs to be used in the webrequest... languages does not include scope so it will bring in 
            //// supported languages for translate AND transliteration filling a seperate list for the transliteration stuff
            string uri = String.Format(TEXT_TRANSLATION_API_ENDPOINT, "languages") + "&scope=transliteration";
            WebRequest WebRequest = WebRequest.Create(uri);
            WebRequest.Headers.Add("Accept-Language", "en");
            WebResponse response = null;

            // Reads then pulls apart the JSON response from the web request and puts it into a list to use for reference later
            response = WebRequest.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream(), UnicodeEncoding.UTF8))
            {
                try
                {
                    //reads in the result from the API call then deserializes it to be processed later
                    var result = reader.ReadToEndAsync().Result;
                    dynamic jsonresponse = JsonConvert.DeserializeObject(result);

                    // variables to be filled by manually pulling apart the response
                    var tname = "";
                    var scriptcd = "";
                    var toscriptcd = "";
                    var toscriptname = "";

                    // this part needs to be done different than the generic translate languages one because the jsonresponse back
                    // is very complex so conditions for what values needed to pull out/ to move to a new iteration have to put into
                    // place. This section took a very long to figure out in general and may have a better way to do it but for now 
                    // it gets the desired results
                    var transliterateInfo = jsonresponse["transliteration"];
                    foreach (var o in transliterateInfo)
                    {
                        Console.WriteLine("Transliterated to {0} script: {1}", o.Value["name"], o.Value["scripts"]);
                        tname = o.Value["name"];
                        foreach (var e in o.Value["scripts"])
                        {
                            foreach (var t in e)
                            {
                                if (t.Name == "code")
                                {
                                    scriptcd = t.Value;
                                }
                                if (t.Name == "toScripts")
                                {
                                    foreach (var f in t)
                                    {
                                        foreach (var g in f)
                                        {
                                            foreach (var b in g)
                                            {
                                                if (b.Name == "code")
                                                {
                                                    toscriptcd = b.Value;
                                                }
                                                else if (b.Name == "name")
                                                {
                                                    toscriptname = b.Value;

                                                    // since the last part of the json response that is needed on this level is the to script name this is the perfect
                                                    // time to add all the elements into the lilanguages list 
                                                    llangItem = new LiterateLanguage { Id = Guid.NewGuid().ToString(), FName = tname, FScript = scriptcd, ToScriptCd = toscriptcd, ToScriptName = toscriptname };
                                                    lilanguages.Add(llangItem);
                                                }


                                            }

                                        }

                                    }
                                }
                            }
                        }
                    }
                    // if anything happens it catches the error and writes to console
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }

        // The code to populate the language collection in the TranslatorViewModel
        public void PopulateLanguageMenus()
        {
            // adds new languages to the languages list for data binding on the form
            foreach (string menuItem in languageCodesAndTitles.Keys)
            {
                Language langItem = new Language { Id = Guid.NewGuid().ToString(), Lang = menuItem };
                languages.Add(langItem);
            }
        }


        public bool ConfirmTranliteratable(string fromName, string toName)
        {
            // perform the lookup to see if the language can be scripted from one to the other
            // if so pulls the script name out of it and sets it to the shared values of them to 
            // pass through methods and bindings easier
            // List<> 
            
            // have to convert English to Latin as there is no "English" script
            if (fromName == "English")
            {
                fromName = "Latin";
            }
            else if (toName == "English") 
            {
                toName = "Latin";
            }

            //temp variables for english round about
            string temp = "";
            string temptemp = "";
            string temptosc = "";
            string tempfsc = "";
            bool confirmed = false;

            // you need to do a round about way of doing the english searches as most go to english
            if (fromName == "Latin") 
            {
                temp = fromName;
                fromName = toName;
                toName = temp;
            }

            foreach(var li in lilanguages)
            {
                if(li.FName == fromName)
                {
                    //check if the to script name matches
                    if(li.ToScriptName == toName)
                    {

                        temptosc = li.ToScriptCd.ToString();
                        tempfsc = li.FScript.ToString();

                        confirmed = true;
                    }
                }
            }

            // swapping the results back
            if (temp == "Latin")
            {
                temptemp = temptosc;
                temptosc = tempfsc;
                tempfsc = temptemp;
            }

            // set the sharedValues variables as a way of returning more than 1 from this method
            sharedValues.ToScript = temptosc;
            sharedValues.FromScript = tempfsc;

            // returns false if not true to skip transliteration entirely
            return confirmed;
        }


        // This method is triggered if the detect option is selected on translate
        public string DetectLanguage(string text)
        {
            // all in a try so that if the language is not supported it won't error out
            try
            {
                // creates the string for the translation detect portion of the the API
                string detectUri = string.Format(TEXT_TRANSLATION_API_ENDPOINT, "detect");

                // Creates the request to Detect languages with Translator
                HttpWebRequest detectLanguageWebRequest = (HttpWebRequest)WebRequest.Create(detectUri);
                detectLanguageWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", COGNITIVE_SERVICES_KEY);
                detectLanguageWebRequest.Headers.Add("Ocp-Apim-Subscription-Region", "eastus");
                detectLanguageWebRequest.ContentType = "application/json; charset=utf-8";
                detectLanguageWebRequest.Method = "POST";

                // serializes the string of text to be translated that was pushed to the method to a string
                string jsonText = JsonConvert.SerializeObject(text);

                // creates the body of the call with the serialized text
                string body = "[{ \"Text\": " + jsonText + " }]";
                byte[] data = Encoding.UTF8.GetBytes(body);

                detectLanguageWebRequest.ContentLength = data.Length;

                // creates and gets the requestStream
                using (var requestStream = detectLanguageWebRequest.GetRequestStream())
                    requestStream.Write(data, 0, data.Length);

                // grabs the response of the request
                HttpWebResponse response = (HttpWebResponse)detectLanguageWebRequest.GetResponse();

                // pulls out the stream from the response and from that stream reads it and deserializes it 
                // the result will end up being the detected langauge of the text if it is one of the 90
                var responseStream = response.GetResponseStream();
                var jsonString = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")).ReadToEnd();
                dynamic jsonResponse = JsonConvert.DeserializeObject(jsonString);

                // This grabs out the detected language but the confidence score from the API must be over 50% for it to 
                // pull the language out otherwise will not return the input language
                var languageInfo = jsonResponse[0];
                if (languageInfo["score"] > (decimal)0.5)
                {
                    // sets the DetectedLanguage
                    DetectedLanguage = languageInfo["language"];

                    // sets dectectedlang to the shared model so that it can be used for both translation and for OCR
                    sharedValues.DetectedLang = DetectedLanguage;

                    // sends the dectected language back to the previous method for processing there
                    return DetectedLanguage;
                }
                else
                    return "Unable to confidently detect input language.";
            // if anything happens it catches the error and writes to console
            }
            catch 
            {
                return "Unable to confidently detect input language.";
            }
}

        // This method is triggered by the TranslateSelected task to transliterate text
        public async Task<string> Transliterate(string ttext, string trans, string flcd, string tlcd, string tln)
        {
            //locally set variables for ease of use
            string textToTranslate = ttext;
            string fromLanguageCode = flcd;
            string toLanguageCode = tlcd;
            string toName = tln;
            string fromName = lilanguageCodesAndTitles[fromLanguageCode];
            string translatedt = trans;
            

            // this confirms if a language is in fact transliterable or not with the combo selected
            // if so it sets the data bound variables FromScript and ToScript
            if (ConfirmTranliteratable(fromName, toName))
            {

                ToScript = sharedValues.ToScript;
                FromScript = sharedValues.FromScript;

                // Creates the strings needed for the API request to perform the tranlation
                string endpoint = string.Format(TEXT_TRANSLATION_API_ENDPOINT, "transliterate");

                //calls lookup to get the scripts for fromScript and ToScript
                try
                {   
                    // this looks backwards but it is going to end up calling it so that the original text is transliteraite to the to script
                    string languageP = string.Format("&language={0}&fromScript={1}&toScript={2}", toLanguageCode, FromScript, ToScript);
                    string uri = string.Format(endpoint + languageP);

                    // creates the request body with serialized data for the API request
                    System.Object[] body = new System.Object[] { new { Text = textToTranslate } };
                    var requestBody = JsonConvert.SerializeObject(body);

                    // creates the elements needed for the API call for the orginal transliterated to call
                    using (var client = new HttpClient())
                    using (var request = new HttpRequestMessage())
                    {
                        // sets up the request with the methods, url, content, and headers needed
                        request.Method = HttpMethod.Post;
                        request.RequestUri = new Uri(uri);
                        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                        request.Headers.Add("Ocp-Apim-Subscription-Key", COGNITIVE_SERVICES_KEY);
                        request.Headers.Add("Ocp-Apim-Subscription-Region", "eastus");
                        request.Headers.Add("X-ClientTraceId", Guid.NewGuid().ToString());

                        // using the client sends the request out - should update this to be similar to the OCR API with the client
                        // authorization happening prior to.
                        var response = await client.SendAsync(request);
                        var responseBody = await response.Content.ReadAsStringAsync();

                        //var responseStream = response.GetResponseStream();
                        //var jsonString = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")).ReadToEnd();
                        dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);


                        // deserialized the results and pulls the translation out of the result array
                       // var result = JsonConvert.DeserializeObject<List<Dictionary<string, List<Dictionary<string, string>>>>>(responseBody);

                        var transliteration = jsonResponse[0]["text"].ToString();
                        sharedValues.TransliteratedText = transliteration;
                    }
                }
                catch
                {
                    // just returning empty for if it can't do the transliteration
                    sharedValues.TransliteratedText = "";
                }
                try { 
                //Then these strings are set up to that the translation can be transliteraited to the oringal script
                string languageF = string.Format("&language={0}&fromScript={1}&toScript={2}", toLanguageCode, ToScript, FromScript);
                    string uriF = string.Format(endpoint + languageF);

                    // creates the request body with serialized data for the API request need to change body or it won't come out right
                    System.Object[] bodyF = new System.Object[] { new { Text = translatedt } };
                    var requestBodyF = JsonConvert.SerializeObject(bodyF);

                    // creates the elements needed for the API call for the new transliterated from call
                    using (var clientF = new HttpClient())
                    using (var requestF = new HttpRequestMessage())
                    {
                        // sets up the request with the methods, url, content, and headers needed
                        requestF.Method = HttpMethod.Post;
                        requestF.RequestUri = new Uri(uriF);
                        requestF.Content = new StringContent(requestBodyF, Encoding.UTF8, "application/json");
                        requestF.Headers.Add("Ocp-Apim-Subscription-Key", COGNITIVE_SERVICES_KEY);
                        requestF.Headers.Add("Ocp-Apim-Subscription-Region", "eastus");
                        requestF.Headers.Add("X-ClientTraceId", Guid.NewGuid().ToString());

                        // using the client sends the request out - should update this to be similar to the OCR API with the client
                        // authorization happening prior to.
                        var responseF = await clientF.SendAsync(requestF);
                        var responseBodyF = await responseF.Content.ReadAsStringAsync();

                        //var responseStream = response.GetResponseStream();
                        //var jsonString = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")).ReadToEnd();
                        dynamic jsonResponseF = JsonConvert.DeserializeObject(responseBodyF);


                        // deserialized the results and pulls the translation out of the result array
                        // var result = JsonConvert.DeserializeObject<List<Dictionary<string, List<Dictionary<string, string>>>>>(responseBody);

                        var transliterationF = jsonResponseF[0]["text"].ToString();
                        sharedValues.TransliteratedTextF = transliterationF;

                        // returning the transliterationF back so it can be used to update the UI 
                        return await Task.FromResult(transliterationF);

                    }
                
                }
                catch
                {
                    // just returning empty for if it can't do the transliteration
                    sharedValues.TransliteratedTextF = "";
                    return "";
                }
            }

            // returns nothing if not transliteratable
            return "";
        }


        // This is the task that is called from the translate command
        public async Task<string> TranslateSelected(string ttext, string tol, string froml)
        {
            //locally set variables for ease of use
            string textToTranslate = ttext;
            string fromLanguage = froml;
            string fromLanguageCode;
            string toLanguage = tol;

            if (fromLanguage == null || toLanguage == null)
            {
                sharedValues.TransliteratedTextF = " ";
                sharedValues.TransliteratedText = " ";
                TransliteratedTextF = sharedValues.TransliteratedTextF;
                TransliteratedText = sharedValues.TransliteratedText;
                return "did not select a to or from language cannot process";
            }

            try
            {
                // auto-detect source language if requested
                if (fromLanguage == "Detect")
                {
                    // sets the from language code if it can detect the language
                    fromLanguageCode = DetectLanguage(textToTranslate);

                    // if the no languagecodes then that means it could not detect the language and returns an error message
                    if (!languageCodes.Contains(fromLanguageCode))
                    {
                        //MessageBox.Show("The source language could not be detected automatically " +
                        //   "or is not supported for translation.", "Language detection failed",
                        //  MessageBoxButton.OK, MessageBoxImage.Error);
                        return await Task.FromResult("Source Language could not be detected automatically or is not supported for translation");
                    }
                }
                // if the language is selected then it looks to grab the code from the array of language codes and titles
                else
                    fromLanguageCode = languageCodesAndTitles[fromLanguage];

                // populates the toLanguageCode based on the language selected for the 'to' section on the UI
                string toLanguageCode = languageCodesAndTitles[toLanguage];

                // handles null, no text or same t0/from selected language
                if (textToTranslate == "" || fromLanguageCode == toLanguageCode)
                {
                    // literally just populates the translation with the original text and clears out
                    sharedValues.TransliteratedTextF = " ";
                    sharedValues.TransliteratedText = " ";
                    TransliteratedTextF = sharedValues.TransliteratedTextF;
                    TransliteratedText = sharedValues.TransliteratedText;
                    return await Task.FromResult(textToTranslate);
                }

                // Creates the strings needed for the API request to perform the tranlation
                string endpoint = string.Format(TEXT_TRANSLATION_API_ENDPOINT, "translate");
                string languageP = string.Format("&from={0}&to={1}", fromLanguageCode, toLanguageCode);
                string uri = string.Format(endpoint + languageP);

                // creates the request body with serialized data for the API request
                System.Object[] body = new System.Object[] { new { Text = textToTranslate } };
                var requestBody = JsonConvert.SerializeObject(body);

                // creates the elements needed for the API call
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    // sets up the request with the methods, url, content, and headers needed
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(uri);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", COGNITIVE_SERVICES_KEY);
                    request.Headers.Add("Ocp-Apim-Subscription-Region", "eastus");
                    request.Headers.Add("X-ClientTraceId", Guid.NewGuid().ToString());

                    // using the client sends the request out - should update this to be similar to the OCR API with the client
                    // authorization happening prior to.
                    var response = await client.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    // deserialized the results and pulls the translation out of the result array
                    var result = JsonConvert.DeserializeObject<List<Dictionary<string, List<Dictionary<string, string>>>>>(responseBody);
                    var translation = result[0]["translations"][0]["text"];

                    // calls the transliterate function to get the script results - does not include from name as it could be pulled in with detect
                    sharedValues.TransliteratedTextF = await Transliterate(ttext, translation, fromLanguageCode, toLanguageCode, toLanguage);
                    TransliteratedTextF = sharedValues.TransliteratedTextF;
                    TransliteratedText = sharedValues.TransliteratedText;

                    // returning the translation back so it can be used to update the UI 
                    return await Task.FromResult(translation);
                }
            }
            catch
            {
                // catches the errors and returns this message
                return "could not translate language";
            }
        }


    }
}
