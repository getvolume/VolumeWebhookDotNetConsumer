using System.Security.Cryptography;
using System.Text;

namespace VolumeSignatureWebApplicationExample.Services;

/*
This service is solely responsible for fetching current public key from Volume domain and verifying 
if a signature string corresponds to some data. There are two possibilities of passing the data 
 - as byte array
 - as a json string
As the signature in Volumes backend is made by using the json string this is also a recommended method to choose.
Providing a modified string (or different bytes), other than the one passed in the request body will not pass verification. 
*/
public class SignatureService : ISignatureService
{
    private readonly ILogger<SignatureService> _logger;
    private readonly byte[] _volumePemBytes;

    public SignatureService(ILogger<SignatureService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _volumePemBytes = GetKeyBytes(configuration["VolumePemUrl"]);
    }

    private static byte[] GetKeyBytes(string volumePemUrl)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        var publicKey = client.GetStringAsync(volumePemUrl);

        var keyBytes = Convert.FromBase64String(publicKey.Result);
        return keyBytes;
    }

    public bool VerifySignature(byte[] data, string signatureHeader)
    {
        var r = RSA.Create();
        r.ImportSubjectPublicKeyInfo(_volumePemBytes, out _);

        var signatureParts = signatureHeader.Split();
        var expectedSignatureBytes = Convert.FromBase64String(signatureParts[1]);

        return r.VerifyData(data, expectedSignatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public bool VerifySignature(string json, string signatureHeader)
    {
        var jsonBytes = Encoding.UTF8.GetBytes(json!);
        return VerifySignature(jsonBytes, signatureHeader);
    }
}