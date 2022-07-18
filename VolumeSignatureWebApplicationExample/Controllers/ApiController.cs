using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VolumeSignatureWebApplicationExample.Data;
using VolumeSignatureWebApplicationExample.Services;

namespace VolumeSignatureWebApplicationExample.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    private ISignatureService _signatureService;

    public ApiController(ISignatureService signatureService)
    {
        _signatureService = signatureService;
    }

    /*
    This example method takes an object body and hence it can easily access a string representation of the JSON.
    This is the safest method possible as byte array created from the json will for sure be the same as the one 
    used while creating the signature in the Volume backend.   
    */
    [HttpPost("Webhook")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public ActionResult<HttpStatusCode> PostWebhook([FromBody] object data, [FromHeader] string authorization)
    {
        var json = data.ToString();
        var signature =  _signatureService.ValidateSignature(json!, authorization);
        if (!signature) return StatusCode(StatusCodes.Status403Forbidden, null);
        
        var validDeserializedSimpleDto = JsonConvert.DeserializeObject<PrimitiveWebhookRequestDto>(json!);
        var validDeserializedMoreComplexDto = JsonConvert.DeserializeObject<ComplexWebhookRequestDto>(json!);
        // business logic cant be applied here as both DTOs are properly deserialized and contain correct data
        
        return StatusCode(StatusCodes.Status202Accepted, data);
    }
    
    /*
    This example method takes a deserialized Dto object but this object has only primitive types so when we serialize
    it to a JSON string it will be the same as the one which was used to generate a signature in the Volume backend.       
    Engineers need to be very careful, as in this case some local Dto object specificities can lead to a completely
    different signature. F.ex. wrapper classes will produce different results than simple strings.    
    */
    [HttpPost("WebhookDto")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public ActionResult<HttpStatusCode> PostWebhookDto([FromBody] PrimitiveWebhookRequestDto data, [FromHeader] string authorization)
    {
        var jsonData = JsonConvert.SerializeObject(data);
        var signature =  _signatureService.ValidateSignature(jsonData, authorization);
        return signature ? StatusCode(StatusCodes.Status202Accepted, data) : StatusCode(StatusCodes.Status403Forbidden, null);
    }
    
    /*
    A FAILURE EXAMPLE
    This example method takes a deserialized Dto object and this object contains an Enum member instead of a simple
    string one for payment status. This lead to a different json string hence invalid signature verification.     
    */
    [HttpPost("WebhookComplexDto")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public ActionResult<HttpStatusCode> PostWebhookComplexDto([FromBody] ComplexWebhookRequestDto data, [FromHeader] string authorization)
    {
        var jsonData = JsonConvert.SerializeObject(data);
        var signature =  _signatureService.ValidateSignature(jsonData, authorization);
        return signature ? StatusCode(StatusCodes.Status202Accepted, data) : StatusCode(StatusCodes.Status403Forbidden, null);
    }
    
}