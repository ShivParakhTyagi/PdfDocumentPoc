using System;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using PdfDocumentPoc.Universal.Helpers.Extensions;

namespace PdfDocumentPoc.Universal.Helpers.Converters
{
    public class PdfPagetoBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BitmapImage image = new BitmapImage();
            Task.Run(() =>
            {
                if (value is PdfPage page)
                {
                    InMemoryRandomAccessStream inMemoryRandomStream = new InMemoryRandomAccessStream();

                    page.RenderToStreamAsync(inMemoryRandomStream).AsTask().ContinueWith(async x =>
                    {
                        if (x.IsCompleted)
                        {
                            await x.RunOnUiThreadAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                using (inMemoryRandomStream)
                                {
                                    image.SetSource(inMemoryRandomStream);
                                }
                            });
                        }
                    });
                }
            });
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}