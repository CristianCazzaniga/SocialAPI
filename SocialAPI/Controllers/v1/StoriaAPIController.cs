using AutoMapper;
using SocialAPI.Data;
using SocialAPI.Models;
using SocialAPI.Models.Dto;
using SocialAPI.Repository.IRepostiory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using System.Security.Claims;

namespace SocialAPI.Controllers.v1
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/StoriaAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class StoriaApiController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IStoriaRepostitory _dbStoria;
        private readonly IUserRepository _dbUser;
        private readonly IMapper _mapper;
        public StoriaApiController(IStoriaRepostitory dbStoria, IUserRepository dbuser, IMapper mapper)
        {
            _dbStoria = dbStoria;
            _mapper = mapper;
            _dbUser = dbuser;
            _response = new();
        }

        [HttpGet("GetStorieUtente")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetStorieUtente(string username)
        {
            try
            {
                ApplicationUser user = await _dbUser.GetAsync(u => u.UserName == username);
                if (user == null)
                {
                    return NotFound();
                }
                IEnumerable<Storia> storiaList;
                storiaList = await _dbStoria.GetAllAsync(u => u.fk_user == user.Id);
                _response.Result = _mapper.Map<List<StoriaDTO>>(storiaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;

        }


        [Authorize]
        [HttpPost("CreaStoria")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateStoria([FromBody] StoriaCreateDTO createDTO)
        {
            try
            {

                if (createDTO == null)
                {
                    return BadRequest(createDTO);
                }
                var claimsIdentity = User.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    var NamesIdentifier = claimsIdentity.FindFirst(ClaimTypes.Name);
                    if (NamesIdentifier != null)
                    {
                        string Username = NamesIdentifier.Value;
                        if (Username != null)
                        {
                            ApplicationUser user = await _dbUser.GetAsync(u => u.UserName == Username);
                            Storia storia = new Storia() { Media = createDTO.Media, fk_user = user.Id, DataPubblicazione = DateTime.Now };
                            await _dbStoria.CreateAsync(storia);
                            _response.Result = _mapper.Map<StoriaDTO>(storia);
                            _response.StatusCode = HttpStatusCode.Created;
                            return Ok(_response);

                        }
                    }

                }



            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("EliminaStoria")]
        public async Task<ActionResult<APIResponse>> DeleteStoria(int id)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    var NamesIdentifier = claimsIdentity.FindFirst(ClaimTypes.Name);
                    if (NamesIdentifier != null)
                    {
                        string Username = NamesIdentifier.Value;
                        if (Username != null)
                        {
                            ApplicationUser user = await _dbUser.GetAsync(u => u.UserName == Username);
                            if (id == 0)
                            {
                                return BadRequest();
                            }
                            var storia = await _dbStoria.GetAsync(u => u.Id == id && u.fk_user == user.Id);
                            if (storia == null)
                            {
                                return NotFound();
                            }
                            await _dbStoria.RemoveAsync(storia);
                            _response.StatusCode = HttpStatusCode.NoContent;
                            _response.IsSuccess = true;
                            return Ok(_response);

                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }




    }
}
