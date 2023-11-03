using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cheetah.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cheetah.WebApi.Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class OpenSearchController : ControllerBase
    {
        private readonly ICheetahOpenSearchClient _opensearchNest;

        public OpenSearchController(ICheetahOpenSearchClient nestClient)
        {
            _opensearchNest = nestClient;
        }

        /// <summary>
        /// Retrieves all indices on the corresponding OpenSearch instance
        /// </summary>
        /// <returns></returns>
        [HttpGet("indices")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetIndices()
        {
            var indicies = await _opensearchNest.GetIndices();
            return Ok(indicies);
        }
    }
}