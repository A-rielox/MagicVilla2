using AutoMapper;
using MagicVilla_WebAPI.Data;
using MagicVilla2_WebAPI.Models;
using MagicVilla2_WebAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla2_WebAPI.Controllers;

[Route("api/villaAPI")]
[ApiController]
public class VillaAPIController : ControllerBase
{
    private readonly ILogger<VillaAPIController> _logger;
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public VillaAPIController(ILogger<VillaAPIController> logger,
                              ApplicationDbContext db,
                              IMapper mapper)
    {
        _logger = logger;
        _db = db;
        _mapper = mapper;
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
    {
        _logger.LogInformation("Getting all villas.");

        var villas = await _db.Villas.ToListAsync();

        return Ok(_mapper.Map<List<VillaDTO>>(villas));
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
    public async Task<ActionResult<VillaDTO>> GetVilla(int id)
    {
        if (id == 0) return BadRequest();

        var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

        if (villa == null) return NotFound();

        return Ok(_mapper.Map<VillaDTO>(villa));
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
    {
        var existingVilla = await _db.Villas.FirstOrDefaultAsync(v => v.Name.ToLower() == createDTO.Name.ToLower());

        if (existingVilla != null)
        {
            ModelState.AddModelError("CustomError", "Villa name already exist.");

            return BadRequest(ModelState);
        }

        if(createDTO == null) return BadRequest(createDTO);

        Villa model = _mapper.Map<Villa>(createDTO);

        await _db.Villas.AddAsync(model);
        await _db.SaveChangesAsync();

        return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
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
    public async Task<IActionResult> DeleteVilla(int id)
    {
        if (id == 0) return BadRequest();

        var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

        if(villa == null) return NotFound();

        _db.Villas.Remove(villa);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///
    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
    {
        if(updateDTO == null || id != updateDTO.Id) return BadRequest();

        Villa model = _mapper.Map<Villa>(updateDTO);

        _db.Villas.Update(model);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///

    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
    {
        if (patchDTO == null || id == 0) return BadRequest();

        var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

        VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

        if (villa == null) return NotFound();

        // los cambios q vienen en patchDTO se le aplican a villaDTO
        patchDTO.ApplyTo(villaDTO, ModelState);

        Villa model = _mapper.Map<Villa>(villaDTO);

        _db.Villas.Update(model);
        await _db.SaveChangesAsync();

        if (!ModelState.IsValid) return BadRequest(ModelState);

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
