using System.Security.Cryptography;
using System.Text;

namespace VolumeSignatureWebApplicationExample.Services;

public class SignatureService: ISignatureService
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
    
    public bool ValidateSignature(byte[] data, string signatureHeader)
    {
        var r = RSA.Create();
        r.ImportSubjectPublicKeyInfo(_volumePemBytes, out _);
        
        var signatureParts = signatureHeader.Split();
        var expectedSignatureBytes = Convert.FromBase64String(signatureParts[1]);

        return r.VerifyData(data, expectedSignatureBytes, HashAlgorithmName.SHA256  , RSASignaturePadding.Pkcs1);
    }

    public bool ValidateSignature(string json, string signatureHeader)
    {
        var jsonBytes = Encoding.UTF8.GetBytes(json!);
        return ValidateSignature(jsonBytes, signatureHeader);
    }
}