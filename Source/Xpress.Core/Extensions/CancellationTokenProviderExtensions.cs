using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xpress.Core.Threading;

namespace Xpress.Core.Extensions
{
    public static class CancellationTokenProviderExtensions
    {
        public static CancellationToken FallbackToProvider(this ICancellationTokenProvider provider, CancellationToken prefferedValue = default(CancellationToken))
        {
            return prefferedValue == default(CancellationToken) || prefferedValue == CancellationToken.None
                ? provider.Token
                : prefferedValue;
        }
    }
}
