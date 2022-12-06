using AutoMapper;
using MagicVilla2_WebAPI.Models;
using MagicVilla2_WebAPI.Models.Dto;
using MagicVilla2_WebAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace MagicVilla2_WebAPI.Controllers;

[Route("api/villaAPI")]
[ApiController]
public class VillaAPIController : ControllerBase
{
    protected APIResponse _response;
    private readonly ILogger<VillaAPIController> _logger;
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;

    public VillaAPIController(ILogger<VillaAPIController> logger,
                              IVillaRepository _dbVilla,
                              IMapper mapper)
    {
        _logger = logger;
        this._dbVilla = _dbVilla;
        _mapper = mapper;
        this._response = new();
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///
    [HttpGet]
    //[Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<APIResponse>> GetVillas([FromQuery] int? occupancy,
                                                           [FromQuery] string? search,
                                                           int pageSize = 0, int pageNumber = 1)
    {
        _logger.LogInformation("Getting all villas.");

        try
        {
            IEnumerable<Villa> villaList;

            // FILTRO POR OCCUPANCY
            if(occupancy > 0)
            {
                villaList = await _dbVilla.GetAllAsync(v => v.Occupancy == occupancy, pageSize: pageSize,
                                                                                      pageNumber: pageNumber);
            } else
            {
                villaList = await _dbVilla.GetAllAsync(pageSize: pageSize,pageNumber: pageNumber);
            }


            // SEARCH POR NOMBRE
            if (!string.IsNullOrEmpty(search))
            {
                villaList = villaList.Where(v => /*v.Amenity.ToLower().Contains(search) ||*/ 
                                                 v.Name.ToLower().Contains(search));
            }



            // para paginacion
            Pagination pagination = new() {  PageNumber = pageNumber, PageSize = pageSize };
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));



            _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///
    [HttpGet("{id:int}", Name = "GetVilla")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> GetVilla(int id)
    {
        try
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var villa = await _dbVilla.GetAsync(v => v.Id == id);

            if (villa == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            };

            _response.Result = _mapper.Map<VillaDTO>(villa);
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
    {
        try
        {
            var existingVilla = await _dbVilla
                                        .GetAsync(v => v.Name.ToLower() == createDTO.Name.ToLower());

            if (existingVilla != null)
            {
                ModelState.AddModelError("CustomError", "Villa name already exist.");

                return BadRequest(ModelState);
            }

            if (createDTO == null) return BadRequest(createDTO);

            Villa villa = _mapper.Map<Villa>(createDTO);

            await _dbVilla.CreateAsync(villa);

            _response.Result = _mapper.Map<VillaDTO>(villa);
            _response.StatusCode = HttpStatusCode.Created;

            return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///
    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [Authorize(Roles = "XD")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
    {
        try
        {
            if (id == 0) return BadRequest();

            var villa = await _dbVilla.GetAsync(v => v.Id == id);

            if (villa == null) return NotFound();

            await _dbVilla.RemoveAsync(villa);

            _response.StatusCode = HttpStatusCode.NoContent;

            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///
    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> UpdateVilla(int id,
                                                        [FromBody] VillaUpdateDTO updateDTO)
    {
        try
        {
            if (updateDTO == null || id != updateDTO.Id) return BadRequest();

            Villa model = _mapper.Map<Villa>(updateDTO);

            await _dbVilla.UpdateAsync(model);

            _response.StatusCode = HttpStatusCode.NoContent;

            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///

    //[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    //[ProducesResponseType(StatusCodes.Status204NoContent)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
    //{
    //    if (patchDTO == null || id == 0) return BadRequest();

    //    var villa = await _dbVilla.GetAsync(v => v.Id == id, tracked: false);

    //    VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

    //    if (villa == null) return NotFound();

    //    // los cambios q vienen en patchDTO se le aplican a villaDTO
    //    patchDTO.ApplyTo(villaDTO, ModelState);

    //    Villa model = _mapper.Map<Villa>(villaDTO);

    //    await _dbVilla.UpdateAsync(model);

    //    if (!ModelState.IsValid) return BadRequest(ModelState);

    //    return NoContent();
    //}

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
