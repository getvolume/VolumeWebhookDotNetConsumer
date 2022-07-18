namespace VolumeSignatureWebApplicationExample.Data;

public record ComplexWebhookRequestDto(Guid paymentId, Guid? merchantPaymentId, VolumePaymentStatus paymentStatus,
    String? errorDescription, PaymentRequestDto paymentRequest);