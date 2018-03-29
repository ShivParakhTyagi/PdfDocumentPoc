using System;
using System.Collections.ObjectModel;
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
        public PdfDocument PdfDocument;
        public ObservableCollection<PdfFilePage> PdfPages;

        public static PdfFile LoadPdfFile(StorageFile storageFile, ObservableCollection<PdfFilePage> collection)
        {
            PdfFile pdfFile = new PdfFile();
            pdfFile.StorageFile = storageFile;
            pdfFile.PdfPages = collection;
            var t = Task.Run(() =>
            {
                Task.Yield();
                return pdfFile.LoadAsync();
            });
            pdfFile.RunningTask = t;
            return pdfFile;
        }

        public string Title
        {
            get { return StorageFile?.DisplayName ?? ""; }
        }

        public Task RunningTask { get; private set; }

        private async Task LoadAsync()
        {
            if (StorageFile == null)
            {
                return;
            }

            var pdfDocument = await PdfDocument.LoadFromFileAsync(StorageFile);
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

    public class PdfFilePage
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
    }
}