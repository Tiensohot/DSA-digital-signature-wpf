namespace CryptoLib.Models
{
    public class DsaKeyPair
    {
        public byte[] PublicKey { get; set; } = Array.Empty<byte>();
        public byte[] PrivateKey { get; set; } = Array.Empty<byte>();
        public int KeySize { get; set; }
    }

}
