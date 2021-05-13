using Xamarin.Forms;

namespace TextToPicTranslator.Models
{
    public class Text
    {
        // This model is used for the OCR IDataStoreO for the text pulled out of the images
        // Using a broken out model allows for ease of moving the values between viewmodels/views/services
        public string Language { get; set; }
        public string Orientation { get; set; }
        public Region[] Regions { get; set; }
        public double? TextAngle { get; set; }

    }
}