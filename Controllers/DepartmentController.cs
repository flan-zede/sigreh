using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using sigreh.Data;
using sigreh.Dtos;
using sigreh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigreh.Wrappers;
using sigreh.Services;
using Microsoft.EntityFrameworkCore;

namespace sigreh.Controllers
{
    [Route("department")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public DepartmentController(SigrehContext _context, IMapper _mapper) { 
            mapper = _mapper; 
            context = _context; 
        }

        [HttpGet]
        public ActionResult<List<DepartmentResponse>> Find([FromQuery] QueryParam filter)
        {
            var ctx = from s in context.Departments.Include(p => p.Region) select s;
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
                return Ok(PaginatorService.Paginate(mapper.Map<List<DepartmentResponse>>(ctx.ToList()), ctx.Count(), page));
            }
            return Ok(mapper.Map<List<DepartmentResponse>>(ctx.ToList()));
        }

        [HttpGet("{id}")]
        public ActionResult<DepartmentResponse> FindOne(int id)
        {
            var res = context.Departments.Include(p => p.Region).FirstOrDefault(p => p.Id == id);
            if (res != null) return Ok(mapper.Map<DepartmentResponse>(res));
            return NotFound();
        }

        [HttpGet("multiple/{ids}")]
        public ActionResult<DepartmentResponse> FindMultiple(string ids)
        {
            int[] intIds = Array.ConvertAll(ids.Split(",", StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
            var res = context.Departments.Where(p => intIds.Contains(p.Id)).Include(p => p.Region).ToList();
            if (res != null) return Ok(mapper.Map<DepartmentResponse>(res));
            return NotFound();
        }

    }

    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, DepartmentResponse>();
        }
    }
}
