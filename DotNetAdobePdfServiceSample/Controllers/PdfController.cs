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
                // 変換処理の実行
                var outputStream = await ConvertToPdfInternal(request.File);

                return File(outputStream, "application/octet-stream", fileDownloadName: Path.GetFileNameWithoutExtension(request.File.FileName) + DateTime.Now.Ticks + ".pdf");
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

                if (!extension1.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase) || !extension2.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Problem("File type must be pdf.", statusCode: (int)HttpStatusCode.BadRequest);
                }

                using var inputStream1 = new MemoryStream();
                await file1.CopyToAsync(inputStream1);

                using var inputStream2 = new MemoryStream();
                await file1.CopyToAsync(inputStream2);

                var inputStreamList = new List<Stream>
                {
                    inputStream1, inputStream2
                };

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
        }

        /// <summary>
        /// PDFファイルリストをマージします。
        /// PDF以外のファイルは全てPDFに変換した後、マージを行います。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("MergePdfList")]
        [Produces("application/octet-stream", Type = typeof(FileResult))]
        public async Task<IActionResult> MergePdfList([FromForm] MergePdfListRequest request)
        {
            List<Stream> inputStreamList = new();

            try
            {
                if (request.FileList.Count < 2)
                {
                    return Problem("Number of files must be 2 or more.", statusCode: (int)HttpStatusCode.BadRequest);
                }

                // 変換処理を並列処理で実施、マージ順番を保持するようにする
                var convertTask = request.FileList
                    .AsParallel()
                    .AsOrdered()
                    .Select(async x =>
                    {
                        string extension = Path.GetExtension(x.FileName);

                        Stream inputStream;
                        if (!extension.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
                        {
                            inputStream = await ConvertToPdfInternal(x);
                        }
                        else
                        {
                            inputStream = new MemoryStream();
                            await x.CopyToAsync(inputStream);
                        }

                        return inputStream;
                    }).ToList();

                inputStreamList = (await Task.WhenAll(convertTask)).ToList();

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

        /// <summary>
        /// PDF変換の内部処理です。
        /// </summary>
        /// <param name="file"><see cref="IFormFile"/></param>
        /// <returns><see cref="Stream"/>の<see cref="Task"/></returns>
        private async Task<Stream> ConvertToPdfInternal(IFormFile file)
        {
            var inputStream = new MemoryStream();
            await file.CopyToAsync(inputStream);
            return _adobePdfService.ConvertToPdf(inputStream, file.FileName);
        }
    }
}
