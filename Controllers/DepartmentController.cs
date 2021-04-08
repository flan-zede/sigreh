using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("department")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public DepartmentController(SigrehContext _context, IMapper _mapper)
        {
            mapper = _mapper;
            context = _context;
        }

        [HttpGet]
        public ActionResult<List<DepartmentResponse>> Find([FromQuery] QueryParam filter)
        {
            var res = from s in context.Departments.Include(p => p.Region) select s;
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

            return Ok(PaginatorService.Paginate(mapper.Map<List<DepartmentResponse>>(res.ToList()), res.Count(), page));
        }

        [HttpGet("all")]
        public ActionResult<List<DepartmentResponse>> Find([FromQuery] string sort)
        {
            var res = from s in context.Departments select s;

            if (sort == "asc")
            {
                res = res.OrderBy(p => p.Name);
            }
            else
            {
                res = res.OrderByDescending(p => p.Name);
            }

            return Ok(mapper.Map<List<DepartmentResponse>>(res.ToList()));
        }

        [HttpGet("{id}")]
        public ActionResult<DepartmentResponse> FindOne(int id)
        {
            var res = context.Departments.Include(p => p.Region).Include(p => p.Cities).FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            return Ok(mapper.Map<DepartmentResponse>(res));
        }

        [HttpGet("region/{id}")]
        public ActionResult<DepartmentResponse> FindByRegion(int id)
        {
            var res = context.Departments.Where(p => p.RegionId == id).OrderBy(p => p.Name).ToList();
            if (res == null) return NotFound();
            return Ok(mapper.Map<List<DepartmentResponse>>(res));
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
