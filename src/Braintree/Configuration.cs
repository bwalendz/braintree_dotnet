using Braintree.Exceptions;
using System.Net;
#if netcore
using System.Net.Http;
#endif

namespace Braintree
{
#if netcore
    public delegate HttpRequestMessage HttpRequestMessageFactory(HttpMethod method, string requestUriString);
#else
    public delegate HttpWebRequest HttpWebRequestFactory(string requestUriString);
#endif

    public class Configuration
    {
        public Environment Environment { get; set; }
        public string AccessToken { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string MerchantId { get; set; }
        public string PrivateKey { get; set; }
        public IWebProxy WebProxy { get; set; }
        public string PublicKey { get; set; }
#if netcore
        public HttpRequestMessageFactory HttpRequestMessageFactory { get; set; }
#else
        public HttpWebRequestFactory HttpWebRequestFactory { get; set; }
#endif

        private int timeout;
        public int Timeout
        {
            get { return timeout == 0 ? 60000 : timeout; }
            set { timeout = value; }
        }

        public Configuration()
        {
#if netcore
            HttpRequestMessageFactory = (method, requestUriString) => new HttpRequestMessage(method, requestUriString);
#else
            HttpWebRequestFactory = requestUriString => WebRequest.Create(requestUriString) as HttpWebRequest;
#endif
        }

        public Configuration(string accessToken) : this()
        {
            CredentialsParser parser = new CredentialsParser(accessToken);
            MerchantId = parser.MerchantId;
            AccessToken = parser.AccessToken;
            Environment = parser.Environment;
        }

        public Configuration(string clientId, string clientSecret) : this()
        {
            CredentialsParser parser = new CredentialsParser(clientId, clientSecret);
            ClientId = parser.ClientId;
            ClientSecret = parser.ClientSecret;
            Environment = parser.Environment;
        }

        public Configuration(Environment environment, string merchantId, string publicKey, string privateKey) : this()
        {
            if (environment == null)
            {
                throw new ConfigurationException("Configuration.environment needs to be set");
            }
            else if (merchantId == null)
            {
                throw new ConfigurationException("Configuration.merchantId needs to be set");
            }
            else if (publicKey == null)
            {
                throw new ConfigurationException("Configuration.publicKey needs to be set");
            }
            else if (privateKey == null)
            {
                throw new ConfigurationException("Configuration.privateKey needs to be set");
            }

            Environment = environment;
            MerchantId = merchantId;
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        public Configuration(string environment, string merchantId, string publicKey, string privateKey) :
            this(Environment.ParseEnvironment(environment), merchantId, publicKey, privateKey)
        {
        }

        public bool IsClientCredentials
        {
            get { return ClientId != null; }
        }

        public bool IsAccessToken
        {
            get { return AccessToken != null; }
        }

        public void AssertHasClientCredentials()
        {
            if (ClientId == null)
            {
                throw new ConfigurationException("Missing ClientId when constructing BraintreeGateway");
            }
            if (ClientSecret == null)
            {
                throw new ConfigurationException("Missing ClientSecret when constructing BraintreeGateway");
            }
            if (AccessToken != null)
            {
                throw new ConfigurationException("AccessToken must not be passed when constructing BraintreeGateway");
            }
        }

        public void AssertHasAccessTokenOrKeys()
        {
            if (AccessToken == null)
            {
                if (MerchantId == null)
                {
                    throw new ConfigurationException("Missing MerchantId (or AccessToken) when constructing BraintreeGateway");
                }
                if (Environment == null)
                {
                    throw new ConfigurationException("Missing Environment when constructing BraintreeGateway");
                }
                if (PublicKey == null)
                {
                    throw new ConfigurationException("Missing PublicKey when constructing BraintreeGateway");
                }
                if (PrivateKey == null)
                {
                    throw new ConfigurationException("Missing PrivateKey when constructing BraintreeGateway");
                }
            }
            if (ClientId != null)
            {
                throw new ConfigurationException("ClientId must not be passed when constructing BraintreeGateway");
            }
            if (ClientSecret != null)
            {
                throw new ConfigurationException("ClientSecret must not be passed when constructing BraintreeGateway");
            }
        }
    }
}
