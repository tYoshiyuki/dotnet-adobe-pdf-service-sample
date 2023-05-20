using System.Net;
using Microsoft.AspNetCore.Mvc;
using DotNetAdobePdfServiceSample.Lib.Interfaces;
using DotNetAdobePdfServiceSample.Lib;
using DotNetAdobePdfServiceSample.Models;

namespace DotNetAdobePdfServiceSample.Controllers
{
    /// <summary>
    /// PDF変換・マージコントローラ
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : Controller
    {
        private readonly IAdobePdfService _adobePdfService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="adobePdfService"><see cref="IAdobePdfService"/></param>
        public PdfController(IAdobePdfService adobePdfService)
        {
            _adobePdfService = adobePdfService;
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
                var file = request.File;
                using var inputStream = new MemoryStream();
                await file.CopyToAsync(inputStream);

                // 変換処理の実行
                var outputStream = _adobePdfService.ConvertToPdf(inputStream, file.FileName);

                return File(outputStream, "application/octet-stream", fileDownloadName: Path.GetFileNameWithoutExtension(file.FileName) + DateTime.Now.Ticks + ".pdf");
            }
            catch (AdobePdfServiceException ex)
            {
                return Problem(ex.Message, statusCode: (int)HttpStatusCode.BadRequest);
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
                var file1 = request.File1;
                var file2 = request.File2;

                string extension1 = Path.GetExtension(file1.FileName);
                string extension2 = Path.GetExtension(file2.FileName);

                if (!extension1.Contains(".pdf", StringComparison.InvariantCultureIgnoreCase) || !extension2.Contains(".pdf", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Problem("File type must be pdf.", statusCode: (int)HttpStatusCode.BadRequest);
                }

                using var inputStream1 = new MemoryStream();
                await file1.CopyToAsync(inputStream1);

                using var inputStream2 = new MemoryStream();
                await file1.CopyToAsync(inputStream2);

                // マージ処理の実行
                var outputStream = _adobePdfService.MergePdf(inputStream1, inputStream2);

                return File(outputStream, "application/octet-stream", fileDownloadName: "Merged" + DateTime.Now.Ticks + ".pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// PDFファイルリストをマージします。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("MergePdfList")]
        [Produces("application/octet-stream", Type = typeof(FileResult))]
        public async Task<IActionResult> MergePdfList([FromForm] MergePdfListRequest request)
        {
            List<MemoryStream> inputStreamList = new();

            try
            {
                if (request.FileList.Count > 1)
                {
                    return Problem("Number of files must be 2 or more.", statusCode: (int)HttpStatusCode.BadRequest);
                }

                foreach (var file in request.FileList)
                {
                    string extension = Path.GetExtension(file.FileName);

                    if (!extension.Contains(".pdf", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return Problem("File type must be pdf.", statusCode: (int)HttpStatusCode.BadRequest);
                    }

                    var inputStream = new MemoryStream();
                    await file.CopyToAsync(inputStream);
                    inputStreamList.Add(inputStream);
                }

                // マージ処理の実行
                var outputStream = _adobePdfService.MergePdfList(inputStreamList);

                return File(outputStream, "application/octet-stream", fileDownloadName: "Merged" + DateTime.Now.Ticks + ".pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                inputStreamList.ForEach(x => x.Dispose());
            }
        }
    }
}
