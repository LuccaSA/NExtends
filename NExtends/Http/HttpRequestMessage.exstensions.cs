using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NExtends.Http
{
	public static class HttpRequestMessageExtensions
	{
		// GetClientIpAddress sources:
		// http://stackoverflow.com/questions/9565889/get-the-ip-address-of-the-remote-host
		// https://gist.github.com/MikeJansen/2653453

		/// <summary>
		/// Gets the IP host address of the remote client.
		/// </summary>
		/// <param name="request"></param>
		/// <returns>The IP address of the remote client OR null if unable to find it</returns>
		public static string GetClientIpAddress(this HttpRequestMessage request)
		{
			if (request.Properties.ContainsKey("MS_HttpContext"))
			{
				return ((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
			}
			else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
			{
				return ((RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name]).Address;
			}

			return null;
		}
	}
}
