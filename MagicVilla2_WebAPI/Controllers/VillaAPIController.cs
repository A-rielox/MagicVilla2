using MagicVilla2_WebAPI.Data;
using MagicVilla2_WebAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MagicVilla2_WebAPI.Controllers;

[Route("api/villaAPI")]
[ApiController]
public class VillaAPIController : ControllerBase
{
    private readonly ILogger<VillaAPIController> _logger;

    public VillaAPIController(ILogger<VillaAPIController> logger)
    {
        _logger = logger;
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IEnumerable<VillaDTO>> GetVilas()
    {
        _logger.LogInformation("Getting all villas.");
        return Ok(VillaStore.villaList);
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///
    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VillaDTO> GetVila(int id)
    {
        if (id == 0) return BadRequest();

        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

        if (villa == null) return NotFound();

        return Ok(villa);
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
    {
        var existingVilla = VillaStore.villaList.FirstOrDefault(v => v.Name.ToLower() == villaDTO.Name.ToLower());

        if (existingVilla != null)
        {
            ModelState.AddModelError("CustomError", "Villa name already exist.");

            return BadRequest(ModelState);
        }

        if(villaDTO == null) return BadRequest(villaDTO);

        if(villaDTO.Id > 0) return StatusCode(StatusCodes.Status500InternalServerError);

        villaDTO.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;

        VillaStore.villaList.Add(villaDTO);

        return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///
    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteVilla(int id)
    {
        if (id == 0) return BadRequest();

        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

        if(villa == null) return NotFound();

        VillaStore.villaList.Remove(villa);

        return NoContent();
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///
    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
    {
        if(villaDTO == null || id != villaDTO.Id) return BadRequest();

        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

        villa.Name = villaDTO.Name;
        villa.Sqft = villaDTO.Sqft;
        villa.Occupancy = villaDTO.Occupancy;

        return NoContent();
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///

    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
    {
        if (patchDTO == null || id == 0) return BadRequest();

        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

        if (villa == null) return NotFound();

        patchDTO.ApplyTo(villa, ModelState);

        if(!ModelState.IsValid) return BadRequest(ModelState);

        return NoContent();
    }

    // el body q voy a necesitar para hacer update al nombre  ( info https://jsonpatch.com/ )
    //[
    //{
    //    "path": "name",
    //    "op": "replace",
    //    "value": "cacuna"
    //  }
    //]


    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///




    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///






    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///
}
