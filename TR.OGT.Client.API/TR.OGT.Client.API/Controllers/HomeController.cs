using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TR.OGT.Client.API.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Place endpoint description here.
        /// </summary>
        /// <remarks>
        /// Here is a sample remarks placeholder.
        /// </remarks>
        /// <param name="id">The ID parameter description.</param>        
        /// <returns>Return type description.</returns>
        [HttpGet("api/home/{id}")]
        public IActionResult Get(int id)
        {
            return View();
        }

        /// <summary>
        /// Creates an etity.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /entity
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "property1": true
        ///     }
        ///
        /// </remarks>
        /// <returns>A newly created entity</returns>
        /// <response code="201">Returns the newly created entity</response>
        /// <response code="400">If the entity is null</response>            
        [HttpPost("api/home")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Create()
        {
            return View();
        }
    }
}
