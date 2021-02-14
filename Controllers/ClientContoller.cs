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
    [Route("client")]
    [ApiController]
    [Authorize(Roles = Role.ADMIN)]
    public class ClientController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public ClientController(SigrehContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<List<ClientResponse>> Find([FromQuery] QueryParam filter)
        {
            var user = context.Users.FirstOrDefault(p => p.Id == int.Parse(User.Identity.Name));
            var ctx = from s in context.Clients select s;
            var page = new Page(filter.Index, filter.Size);

            if (filter.Search != null)
            {
                string[] keys = filter.Search.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                ctx = ctx.Where(p => keys.Contains(p.Name) || keys.Contains(p.Firstname) || keys.Contains(p.Idnumber));
            }
            if (filter.Sort == "asc") ctx = ctx.OrderBy(p => p.Id); else ctx = ctx.OrderByDescending(p => p.Id);
            if (filter.Index > 0) ctx = ctx.Skip((page.Index - 1) * page.Size).Take(page.Size);

            if (User.IsInRole(Role.REH) || User.IsInRole(Role.GEH))
            {
                int[] ids = Array.ConvertAll(user.Establishments.Split(",", StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
                ctx = ctx.Where(p => ids.Contains(p.EstablishmentID));
                if (filter.Index > 0) return Ok(PaginatorService.Paginate(mapper.Map<List<REHClientResponse>>(ctx.ToList()), ctx.Count(), page));
                return Ok(mapper.Map<List<REHClientResponse>>(ctx.ToList()));
            }
            else if (User.IsInRole(Role.PP))
            {
                int[] ids = Array.ConvertAll(user.Subprefectures.Split(",", StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
                ctx = ctx.Where(p => ids.Contains(p.Establishment.City.SubprefectureID));
                if (filter.Index > 0) return Ok(PaginatorService.Paginate(mapper.Map<List<PPClientResponse>>(ctx.ToList()), ctx.Count(), page));
                return Ok(mapper.Map<List<PPClientResponse>>(ctx.ToList()));
            }
            else if (User.IsInRole(Role.DDMT))
            {
                int[] ids = Array.ConvertAll(user.Departments.Split(",", StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
                ctx = ctx.Where(p => ids.Contains(p.Establishment.City.Subprefecture.DepartmentID));
                if (filter.Index > 0) return Ok(PaginatorService.Paginate(mapper.Map<List<DDMTClientResponse>>(ctx.ToList()), ctx.Count(), page));
                return Ok(mapper.Map<List<DDMTClientResponse>>(ctx.ToList()));
            }
            else if (User.IsInRole(Role.DRMT))
            {
                int[] ids = Array.ConvertAll(user.Regions.Split(",", StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
                ctx = ctx.Where(p => ids.Contains(p.Establishment.City.Subprefecture.Department.RegionID));
                if (filter.Index > 0) return Ok(PaginatorService.Paginate(mapper.Map<List<DRMTClientResponse>>(ctx.ToList()), ctx.Count(), page));
                return Ok(mapper.Map<List<DRMTClientResponse>>(ctx.ToList()));
            }
            else if (User.IsInRole(Role.DSMT))
            {
                if (filter.Index > 0) return Ok(PaginatorService.Paginate(mapper.Map<List<DSMTClientResponse>>(ctx.ToList()), ctx.Count(), page));
                return Ok(mapper.Map<List<DSMTClientResponse>>(ctx.ToList()));
            }
            else if (User.IsInRole(Role.SMI))
            {
                if (filter.Index > 0) return Ok(PaginatorService.Paginate(mapper.Map<List<SMIClientResponse>>(ctx.ToList()), ctx.Count(), page));
                return Ok(mapper.Map<List<SMIClientResponse>>(ctx.ToList()));
            }
            else
            {
                if (filter.Index > 0) return Ok(PaginatorService.Paginate(mapper.Map<List<ClientResponse>>(ctx.ToList()), ctx.Count(), page));
                return Ok(mapper.Map<List<ClientResponse>>(ctx.ToList()));
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<ClientResponse> FindOne(int id)
        {
            var user = context.Users.FirstOrDefault(p => p.Id == int.Parse(User.Identity.Name));

            if (User.IsInRole(Role.REH) || User.IsInRole(Role.GEH))
            {
                int[] ids = Array.ConvertAll(user.Establishments.Split(",", StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
                var res = context.Clients.FirstOrDefault(p => p.Id == id && ids.Contains(p.EstablishmentID));
                if (res == null) return NotFound();
                return Ok(mapper.Map<List<REHClientResponse>>(res));
            }
            else if (User.IsInRole(Role.PP))
            {
                int[] ids = Array.ConvertAll(user.Subprefectures.Split(",", StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
                var res = context.Clients.FirstOrDefault(p => p.Id == id && ids.Contains(p.Establishment.City.SubprefectureID));
                if (res == null) return NotFound();
                return Ok(mapper.Map<List<PPClientResponse>>(res));
            }
            else if (User.IsInRole(Role.DDMT))
            {
                int[] ids = Array.ConvertAll(user.Departments.Split(",", StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
                var res = context.Clients.FirstOrDefault(p => p.Id == id && ids.Contains(p.Establishment.City.Subprefecture.DepartmentID));
                if (res == null) return NotFound();
                return Ok(mapper.Map<List<DDMTClientResponse>>(res));
            }
            else if (User.IsInRole(Role.DRMT))
            {
                int[] ids = Array.ConvertAll(user.Regions.Split(",", StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
                var res = context.Clients.FirstOrDefault(p => p.Id == id && ids.Contains(p.Establishment.City.Subprefecture.Department.RegionID));
                if (res == null) return NotFound();
                return Ok(mapper.Map<List<DRMTClientResponse>>(res));
            }
            else
            {
                var res = context.Clients.FirstOrDefault(p => p.Id == id);
                if (res == null) return NotFound();
                if (User.IsInRole(Role.DSMT)) return Ok(mapper.Map<List<DSMTClientResponse>>(res));
                else if (User.IsInRole(Role.SMI)) return Ok(mapper.Map<List<SMIClientResponse>>(res));
                else return Ok(mapper.Map<List<ClientResponse>>(res));
            }
        }

        [HttpPost]
        [Authorize(Roles = Role.REH)]
        public ActionResult<ClientResponse> Create(ClientCreate data)
        {
            var item = mapper.Map<Client>(data);
            if (item == null) throw new ArgumentNullException(nameof(item));
            context.Clients.Add(item);
            context.SaveChanges();
            return Ok(mapper.Map<ClientResponse>(item));
        }

        [HttpPut("{id}")]
        public ActionResult<ClientResponse> Update(int id, ClientUpdate data)
        {
            var res = context.Clients.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            mapper.Map(data, res);
            context.Clients.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult PartialUpdate(int id, JsonPatchDocument<ClientUpdate> data)
        {
            var res = context.Clients.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            var item = mapper.Map<ClientUpdate>(res);
            data.ApplyTo(item, ModelState);
            if (!TryValidateModel(item)) return ValidationProblem(ModelState);
            mapper.Map(item, res);
            context.Clients.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var res = context.Clients.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            context.Clients.Remove(res);
            context.SaveChanges();
            return NoContent();
        }
    }

    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<Client, ClientResponse>(); CreateMap<ClientCreate, Client>();
            CreateMap<ClientUpdate, Client>(); CreateMap<Client, ClientUpdate>();
        }
    }
}
