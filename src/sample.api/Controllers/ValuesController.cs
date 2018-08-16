using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace sample.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly GithubClient _client;
        public ValuesController(GithubClient githubClient)
        {
            _client = githubClient; 
        }
        // GET api/values
        [HttpGet]
        public async Task<int> Get()
        {
            return await _client.GetPageStatusCode(new Uri("https://google.com"));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
