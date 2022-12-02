using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla2_WebAPI.Models;

public class VillaNumber
{

    // p' q sea primary-key
    // DatabaseGenerated.. para q no se genere sola en la db
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int VillaNo { get; set; }
    public string SpecialDetails { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    // VillaID es una foreign key, q va a apuntar a una tabla q se llama Villa
    // se ponen las 2 lineas juntas, la de abajo es la mencion a la tabla, asi se 
    // deja expresado que VillaID es una foreign-key a la tabla Villa
    // el normal con MAGIC STRING "[ForeignKey("VillaID"))]" no avisa si hay
    // error xq cambie el nombre o algo asi, xeso se ocupa mejor este
    //[ForeignKey(nameof(VillaID))]
    //public int VillaID { get; set; }
    //public Villa Villa { get; set; }
}
