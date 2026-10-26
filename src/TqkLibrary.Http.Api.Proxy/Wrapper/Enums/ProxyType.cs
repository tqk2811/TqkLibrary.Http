using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Proxy.Wrapper.Enums
{
    [Flags]
    public enum ProxyType
    {
        Invalid = 0,
        Http = 1 << 0,
        Socks4 = 1 << 1,
        Socks5 = 1 << 2,
        Socks = Socks4 | Socks5,
    }
}
