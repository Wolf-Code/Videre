using System.Globalization;
using CookComputing.XmlRpc;

namespace VidereSubs
{
    internal static class XmlRpcStructExtensions
    {
        /// <summary>
        /// Retrieves data from the <see cref="XmlRpcStruct"/> as a string.
        /// </summary>
        /// <param name="data">The <see cref="XmlRpcStruct"/> containing the data.</param>
        /// <param name="key">The key.</param>
        /// <returns>The object as a string.</returns>
        public static string GetString( this XmlRpcStruct data, string key )
        {
            return ( string ) data[ key ];
        }

        /// <summary>
        /// Retrieves data from the <see cref="XmlRpcStruct"/> as a float.
        /// </summary>
        /// <param name="data">The <see cref="XmlRpcStruct"/> containing the data.</param>
        /// <param name="key">The key.</param>
        /// <returns>The object as a float.</returns>
        public static float GetFloat( this XmlRpcStruct data, string key )
        {
            return float.Parse( GetString( data, key ), CultureInfo.InvariantCulture );
        }

        /// <summary>
        /// Retrieves data from the <see cref="XmlRpcStruct"/> as an unsigned integer.
        /// </summary>
        /// <param name="data">The <see cref="XmlRpcStruct"/> containing the data.</param>
        /// <param name="key">The key.</param>
        /// <returns>The object as an unsigned integer.</returns>
        public static uint GetUInt( this XmlRpcStruct data, string key )
        {
            return uint.Parse( GetString( data, key ), CultureInfo.InvariantCulture );
        }

        /// <summary>
        /// Retrieves data from the <see cref="XmlRpcStruct"/> as a long.
        /// </summary>
        /// <param name="data">The <see cref="XmlRpcStruct"/> containing the data.</param>
        /// <param name="key">The key.</param>
        /// <returns>The object as a long.</returns>
        public static long GetLong( this XmlRpcStruct data, string key )
        {
            return long.Parse( GetString( data, key ), CultureInfo.InvariantCulture );
        }

        /// <summary>
        /// Retrieves data from the <see cref="XmlRpcStruct"/> as an ulong.
        /// </summary>
        /// <param name="data">The <see cref="XmlRpcStruct"/> containing the data.</param>
        /// <param name="key">The key.</param>
        /// <returns>The object as an ulong.</returns>
        public static ulong GetULong( this XmlRpcStruct data, string key )
        {
            return ulong.Parse( GetString( data, key ), CultureInfo.InvariantCulture );
        }

        /// <summary>
        /// Retrieves data from the <see cref="XmlRpcStruct"/> as an array of <see cref="XmlRpcStruct"/>.
        /// </summary>
        /// <param name="data">The <see cref="XmlRpcStruct"/> containing the data.</param>
        /// <param name="key">The key.</param>
        /// <returns>The object as an array.</returns>
        public static XmlRpcStruct[ ] GetXmlRpcStructArray( this XmlRpcStruct data, string key )
        {
            return data[ key ] as XmlRpcStruct[ ];
        }
    }
}
