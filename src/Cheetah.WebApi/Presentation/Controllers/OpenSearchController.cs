using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenSearch.Client;

namespace Cheetah.WebApi.Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class OpenSearchController : ControllerBase
    {
        private readonly IOpenSearchClient _opensearch;

        public OpenSearchController(IOpenSearchClient nestClient)
        {
            _opensearch = nestClient;
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
            var result = await _opensearch.Indices.GetAsync(new GetIndexRequest(Indices.All));
            var indicies = result
                .Indices.Select(index => index.Key.ToString())
                .Where(x => !x.StartsWith('.'))
                .ToList();

            return Ok(indicies);
        }
    }
}
