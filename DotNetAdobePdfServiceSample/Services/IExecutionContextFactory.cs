using ExecutionContext = Adobe.PDFServicesSDK.ExecutionContext;

namespace DotNetAdobePdfServiceSample.Services
{
    /// <summary>
    /// <see cref="ExecutionContext"/> を生成するインターフェースです。
    /// </summary>
    public interface IExecutionContextFactory
    {
        /// <summary>
        /// <see cref="ExecutionContext"/> を生成します。
        /// </summary>
        /// <returns><see cref="ExecutionContext"/></returns>
        ExecutionContext Create();
    }
}
