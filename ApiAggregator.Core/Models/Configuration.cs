
namespace ApiAggregator.Core.Models
{
    public enum SecurityOption
    {
        None,
        KeyInQueryString,
        KeyInRequestHeader,
        SignedRequest
    };

    public class Configuration : Root
    {
        public string Apikey { get; set; }

        public SecurityOption SecurityOption { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool RequireLogin { get; set; }

        public bool RequireAuthenticator { get; set; }

        public string AuthenticatorSecret { get; set; }

        public bool DescribeAtRoot { get; set; }
    }
}