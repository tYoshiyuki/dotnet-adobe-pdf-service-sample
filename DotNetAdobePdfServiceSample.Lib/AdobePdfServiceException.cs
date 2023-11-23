using System.Runtime.Serialization;
using Adobe.PDFServicesSDK.exception;

namespace DotNetAdobePdfServiceSample.Lib
{
    /// <summary>
    /// PDFサービスの例外クラス
    /// </summary>
    public class AdobePdfServiceException : Exception
    {
        /// <summary>
        /// ステータスコード
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// エラーコード
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// <see cref="AdobePdfServiceErrorType"/>
        /// </summary>
        public AdobePdfServiceErrorType ErrorType { get; set; }

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
        public AdobePdfServiceException(string? message, Exception exception) : base(message, exception)
        {
            switch (exception)
            {
                case ArgumentException argumentException 
                when argumentException.Message.StartsWith("Not supported file type:"):
                    this.StatusCode = 400;
                    this.ErrorCode = argumentException.Message;
                    this.ErrorType = AdobePdfServiceErrorType.UnsupportedFormat;
                    break;
                case ServiceApiException apiException:
                    {
                        this.StatusCode = apiException.StatusCode;
                        this.ErrorCode = apiException.ErrorCode;

                        if (this.ErrorCode == "PASSWORD_PROTECTED")
                        {
                            this.ErrorType = AdobePdfServiceErrorType.LockedWithPassword;
                        }
                        else if (this.ErrorCode == "CORRUPT_DOCUMENT")
                        {
                            this.ErrorType = AdobePdfServiceErrorType.InvalidFileFormat;
                        }
                        else if (this.StatusCode == 429)
                        {
                            this.ErrorType = AdobePdfServiceErrorType.TooManyRequests;
                        }
                        else
                        {
                            this.ErrorType = AdobePdfServiceErrorType.UnexpectedError;
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected AdobePdfServiceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
