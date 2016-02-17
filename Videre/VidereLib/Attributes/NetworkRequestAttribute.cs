using System;

namespace VidereLib.Attributes
{
    /// <summary>
    /// The attribute for handling network requests.
    /// </summary>
    [AttributeUsage( AttributeTargets.Method )]
    public class NetworkRequestAttribute : Attribute
    {
        /// <summary>
        /// The possible identifiers for a network request.
        /// </summary>
        public enum RequestIdentifier
        {
            /// <summary>
            /// The request for playing the media.
            /// </summary>
            Play = 0,

            /// <summary>
            /// The request for pausing the media.
            /// </summary>
            Pause = 1,

            /// <summary>
            /// The request for pausing or resuming the media, depending on the current state.
            /// </summary>
            PauseOrResume = 2,
        }
        /// <summary>
        /// The network request identifier.
        /// </summary>
        public RequestIdentifier Identifier { protected set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="identifier">The network request identifier.</param>
        public NetworkRequestAttribute( RequestIdentifier identifier )
        {
            Identifier = identifier;
        }
    }
}
