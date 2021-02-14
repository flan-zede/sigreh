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
    [Route("region")]
    [ApiController]
    [Authorize(Roles = Role.ADMIN)]
    public class RegionController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public RegionController(SigrehContext _context, IMapper _mapper) { 
            mapper = _mapper; 
            context = _context; 
        }

        [HttpGet]
        public ActionResult <List<RegionResponse>> Find([FromQuery] QueryParam filter)
        {
            var ctx = from s in context.Regions select s;
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
                return Ok(PaginatorService.Paginate(mapper.Map<List<RegionResponse>>(ctx.ToList()), ctx.Count(), page));
            }
            return Ok(mapper.Map<List<RegionResponse>>(ctx.ToList()));
        }

        [HttpGet("{id}")]
        public ActionResult<RegionResponse> FindOne(int id)
        {
            var res = context.Regions.FirstOrDefault(p => p.Id == id);
            if (res != null) return Ok(mapper.Map<RegionResponse>(res));
            return NotFound();
        }

        [HttpPost]
        public ActionResult <RegionResponse> Create(RegionCreate data)
        {
            var item = mapper.Map<Region>(data);
            if (item == null) throw new ArgumentNullException(nameof(item));
            context.Regions.Add(item);
            context.SaveChanges();
            return Ok(mapper.Map<RegionResponse>(item));
        }

        [HttpPut("{id}")]
        public ActionResult<RegionResponse> Update(int id, RegionUpdate data)
        {
            var res = context.Regions.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            mapper.Map(data, res);
            context.Regions.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult PartialUpdate(int id, JsonPatchDocument<RegionUpdate> data)
        {
            var res = context.Regions.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            var item = mapper.Map<RegionUpdate>(res);
            data.ApplyTo(item, ModelState);
            if(!TryValidateModel(item)) return ValidationProblem(ModelState);
            mapper.Map(item, res);
            context.Regions.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var res = context.Regions.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            context.Regions.Remove(res);
            context.SaveChanges();
            return NoContent();
        }
    }

    public class RegionProfile : Profile
    {
        public RegionProfile()
        {
            CreateMap<Region, RegionResponse>(); CreateMap<RegionCreate, Region>();
            CreateMap<RegionUpdate, Region>(); CreateMap<Region, RegionUpdate>();
        }
    }
}
