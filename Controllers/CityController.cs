using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Route("city")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public CityController(SigrehContext _context, IMapper _mapper) { 
            mapper = _mapper; 
            context = _context; 
        }

        [HttpGet]
        public ActionResult<List<CityResponse>> Find([FromQuery] QueryParam filter)
        {
            var ctx = from s in context.Cities.Include(p => p.Department).ThenInclude(p => p.Region) select s;
            if (filter.Sort == "asc") ctx = ctx.OrderBy(p => p.Name); else ctx = ctx.OrderByDescending(p => p.Name);
            if (filter.Search != null)
            {
                string[] keys = filter.Search.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                ctx = ctx.Where(p => keys.Contains(p.Name));
            }
            if (filter.Index > 0)
            {
                var page = new Page(filter.Index, filter.Size);
                ctx = ctx.Skip((page.Index - 1) * page.Size).Take(page.Size);
                return Ok(PaginatorService.Paginate(mapper.Map<List<CityResponse>>(ctx.ToList()), ctx.Count(), page));
            }
            return Ok(mapper.Map<List<CityResponse>>(ctx.ToList()));
        }

        [HttpGet("{id}")]
        public ActionResult<CityResponse> FindOne(int id)
        {
            var res = context.Cities.Include(p => p.Department).ThenInclude(p => p.Region).FirstOrDefault(p => p.Id == id);
            if (res != null) return Ok(mapper.Map<CityResponse>(res));
            return NotFound();
        }

        [HttpGet("multiple/{ids}")]
        public ActionResult<CityResponse> FindMultiple(string ids)
        {
            int[] intIds = Array.ConvertAll(ids.Split(",", StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
            var res = context.Cities.Where(p => intIds.Contains(p.Id)).Include(p => p.Department).ThenInclude(p => p.Region).ToList();
            if (res != null) return Ok(mapper.Map<CityResponse>(res));
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult<CityResponse> Create(CityCreate data)
        {
            var item = mapper.Map<City>(data);
            if (item == null) throw new ArgumentNullException(nameof(item));
            context.Cities.Add(item);
            context.SaveChanges();
            return Ok(mapper.Map<CityResponse>(item));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult<CityResponse> Update(int id, CityUpdate data)
        {
            var res = context.Cities.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            mapper.Map(data, res);
            context.Cities.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult PartialUpdate(int id, JsonPatchDocument<CityUpdate> data)
        {
            var res = context.Cities.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            var item = mapper.Map<CityUpdate>(res);
            data.ApplyTo(item, ModelState);
            if (!TryValidateModel(item)) return ValidationProblem(ModelState);
            mapper.Map(item, res);
            context.Cities.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult Delete(int id)
        {
            var res = context.Cities.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            context.Cities.Remove(res);
            context.SaveChanges();
            return NoContent();
        }
    }

    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<City, CityResponse>(); CreateMap<CityCreate, City>();
            CreateMap<CityUpdate, City>(); CreateMap<City, CityUpdate>();
        }
    }
}
