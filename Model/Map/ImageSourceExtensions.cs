
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace MiMototaxxi.Model.Map
{
    public static class ImageSourceExtensions
    {
        /*public static Android.Graphics.Bitmap LoadPlatformImage(this ImageSource imageSource)
        {
            if (imageSource is FileImageSource fileImageSource)
            {
                // Carga la imagen desde un archivo
                var context = Android.App.Application.Context;
                var drawable = context.Resources.GetDrawable(context.Resources.GetIdentifier(
                    fileImageSource.File, "drawable", context.PackageName));
                return ((Android.Graphics.Drawables.BitmapDrawable)drawable).Bitmap;
            }
            else if (imageSource is StreamImageSource streamImageSource)
            {
                // Carga la imagen desde un stream
                var stream = streamImageSource.Stream(CancellationToken.None).Result;
                return Android.Graphics.BitmapFactory.DecodeStream(stream);
            }

            return null;
        }*/
    }
}
