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
        /// <param name="setting"><see cref="AdobePdfServiceSetting"/></param>
        public ExecutionContextFactory(AdobePdfServiceSetting setting)
        {
            _credentials = Credentials.ServicePrincipalCredentialsBuilder()
                .WithClientId(setting.ClientId)
                .WithClientSecret(setting.ClientSecret)
                .Build();
        }

        /// <inheritdoc />
        public ExecutionContext Create() => ExecutionContext.Create(_credentials);
    }
}
