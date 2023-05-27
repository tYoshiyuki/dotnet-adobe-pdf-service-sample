using DotNetAdobePdfServiceSample.Lib.Interfaces;

namespace DotNetAdobePdfServiceSample.Lib
{
    /// <summary>
    /// <see cref="IAdobePdfService.ConvertToPdf"/> の入力モデル
    /// </summary>
    public class ConvertToPdfInput
    {
        /// <summary>
        /// 変換対象ストリーム
        /// </summary>
        public Stream Stream { get; set; }

        /// <summary>
        /// 変換対象ファイル名
        /// </summary>
        public string FileName { get; set; }
    }
}
