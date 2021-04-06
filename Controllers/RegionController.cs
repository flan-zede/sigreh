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
            var res = from s in context.Regions select s;
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
            
            return Ok(PaginatorService.Paginate(mapper.Map<List<RegionResponse>>(res.ToList()), res.Count(), page));
        }

        [HttpGet("all")]
        public ActionResult <List<RegionResponse>> FindAll(string sort)
        {
            var res = from s in context.Regions select s;
            
            if (sort == "asc") 
            {
                res = res.OrderBy(p => p.Name);
            }
            else
            {
                res = res.OrderByDescending(p => p.Name);
            }

            return Ok(mapper.Map<List<RegionResponse>>(res.ToList()));
        }

        [HttpGet("{id}")]
        public ActionResult<RegionResponse> FindOne(int id)
        {
            var res = context.Regions.Include(p => p.Departments).FirstOrDefault(p => p.Id == id);
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
