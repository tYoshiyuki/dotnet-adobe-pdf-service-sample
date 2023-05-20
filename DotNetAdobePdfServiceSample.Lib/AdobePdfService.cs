using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.pdfops;
using DotNetAdobePdfServiceSample.Lib.Interfaces;
using ExecutionContext = Adobe.PDFServicesSDK.ExecutionContext;

namespace DotNetAdobePdfServiceSample.Lib
{
    /// <summary>
    /// PDFサービスクラス
    /// </summary>
    public class AdobePdfService : IAdobePdfService
    {
        private readonly ExecutionContext _executionContext;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="executionContext"><see cref="ExecutionContext"/></param>
        public AdobePdfService(ExecutionContext executionContext)
        {
            _executionContext = executionContext;
        }

        /// <inheritdoc />
        /// <exception cref="AdobePdfServiceException"></exception>
        public Stream ConvertToPdf(Stream stream, string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string mediaType;

            // NOTE 拡張子によってメディアタイプを選定します。必要に応じて判定処理を追加します。
            if (extension.Contains(".doc", StringComparison.OrdinalIgnoreCase))
            {
                mediaType = CreatePDFOperation.SupportedSourceFormat.DOC.GetMediaType();
            }
            else if (extension.Contains(".docx", StringComparison.OrdinalIgnoreCase))
            {
                mediaType = CreatePDFOperation.SupportedSourceFormat.DOCX.GetMediaType();
            }
            else if (extension.Contains(".xls", StringComparison.OrdinalIgnoreCase))
            {
                mediaType = CreatePDFOperation.SupportedSourceFormat.XLS.GetMediaType();
            }
            else if (extension.Contains(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                mediaType = CreatePDFOperation.SupportedSourceFormat.XLSX.GetMediaType();
            }
            else
            {
                throw new AdobePdfServiceException($"Not supported file type:{extension}.");
            }

            // 入力ファイルを FileRef に変換
            stream.Position = 0;
            var source = FileRef.CreateFromStream(stream, mediaType);

            var createPdfOperation = CreatePDFOperation.CreateNew();
            createPdfOperation.SetInput(source);

            // 変換処理の実行
            var result = createPdfOperation.Execute(_executionContext);

            // FileRef を Stream に変換
            var outputStream = new MemoryStream();
            result.SaveAs(outputStream);
            outputStream.Position = 0;

            return outputStream;
        }

        /// <inheritdoc />
        public Stream MergePdf(Stream stream1, Stream stream2)
        {
            var combineFilesOperation = CombineFilesOperation.CreateNew();

            // 入力ファイルを FileRef に変換
            stream1.Position = 0;
            var source1 = FileRef.CreateFromStream(stream1, CombineFilesOperation.SupportedSourceFormat.PDF.GetMediaType());
            combineFilesOperation.AddInput(source1);

            stream2.Position = 0;
            var source2 = FileRef.CreateFromStream(stream2, CombineFilesOperation.SupportedSourceFormat.PDF.GetMediaType());
            combineFilesOperation.AddInput(source2);

            // マージ処理の実行
            var result = combineFilesOperation.Execute(_executionContext);

            // FileRef を Stream に変換
            var outputStream = new MemoryStream();
            result.SaveAs(outputStream);
            outputStream.Position = 0;

            return outputStream;
        }

        /// <inheritdoc />
        public Stream MergePdfList(IEnumerable<Stream> streams)
        {
            var combineFilesOperation = CombineFilesOperation.CreateNew();

            // 入力ファイルを FileRef に変換
            foreach (var stream in streams)
            {
                stream.Position = 0;
                var source = FileRef.CreateFromStream(stream, CombineFilesOperation.SupportedSourceFormat.PDF.GetMediaType());
                combineFilesOperation.AddInput(source);
            }

            // マージ処理の実行
            var result = combineFilesOperation.Execute(_executionContext);

            // FileRef を Stream に変換
            var outputStream = new MemoryStream();
            result.SaveAs(outputStream);
            outputStream.Position = 0;

            return outputStream;
        }
    }
}
