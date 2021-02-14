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
    [Route("department")]
    [ApiController]
    [Authorize(Roles = Role.ADMIN)]
    public class DepartmentController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public DepartmentController(SigrehContext _context, IMapper _mapper) { 
            mapper = _mapper; 
            context = _context; 
        }

        [HttpGet]
        public ActionResult <List<DepartmentResponse>> Find([FromQuery] QueryParam filter)
        {
            var ctx = from s in context.Departments select s;
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
                return Ok(PaginatorService.Paginate(mapper.Map<List<DepartmentResponse>>(ctx.ToList()), ctx.Count(), page));
            }
            return Ok(mapper.Map<List<DepartmentResponse>>(ctx.ToList()));
        }

        [HttpGet("{id}")]
        public ActionResult<DepartmentResponse> FindOne(int id)
        {
            var res = context.Departments.FirstOrDefault(p => p.Id == id);
            if (res != null) return Ok(mapper.Map<DepartmentResponse>(res));
            return NotFound();
        }

        [HttpPost]
        public ActionResult <DepartmentResponse> Create(DepartmentCreate data)
        {
            var item = mapper.Map<Department>(data);
            if (item == null) throw new ArgumentNullException(nameof(item));
            context.Departments.Add(item);
            context.SaveChanges();
            return Ok(mapper.Map<DepartmentResponse>(item));
        }

        [HttpPut("{id}")]
        public ActionResult<DepartmentResponse> Update(int id, DepartmentUpdate data)
        {
            var res = context.Departments.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            mapper.Map(data, res);
            context.Departments.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult PartialUpdate(int id, JsonPatchDocument<DepartmentUpdate> data)
        {
            var res = context.Departments.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            var item = mapper.Map<DepartmentUpdate>(res);
            data.ApplyTo(item, ModelState);
            if(!TryValidateModel(item)) return ValidationProblem(ModelState);
            mapper.Map(item, res);
            context.Departments.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var res = context.Departments.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            context.Departments.Remove(res);
            context.SaveChanges();
            return NoContent();
        }
    }

    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, DepartmentResponse>(); CreateMap<DepartmentCreate, Department>();
            CreateMap<DepartmentUpdate, Department>(); CreateMap<Department, DepartmentUpdate>();
        }
    }
}
