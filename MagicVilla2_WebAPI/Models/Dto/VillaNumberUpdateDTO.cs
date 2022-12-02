using System.ComponentModel.DataAnnotations;

namespace MagicVilla2_WebAPI.Models.Dto;

public class VillaNumberUpdateDTO
{
    [Required]
    public int VillaNo { get; set; }

    [Required]
    public int VillaID { get; set; }
    public string SpecialDetails { get; set; }
}
