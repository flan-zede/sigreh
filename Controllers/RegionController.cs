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
using Microsoft.EntityFrameworkCore;

namespace sigreh.Controllers
{
    [Route("region")]
    [ApiController]
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
            var ctx = from s in context.Regions.Include(p => p.Departments) select s;
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
                return Ok(PaginatorService.Paginate(mapper.Map<List<RegionResponse>>(ctx.ToList()), ctx.Count(), page));
            }
            return Ok(mapper.Map<List<RegionResponse>>(ctx.ToList()));
        }

        [HttpGet("{id}")]
        public ActionResult<RegionResponse> FindOne(int id)
        {
            var res = context.Regions.Include(p => p.Departments).FirstOrDefault(p => p.Id == id);
            if (res != null) return Ok(mapper.Map<RegionResponse>(res));
            return NotFound();
        }

        [HttpGet("multiple/{ids}")]
        public ActionResult<RegionResponse> FindMultiple(string ids)
        {
            int[] intIds = Array.ConvertAll(ids.Split(",", StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
            var res = context.Regions.Where(p => intIds.Contains(p.Id)).Include(p => p.Departments).ToList();
            if (res != null) return Ok(mapper.Map<RegionResponse>(res));
            return NotFound();
        }

    }

    public class RegionProfile : Profile
    {
        public RegionProfile()
        {
            CreateMap<Region, RegionResponse>(); 
        }
    }
}
