using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Zoo.Services.Abstractions;

namespace Zoo.FoodManagement.Controllers
{
    /// <summary>
    /// Zoo API Controller
    /// </summary>
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("[controller]")]
    public class ZooController : ControllerBase
    {
        private readonly IZooPriceService _zooPriceService;

        /// <summary>
        /// Zoo Controller constructor
        /// </summary>
        /// <param name="zooPriceService">injected zoo price calculation service</param>
        public ZooController(IZooPriceService zooPriceService)
        {
            _zooPriceService = zooPriceService;
        }


        /// <summary>
        /// Calculate total food price per day in a zoo
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Zoo
        /// 
        /// </remarks>
        /// <returns>Calculated total food cost</returns>
        /// <response code="200">Returns the calculated amount</response>
        /// <response code="400">If there is some input error</response>
        /// <response code="405">Method not allowed</response>  
        /// <response code="500">server side error</response>
        [HttpPost("", Name = nameof(TotalFoodPrice))]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TotalFoodPrice(CancellationToken ct)
        {
            var result = await _zooPriceService.CalculatePrice(ct);
            return Ok(result);
        }
    }
}
