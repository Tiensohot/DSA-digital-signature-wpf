using System.Security.Cryptography;
using System.Text;
using CryptoLib.Models;

namespace CryptoLib.Services
{
    public class DsaService : IDsaService
    {
        private readonly HashAlgorithmName _hashAlgo = HashAlgorithmName.SHA256;

        public DsaKeyPair GenerateKey(int keySize)
        {
            using var dsa = DSA.Create();
            dsa.KeySize = keySize;

            var pub = dsa.ExportSubjectPublicKeyInfo(); // public key chuẩn X.509
            var priv = dsa.ExportPkcs8PrivateKey();     // private key chuẩn PKCS#8

            return new DsaKeyPair
            {
                KeySize = dsa.KeySize,
                PublicKey = pub,
                PrivateKey = priv
            };
        }

        public byte[] Sign(string message, DsaKeyPair key)
        {
            using var dsa = DSA.Create();

            // Import private key
            dsa.ImportPkcs8PrivateKey(key.PrivateKey, out _);

            var data = Encoding.UTF8.GetBytes(message);
            return dsa.SignData(data, _hashAlgo);
        }

        public bool Verify(string message, byte[] signature, DsaKeyPair key)
        {
            using var dsa = DSA.Create();

            // Import public key
            dsa.ImportSubjectPublicKeyInfo(key.PublicKey, out _);

            var data = Encoding.UTF8.GetBytes(message);
            return dsa.VerifyData(data, signature, _hashAlgo);
        }
    }
}
