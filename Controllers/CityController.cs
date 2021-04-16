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
    [Route("city")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public CityController(SigrehContext _context, IMapper _mapper)
        {
            mapper = _mapper;
            context = _context;
        }

        [HttpGet]
        public ActionResult<List<CityResponse>> Find([FromQuery] QueryParam filter)
        {
            var res = from s in context.Cities.Include(p => p.Department) select s;
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

            return Ok(PaginatorService.Paginate(mapper.Map<List<CityResponse>>(res.ToList()), res.Count(), page));
        }

        [HttpGet("all")]
        public ActionResult<List<CityResponse>> Find([FromQuery] string sort)
        {
            var res = from s in context.Cities select s;

            if (sort == "asc")
            {
                res = res.OrderBy(p => p.Name);
            }
            else
            {
                res = res.OrderByDescending(p => p.Name);
            }

            return Ok(mapper.Map<List<CityResponse>>(res.ToList()));
        }

        [HttpGet("{id}")]
        public ActionResult<CityResponse> FindOne(int id)
        {
            var res = context.Cities.Include(p => p.Department).Include(p => p.Establishments).FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            return Ok(mapper.Map<CityResponse>(res));
        }

        [HttpGet("department/{id}")]
        public ActionResult<CityResponse> FindByDepartment(int id)
        {
            var res = context.Cities.Where(p => p.DepartmentId == id).OrderBy(p => p.Name).ToList();
            if (res == null) return NotFound();
            return Ok(mapper.Map<List<CityResponse>>(res));
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

}
