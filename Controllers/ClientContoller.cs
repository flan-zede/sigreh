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

        [HttpGet]
        [Authorize]
        public ActionResult<List<ClientResponse>> Find([FromQuery] QueryParam filter)
        {
            var ctx = from s in context.Clients.Include(p => p.Establishment).ThenInclude(p => p.City).ThenInclude(p => p.Department).ThenInclude(p => p.Region).Include(p => p.User).Include(p => p.Partners) select s;
            var page = new Page(filter.Index, filter.Size);
            List<int> ids = new List<int>();

            if (filter.Search != null)
            {
                string[] keys = filter.Search.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                ctx = ctx.Where(p => keys.Contains(p.Name) || keys.Contains(p.Firstname) || keys.Contains(p.Idnumber));
            }
            if (filter.Sort == "asc") ctx = ctx.OrderBy(p => p.Id); else ctx = ctx.OrderByDescending(p => p.Id);
            if (filter.Index > 0) ctx = ctx.Skip((page.Index - 1) * page.Size).Take(page.Size);

            if (User.IsInRole(Role.REH) || User.IsInRole(Role.GEH))
            {
                var user = context.Users.Include(p => p.Establishments).FirstOrDefault(p => p.Id == int.Parse(User.Identity.Name));
                if(user.Establishments != null) foreach (var ue in user.Establishments) ids.Add(ue.Id);
                ctx = ctx.Where(p => ids.Contains(p.EstablishmentId));
                if (filter.Index > 0) return Ok(PaginatorService.Paginate(mapper.Map<List<REHClientResponse>>(ctx.ToList()), ctx.Count(), page));
                return Ok(mapper.Map<List<REHClientResponse>>(ctx.ToList()));
            }
            else if (User.IsInRole(Role.DDMT) || User.IsInRole(Role.PP))
            {
                var user = context.Users.Include(p => p.Departments).ThenInclude(p => p.Cities).ThenInclude(p => p.Establishments).FirstOrDefault(p => p.Id == int.Parse(User.Identity.Name));
                if(user.Departments != null) foreach (var ud in user.Departments) { 
                    foreach (var c in ud.Cities) { 
                        foreach (var e in c.Establishments) { 
                            ids.Add(e.Id); 
                        } 
                    }  
                }
                ctx = ctx.Where(p => ids.Contains(p.EstablishmentId));
                if (filter.Index > 0) return Ok(PaginatorService.Paginate(mapper.Map<List<DDMTClientResponse>>(ctx.ToList()), ctx.Count(), page));
                return Ok(mapper.Map<List<DDMTClientResponse>>(ctx.ToList()));
            }
            else if (User.IsInRole(Role.DRMT))
            {
                var user = context.Users.Include(p => p.Regions).ThenInclude(p => p.Departments).ThenInclude(p => p.Cities).ThenInclude(p => p.Establishments).FirstOrDefault(p => p.Id == int.Parse(User.Identity.Name));
                if(user.Regions != null) foreach (var ur in user.Regions) { 
                    foreach (var d in ur.Departments) { 
                        foreach (var cities in d.Cities) { 
                            foreach (var e in cities.Establishments) { 
                                ids.Add(e.Id); 
                            } 
                        }  
                    }
                }
                ctx = ctx.Where(p => ids.Contains(p.EstablishmentId));
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
            var ctx = from s in context.Clients.Include(p => p.Establishment).ThenInclude(p => p.City).ThenInclude(p => p.Department).ThenInclude(p => p.Region).Include(p => p.User).Include(p => p.Partners) select s;
            List<int> ids = new List<int>();

            if (User.IsInRole(Role.REH) || User.IsInRole(Role.GEH))
            {
                var user = context.Users.Include(p => p.Establishments).FirstOrDefault(p => p.Id == int.Parse(User.Identity.Name));
                if(user.Establishments != null) foreach (var ue in user.Establishments) ids.Add(ue.Id);
                var res = ctx.Where(p => ids.Contains(p.EstablishmentId)).FirstOrDefault(p => p.Id == id);
                if(res == null) return NotFound();
                return Ok(mapper.Map<REHClientResponse>(res));
            }
            else if (User.IsInRole(Role.DDMT) || User.IsInRole(Role.PP))
            {
                var user = context.Users.Include(p => p.Departments).ThenInclude(p => p.Cities).ThenInclude(p => p.Establishments).FirstOrDefault(p => p.Id == int.Parse(User.Identity.Name));
                if(user.Departments != null) foreach (var ud in user.Departments) { 
                    foreach (var c in ud.Cities) { 
                        foreach (var e in c.Establishments) { 
                            ids.Add(e.Id); 
                        } 
                    }  
                }
                var res = ctx.Where(p => ids.Contains(p.EstablishmentId)).FirstOrDefault(p => p.Id == id);
                if(res == null) return NotFound();
                return Ok(mapper.Map<DDMTClientResponse>(res));
            }
            else if (User.IsInRole(Role.DRMT))
            {
                var user = context.Users.Include(p => p.Regions).ThenInclude(p => p.Departments).ThenInclude(p => p.Cities).ThenInclude(p => p.Establishments).FirstOrDefault(p => p.Id == int.Parse(User.Identity.Name));
                if(user.Regions != null) foreach (var ur in user.Regions) { 
                    foreach (var d in ur.Departments) { 
                        foreach (var cities in d.Cities) { 
                            foreach (var e in cities.Establishments) { 
                                ids.Add(e.Id); 
                            } 
                        }  
                    }
                }
                var res = ctx.Where(p => ids.Contains(p.EstablishmentId)).FirstOrDefault(p => p.Id == id);
                if(res == null) return NotFound();
                return Ok(mapper.Map<DRMTClientResponse>(res));
            }
            else {
                var res = ctx.FirstOrDefault(p => p.Id == id);
                if(res == null) return NotFound();
                if (User.IsInRole(Role.DSMT)) return Ok(mapper.Map<DSMTClientResponse>(res));
                else if (User.IsInRole(Role.SMI)) return Ok(mapper.Map<SMIClientResponse>(res));
                else return Ok(mapper.Map<ClientResponse>(res));
            }
            
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
