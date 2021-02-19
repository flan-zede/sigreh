using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using sigreh.Data;
using sigreh.Dtos;
using sigreh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigreh.Wrappers;
using sigreh.Services;
using Microsoft.EntityFrameworkCore;

namespace sigreh.Controllers
{
    [Route("partner")]
    [ApiController]
    [Authorize(Roles = Role.ADMIN)]
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
