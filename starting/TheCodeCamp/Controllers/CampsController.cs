using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
    [RoutePrefix("api/camps")]
    public class CampsController : ApiController
    {
        private CampContext db = new CampContext();

        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;

        public CampsController(ICampRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/Camps
        [Route("")]
        public async Task<IHttpActionResult> GetCamps(bool includeTalks = false)
        {
            try
            {
                var result = await _repository.GetAllCampsAsync(includeTalks);

                //Mapping
                var mappedResult = _mapper.Map<IEnumerable<CampModel>>(result);

                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //// GET: api/Camps/5
        //[ResponseType(typeof(Camp))]
        //[Route("{id}")]
        //public IHttpActionResult GetCamp(int id)
        //{
        //    Camp camp = db.Camps.Find(id);
        //    if (camp == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(camp);
        //}

        //GET: api/Camps/{moniker}
        [Route("{moniker}", Name = "GetCamp")]
        public async Task<IHttpActionResult> GetCampByMoniker(string moniker, bool includeTalks = false)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker, includeTalks);

                if(result == null) return NotFound();

                return Ok(_mapper.Map<CampModel>(result));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //Search by EventDate
        [Route("searchByDate/{eventDate:datetime}")]
        [HttpGet]
        public async Task<IHttpActionResult> SearchByEventDate(DateTime eventDate, bool includeTalks = false)
        {
            try
            {
                var result = _repository.GetAllCampsByEventDate(eventDate, includeTalks);

                return Ok(_mapper.Map<CampModel[]>(result));
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        // PUT: api/Camps/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCamp(int id, Camp camp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != camp.CampId)
            {
                return BadRequest();
            }

            db.Entry(camp).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CampExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Camps
        [Route()]
        public async Task<IHttpActionResult> PostCamp(CampModel campModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var camp = _mapper.Map<Camp>(campModel);
                    _repository.AddCamp(camp);
                   

                    if (await _repository.SaveChangesAsync())
                    {
                        var newModel = _mapper.Map<CampModel>(camp);

                        //var location = $"/api/camps/{newModel.Moniker}";
                        //var location = Url.Link("GetCamp", new { moniker = newModel.Moniker });

                        return CreatedAtRoute("GetCamp", new { moniker = newModel.Moniker }, newModel);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return BadRequest();

            //db.Camps.Add(camp);
            //db.SaveChanges();

            //return CreatedAtRoute("DefaultApi", new { id = camp.CampId }, camp);
        }

        // DELETE: api/Camps/5
        [ResponseType(typeof(Camp))]
        public IHttpActionResult DeleteCamp(int id)
        {
            Camp camp = db.Camps.Find(id);
            if (camp == null)
            {
                return NotFound();
            }

            db.Camps.Remove(camp);
            db.SaveChanges();

            return Ok(camp);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CampExists(int id)
        {
            return db.Camps.Count(e => e.CampId == id) > 0;
        }
    }
}