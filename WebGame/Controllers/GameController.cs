using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebGame.Controllers
{
    [Route("api/v1/games")]
    [ApiController]
    public class GameController : ControllerBase
    {
        // GET: api/<GameController>
        [HttpGet]
        public ActionResult<string> Get()
        {
            DataLayer dl = new();
            List<Game> games = dl.GetGames();
            if (games == null)
            {
                return NotFound($"Games not found");
            }
            return Ok(games);
        }

        // GET api/<GameController>/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            DataLayer dl = new();
            Game? game= dl.GetGameById(id);
            if (game == null)
            {
                return NotFound($"Game {id} not found."); //status 404
            }

            //all is good, return the game
            return Ok(game);
        }

        // POST api/<GameController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<GameController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GameController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
