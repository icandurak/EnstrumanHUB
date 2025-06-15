using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EnstrümanHub.Models;
using EnstrümanHub.Data;

namespace EnstrümanHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnstrumanController : ControllerBase
    {
        [HttpGet("Gitar")]
        public IActionResult GetAllGuitar()
        {
            var Gitarlar = ApplicationContext.Gitarlar;
            return Ok(Gitarlar);
        }

        [HttpGet("Gitar/{id:int}")]
        public IActionResult GetOneGuitar(int id)
        {
            var gitar = ApplicationContext
                .Gitarlar
                .Where(x => x.Id.Equals(id))
                .SingleOrDefault();

            if (gitar == null)
            {
                return NotFound();
            }
            return Ok(gitar);
        }

        [HttpGet("Bass")]
        public IActionResult GetAllBass()
        {
            var bass = ApplicationContext.BassGitarlar;
            return Ok(bass);
        }

        [HttpGet("Bass/{id:int}")]
        public IActionResult GetOneBass( int id)                
        {
            var bass = ApplicationContext
                .BassGitarlar
                .Where(x => x.Id.Equals(id))
                .SingleOrDefault();

            if (bass == null)
            {
                return NotFound();
            }

            return Ok(bass);
        }
        [HttpGet("Bateri")]
        public IActionResult GetAllDrums()
        {
            var drums = ApplicationContext.Bateriler;
            return Ok(drums);
        }

        [HttpGet("Bateri/{id:int}")]
        public IActionResult GetOneDrum( int id)                //FromRoute Route'dan verinin geleceğini gösteriyor
        {
            var drums = ApplicationContext
                .Bateriler
                .Where(x => x.Id.Equals(id))
                .SingleOrDefault();

            if (drums == null)
            {
                return NotFound();
            }

            return Ok(drums);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// HTTP POST /////////////////

        [HttpPost("Gitar")]
        public IActionResult CreateOneGuitar(Gitar Gitar)
        {
            try
            {
                if (Gitar == null)
                {
                    return BadRequest();
                }

                ApplicationContext.Gitarlar.Add(Gitar);
                return StatusCode(201, Gitar);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Bass")]
        public IActionResult CreateOneBass(BassGitar Bass)
        {
            try
            {
                if (Bass == null)
                {
                    return BadRequest();
                }

                ApplicationContext.BassGitarlar.Add(Bass);
                return StatusCode(201, Bass);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Bateri")]
        public IActionResult CreateOneDrum(Bateri bateri)
        {
            try
            {
                if (bateri == null)
                {
                    return BadRequest();
                }

                ApplicationContext.Bateriler.Add(bateri);
                return StatusCode(201, bateri);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        [HttpPut("Gitar/{id:int}")]
        public IActionResult UpdateOneGitar(int id, Gitar gitar)
        {
            var entity = ApplicationContext
                .Gitarlar
                .Find(x => x.Id.Equals(id));

            if (entity is null)
            {
                return NotFound();                          //404
            }

            // id kontrolu
            if (id != gitar.Id)
            {
                return BadRequest();                        //400
            }

            ApplicationContext.Gitarlar.Remove(entity);
            gitar.Id = entity.Id;
            ApplicationContext.Gitarlar.Add(gitar);
            return Ok(gitar);
        }

        [HttpPut("Bass/{id:int}")]
        public IActionResult UpdateOneBass(int id, [FromBody] BassGitar bass)
        {
            var entity = ApplicationContext
                .BassGitarlar
                .Find(x => x.Id.Equals(id));

            if (entity is null)
            {
                return NotFound();                          //404
            }

            // id kontrolu
            if (id != bass.Id)
            {
                return BadRequest();                        //400
            }

            ApplicationContext.BassGitarlar.Remove(entity);
            bass.Id = entity.Id;
            ApplicationContext.BassGitarlar.Add(bass);
            return Ok(bass);
        }

        [HttpPut("Bateri/{id:int}")]
        public IActionResult UpdateOneDrums(int id, [FromBody] Bateri bateri)
        {
            var entity = ApplicationContext
                .Bateriler
                .Find(x => x.Id.Equals(id));

            if (entity is null)
            {
                return NotFound();                          //404
            }

            // id kontrolu
            if (id != bateri.Id)
            {
                return BadRequest();                        //400
            }

            ApplicationContext.Bateriler.Remove(entity);
            bateri.Id = entity.Id;
            ApplicationContext.Bateriler.Add(bateri);
            return Ok(bateri);
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        [HttpDelete("Gitar")]

        public IActionResult DeleteAllGitar()
        {
            ApplicationContext.Gitarlar.Clear();
            return NoContent();                             //204
        }


        [HttpDelete("Gitar/{id:int}")]
        public IActionResult DeleteGitar(int id)
        {
            var entitiy = ApplicationContext
                .Gitarlar
                .Find(x => x.Id.Equals(id));
            if (entitiy is null)
            {
                return NotFound(new { message = $"Book with id:{id} could not found." });                          //404
            }

            ApplicationContext.Gitarlar.Remove(entitiy);
            return NoContent();                                                                                //204


        }

        [HttpDelete("Bass")]

        public IActionResult DeleteAllBass()
        {
            ApplicationContext.BassGitarlar.Clear();
            return NoContent();                             //204
        }


        [HttpDelete("Bass/{id:int}")]
        public IActionResult DeleteBass(int id)
        {
            var entitiy = ApplicationContext
                .BassGitarlar
                .Find(x => x.Id.Equals(id));
            if (entitiy is null)
            {
                return NotFound(new { message = $"Book with id:{id} could not found." });                          //404
            }

            ApplicationContext.BassGitarlar.Remove(entitiy);
            return NoContent();                                                                                //204


        }

        [HttpDelete("Bateri")]

        public IActionResult DeleteAllBateri()
        {
            ApplicationContext.Bateriler.Clear();
            return NoContent();                             //204
        }


        [HttpDelete("Bateri/{id:int}")]
        public IActionResult DeleteBateri(int id)
        {
            var entitiy = ApplicationContext
                .Bateriler
                .Find(x => x.Id.Equals(id));
            if (entitiy is null)
            {
                return NotFound(new { message = $"Book with id:{id} could not found." });                          //404
            }

            ApplicationContext.Bateriler.Remove(entitiy);
            return NoContent();                                                                                //204


        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        [HttpPatch("Gitar/{id:int}")]
        public IActionResult PartiallUpdateOneGitar(int id, [FromBody] JsonPatchDocument<Gitar> GitarPatch)
        {

            var entity = ApplicationContext.Gitarlar.Find(x => x.Id.Equals(id));
            if (entity is null)
            {
                return NotFound();
            }

            GitarPatch.ApplyToGitar(entity);
            return NoContent();
        }

        [HttpPatch("Bass/{id:int}")]
        public IActionResult PartiallUpdateOneBass(int id, [FromBody] JsonPatchDocument<BassGitar> BassPatch)
        {

            var entity = ApplicationContext.BassGitarlar.Find(x => x.Id.Equals(id));
            if (entity is null)
            {
                return NotFound();
            }

            BassPatch.ApplyToBass(entity);
            return NoContent();
        }

        [HttpPatch("Bateri/{id:int}")]
        public IActionResult PartiallUpdateOneBateri(int id, [FromBody] JsonPatchDocument<Bateri> BateriPatch)
        {

            var entity = ApplicationContext.Bateriler.Find(x => x.Id.Equals(id));
            if (entity is null)
            {
                return NotFound();
            }

            BateriPatch.ApplyToDrum(entity);
            return NoContent();
        }

    }
}
