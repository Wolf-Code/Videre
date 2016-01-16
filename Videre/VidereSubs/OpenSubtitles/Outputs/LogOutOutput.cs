
using CookComputing.XmlRpc;

namespace VidereSubs.OpenSubtitles.Outputs
{
    /// <summary>
    /// The output for the logout request.
    /// </summary>
    public class LogOutOutput : Output
    {
        /// <summary>
        /// Whether or not the logout was succesful.
        /// </summary>
        public bool LogOutSuccesful => Status == StatusCode.Succesful_200_OK;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="output">The logout request output.</param>
        public LogOutOutput( XmlRpcStruct output ) : base( output )
        {

        }
    }
}
