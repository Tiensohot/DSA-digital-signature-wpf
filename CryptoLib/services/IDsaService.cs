using CryptoLib.Models;

namespace CryptoLib.Services
{
    public interface IDsaService
    {
        DsaKeyPair GenerateKey(int keySize);

        byte[] Sign(string message, DsaKeyPair key);

        bool Verify(string message, byte[] signature, DsaKeyPair key);
    }
}
