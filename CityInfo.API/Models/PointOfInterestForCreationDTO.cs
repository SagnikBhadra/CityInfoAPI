using System;
using System.ComponentModel.DataAnnotations;

namespace CityInfo.API
{
    public class PointOfInterestForCreationDTO
    {
        // From System.ComponentModel.DataAnnotations. Used for vadilating input.
        [Required(ErrorMessage ="You should provide a name value.")]  
        [MaxLength(50)]
         public string Name { get; set; }
        [MaxLength(200)]
         public string Description { get; set; }
    }
}
