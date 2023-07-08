using DotNetAdobePdfServiceSample.Lib.Interfaces;

namespace DotNetAdobePdfServiceSample.Lib
{
    /// <summary>
    /// <see cref="IAdobePdfService.ConvertToPdf"/> の入力モデル
    /// </summary>
    public class ConvertToPdfInput
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConvertToPdfInput(Stream stream, string fileName)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }

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
