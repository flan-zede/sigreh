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
    [Route("notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public NotificationController(SigrehContext _context, IMapper _mapper)
        {
            mapper = _mapper;
            context = _context;
        }

        [HttpGet]
        public ActionResult<List<NotificationResponse>> Find()
        {
            var res = context.Notifications.OrderByDescending(p => p.Id).Where(p => p.Viewed == false).ToList();
            foreach (var item in res)
            {
                item.Viewed = true;
            }
            context.SaveChanges();
            return Ok(mapper.Map<List<NotificationResponse>>(res.ToList()));
        }

        [HttpGet("count")]
        public ActionResult<List<NotificationResponse>> Count()
        {
            var res = from s in context.Notifications.OrderByDescending(p => p.Id).Where(p => p.Viewed == false) select s;
            return Ok(res.Count());
        }

        [HttpDelete()]
        [Authorize(Roles = Role.ADMIN)]
        public ActionResult Delete()
        {
            
            return NoContent();
        }
    }

}
