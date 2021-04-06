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
    [Route("client")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public ClientController(SigrehContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }

        [HttpGet("establishment/{id}")]
        [Authorize]
        public ActionResult<List<ClientResponse>> Find(int id, [FromQuery] QueryParam filter)
        {
            var establishment = context.Establishments.Include(p => p.City).ThenInclude(p => p.Department).ThenInclude(p => p.Region).FirstOrDefault(p => p.Id == id);
            if(establishment == null) return NotFound();
            
            var res = from s in context.Clients.Include(p => p.Partners).Where(p => p.EstablishmentId == establishment.Id) select s;
            var page = new Page(filter.Index, filter.Size);
            List<int> ids = new List<int>();

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
                res = res.Where(p => keys.Contains(p.Name) || keys.Contains(p.Firstname) || keys.Contains(p.Idnumber));
            }
            
            if (User.IsInRole(Role.REH) || User.IsInRole(Role.GEH))
            {
                var user = context.Users.Include(p => p.Establishments).FirstOrDefault(p => p.Id == int.Parse(User.Identity.Name));
                if(user.Establishments == null) return NotFound();
                foreach (var ue in user.Establishments)
                {
                    if(ue.Id == establishment.Id)
                    {
                        return Ok(PaginatorService.Paginate(mapper.Map<List<REHClientResponse>>(res.ToList()), res.Count(), page));
                    }
                }
            }
            else if (User.IsInRole(Role.DDMT) || User.IsInRole(Role.PP))
            {
                var user = context.Users.Include(p => p.Departments).FirstOrDefault(p => p.Id == int.Parse(User.Identity.Name));
                if(user.Departments == null) return NotFound();
                foreach (var ud in user.Departments)
                {
                    if (ud.Id == establishment.City.DepartmentId)
                    {
                        return Ok(PaginatorService.Paginate(mapper.Map<List<DDMTClientResponse>>(res.ToList()), res.Count(), page));
                    }
                }
            }
            else if (User.IsInRole(Role.DRMT))
            {
                var user = context.Users.Include(p => p.Regions).FirstOrDefault(p => p.Id == int.Parse(User.Identity.Name));
                if(user.Regions == null) return NotFound();
                foreach (var ur in user.Regions)
                { 
                    if (ur.Id == establishment.City.Department.RegionId)
                    {
                        return Ok(PaginatorService.Paginate(mapper.Map<List<DRMTClientResponse>>(res.ToList()), res.Count(), page));
                    }
                }
            }
            else if (User.IsInRole(Role.DSMT))
            {
                return Ok(PaginatorService.Paginate(mapper.Map<List<DSMTClientResponse>>(res.ToList()), res.Count(), page));
            }
            else if (User.IsInRole(Role.SMI))
            {
                return Ok(PaginatorService.Paginate(mapper.Map<List<SMIClientResponse>>(res.ToList()), res.Count(), page));
            }
            else if (User.IsInRole(Role.ADMIN))
            {
                return Ok(PaginatorService.Paginate(mapper.Map<List<ClientResponse>>(res.ToList()), res.Count(), page));
            }
            
            return NotFound();
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<ClientResponse> FindOne(int id)
        {
            var res = context.Clients.Include(p => p.Partners).Include(p => p.Establishment).ThenInclude(p => p.City).ThenInclude(p => p.Department).ThenInclude(p => p.Region).FirstOrDefault(p => p.Id == id);
            if(res == null) return NotFound();

            if (User.IsInRole(Role.REH) || User.IsInRole(Role.GEH))
            {
                var user = context.Users.Include(p => p.Establishments).FirstOrDefault(p => p.Id == int.Parse(User.Identity.Name));
                if(user.Establishments == null) return NotFound();
                foreach (var ue in user.Establishments)
                {
                    if(ue.Id == res.EstablishmentId)
                    {
                        return Ok(mapper.Map<REHClientResponse>(res));
                    }
                }
            }
            else if (User.IsInRole(Role.DDMT) || User.IsInRole(Role.PP))
            {
                var user = context.Users.Include(p => p.Departments).FirstOrDefault(p => p.Id == int.Parse(User.Identity.Name));
                if(user.Departments == null) return NotFound();
                foreach (var ud in user.Departments)
                {
                    if (ud.Id == res.Establishment.City.DepartmentId)
                    {
                        return Ok(mapper.Map<DDMTClientResponse>(res));
                    }
                }
            }
            else if (User.IsInRole(Role.DRMT))
            {
                var user = context.Users.Include(p => p.Regions).FirstOrDefault(p => p.Id == int.Parse(User.Identity.Name));
                if(user.Regions == null) return NotFound();
                foreach (var ur in user.Regions)
                { 
                    if (ur.Id == res.Establishment.City.Department.RegionId)
                    {
                        return Ok(mapper.Map<DRMTClientResponse>(res));
                    }
                }
            }
            else if (User.IsInRole(Role.DSMT))
            {
                return Ok(mapper.Map<DSMTClientResponse>(res));
            }
            else if (User.IsInRole(Role.SMI))
            {
                return Ok(mapper.Map<SMIClientResponse>(res));
            }
            else if (User.IsInRole(Role.ADMIN))
            {
                return Ok(mapper.Map<ClientResponse>(res));
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = Role.REH)]
        public ActionResult<ClientResponse> Create(ClientCreate data)
        {
            var item = mapper.Map<Client>(data);
            if (item == null) throw new ArgumentNullException(nameof(item));
            item.CreatedAt = DateTime.UtcNow.Date;
            context.Clients.Add(item);
            context.SaveChanges();
            return Ok(mapper.Map<ClientResponse>(item));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Role.REH + "," + Role.ADMIN)]
        public ActionResult<ClientResponse> Update(int id, ClientUpdate data)
        {
            var res = context.Clients.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            mapper.Map(data, res);
            res.UpdatedAt = DateTime.UtcNow.Date;
            context.Clients.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = Role.REH + "," + Role.ADMIN)]
        public ActionResult PartialUpdate(int id, JsonPatchDocument<ClientUpdate> data)
        {
            var res = context.Clients.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            var item = mapper.Map<ClientUpdate>(res);
            data.ApplyTo(item, ModelState);
            if (!TryValidateModel(item)) return ValidationProblem(ModelState);
            mapper.Map(item, res);
            res.UpdatedAt = DateTime.UtcNow.Date;
            context.Clients.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.REH + "," + Role.ADMIN)]
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
            CreateMap<Client, REHClientResponse>();
            CreateMap<Client, PPClientResponse>();
            CreateMap<Client, DDMTClientResponse>();
            CreateMap<Client, DRMTClientResponse>();
            CreateMap<Client, DSMTClientResponse>();
            CreateMap<Client, SMIClientResponse>();
        }
    }
}
