using System.Security.Cryptography;
using System.Text;

namespace Solid.Api.Common;

public static class Hashing
{
    public static string Sha256(string value)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
    }

    public static string RandomToken(int bytes = 40)
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(bytes)).ToLowerInvariant();
    }
}
