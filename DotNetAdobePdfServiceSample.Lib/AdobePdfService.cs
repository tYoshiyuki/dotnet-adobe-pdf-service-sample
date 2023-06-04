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
        public Stream ConvertToPdf(ConvertToPdfInput convertToPdfInput)
        {
            try
            {
                return ConvertToPdfInternal(convertToPdfInput);
            }
            catch (Exception ex)
            {
                throw new AdobePdfServiceException("ConvertToPdf failed.", ex);
            }
        }

        /// <inheritdoc />
        public IEnumerable<Stream> ConvertToPdfList(IEnumerable<ConvertToPdfInput> convertToPdfInputs)
        {
            try
            {
                // 変換処理を並列処理で実施、マージ順番を保持するようにする
                return convertToPdfInputs
                    .AsParallel()
                    .AsOrdered()
                    .Select(x =>
                    {
                        // PDFファイルの場合は、そのままストリームを返却する
                        string extension = Path.GetExtension(x.FileName);
                        return !extension.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase)
                            ? ConvertToPdf(x)
                            : x.Stream;
                    }).ToList();
            }
            catch (Exception ex)
            {
                throw new AdobePdfServiceException("ConvertToPdfList failed.", ex);
            }
        }

        /// <inheritdoc />
        public Stream MergePdfList(IEnumerable<Stream> streams)
        {
            try
            {
                var combineFilesOperation = CombineFilesOperation.CreateNew();

                // 入力ファイルを FileRef に変換
                foreach (var stream in streams)
                {
                    combineFilesOperation.AddInput(CreateMergePdfFileRef(stream));
                }

                // マージ処理の実行
                var result = combineFilesOperation.Execute(_executionContext);

                // FileRef を Stream に変換
                var outputStream = new MemoryStream();
                result.SaveAs(outputStream);
                outputStream.Position = 0;

                return outputStream;
            }
            catch (Exception ex)
            {
                throw new AdobePdfServiceException("MergePdfList failed.", ex);
            }
        }

        /// <summary>
        /// PDFマージ向けに<see cref="Stream"/>を元に<see cref="FileRef"/>を生成します。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns><see cref="FileRef"/></returns>
        private static FileRef CreateMergePdfFileRef(Stream stream)
        {
            stream.Position = 0;
            return FileRef.CreateFromStream(stream, CombineFilesOperation.SupportedSourceFormat.PDF.GetMediaType());
        }

        /// <summary>
        /// PDFファイルへ変換する内部処理です。
        /// </summary>
        /// <param name="convertToPdfInput"><see cref="ConvertToPdfInput"/></param>
        /// <returns>変換結果のストリーム</returns>
        /// <exception cref="AdobePdfServiceException"></exception>
        private Stream ConvertToPdfInternal(ConvertToPdfInput convertToPdfInput)
        {
            string extension = Path.GetExtension(convertToPdfInput.FileName);
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
                throw new ArgumentException($"Not supported file type:{extension}.");
            }

            // 入力ファイルを FileRef に変換
            convertToPdfInput.Stream.Position = 0;
            var source = FileRef.CreateFromStream(convertToPdfInput.Stream, mediaType);

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
    }
}
