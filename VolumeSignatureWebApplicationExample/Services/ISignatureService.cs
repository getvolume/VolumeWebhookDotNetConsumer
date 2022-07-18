namespace VolumeSignatureWebApplicationExample.Services;

public interface ISignatureService
{
    bool ValidateSignature(byte[] data, string signatureHeader);
    bool ValidateSignature(string json, string signatureHeader);
}