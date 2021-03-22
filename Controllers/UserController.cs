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
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public UserController(SigrehContext _context, IMapper _mapper) 
        { 
            mapper = _mapper; 
            context = _context;
        }

        [HttpGet]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult <List<UserResponse>> Find([FromQuery] QueryParam filter)
        {
            var ctx = from s in context.Users.Include(p => p.Regions).Include(p => p.Departments).Include(p => p.Establishments) select s;
            if (filter.Search != null) {
                string[] keys = filter.Search.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                ctx = ctx.Where(p => keys.Contains(p.Name) || keys.Contains(p.Firstname) || keys.Contains(p.Email) || keys.Contains(p.Idnumber));
            }
            if (filter.Sort == "asc") ctx = ctx.OrderBy(p => p.Id); else ctx = ctx.OrderByDescending(p => p.Id);
            if (filter.Index > 0) {
                var page = new Page(filter.Index, filter.Size);
                ctx = ctx.Skip((page.Index - 1) * page.Size).Take(page.Size);
                return Ok(PaginatorService.Paginate(mapper.Map<List<UserResponse>>(ctx.ToList()), ctx.Count(), page));
            }
            return Ok(mapper.Map<List<UserResponse>>(ctx.ToList()));
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<UserResponse> FindOne(int id)
        {
            var userId = int.Parse(User.Identity.Name);
            if(id!=userId && !User.IsInRole(Role.ADMIN)) return Forbid();
            var res = context.Users.Include(p => p.Regions).Include(p => p.Departments).Include(p => p.Establishments).FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            return Ok(mapper.Map<UserResponse>(res));
        }

        [HttpGet("multiple/{ids}")]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult<UserResponse> FindMultiple(string ids)
        {
            int[] intIds = Array.ConvertAll(ids.Split(",", StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
            var res = context.Users.Where(p => intIds.Contains(p.Id)).Include(p => p.Regions).Include(p => p.Departments).Include(p => p.Establishments).ToList();
            return Ok(mapper.Map<List<UserResponse>>(res));
        }

        [HttpPost]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult<UserResponse> Create(UserCreate data)
        {
            data.Password = BCrypt.Net.BCrypt.HashPassword(data.Password);
            var item = mapper.Map<User>(data);
            item.CreatedAt = DateTime.UtcNow.Date;
            if (item == null) throw new ArgumentNullException(nameof(item));
            context.Users.Add(item);
            context.SaveChanges();
            return Ok(mapper.Map<UserResponse>(item));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult<UserResponse> Update(int id, UserUpdate data)
        {
            var res = context.Users.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            data.Password = BCrypt.Net.BCrypt.HashPassword(data.Password);
            mapper.Map(data, res);
            res.UpdatedAt = DateTime.UtcNow.Date;
            context.Users.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult PartialUpdate(int id, JsonPatchDocument<UserUpdate> data)
        {
            var res = context.Users.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            var item = mapper.Map<UserUpdate>(res);
            data.ApplyTo(item, ModelState);
            if(!TryValidateModel(item)) return ValidationProblem(ModelState);
            mapper.Map(item, res);
            res.UpdatedAt = DateTime.UtcNow.Date;
            context.Users.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult Delete(int id)
        {
            var res = context.Users.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            context.Users.Remove(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpPost, Route("login")]
        [AllowAnonymous]
        public ActionResult<AuthResponse> Login(Login data)
        {
            var item = mapper.Map<Login>(data);
            if (item == null) throw new ArgumentNullException(nameof(item));
            var res = context.Users.Include(p => p.Regions).Include(p => p.Departments).Include(p => p.Establishments).FirstOrDefault(p => p.Username == item.Username);
            if (res != null && BCrypt.Net.BCrypt.Verify(item.Password, res.Password))
            {
                var Jwt = JwtService.CreateJwt(res);
                var User = mapper.Map<UserResponse>(res);
                return Ok(new { User, Jwt });
            }
            return NotFound(new { message = "Username or password is incorrect" });
        }

        [HttpGet, Route("me")]
        [Authorize]
        public ActionResult<UserResponse> Me()
        {
            var userId = int.Parse(User.Identity.Name);
            var res = context.Users.Include(p => p.Regions).Include(p => p.Departments).Include(p => p.Establishments).FirstOrDefault(p => p.Id == userId);
            if (res != null) return Ok(mapper.Map<UserResponse>(res));
            return NotFound(new { message = "Unknowed user" });
        }

        [HttpPost(), Route("{id}/relate")]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult PostRelationship(int id, Relationship body)
        {
            var user = context.Users.Include(p => p.Regions).Include(p => p.Departments).Include(p => p.Establishments).FirstOrDefault(p => p.Id == id);
            if (user == null) return NotFound(new { message = "Unknow user" });
            switch (body.Table)
            {
                case "region":
                    {
                        var res = context.Regions.FirstOrDefault(p => p.Id == body.Id);
                        if (res == null) return NotFound(new { message = "Unknow region" });
                        if(body.Add == true) user.Regions.Add(res); else user.Regions.Remove(res);
                    }
                    break;
                case "department":
                    {
                        var res = context.Departments.FirstOrDefault(p => p.Id == body.Id);
                        if (res == null) return NotFound(new { message = "Unknow department" });
                        if(body.Add == true) user.Departments.Add(res); else user.Departments.Remove(res);
                    }
                    break;
                case "establishment":
                    {
                        var res = context.Establishments.FirstOrDefault(p => p.Id == body.Id);
                        if (res == null) return NotFound(new { message = "Unknow establishment" });
                        if(body.Add == true) user.Establishments.Add(res); else user.Establishments.Remove(res);
                    }
                    break;
                default: break;
            }
            context.SaveChanges();
            return NoContent();
        }

    }

    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResponse>(); 
            CreateMap<UserCreate, User>();
            CreateMap<UserUpdate, User>(); 
            CreateMap<User, UserUpdate>();
            CreateMap<Login, User>();
        }
    }
}
