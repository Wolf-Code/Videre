using System;

namespace VidereSubs.Attributes
{
    /// <summary>
    /// An attribute to be used on subtitle loader classes.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class )]
    public class SubtitleLoaderAttribute : Attribute
    {
        /// <summary>
        /// The extensions for which the loader should be used.
        /// </summary>
        public string[ ] Extensions { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="extensions">The extensions for which this loader should be used.</param>
        public SubtitleLoaderAttribute( params string[ ] extensions )
        {
            Extensions = extensions;
        }
    }
}