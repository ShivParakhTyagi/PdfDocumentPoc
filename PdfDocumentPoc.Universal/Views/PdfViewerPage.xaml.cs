using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using PdfDocumentPoc.Universal.Helpers.Extensions;
using PdfDocumentPoc.Universal.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PdfDocumentPoc.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PdfViewerPage : Page
    {
        private Task _filePickerTask;

        public PdfViewerPage()
        {
            this.InitializeComponent();
        }

        private void FilePickerButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_filePickerTask != null && _filePickerTask.IsCompleted == false)
                {
                    return;
                }
                var fileOpenPicker = new FileOpenPicker();
                fileOpenPicker.FileTypeFilter.Add(".pdf");
                _filePickerTask = fileOpenPicker.PickMultipleFilesAsync().AsTask().ContinueWith(ContinuationAction);
            }
            catch (Exception ex)
            {
                ShowMessage($"Exception:\n{ex.Message}");
            }
        }

        private void ShowMessage(string message)
        {
            var t = Task.Factory.RunOnUiThreadAsync(CoreDispatcherPriority.Normal,
                () => { StatusTextBlock.Text = message ?? ""; });
        }


        private void ContinuationAction(Task<IReadOnlyList<StorageFile>> task)
        {
            if (task.IsCompleted == false)
            {
                return;
            }

            var source = new ObservableCollection<PdfFilePage>();
            task.RunOnUiThreadAsync(CoreDispatcherPriority.Normal, () =>
            {
                PdfPagesListView.ItemsSource = source;

                PdfPagesPreviewListView.ItemsSource = source;
                PdfPagesTextPreviewListView.ItemsSource = source;
            });
            var files = task.Result;
            if (files != null)
            {
                foreach (var storageFile in files)
                {
                    PdfFile.LoadPdfFile(storageFile, source);
                }
            }
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            var src = PdfPagesListView.ItemsSource;
            var source = new ObservableCollection<PdfFilePage>();
            PdfPagesListView.ItemsSource = source;
            Task.Delay(TimeSpan.FromSeconds(5));
            PdfPagesListView.ItemsSource = src;

        }

        private void PreviewGrid_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Grid grid)
            {
                if (grid.DataContext is PdfFilePage page)
                {
                    PdfPagesListView?.ScrollIntoView(page, ScrollIntoViewAlignment.Leading);
                    this.DataContext = page;
                }
            }
        }

        private void PdfPagesPreviewListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (sender is ListView listView)
            {
                
                if (listView.SelectedItem is PdfFilePage page)
                {
                    PdfPagesListView.ScrollIntoView(page, ScrollIntoViewAlignment.Leading);
                    this.DataContext = page;

                    PdfPagesPreviewListView.SelectedItem = page;
                    PdfPagesTextPreviewListView.SelectedItem = page;
                    PdfPagesPreviewListView.ScrollIntoView(PdfPagesPreviewListView.SelectedItem);
                    PdfPagesTextPreviewListView.ScrollIntoView(PdfPagesTextPreviewListView.SelectedItem);
                    page.Reload();
                }
            }
        }

        private void HamburgerMenuButton_Clicked(object sender, RoutedEventArgs e)
        {
            HamburgerMenu.IsPaneOpen = !HamburgerMenu.IsPaneOpen;
        }
    }
}
