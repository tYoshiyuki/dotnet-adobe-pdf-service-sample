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
        /// <param name="convertToPdfInput"><see cref="ConvertToPdfInput"/></param>
        /// <returns>変換結果のストリーム</returns>
        Stream ConvertToPdf(ConvertToPdfInput　convertToPdfInput);

        /// <summary>
        /// リストをPDFへ変換します。
        /// </summary>
        /// <param name="convertToPdfInputs"><see cref="ConvertToPdfInput"/>のリスト</param>
        /// <returns>変換結果のストリームリスト</returns>
        IEnumerable<Stream> ConvertToPdfList(IEnumerable<ConvertToPdfInput> convertToPdfInputs);

        /// <summary>
        /// PDFファイルリストをマージします。
        /// </summary>
        /// <param name="streams">マージ対象のストリームリスト</param>
        /// <returns>変換結果のストリーム</returns>
        Stream MergePdfList(IEnumerable<Stream> streams);
    }
}
