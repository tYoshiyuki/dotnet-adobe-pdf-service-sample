namespace DotNetAdobePdfServiceSample.Lib.Interfaces
{
    /// <summary>
    /// PDFサービスインターフェース
    /// </summary>
    public interface IAdobePdfService
    {
        /// <summary>
        /// PDFファイルへ変換します。
        /// </summary>
        /// <param name="stream">変換対象のストリーム</param>
        /// <param name="fileName">変換対象ファイル名</param>
        /// <returns>変換結果のストリーム</returns>
        Stream ConvertToPdf(Stream stream, string fileName);

        /// <summary>
        /// PDFファイルをマージします。
        /// </summary>
        /// <param name="stream1">マージ対象のストリーム1</param>
        /// <param name="stream2">マージ対象のストリーム2</param>
        /// <returns>変換結果のストリーム</returns>
        Stream MergePdf(Stream stream1, Stream stream2);

        /// <summary>
        /// PDFファイルリストをマージします。
        /// </summary>
        /// <param name="streams">マージ対象のストリームリスト</param>
        /// <returns>変換結果のストリーム</returns>
        Stream MergePdfList(IEnumerable<Stream> streams);
    }
}
