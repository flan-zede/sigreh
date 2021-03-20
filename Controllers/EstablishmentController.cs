﻿using Microsoft.AspNetCore.Mvc;
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
    [Route("establishment")]
    [ApiController]
    public class EstablishmentController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public EstablishmentController(SigrehContext _context, IMapper _mapper) { 
            mapper = _mapper; 
            context = _context; 
        }

        [HttpGet]
        [Authorize]
        public ActionResult <List<EstablishmentResponse>> Find([FromQuery] QueryParam filter)
        {
            var ctx = from s in context.Establishments.Include(p => p.City) select s;
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
                return Ok(PaginatorService.Paginate(mapper.Map<List<EstablishmentResponse>>(ctx.ToList()), ctx.Count(), page));
            }
            return Ok(mapper.Map<List<EstablishmentResponse>>(ctx.ToList()));
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<EstablishmentResponse> FindOne(int id)
        {
            var res = context.Establishments.Include(p => p.City).FirstOrDefault(p => p.Id == id);
            if (res != null) return Ok(mapper.Map<EstablishmentResponse>(res));
            return NotFound();
        }

        [HttpGet("multiple/{ids}")]
        [Authorize]
        public ActionResult<EstablishmentResponse> FindMultiple(string ids)
        {
            int[] intIds = Array.ConvertAll(ids.Split(",", StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
            var res = context.Establishments.Where(p => intIds.Contains(p.Id)).Include(p => p.City).ToList();
            if (res != null) return Ok(mapper.Map<EstablishmentResponse>(res));
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult <EstablishmentResponse> Create(EstablishmentCreate data)
        {
            var item = mapper.Map<Establishment>(data);
            if (item == null) throw new ArgumentNullException(nameof(item));
            context.Establishments.Add(item);
            context.SaveChanges();
            return Ok(mapper.Map<EstablishmentResponse>(item));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult<EstablishmentResponse> Update(int id, EstablishmentUpdate data)
        {
            var res = context.Establishments.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            mapper.Map(data, res);
            context.Establishments.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult PartialUpdate(int id, JsonPatchDocument<EstablishmentUpdate> data)
        {
            var res = context.Establishments.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            var item = mapper.Map<EstablishmentUpdate>(res);
            data.ApplyTo(item, ModelState);
            if(!TryValidateModel(item)) return ValidationProblem(ModelState);
            mapper.Map(item, res);
            context.Establishments.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult Delete(int id)
        {
            var res = context.Establishments.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            context.Establishments.Remove(res);
            context.SaveChanges();
            return NoContent();
        }
    }

    public class EstablishmentProfile : Profile
    {
        public EstablishmentProfile()
        {
            CreateMap<Establishment, EstablishmentResponse>(); CreateMap<EstablishmentCreate, Establishment>();
            CreateMap<EstablishmentUpdate, Establishment>(); CreateMap<Establishment, EstablishmentUpdate>();
        }
    }
}
