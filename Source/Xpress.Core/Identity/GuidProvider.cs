using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Xpress.Core.Extensions;

namespace Xpress.Core.Identity
{
     public static  class GuidProvider
    {
        public static class Comb
        {
            private static readonly RandomNumberGenerator RandomNumberGenerator = RandomNumberGenerator.Create();

            public static Guid Create()
            {
                return Create(SequentialGuidType.SequentialAtEnd);
            }

            public static Guid Create(SequentialGuidType sequentialGuidType)
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

        public static class Deterministic
        {
            public static class Namespaces
            {
                public static readonly Guid Events = Guid.Parse("387F5B61-9E98-439A-BFF1-15AD0EA91EA0");
                public static readonly Guid Commands = Guid.Parse("4286D89F-7F92-430B-8E00-E468FE3C3F59");
            }

            public static Guid Create(Guid namespaceId, string name)
            {
                if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

                var nameBytes = Encoding.UTF8.GetBytes(name);

                return Create(namespaceId, nameBytes);
            }

            public static Guid Create(Guid namespaceId, byte[] nameBytes)
            {
                // Always use version 5 (version 3 is MD5, version 5 is SHA1)
                const int version = 5;

                if (namespaceId == default(Guid)) throw new ArgumentNullException(nameof(namespaceId));
                if (nameBytes.Length == 0) throw new ArgumentNullException(nameof(nameBytes));

                // Convert the namespace UUID to network order (step 3)
                var namespaceBytes = namespaceId.ToByteArray();
                SwapByteOrder(namespaceBytes);

                // Compute the hash of the name space ID concatenated with the name (step 4)
                byte[] hash;
                using (var algorithm = SHA1.Create())
                {
                    var combinedBytes = new byte[namespaceBytes.Length + nameBytes.Length];
                    Buffer.BlockCopy(namespaceBytes, 0, combinedBytes, 0, namespaceBytes.Length);
                    Buffer.BlockCopy(nameBytes, 0, combinedBytes, namespaceBytes.Length, nameBytes.Length);

                    hash = algorithm.ComputeHash(combinedBytes);
                }

                // Most bytes from the hash are copied straight to the bytes of the new
                // GUID (steps 5-7, 9, 11-12)
                var newGuid = new byte[16];
                Array.Copy(hash, 0, newGuid, 0, 16);

                // Set the four most significant bits (bits 12 through 15) of the time_hi_and_version
                // field to the appropriate 4-bit version number from Section 4.1.3 (step 8)
                newGuid[6] = (byte)((newGuid[6] & 0x0F) | (version << 4));

                // Set the two most significant bits (bits 6 and 7) of the clock_seq_hi_and_reserved
                // to zero and one, respectively (step 10)
                newGuid[8] = (byte)((newGuid[8] & 0x3F) | 0x80);

                // Convert the resulting UUID to local byte order (step 13)
                SwapByteOrder(newGuid);
                return new Guid(newGuid);
            }

            internal static void SwapByteOrder(byte[] guid)
            {
                SwapBytes(guid, 0, 3);
                SwapBytes(guid, 1, 2);
                SwapBytes(guid, 4, 5);
                SwapBytes(guid, 6, 7);
            }

            internal static void SwapBytes(byte[] guid, int left, int right)
            {
                var temp = guid[left];
                guid[left] = guid[right];
                guid[right] = temp;
            }
        }
    }
}
