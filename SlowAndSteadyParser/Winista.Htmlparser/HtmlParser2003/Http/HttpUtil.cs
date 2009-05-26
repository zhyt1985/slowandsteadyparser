// ***************************************************************
//  HttpUtil   version:  1.0   Date: 12/20/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Web;
using System.Net;

namespace Winista.Text.HtmlParser.Http
{
	/// <summary>
	/// Summary description for HttpUtil.
	/// </summary>
	public class HttpUtil
	{
		public static String GetWebExceptionDescription(WebException ex)
		{
			switch(ex.Status)
			{
				case WebExceptionStatus.ConnectFailure:
					return "The remote service point could not be contacted at the transport level.";
				case WebExceptionStatus.ConnectionClosed:
					return "The connection was prematurely closed.";
				case WebExceptionStatus.KeepAliveFailure:
					return "The connection for a request that specifies the Keep-alive header was closed unexpectedly.";
				case WebExceptionStatus.MessageLengthLimitExceeded:
					return "A message was received that exceeded the specified limit when sending a request or receiving a response from the server.";
				case WebExceptionStatus.NameResolutionFailure:
					return "The name resolver service could not resolve the host name.";
				case WebExceptionStatus.Pending:
					return "An internal asynchronous request is pending.";
				case WebExceptionStatus.PipelineFailure:
					return "This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.";
				case WebExceptionStatus.ProtocolError:
					return "The response received from the server was complete but indicated a protocol-level error. For example, an HTTP protocol error such as 401 Access Denied would use this status";
				case WebExceptionStatus.ProxyNameResolutionFailure:
					return "The name resolver service could not resolve the proxy host name.";
				case WebExceptionStatus.ReceiveFailure:
					return "A complete response was not received from the remote server";
				case WebExceptionStatus.RequestCanceled:
					return "The request was canceled, the WebRequest.Abort method was called, or an unclassifiable error occurred. This is the default value for Status.";
				case WebExceptionStatus.SecureChannelFailure:
					return "An error occurred in a secure channel link.";
				case WebExceptionStatus.SendFailure:
					return "A complete request could not be sent to the remote server.";
				case WebExceptionStatus.ServerProtocolViolation:
					return "The server response was not a valid HTTP response.";
				case WebExceptionStatus.Success:
					return "No error was encountered.";
				case WebExceptionStatus.Timeout:
					return "No response was received during the time-out period for a request.";
				case WebExceptionStatus.TrustFailure:
					return "A server certificate could not be validated.";
				case WebExceptionStatus.UnknownError:
				default:
					return "An exception of unknown type has occurred.";
			}
		}
	}
}
