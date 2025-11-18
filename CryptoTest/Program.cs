using System;
using System.Text;
using CryptoLib.Models;
using CryptoLib.Services;

class Program
{
    static void Main()
    {
        IDsaService dsa = new DsaService();

        Console.WriteLine("=== TEST DSA ===");
        var key = dsa.GenerateKey(2048);

        string message = "Hello DSA demo";
        Console.WriteLine("Message: " + message);

        var sig = dsa.Sign(message, key);
        string sigBase64 = Convert.ToBase64String(sig);
        Console.WriteLine("Signature (Base64): " + sigBase64);

        bool ok = dsa.Verify(message, sig, new DsaKeyPair
        {
            PublicKey = key.PublicKey,
            KeySize = key.KeySize
        });

        Console.WriteLine("Verify đúng message: " + ok);

        // Test sửa message
        bool ok2 = dsa.Verify("Hello DSA demo (changed)", sig, new DsaKeyPair
        {
            PublicKey = key.PublicKey,
            KeySize = key.KeySize
        });

        Console.WriteLine("Verify sai message: " + ok2);
        Console.ReadLine();
    }
}
