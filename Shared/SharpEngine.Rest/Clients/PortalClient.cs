using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEngine.Rest.Clients;

internal class PortalClient : RestClient, IPortalClient
{
    public PortalClient(HttpClient httpClient) : base(httpClient) { }
}
