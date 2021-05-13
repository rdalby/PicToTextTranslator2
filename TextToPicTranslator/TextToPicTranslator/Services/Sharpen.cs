using Plugin.Media;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Threading.Tasks;

namespace TextToPicTranslator.Services
{

    // Sets the alias for the "Image" reference because they both have one and this way you can specify how it works 
    using Images = Xamarin.Forms.Image;
    using Image = SixLabors.ImageSharp.Image;
    class Sharpen
    {
        // start of the service to sharpen the images if failure to bring back a result on the initial OCR attempt
        // this is not currently being used and right now contains sample code 
        public static Image sharpen (Image image)
        {
            Image sharpenededImage = image;
            sharpenededImage.Mutate(x => x.GaussianSharpen(10));

            sharpenededImage.Save("test2.jpg");
            //Resize(image.Width / 2, image.Height / 2));

            //image.Mutate(x => x.Resize(image.Width / 2, image.Height / 2));
            //IImageProcessingContext context = sharpenededImage.Mutate(GaussianSharpenExtensions.GaussianSharpen(test));

            //GaussianSharpenExtensions.GaussianSharpen(image);

            return sharpenededImage;
        }


        // sharpen properties - start of sharpen task this is not active as of right yet but contains the start for
        // improvements that will happen later. Right now the second phase of photo processing happens from another api call
        private async Task<Images> sharpen(string filename)
        {
            // waits to be called before executing
            await CrossMedia.Current.Initialize();
            Images temp = new Images();
            //temp.Source = ImageSource;
            //source
            //temp.source = ImageSource;

            Image sharpenededImage = Image.Load("test.jpg");
            //sharpenededImage.So
            sharpenededImage.Mutate(x => x.GaussianSharpen(10));
            sharpenededImage.Save("test2.jpg");

            temp = new Images { Source = "test2.jpg" };
            //ImageSource = temp.Source;
            //Resize(image.Width / 2, image.Height / 2));

            //image.Mutate(x => x.Resize(image.Width / 2, image.Height / 2));
            //IImageProcessingContext context = sharpenededImage.Mutate(GaussianSharpenExtensions.GaussianSharpen(test));

            //GaussianSharpenExtensions.GaussianSharpen(image);

            return temp;
        }
    }

}

