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
    [Route("subprefecture")]
    [ApiController]
    [Authorize(Roles = Role.ADMIN)]
    public class SubprefectureController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public SubprefectureController(SigrehContext _context, IMapper _mapper) 
        { 
            mapper = _mapper; 
            context = _context; 
        }

        [HttpGet]
        public ActionResult <List<SubprefectureResponse>> Find([FromQuery] QueryParam filter)
        {
            var ctx = from s in context.Subprefectures.Include(p => p.Department).Include(p => p.Cities) select s;
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
                return Ok(PaginatorService.Paginate(mapper.Map<List<SubprefectureResponse>>(ctx.ToList()), ctx.Count(), page));
            }
            return Ok(mapper.Map<List<SubprefectureResponse>>(ctx.ToList()));
        }

        [HttpGet("{id}")]
        public ActionResult<SubprefectureResponse> FindOne(int id)
        {
            var res = context.Subprefectures.Include(p => p.Department).Include(p => p.Cities).FirstOrDefault(p => p.Id == id);
            if (res != null) return Ok(mapper.Map<SubprefectureResponse>(res));
            return NotFound();
        }

        [HttpGet("multiple/{ids}")]
        public ActionResult<SubprefectureResponse> FindMultiple(string ids)
        {
            int[] intIds = Array.ConvertAll(ids.Split(",", StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
            var res = context.Subprefectures.Where(p => intIds.Contains(p.Id)).Include(p => p.Department).ToList();
            return Ok(mapper.Map<List<SubprefectureResponse>>(res));
        }

        [HttpPost]
        public ActionResult <SubprefectureResponse> Create(SubprefectureCreate data)
        {
            var item = mapper.Map<Subprefecture>(data);
            if (item == null) throw new ArgumentNullException(nameof(item));
            context.Subprefectures.Add(item);
            context.SaveChanges();
            return Ok(mapper.Map<SubprefectureResponse>(item));
        }

        [HttpPut("{id}")]
        public ActionResult<SubprefectureResponse> Update(int id, SubprefectureUpdate data)
        {
            var res = context.Subprefectures.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            mapper.Map(data, res);
            context.Subprefectures.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult PartialUpdate(int id, JsonPatchDocument<SubprefectureUpdate> data)
        {
            var res = context.Subprefectures.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            var item = mapper.Map<SubprefectureUpdate>(res);
            data.ApplyTo(item, ModelState);
            if(!TryValidateModel(item)) return ValidationProblem(ModelState);
            mapper.Map(item, res);
            context.Subprefectures.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var res = context.Subprefectures.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            context.Subprefectures.Remove(res);
            context.SaveChanges();
            return NoContent();
        }
    }

    public class SubprefectureProfile : Profile
    {
        public SubprefectureProfile()
        {
            CreateMap<Subprefecture, SubprefectureResponse>(); CreateMap<SubprefectureCreate, Subprefecture>();
            CreateMap<SubprefectureUpdate, Subprefecture>(); CreateMap<Subprefecture, SubprefectureUpdate>();
        }
    }
}
