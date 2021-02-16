using sigreh.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Dtos
{
    public class CityCreate
    {
        [Required]
        public string Name { get; set; }

        public int SubprefectureId { get; set; }

        public Boolean DistrictCapital { get; set; }

        public Boolean RegionCapital { get; set; }

        public Boolean DepartmentCapital { get; set; }

        public Boolean SubprefectureCapital { get; set; }
    }

    public class CityResponse : CityCreate
    {
        public int Id { get; set; }

        public Subprefecture Subprefecture { get; set; }
        
        public ICollection<Establishment> Establishments { get; set; }
    }

    public class CityUpdate : CityCreate
    {
    }
}
