using Adobe.PDFServicesSDK.auth;
using ExecutionContext = Adobe.PDFServicesSDK.ExecutionContext;

namespace DotNetAdobePdfServiceSample.Services
{
    /// <summary>
    /// <see cref="ExecutionContext"/> を生成するクラスです。
    /// </summary>
    public class ExecutionContextFactory : IExecutionContextFactory
    {
        private readonly Credentials credentials;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="environment"><see cref="IHostEnvironment"/></param>
        public ExecutionContextFactory(IHostEnvironment environment)
        {
            credentials = Credentials.ServiceAccountCredentialsBuilder()
                .FromFile(Path.Combine(environment.ContentRootPath, "pdfservices-api-credentials.json"))
                .Build();
        }

        /// <inheritdoc />
        public ExecutionContext Create() => ExecutionContext.Create(credentials);
    }
}
