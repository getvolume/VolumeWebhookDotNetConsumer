namespace VolumeSignatureWebApplicationExample.Data;

public record PaymentRequestDto(decimal? amount, String? currency, String? reference);