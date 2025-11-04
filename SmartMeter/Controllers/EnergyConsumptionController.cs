//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using SmartMeter.Models.DTOs;
//using System.Security.Claims;
//using SmartMeter.Services.Interface;

//namespace SmartMeter.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Authorize]
//    public class EnergyConsumptionController : Controller
//    {

//        private readonly IEnergyConsumptionService _energyService;
//        private readonly ILogger<EnergyConsumptionController> _logger;

//        public EnergyConsumptionController(IEnergyConsumptionService energyService, ILogger<EnergyConsumptionController> logger)
//        {
//            _energyService = energyService;
//            _logger = logger;
//        }


//        //[HttpPost("record")]
//        //public async Task<ActionResult<ApiResponse<bool>>> RecordConsumption([FromBody] EnergyConsumptionRecordDto record)
//        //{
//        //    try
//        //    {
//        //        var result = await _energyService.RecordEnergyConsumptionAsync(record);
//        //        if (result)
//        //            return Ok(ApiResponse<bool>.SuccessResponse(true, "Energy consumption recorded successfully"));
//        //        else
//        //            return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to record energy consumption"));
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        _logger.LogError(ex, "Error recording energy consumption");
//        //        return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while recording energy consumption"));
//        //    }
//        //}

//        [HttpPost("record")]
//        public async Task<ActionResult<ApiResponse<bool>>> RecordConsumption([FromBody] EnergyConsumptionRecordDto record)
//        {
//            try
//            {
//                _logger.LogInformation(" Received record request for meter: {Meter}", record.MeterSerialNo);

//                var result = await _energyService.RecordEnergyConsumptionAsync(record);

//                _logger.LogInformation("Service returned: {Result}", result);

//                if (result)
//                    return Ok(ApiResponse<bool>.SuccessResponse(true, "Energy consumption recorded successfully"));
//                else
//                    return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to record energy consumption 1"));
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Controller error: {Message}", ex.Message);
//                return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while recording energy consumption"));
//            }
//        }


//        [HttpGet("consumption")]
//        public async Task<ActionResult<ApiResponse<List<EnergyConsumptionResponseDto>>>> GetConsumption(
//           [FromQuery] string meterSerialNo,
//           [FromQuery] DateTime fromDate,
//           [FromQuery] DateTime toDate)
//        {
//            try
//            {
//                var userId = GetUserId();
//                var query = new EnergyConsumptionQueryDto
//                {
//                    MeterSerialNo = meterSerialNo,
//                    FromDate = fromDate,
//                    ToDate = toDate
//                };

//                var consumption = await _energyService.GetEnergyConsumptionAsync(query, userId);
//                return Ok(ApiResponse<List<EnergyConsumptionResponseDto>>.SuccessResponse(consumption, "Energy consumption data retrieved successfully"));
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                return Forbid(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error retrieving energy consumption for meter {MeterSerialNo}", meterSerialNo);
//                return StatusCode(500, ApiResponse<List<EnergyConsumptionResponseDto>>.ErrorResponse("An error occurred while retrieving energy consumption"));
//            }
//        }

//        private int GetUserId()
//        {
//            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            return int.Parse(userIdClaim);
//        }
//        //public IActionResult Index()
//        //{
//        //    return View();
//        //}
//    }
//}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMeter.Models.DTOs;
using SmartMeter.Models.DTOs.EnergyConsumptionDto;
using SmartMeter.Services.Interface;

namespace SmartMeter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnergyConsumptionController : ControllerBase
    {
        private readonly IEnergyConsumptionService _energyService;
        private readonly ILogger<EnergyConsumptionController> _logger;

        public EnergyConsumptionController(IEnergyConsumptionService energyService, ILogger<EnergyConsumptionController> logger)
        {
            _energyService = energyService;
            _logger = logger;
        }

        [HttpPost("record")]
        public async Task<ActionResult<ApiResponse<bool>>> RecordConsumption([FromBody] EnergyConsumptionRecordDto record)
        {
            try
            {
                _logger.LogInformation("Recording energy consumption request for meter: {Meter}", record.MeterSerialNo);

                var result = await _energyService.RecordEnergyConsumptionAsync(record);
                if (result)
                {
                    _logger.LogInformation("Energy consumption recorded successfully for meter: {Meter}", record.MeterSerialNo);
                    return Ok(ApiResponse<bool>.SuccessResponse(true, "Energy consumption recorded successfully"));
                }
                else
                {
                    _logger.LogWarning("Failed to record energy consumption for meter: {Meter}", record.MeterSerialNo);
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to record energy consumption"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording energy consumption for meter: {Meter}", record.MeterSerialNo);
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while recording energy consumption"));
            }
        }

        //[HttpGet("consumption")]
        //public async Task<ActionResult<ApiResponse<List<EnergyConsumptionResponseDto>>>> GetConsumption(
        //    [FromQuery] string meterSerialNo,
        //    [FromQuery] string fromDate,
        //    [FromQuery] string toDate)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Getting energy consumption for meter: {Meter}, From: {FromDate}, To: {ToDate}",
        //            meterSerialNo, fromDate, toDate);

        //        var userId = GetUserId();
        //        var query = new EnergyConsumptionQueryDto
        //        {
        //            MeterSerialNo = meterSerialNo,
        //            FromDate = fromDate,
        //            ToDate = toDate
        //        };

        //        var consumption = await _energyService.GetEnergyConsumptionAsync(query, userId);

        //        _logger.LogInformation("Successfully retrieved {Count} consumption records for meter: {Meter}",
        //            consumption.Count, meterSerialNo);

        //        return Ok(ApiResponse<List<EnergyConsumptionResponseDto>>.SuccessResponse(consumption, "Energy consumption data retrieved successfully"));
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        _logger.LogWarning("Unauthorized access attempt for meter: {Meter}, User: {UserId}",
        //            meterSerialNo, GetUserId());
        //        return Unauthorized(ApiResponse<List<EnergyConsumptionResponseDto>>.ErrorResponse(ex.Message));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error retrieving energy consumption for meter {MeterSerialNo}", meterSerialNo);
        //        return StatusCode(500, ApiResponse<List<EnergyConsumptionResponseDto>>.ErrorResponse("An error occurred while retrieving energy consumption"));
        //    }
        //}

        [HttpGet("total")]
        public async Task<ActionResult<ApiResponse<decimal>>> GetTotalConsumption(
            [FromQuery] string meterSerialNo,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Getting total consumption for meter: {Meter}, From: {FromDate}, To: {ToDate}",
                    meterSerialNo, fromDate, toDate);

                var userId = GetUserId();
                var total = await _energyService.GetTotalConsumptionAsync(meterSerialNo, fromDate, toDate, userId);

                _logger.LogInformation("Total consumption for meter {Meter}: {Total}kWh", meterSerialNo, total);

                return Ok(ApiResponse<decimal>.SuccessResponse(total, $"Total consumption: {total} kWh"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized access attempt for meter: {Meter}, User: {UserId}",
                    meterSerialNo, GetUserId());
                return Unauthorized(ApiResponse<decimal>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving total consumption for meter {MeterSerialNo}", meterSerialNo);
                return StatusCode(500, ApiResponse<decimal>.ErrorResponse("An error occurred while retrieving total consumption"));
            }
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return int.Parse(userIdClaim);
        }
    }
}