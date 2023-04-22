using System.ComponentModel.DataAnnotations;

namespace DotNetEasyPdfSample.Web.Models
{
    /// <summary>
    /// PDF変換リクエスト
    /// </summary>
    public class ConvertToPdfRequest
    {
        /// <summary>
        /// 変換対象となるファイル
        /// </summary>
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
