using AutoMapper;
using MagicVilla2_WebAPI.Models;
using MagicVilla2_WebAPI.Models.Dto;
using MagicVilla2_WebAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla2_WebAPI.Controllers;

[Route("api/villaNumberAPI")]
[ApiController]
public class VillaNumberAPIController : ControllerBase
{
    protected APIResponse _response;
    private readonly ILogger<VillaNumberAPIController> _logger;
    private readonly IVillaNumberRepository _dbVillaNumber;
    private readonly IMapper _mapper;
    private readonly IVillaRepository _dbVilla;

    public VillaNumberAPIController(ILogger<VillaNumberAPIController> logger,
                              IVillaNumberRepository dbVillaNumber,
                              IMapper mapper,
                              IVillaRepository dbVilla)
    {
        _logger = logger;
        _dbVillaNumber = dbVillaNumber;
        _mapper = mapper;
        _dbVilla = dbVilla;
        this._response = new();
    }

    ///////////////////////////////////////////////
    /// ///////////////////////////////////////////////
    ///
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<APIResponse>> GetVillaNumbers()
    {
        _logger.LogInformation("Getting all villas.");

        try
        {
            var villaNumbers = await _dbVillaNumber.GetAllAsync(includeProperties: "Villa");

            _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumbers);
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
    [HttpGet("{id:int}", Name = "GetVillaNumber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
    {
        try
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNo == id);

            if (villaNumber == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            };

            _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
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
    public async Task<ActionResult<APIResponse>> CreateVillaNumber(
                                            [FromBody] VillaNumberCreateDTO createDTO)
    {
        try
        {
            var existingVillaNumber = await _dbVillaNumber
                                        .GetAsync(v => v.VillaNo == createDTO.VillaNo);

            if (existingVillaNumber != null)
            {
                ModelState.AddModelError("CustomError", "Villa number already exist.");

                return BadRequest(ModelState);
            }

            if (await _dbVilla.GetAsync(v => v.Id == createDTO.VillaID) == null)
            {
                ModelState.AddModelError("ErrorMessages", "Villa Id is invalid.");

                return BadRequest(ModelState);
            }

            if (createDTO == null) return BadRequest(createDTO);

            VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);

            await _dbVillaNumber.CreateAsync(villaNumber);

            _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
            _response.StatusCode = HttpStatusCode.Created;

            return CreatedAtRoute("GetVilla", new { id = villaNumber.VillaNo }, _response);
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
    [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
    {
        try
        {
            if (id == 0) return BadRequest();

            var villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNo == id);

            if (villaNumber == null) return NotFound();

            await _dbVillaNumber.RemoveAsync(villaNumber);

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
    [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> UpdateVilla(int id,
                                                        [FromBody] VillaNumberUpdateDTO updateDTO)
    {
        try
        {
            if (updateDTO == null || id != updateDTO.VillaNo) return BadRequest();


            if (await _dbVilla.GetAsync(v => v.Id == updateDTO.VillaID) == null)
            {
                ModelState.AddModelError("ErrorMessages", "Villa Id is invalid.");

                return BadRequest(ModelState);
            }

            VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);

            await _dbVillaNumber.UpdateAsync(model);

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

}
