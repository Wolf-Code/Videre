using CookComputing.XmlRpc;

namespace VidereSubs.OpenSubtitles.Outputs
{
    /// <summary>
    /// The default request output base class.
    /// </summary>
    public abstract class Output
    {
        /// <summary>
        /// The possible status codes when requesting data from the server.
        /// </summary>
        public enum StatusCode
        {
            /// <summary>
            /// The request was OK and nothing went wrong.
            /// </summary>
            Succesful_200_OK,
            /// <summary>
            /// Partial content, message.
            /// </summary>
            Succesful_206_Partial,

            /// <summary>
            /// Host has been moved.
            /// </summary>
            Moved_301,

            /// <summary>
            /// The client is unauthorized.
            /// </summary>
            Error_401_Unauthorized,
            /// <summary>
            /// The subtitles have an invalid format.
            /// </summary>
            Error_402_SubInvalidFormat,
            /// <summary>
            /// The subhashes (content and sent subhash) are not the same.
            /// </summary>
            Error_403_SubHashesNotSame,
            /// <summary>
            /// The subtitles have an invalid language.
            /// </summary>
            Error_404_SubtitlesInvalidLanguage,
            /// <summary>
            /// Not all mandatory parameters have been specified.
            /// </summary>
            Error_405_NotAllMandatoryParametersSpecified,
            /// <summary>
            /// There was no session active.
            /// </summary>
            Error_406_NoSession,
            /// <summary>
            /// The download limit has been reached.
            /// </summary>
            Error_407_DownloadLimitReached,
            /// <summary>
            /// Invalid parameters were supplied.
            /// </summary>
            Error_408_InvalidParameters,
            /// <summary>
            /// The method was not found.
            /// </summary>
            Error_409_MethodNotFound,
            /// <summary>
            /// An unknown error has occurred.
            /// </summary>
            Error_410_UnknownError,
            /// <summary>
            /// An empty or invalid user agent was supplied.
            /// </summary>
            Error_411_InvalidUserAgent,
            /// <summary>
            /// %s has an invalid format.
            /// </summary>
            Error_412_InvalidFormat,
            /// <summary>
            /// An invlaid IMDB ID was supplied.
            /// </summary>
            Error_413_InvalidIMDBID,
            /// <summary>
            /// An unknown user agent was supplied.
            /// </summary>
            Error_414_UnknownUserAgent,
            /// <summary>
            /// A disabled user agent was supplied.
            /// </summary>
            Error_415_DisabledUserAgent,
            /// <summary>
            /// The internal subtitle validation has failed.
            /// </summary>
            Error_416_InternalSubtitleValidationFailed,

            /// <summary>
            /// The service is unavailable.
            /// </summary>
            ServerError_503_ServiceUnavailable,
            /// <summary>
            /// The server is under maintenance.
            /// </summary>
            ServerError_506_ServerUnderMaintenance,

            /// <summary>
            /// An unknown status code was encountered.
            /// </summary>
            UnknownStatusCode,
        }

        /// <summary>
        /// The request's status code.
        /// </summary>
        public StatusCode Status { private set; get; }

        /// <summary>
        /// The status string.
        /// </summary>
        public string StatusString { get; }

        /// <summary>
        /// The status string without the code in front of it.
        /// </summary>
        public string StatusStringWithoutCode => StatusString.Substring( 4 );

        /// <summary>
        /// The amount of seconds it took to process this request.
        /// </summary>
        public double Seconds { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="output">The output from the server request.</param>
        protected Output( XmlRpcStruct output )
        {
            if ( output.ContainsKey( "status" ) )
                StatusString = ( string ) output[ "status" ];
            else
                StatusString = "200 OK";

            Status = ConvertStatusToStatusCode( StatusString );
            Seconds = ( double ) output[ "seconds" ];
        }

        /// <summary>
        /// Converts a status string into a status code.
        /// </summary>
        /// <param name="status">The status string.</param>
        /// <returns>The status code.</returns>
        public static StatusCode ConvertStatusToStatusCode( string status )
        {
            int statusCode = int.Parse( status.Substring( 0, 3 ) );

            switch ( statusCode )
            {
                case 200:
                    return StatusCode.Succesful_200_OK;

                case 206:
                    return StatusCode.Succesful_206_Partial;

                case 301:
                    return StatusCode.Moved_301;

                case 401:
                    return StatusCode.Error_401_Unauthorized;

                case 402:
                    return StatusCode.Error_402_SubInvalidFormat;

                case 403:
                    return StatusCode.Error_403_SubHashesNotSame;

                case 404:
                    return StatusCode.Error_404_SubtitlesInvalidLanguage;

                case 405:
                    return StatusCode.Error_405_NotAllMandatoryParametersSpecified;

                case 406:
                    return StatusCode.Error_406_NoSession;

                case 407:
                    return StatusCode.Error_407_DownloadLimitReached;

                case 408:
                    return StatusCode.Error_408_InvalidParameters;

                case 409:
                    return StatusCode.Error_409_MethodNotFound;

                case 410:
                    return StatusCode.Error_410_UnknownError;

                case 411:
                    return StatusCode.Error_411_InvalidUserAgent;

                case 412:
                    return StatusCode.Error_412_InvalidFormat;

                case 413:
                    return StatusCode.Error_413_InvalidIMDBID;

                case 414:
                    return StatusCode.Error_414_UnknownUserAgent;

                case 415:
                    return StatusCode.Error_415_DisabledUserAgent;

                case 416:
                    return StatusCode.Error_416_InternalSubtitleValidationFailed;

                case 503:
                    return StatusCode.ServerError_503_ServiceUnavailable;

                case 506:
                    return StatusCode.ServerError_506_ServerUnderMaintenance;

                default:
                    return StatusCode.UnknownStatusCode;
            }
        }
    }
}
