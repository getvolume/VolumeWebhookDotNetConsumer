namespace VolumeSignatureWebApplicationExample.Data;

public record PrimitiveWebhookRequestDto(String paymentId, String? merchantPaymentId, String paymentStatus,
    String? errorDescription, PaymentRequestDto paymentRequest);