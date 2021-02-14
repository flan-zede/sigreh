using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigreh.Models;
using sigreh.Data;
using AutoMapper;
using sigreh.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using sigreh.Wrappers;
using sigreh.Services;

namespace sigreh.Controllers
{
    [Route("establishment")]
    [ApiController]
    [Authorize(Roles = Role.ADMIN)]
    public class EstablishmentController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public EstablishmentController(SigrehContext _context, IMapper _mapper) { 
            mapper = _mapper; 
            context = _context; 
        }

        [HttpGet]
        public ActionResult <List<EstablishmentResponse>> Find([FromQuery] QueryParam filter)
        {
            var ctx = from s in context.Establishments select s;
            if (filter.Sort == "asc") ctx = ctx.OrderBy(p => p.Id); else ctx = ctx.OrderByDescending(p => p.Id);
            if (filter.Search != null)
            {
                string[] keys = filter.Search.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                ctx = ctx.Where(p => keys.Contains(p.Name));
            }
            if (filter.Index > 0)
            {
                var page = new Page(filter.Index, filter.Size);
                ctx = ctx.Skip((page.Index - 1) * page.Size).Take(page.Size);
                return Ok(PaginatorService.Paginate(mapper.Map<List<EstablishmentResponse>>(ctx.ToList()), ctx.Count(), page));
            }
            return Ok(mapper.Map<List<EstablishmentResponse>>(ctx.ToList()));
        }

        [HttpGet("{id}")]
        public ActionResult<EstablishmentResponse> FindOne(int id)
        {
            var res = context.Establishments.FirstOrDefault(p => p.Id == id);
            if (res != null) return Ok(mapper.Map<EstablishmentResponse>(res));
            return NotFound();
        }

        [HttpPost]
        public ActionResult <EstablishmentResponse> Create(EstablishmentCreate data)
        {
            var item = mapper.Map<Establishment>(data);
            if (item == null) throw new ArgumentNullException(nameof(item));
            context.Establishments.Add(item);
            context.SaveChanges();
            return Ok(mapper.Map<EstablishmentResponse>(item));
        }

        [HttpPut("{id}")]
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
        public ActionResult PartialUpdate(int id, JsonPatchDocument<EstablishmentUpdate> data)
        {
            var res = context.Establishments.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            var item = mapper.Map<EstablishmentUpdate>(res);
            data.ApplyTo(item, ModelState);
            if(!TryValidateModel(item)) return ValidationProblem(ModelState);
            mapper.Map(item, res);
            context.Establishments.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
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
