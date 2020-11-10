using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFaceApi.Controllers
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class ExampleController : ControllerBase
	{
		private readonly ILogger<ExampleController> _logger;
		public ExampleController(ILogger<ExampleController> logger)
		{
			_logger = logger;
			_logger.LogInformation("Example controller created");
		}
		/// <summary>
		/// Creates a TodoItem.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		///     POST /Todo
		///     {
		///        "id": 1,
		///        "name": "Item1",
		///        "isComplete": true
		///     }
		///
		/// </remarks>
		/// <param name="item"></param>
		/// <returns>A newly created TodoItem</returns>
		/// <response code="201">Returns the newly created item</response>
		/// <response code="400">If the item is null</response>    
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/<ValuesController>/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/<ValuesController>
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/<ValuesController>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<ValuesController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
