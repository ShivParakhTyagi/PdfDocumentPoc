using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.UI.Core;
using PdfDocumentPoc.Universal.Helpers.Extensions;

namespace PdfDocumentPoc.Universal.Models
{
    public class PdfFile
    {
        protected StorageFile StorageFile;
        protected StorageFile LocalStorageFile;
        public const string LocalFileName = "Test.pdf";
        public PdfDocument PdfDocument;
        public ObservableCollection<PdfFilePage> PdfPages;

        public static PdfFile LoadPdfFile(StorageFile storageFile, ObservableCollection<PdfFilePage> collection)
        {
            PdfFile pdfFile = new PdfFile();
            pdfFile.PdfPages = collection;
            pdfFile.Import(storageFile).ContinueWith(x =>
            {
                var t = Task.Run(() =>
                {
                    Task.Yield();
                    return pdfFile.LoadAsync();
                });
                pdfFile.RunningTask = t;
            });
            return pdfFile;
        }

        private async Task Import(StorageFile storageFile)
        {
            StorageFile = storageFile;

            await Task.Yield();

            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(LocalFileName);
            await storageFile.CopyAndReplaceAsync(file);
            LocalStorageFile = file;
        }

        public string Title
        {
            get { return LocalStorageFile?.DisplayName ?? ""; }
        }

        public Task RunningTask { get; private set; }

        private async Task LoadAsync()
        {
            var file = LocalStorageFile;
            if (file == null)
            {
                return;
            }

            var pdfDocument = await PdfDocument.LoadFromFileAsync(file);
            for (uint i = 0; i < pdfDocument.PageCount; i++)
            {
                var pdfPage = pdfDocument.GetPage(i);
                var page = PdfFilePage.CreatePage(this, pdfPage, i);
                var t = Task.Factory.RunOnUiThreadAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PdfPages.Add(page);
                });
            }

        }
    }

    public class PdfFilePage:INotifyPropertyChanged
    {
        public string Title
        {
            get { return $"{PdfFile.Title}: {Index + 1}"; }
        }

        public PdfFile PdfFile { get; private set; }
        public PdfPage PdfPage { get; private set; }
        public uint Index{ get; private set; }

        public static PdfFilePage CreatePage(PdfFile pdfFile, PdfPage pdfPage, uint index)
        {
            PdfFilePage page = new PdfFilePage
            {
                PdfFile = pdfFile,
                PdfPage = pdfPage,
                Index = index
            };
            return page;
        }

        private async Task LoadAsync()
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(PdfFile.LocalFileName);
            if (file == null)
            {
                return;
            }

            var pdfDocument = await PdfDocument.LoadFromFileAsync(file);
            PdfFile.PdfDocument = pdfDocument;
            for (uint i = 0; i < pdfDocument.PageCount; i++)
            {
                var pdfPage = pdfDocument.GetPage(Index);
                PdfPage = pdfPage;
            }
        }

        public void Reload()
        {
            LoadAsync().ContinueWith(x =>
                x.RunOnUiThreadAsync(CoreDispatcherPriority.Normal, () => { OnPropertyChanged(nameof(PdfPage)); }));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}