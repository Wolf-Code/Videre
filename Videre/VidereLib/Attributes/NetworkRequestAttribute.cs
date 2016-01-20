using System;

namespace VidereLib.Attributes
{
    /// <summary>
    /// The attribute for handling network requests.
    /// </summary>
    public class NetworkRequestAttribute : Attribute
    {
        /// <summary>
        /// The network request identifier.
        /// </summary>
        public byte Identifier { protected set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="identifier">The network request identifier.</param>
        public NetworkRequestAttribute( byte identifier )
        {
            this.Identifier = identifier;
        }
    }
}
