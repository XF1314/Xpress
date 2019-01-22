using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Xpress.Core.Threading
{
    public class NullCancellationTokenProvider : ICancellationTokenProvider
    {
        public static NullCancellationTokenProvider Instance { get; } = new NullCancellationTokenProvider();

        public CancellationToken Token { get; } = default(CancellationToken);

        private NullCancellationTokenProvider()
        {

        }
    }
}
