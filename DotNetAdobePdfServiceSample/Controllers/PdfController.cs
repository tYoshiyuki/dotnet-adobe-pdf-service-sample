using System.Net;
using DotNetEasyPdfSample.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.pdfops;
using ExecutionContext = Adobe.PDFServicesSDK.ExecutionContext;

namespace DotNetAdobePdfServiceSample.Controllers
{
    /// <summary>
    /// PDF変換・マージコントローラ
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : Controller
    {
        private readonly ExecutionContext executionContext;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="executionContext"></param>
        public PdfController(ExecutionContext executionContext)
        {
            this.executionContext = executionContext;
        }

        /// <summary>
        /// PDFファイルへ変換します。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ConvertToPdf")]
        [Produces("application/octet-stream", Type = typeof(FileResult))]
        public async Task<IActionResult> ConvertToPdf([FromForm] ConvertToPdfRequest request)
        {
            try
            {
                var createPdfOperation = CreatePDFOperation.CreateNew();

                string mediaType;
                var file = request.File;
                var extension = Path.GetExtension(file.FileName);
                
                // NOTE 拡張子によってメディアタイプを選定します。必要に応じて判定処理を追加します。
                if (extension.Contains(".doc", StringComparison.OrdinalIgnoreCase))
                {
                    mediaType = CreatePDFOperation.SupportedSourceFormat.DOC.GetMediaType();
                }
                else if(extension.Contains(".docx", StringComparison.OrdinalIgnoreCase))
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
                    return Problem($"Not supported file type:{extension}.", statusCode: (int)HttpStatusCode.BadRequest);
                }

                // 入力ファイルを FileRef に変換
                using var inputStream = new MemoryStream();
                await file.CopyToAsync(inputStream);
                inputStream.Position = 0;
                var source = FileRef.CreateFromStream(inputStream, mediaType);
                createPdfOperation.SetInput(source);

                // 変換処理の実行
                var result = createPdfOperation.Execute(executionContext);

                // FileRef を Stream に変換
                var outputStream = new MemoryStream();
                result.SaveAs(outputStream);
                outputStream.Position = 0;

                return File(outputStream, "application/octet-stream", fileDownloadName: Path.GetFileNameWithoutExtension(file.FileName) + DateTime.Now.Ticks + ".pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// PDFファイルをマージします。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("MergePdf")]
        [Produces("application/octet-stream", Type = typeof(FileResult))]
        public async Task<IActionResult> MergePdf([FromForm] MergePdfRequest request)
        {
            try
            {
                var combineFilesOperation = CombineFilesOperation.CreateNew();

                var file1 = request.File1;
                var file2 = request.File2;

                var extension1 = Path.GetExtension(file1.FileName);
                var extension2 = Path.GetExtension(file2.FileName);

                if (!extension1.Contains(".pdf", StringComparison.InvariantCultureIgnoreCase) || !extension2.Contains(".pdf", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Problem("File type must be pdf.", statusCode: (int)HttpStatusCode.BadRequest);
                }

                // 入力ファイルを FileRef に変換
                using var inputStream1 = new MemoryStream();
                await file1.CopyToAsync(inputStream1);
                inputStream1.Position = 0;
                var source1 = FileRef.CreateFromStream(inputStream1, CombineFilesOperation.SupportedSourceFormat.PDF.GetMediaType());
                combineFilesOperation.AddInput(source1);

                using var inputStream2 = new MemoryStream();
                await file1.CopyToAsync(inputStream2);
                inputStream2.Position = 0;
                var source2 = FileRef.CreateFromStream(inputStream2, CombineFilesOperation.SupportedSourceFormat.PDF.GetMediaType());
                combineFilesOperation.AddInput(source2);

                // マージ処理の実行
                var result = combineFilesOperation.Execute(executionContext);

                // FileRef を Stream に変換
                var outputStream = new MemoryStream();
                result.SaveAs(outputStream);
                outputStream.Position = 0;

                return File(outputStream, "application/octet-stream", fileDownloadName: "Merged" + DateTime.Now.Ticks + ".pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}
