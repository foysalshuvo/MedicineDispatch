using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Drones;
using System.Net;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MedicineDispatch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DronesController : ControllerBase
    {

        private readonly ILogger<DronesController> _logger;

        private readonly IDroneService _iDroneService;
        private readonly IDispatchMedicineService _iDispatchMedicineService;

        public DronesController(IDroneService iDroneService, IDispatchMedicineService iDispatchMedicineService, ILogger<DronesController> logger)
        {
            _iDroneService = iDroneService;
            _iDispatchMedicineService = iDispatchMedicineService;
            _logger = logger;
        }


        [HttpPost("Add")]
        public ApiResponse Add([FromBody] Drone drone)
        {
            try
            {
                if (drone == null) 
                {
                    return new ApiResponse(Convert.ToInt16(HttpStatusCode.BadRequest));
                }

                if (ModelState.IsValid)
                { 
                    var _vResult = _iDroneService.Create(drone);
                    var _vJSONResult = JsonSerializer.Serialize(_vResult);
                    string _vCombindedString = string.Join(",", _vResult);
                    _logger.LogInformation("Response msg: " + _vCombindedString);
                    return new ApiResponse("Request Successful", _vResult, 200, "1.0.0.0");
                }
                else
                {
                    var _errorMessage = string.Join(" | ", ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage));
                  
                    _logger.LogError(""+ _errorMessage + "");
                    return new ApiResponse(Convert.ToInt16(HttpStatusCode.BadRequest), _errorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new ApiResponse(Convert.ToInt16(HttpStatusCode.BadRequest),ex.Message);
            }
        }

        [HttpPost("Dispatch")]
        public ApiResponse Dispatch([FromBody] DispatchMedicine dispatchMedicine)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var _vResult = _iDispatchMedicineService.DispatchMedicine(dispatchMedicine);
                    var _vJSONResult = JsonSerializer.Serialize(_vResult);
                    string _vCombindedString = string.Join(",", _vResult);
                    _logger.LogInformation("Response msg: " + _vCombindedString);
                    return new ApiResponse("Request Successful", _vResult, 200, "1.0.0.0");
                }
                else
                {
                    var _errorMessage = string.Join(" | ", ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage));

                    _logger.LogError("" + _errorMessage + "");
                    return new ApiResponse(Convert.ToInt16(HttpStatusCode.BadRequest), _errorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new ApiResponse(ex.Message);
            }
        }

    }
}
