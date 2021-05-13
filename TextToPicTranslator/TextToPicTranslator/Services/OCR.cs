using System;
using System.Threading.Tasks;
using TextToPicTranslator.Models;
using Xamarin.Forms;
using Plugin.Connectivity;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System.Text;
using TextToPicTranslator.ViewModels;

namespace TextToPicTranslator.Services
{

    public class OCR : OCRViewModel, IDataStoreO<Text>
    {
        // initiates the Shared Model to pass values back and forth
        readonly Shared sharedValues = new Shared();
        public OCR()
        {
        }

        // Theses next few task are implementations of the dependency service They get the values for the Data binding on the UI
        public async Task<string> TranslateSelected(string ttext, string tol, string froml)
        {
            return await Task.FromResult("textToTranslate");
        }

        public async Task<ImageSource> GetImageSourceAsync()
        {
            return await Task.FromResult(sharedValues.PImageSource);
        }
        public async Task<string> GetTextToTranslateAsync()
        {
            return await Task.FromResult(sharedValues.TToTranslate);
        }

        public async Task<string> GetIsFromPicAsync()
        {
            return await Task.FromResult(sharedValues.IsFromPic);
        }

        // the subscription key for the OCR service
        private const string subscriptionKey = "";

        // the endpoint for the OCR service 
        private const string uriBase = "https://pictotextfinalproj.cognitiveservices.azure.com/";

        // This task gets a pic from the user's gallery
        public async Task<MediaFile> PicToPick()
        {
            // Checks for permissions to get the picture uploaded if not allowed displays alert
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                MessagingCenter.Send<OCR>(this, "Picking a photo is not supported");
               // await DisplayAlert("No upload", "Picking a photo is not supported.", "OK");
               // return "Picking a photo is not supported";
            }

            // if allowed grabs file and then checks to make sure file is not null
            var file = await CrossMedia.Current.PickPhotoAsync();
            if (file == null)
            {
                MessagingCenter.Send<OCR>(this, "File is NULL");
                //return "File is NULL";
            }

            // returns the file 
            return file;
        }



        // This task is activated when the camera button is selected and gets an image from the camera
        public async Task<MediaFile> PicToTake()
        {
            // waits to be called before executing
            await CrossMedia.Current.Initialize();

            // checks to see if camera isn't already busy, is supported, has permission
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.
              IsTakePhotoSupported)
            {
                //                await DisplayAlert("No Camera", "No camera available.", "OK");
                MessagingCenter.Send<OCR>(this, "No camera available");
              //  return "No camera available";
            }
            //get timestamp for file name to avoid dup filenames
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string filename = "PtoT" + timestamp + ".jpg";

            // opens camera and allows user to take pic and saves to album result is file
            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                SaveToAlbum = true,
                Name = filename
            });

            // if the file ends up null can't process it
            if (file == null)
            {
                MessagingCenter.Send<OCR>(this, "File is NULL");
               // return "file is NULL";
            }

            // returns the file for processing
            return file;
        }

        // This task sets it up so that the image file can be processed
        public async Task<MediaFile> ProcessStream(MediaFile file)
        {
            // waits to be called before executing
            await CrossMedia.Current.Initialize();

            //sends the file stream to the method to pull out the text
            var res = await TextPictureAsync(file);

            // the resulting string is what goes to the sharedValues Model for the text to translate/ result
            sharedValues.TToTranslate = res;

            // Setting up the ImageSource databinding with the file stream and fills in the sharedValue version of it
            ImageSource = StreamImageSource.FromStream(() => file.GetStream());
            sharedValues.PImageSource = ImageSource;

            // returns the filesource of the stream
            return sharedValues.FileSource;
        }
        
        // This function pulls the text from the image sent
        private async Task<string> TextPictureAsync(MediaFile inputFile)
        {
            // need internet so need to perform an internet check before you can send the 
            // the file stream to the OCR API
            if (!CrossConnectivity.Current.IsConnected)
            {
                //                await DisplayAlert("Network error", "Please check your network connection and retry.", "OK");
                return null;
            }

            // if you have a good network connection then it tries to pull the text from the image
            try
            {
                // This is the base of the API call for the OCR API where it combined the key and endpoint
                // into a variable called client
                using (var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey))
                { Endpoint = uriBase })
                {

                    // This is the count of characters in the Operator Location that will be needed to process the OCR results
                    const int countOL = 36;

                    // This sends the API call to get the result of the Text pulled from the image then grabs the OperationLocation
                    // Using the .GetStream function to avoid any stream issues later/ be able to use the same input if a second
                    // call if nothing was found during this first call
                    var result1 = client.ReadInStreamWithHttpMessagesAsync(inputFile.GetStream()).Result;
                    string operationLocation = result1.Headers.OperationLocation;

                    // This is really important it grabs the operationId from the original call to grab the actual results
                    string operationId = operationLocation.Substring(operationLocation.Length - countOL);

                    // This will grab the results of the first API call and actually return what you data you need
                    ReadOperationResult results;
                    do
                    {
                        results = await client.GetReadResultAsync(Guid.Parse(operationId));
                    }
                    while ((results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted));

                    // Converts the results of the API call into a string to be displayed later
                    var textFileResults = results.AnalyzeResult.ReadResults;
                    var resultString1 = new StringBuilder();
                    foreach (ReadResult page in textFileResults)
                    {
                        foreach (Line line in page.Lines)
                        {
                            resultString1.Append(line.Text);
                            resultString1.Append(" ");
                            resultString1.AppendLine();
                        }
                        // don't take the lines out or you will end up with text mushed together when they should be on a different line
                        resultString1.AppendLine();
                    }

                    // Once the resultString is complete it checks if it's empty or not
                    if (resultString1.ToString() == "")
                    {
                        // if empty tries the Recongized printed test in stream function as both are slightly different
                        // then  if still can't then sends a no result/ blurry pic to the result

                        // gets the result of the client call using the RecognizePrintedTextInStreamAsync 
                        // function where the parameters are based on detect/fix orientation and the input stream
                        OcrResult result = await client.RecognizePrintedTextInStreamAsync(true, inputFile.GetStream());

                        // grabs and parses out the result into a string
                        var resultString2 = new StringBuilder();
                        if (result != null)
                        {
                            foreach (var region in result.Regions)
                            {
                                foreach (var line in region.Lines)
                                {
                                    foreach (var word in line.Words)
                                    {
                                        resultString2.Append(word.Text);
                                        resultString2.Append(" ");
                                    }
                                    resultString2.AppendLine();
                                }
                                resultString2.AppendLine();
                            }
                        }
                        
                        // checks the new result to see if it got a value or not. If not it sends that it was to blurry or not text
                        if (resultString2.ToString() == "")
                        {
                            string noresult = "no text result or picture to blurry";
                            return noresult;
                        }
                        else
                        {
                            // if there was a result then returns this result as the translated text
                            return resultString2.ToString();
                        }
                    }
                    else
                    {
                        // if it isn't empty then sends the non-empty strings back
                        return resultString1.ToString();
                    }
                }
            }
            // catches the exceptions for the try and will display to the user as the result
            catch (Exception e)
            {
                return e.Message;
            }
        }

    }
}

