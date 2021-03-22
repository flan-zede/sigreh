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
    [Route("partner")]
    [ApiController]
    [Authorize]
    public class PartnerController : ControllerBase
    {
        private readonly SigrehContext context;
        private readonly IMapper mapper;

        public PartnerController(SigrehContext _context, IMapper _mapper) { 
            mapper = _mapper; 
            context = _context; 
        }

        [HttpPost]
        public ActionResult<PartnerResponse> Create(PartnerCreate data)
        {
            var item = mapper.Map<Partner>(data);
            if (item == null) throw new ArgumentNullException(nameof(item));
            context.Partners.Add(item);
            context.SaveChanges();
            return Ok(mapper.Map<PartnerResponse>(item));
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var res = context.Partners.FirstOrDefault(p => p.Id == id);
            if (res == null) return NotFound();
            context.Partners.Remove(res);
            context.SaveChanges();
            return NoContent();
        }
    }

    public class PartnerProfile : Profile
    {
        public PartnerProfile()
        {
            CreateMap<Partner, PartnerResponse>(); 
            CreateMap<PartnerCreate, Partner>();
        }
    }
}
