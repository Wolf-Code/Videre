
using CookComputing.XmlRpc;

namespace VidereSubs.OpenSubtitles.Outputs
{
    /// <summary>
    /// The output for the login request.
    /// </summary>
    public class LogInOutput : Output
    {
        /// <summary>
        /// The session token.
        /// </summary>
        public string Token { internal set; get; }

        /// <summary>
        /// Whether or not the login was succesful.
        /// </summary>
        public bool LogInSuccesful => Status == StatusCode.Succesful_200_OK;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="output">The login request output.</param>
        public LogInOutput( XmlRpcStruct output ) : base( output )
        {
        }
    }
}
