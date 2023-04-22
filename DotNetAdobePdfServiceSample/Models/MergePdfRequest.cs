using System.ComponentModel.DataAnnotations;

namespace DotNetEasyPdfSample.Web.Models
{
    /// <summary>
    /// PDFマージリクエスト
    /// </summary>
    public class MergePdfRequest
    {
        /// <summary>
        /// マージ対象となるファイル1
        /// </summary>
        [Required]
        public IFormFile File1 { get; set; } = null!;

        /// <summary>
        /// マージ対象となるファイル2
        /// </summary>
        [Required]
        public IFormFile File2 { get; set; } = null!;
    }
}
