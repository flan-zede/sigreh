using sigreh.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace sigreh.Dtos
{
    public class CityCreate
    {
        [Required]
        public string Name { get; set; }

        public int DepartmentId { get; set; }
    }

    public class CityResponse : CityCreate
    {
        public int Id { get; set; }

        public Department Department { get; set; }

        public IList<Establishment> Establishments { get; set; }
    }

    public class CityUpdate : CityCreate
    {
    }
    
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<City, CityResponse>();
            CreateMap<CityCreate, City>();
            CreateMap<CityUpdate, City>();
            CreateMap<City, CityUpdate>();
        }
    }
}
