using System.ComponentModel.DataAnnotations;

namespace DotNetAdobePdfServiceSample.Models
{
    /// <summary>
    /// PDFリストマージリクエスト
    /// </summary>
    public class MergePdfListRequest
    {
        /// <summary>
        /// マージ対象となるファイルリスト
        /// </summary>
        [Required]
        public IFormFileCollection FileList { get; set; } = null!;
    }
}
