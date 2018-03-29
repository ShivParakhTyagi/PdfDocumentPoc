using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;

namespace PdfDocumentPoc.Universal.Helpers.Extensions
{
    public static class TaskFactoryExtension
    {

        public static IAsyncAction RunOnUiThreadAsync(this Task _, CoreDispatcherPriority priority,
            DispatchedHandler agileCallback)
        {
            return RunOnUiThreadAsync(priority, agileCallback);
        }

        public static IAsyncAction RunOnUiThreadAsync(this TaskFactory _, CoreDispatcherPriority priority,
            DispatchedHandler agileCallback)
        {
            return RunOnUiThreadAsync(priority, agileCallback);
        }

        public static IAsyncAction RunOnUiThreadAsync(CoreDispatcherPriority priority,
            DispatchedHandler agileCallback)
        {
            return CoreApplication.MainView.Dispatcher.RunAsync(priority, agileCallback);
        }

        public static IAsyncOperation<bool> TryRunOnUiThreadAsync(this TaskFactory _, CoreDispatcherPriority priority,
            DispatchedHandler agileCallback)
        {
            return TryRunOnUiThreadAsync(priority, agileCallback);
        }

        public static IAsyncOperation<bool> TryRunOnUiThreadAsync(CoreDispatcherPriority priority,
            DispatchedHandler agileCallback)
        {
            return CoreApplication.MainView.Dispatcher.TryRunAsync(priority, agileCallback);
        }

    }
}