namespace VolumeSignatureWebApplicationExample.Services;

public interface ISignatureService
{
    bool VerifySignature(byte[] data, string signatureHeader);
    bool VerifySignature(string json, string signatureHeader);
}