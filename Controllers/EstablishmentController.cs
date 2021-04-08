using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using sigreh.Data;
using sigreh.Dtos;
using sigreh.Models;
using sigreh.Services;
using sigreh.Wrappers;

namespace sigreh.Controllers
{
    [Route("establishment")]
    [ApiController]
    public class EstablishmentController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public EstablishmentController(SigrehContext _context, IMapper _mapper)
        {
            mapper = _mapper;
            context = _context;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<List<EstablishmentResponse>> Find([FromQuery] QueryParam filter)
        {
            var res = from s in context.Establishments.Include(p => p.City) select s;
            var page = new Page(filter.Index, filter.Size);

            res = res.Skip((page.Index - 1) * page.Size).Take(page.Size);

            if (filter.Sort == "asc")
            {
                res = res.OrderBy(p => p.Name);
            }
            else
            {
                res = res.OrderByDescending(p => p.Name);
            }

            if (filter.Search != null)
            {
                string[] keys = filter.Search.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                res = res.Where(p => keys.Contains(p.Name));
            }

            return Ok(PaginatorService.Paginate(mapper.Map<List<EstablishmentResponse>>(res.ToList()), res.Count(), page));
        }

        [HttpGet("all")]
        [Authorize]
        public ActionResult<List<EstablishmentResponse>> Find([FromQuery] string sort)
        {
            var res = from s in context.Establishments select s;

            if (sort == "asc")
            {
                res = res.OrderBy(p => p.Name);
            }
            else
            {
                res = res.OrderByDescending(p => p.Name);
            }

            return Ok(mapper.Map<List<EstablishmentResponse>>(res.ToList()));
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<EstablishmentResponse> FindOne(int id)
        {
            var res = context.Establishments.Include(p => p.City).FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            return Ok(mapper.Map<EstablishmentResponse>(res));
        }

        [HttpGet("city/{id}")]
        public ActionResult<EstablishmentResponse> FindByCity(int id)
        {
            var res = context.Establishments.Where(p => p.CityId == id).OrderBy(p => p.Name).ToList();
            if (res == null) return NotFound();
            return Ok(mapper.Map<List<EstablishmentResponse>>(res));
        }

        [HttpPost]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult<EstablishmentResponse> Create(EstablishmentCreate data)
        {
            var item = mapper.Map<Establishment>(data);
            if (item == null) throw new ArgumentNullException(nameof(item));
            context.Establishments.Add(item);
            context.SaveChanges();
            return Ok(mapper.Map<EstablishmentResponse>(item));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult<EstablishmentResponse> Update(int id, EstablishmentUpdate data)
        {
            var res = context.Establishments.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            mapper.Map(data, res);
            context.Establishments.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult PartialUpdate(int id, JsonPatchDocument<EstablishmentUpdate> data)
        {
            var res = context.Establishments.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            var item = mapper.Map<EstablishmentUpdate>(res);
            data.ApplyTo(item, ModelState);
            if (!TryValidateModel(item)) return ValidationProblem(ModelState);
            mapper.Map(item, res);
            context.Establishments.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult Delete(int id)
        {
            var res = context.Establishments.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            context.Establishments.Remove(res);
            context.SaveChanges();
            return NoContent();
        }
    }

    public class EstablishmentProfile : Profile
    {
        public EstablishmentProfile()
        {
            CreateMap<Establishment, EstablishmentResponse>(); CreateMap<EstablishmentCreate, Establishment>();
            CreateMap<EstablishmentUpdate, Establishment>(); CreateMap<Establishment, EstablishmentUpdate>();
        }
    }
}
