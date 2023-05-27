using Adobe.PDFServicesSDK.auth;
using DotNetAdobePdfServiceSample.Lib.Interfaces;
using ExecutionContext = Adobe.PDFServicesSDK.ExecutionContext;

namespace DotNetAdobePdfServiceSample.Lib
{
    /// <summary>
    /// <see cref="ExecutionContext"/> を生成するクラスです。
    /// </summary>
    public class ExecutionContextFactory : IExecutionContextFactory
    {
        private readonly Credentials _credentials;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="rootPath">設定ファイルのルートパス</param>
        public ExecutionContextFactory(string rootPath)
        {
            // NOTE pdfservices-api-credentials.json と private.key は同一階層である必要があります。
            _credentials = Credentials.ServiceAccountCredentialsBuilder()
                .FromFile(Path.Combine(rootPath, "pdfservices-api-credentials.json"))
                .Build();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="setting"><see cref="AdobePdfServiceSetting"/></param>
        public ExecutionContextFactory(AdobePdfServiceSetting setting)
        {
            _credentials = Credentials.ServiceAccountCredentialsBuilder()
                .WithClientId(setting.ClientId)
                .WithClientSecret(setting.ClientSecret)
                .WithOrganizationId(setting.OrganizationId)
                .WithAccountId(setting.AccountId)
                .WithPrivateKey(setting.PrivateKey)
                .Build();
        }

        /// <inheritdoc />
        public ExecutionContext Create() => ExecutionContext.Create(_credentials);
    }
}
