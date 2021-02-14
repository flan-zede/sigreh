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
    [Route("district")]
    [ApiController]
    [Authorize(Roles = Role.ADMIN)]
    public class DistrictController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public DistrictController(SigrehContext _context, IMapper _mapper) { 
            mapper = _mapper; 
            context = _context; 
        }

        [HttpGet]
        [Authorize]
        public ActionResult <List<DistrictResponse>> Find([FromQuery] QueryParam filter)
        {
            var ctx = from s in context.Districts select s;
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
                return Ok(PaginatorService.Paginate(mapper.Map<List<DistrictResponse>>(ctx.ToList()), ctx.Count(), page));
            }
            return Ok(mapper.Map<List<DistrictResponse>>(ctx.ToList()));
        }

        [HttpGet("{id}")]
        public ActionResult<DistrictResponse> FindOne(int id)
        {
            var res = context.Districts.FirstOrDefault(p => p.Id == id);
            if (res != null) return Ok(mapper.Map<DistrictResponse>(res));
            return NotFound();
        }

        [HttpPost]
        public ActionResult <DistrictResponse> Create(DistrictCreate data)
        {
            var item = mapper.Map<District>(data);
            if (item == null) throw new ArgumentNullException(nameof(item));
            context.Districts.Add(item);
            context.SaveChanges();
            return Ok(mapper.Map<DistrictResponse>(item));
        }

        [HttpPut("{id}")]
        public ActionResult<DistrictResponse> Update(int id, DistrictUpdate data)
        {
            var res = context.Districts.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            mapper.Map(data, res);
            context.Districts.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult PartialUpdate(int id, JsonPatchDocument<DistrictUpdate> data)
        {
            var res = context.Districts.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            var item = mapper.Map<DistrictUpdate>(res);
            data.ApplyTo(item, ModelState);
            if(!TryValidateModel(item)) return ValidationProblem(ModelState);
            mapper.Map(item, res);
            context.Districts.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var res = context.Districts.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            context.Districts.Remove(res);
            context.SaveChanges();
            return NoContent();
        }
    }

    public class DistrictProfile : Profile
    {
        public DistrictProfile()
        {
            CreateMap<District, DistrictResponse>(); CreateMap<DistrictCreate, District>();
            CreateMap<DistrictUpdate, District>(); CreateMap<District, DistrictUpdate>();
        }
    }
}
