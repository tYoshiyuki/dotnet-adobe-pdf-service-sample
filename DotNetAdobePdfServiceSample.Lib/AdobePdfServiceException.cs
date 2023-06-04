using System.Runtime.Serialization;

namespace DotNetAdobePdfServiceSample.Lib
{
    /// <summary>
    /// PDFサービスの例外クラス
    /// </summary>
    public class AdobePdfServiceException : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AdobePdfServiceException(){ }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message"></param>
        public AdobePdfServiceException(string? message) : base(message) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public AdobePdfServiceException(string? message, Exception exception) : base(message, exception) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected AdobePdfServiceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
