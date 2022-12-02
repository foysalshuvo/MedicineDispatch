using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Drones;
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

        public DronesController(IDroneService iDroneService, ILogger<DronesController> logger)
        {
            _iDroneService = iDroneService;
            _logger = logger;
        }


        [HttpPost("Add")]
        public ApiResponse Add([FromBody] Drone drone)
        {
            try
            {

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
                    _logger.LogError("Model state is not Valid");
                    return new ApiResponse("Model state is not Valid");
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
