using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Xpress.Core.DependencyInjection;
using Xpress.Core.Extensions;

namespace Xpress.Core.Identity
{
    /// <summary>
    /// Implements <see cref="IGuidGenerator"/> by creating sequential Guids.
    /// Use <see cref="SequentialGuidGeneratorOptions"/> to configure.
    /// </summary>
    public class SequentialGuidGenerator : IGuidGenerator, ITransientDependency
    {
        private static readonly RandomNumberGenerator RandomNumberGenerator = RandomNumberGenerator.Create();

        public SequentialGuidGenerator()
        {
        }

        public Guid Create()
        {
            return Create(SequentialGuidType.SequentialAtEnd);
        }

        public Guid Create(SequentialGuidType sequentialGuidType)
        {
            // We start with 16 bytes of cryptographically strong random data.
            var randomBytes = new byte[10];
            RandomNumberGenerator.Locking(r => r.GetBytes(randomBytes));

            var timestamp = DateTime.UtcNow.Ticks / 10000L;

            // Then get the bytes
            var timestampBytes = BitConverter.GetBytes(timestamp);

            // Since we're converting from an Int64, we have to reverse on
            // little-endian systems.
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampBytes);
            }

            var guidBytes = new byte[16];
            switch (sequentialGuidType)
            {
                case SequentialGuidType.SequentialAsString:
                case SequentialGuidType.SequentialAsBinary:

                    // For string and byte-array version, we copy the timestamp first, followed
                    // by the random data.
                    Buffer.BlockCopy(timestampBytes, 2, guidBytes, 0, 6);
                    Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);

                    // If formatting as a string, we have to compensate for the fact
                    // that .NET regards the Data1 and Data2 block as an Int32 and an Int16,
                    // respectively.  That means that it switches the order on little-endian
                    // systems.  So again, we have to reverse.
                    if (sequentialGuidType == SequentialGuidType.SequentialAsString && BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(guidBytes, 0, 4);
                        Array.Reverse(guidBytes, 4, 2);
                    }

                    break;

                case SequentialGuidType.SequentialAtEnd:

                    // For sequential-at-the-end versions, we copy the random data first,
                    // followed by the timestamp.
                    Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
                    Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);
                    break;
            }

            return new Guid(guidBytes);
        }
    }
}
