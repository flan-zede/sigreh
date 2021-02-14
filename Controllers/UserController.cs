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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using sigreh.Wrappers;
using sigreh.Services;

namespace sigreh.Controllers
{
    [Route("user")]
    [ApiController]
    [Authorize(Roles = Role.ADMIN)]
    public class UserController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IConfiguration config;
        private readonly IMapper mapper;

        public UserController(SigrehContext _context, IMapper _mapper, IConfiguration _config) 
        { 
            mapper = _mapper; 
            context = _context;
            config = _config;
        }

        [HttpGet]
        public ActionResult <List<UserResponse>> Find([FromQuery] QueryParam filter)
        {
            var ctx = from s in context.Users select s;
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
            var res = context.Users.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            return Ok(mapper.Map<UserResponse>(res));
        }

        [HttpPost]
        public ActionResult <UserResponse> Create(UserCreate data)
        {
            data.Password = BCrypt.Net.BCrypt.HashPassword(data.Password);
            var item = mapper.Map<User>(data);
            if (item == null) throw new ArgumentNullException(nameof(item));
            context.Users.Add(item);
            context.SaveChanges();
            return Ok(mapper.Map<UserResponse>(item));
        }

        [HttpPut("{id}")]
        public ActionResult<UserResponse> Update(int id, UserUpdate data)
        {
            var res = context.Users.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            data.Password = BCrypt.Net.BCrypt.HashPassword(data.Password);
            mapper.Map(data, res);
            context.Users.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult PartialUpdate(int id, JsonPatchDocument<UserUpdate> data)
        {
            var res = context.Users.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            var item = mapper.Map<UserUpdate>(res);
            data.ApplyTo(item, ModelState);
            if(!TryValidateModel(item)) return ValidationProblem(ModelState);
            mapper.Map(item, res);
            context.Users.Update(res);
            context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
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
            var res = context.Users.FirstOrDefault(p => p.Username == item.Username);
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
            var res = context.Users.FirstOrDefault(p => p.Id == userId);
            if (res != null)
            {
                return Ok(mapper.Map<UserResponse>(res));
            }
            return Unauthorized();
        }

    }

    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResponse>(); CreateMap<UserCreate, User>();
            CreateMap<UserUpdate, User>(); CreateMap<User, UserUpdate>();
            CreateMap<Login, User>();
        }
    }
}
