using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using AyanaWebApi.Services.Interfaces;

namespace AyanaWebApi.Services
{
    public class ImgsConverterService : IImgsConverterService
    {

        public string ConvertToJpg(string fullImgName, long quality)
        {
            Image img = Image.FromFile(fullImgName);
            using (var b = new Bitmap(img.Width, img.Height))
            {
                b.SetResolution(img.HorizontalResolution, img.VerticalResolution);

                using (var g = Graphics.FromImage(b))
                {
                    g.Clear(Color.White);
                    g.DrawImageUnscaled(img, 0, 0);
                }

                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                Encoder myEncoder = Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
                myEncoderParameters.Param[0] = myEncoderParameter;

                string extImg = Path.GetExtension(fullImgName);
                string compressedFullImgName = fullImgName.Replace(extImg, "_compressed.jpg");
                b.Save(compressedFullImgName, jpgEncoder, myEncoderParameters);
                return compressedFullImgName;
            }
        }

        ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid) { return codec; }
            }
            return null;
        }
    }
}
