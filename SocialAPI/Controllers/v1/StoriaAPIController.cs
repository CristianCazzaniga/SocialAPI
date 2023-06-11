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
using Swashbuckle.AspNetCore.Annotations;

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
        private readonly ISeguiRepository _dbSegui;
        private readonly IMapper _mapper;
        public StoriaApiController(IStoriaRepostitory dbStoria, ISeguiRepository dbSegui, IUserRepository dbuser, IMapper mapper)
        {
            _dbStoria = dbStoria;
            _dbSegui=dbSegui;
            _mapper = mapper;
            _dbUser = dbuser;
            _response = new();
        }

        [HttpGet("GetStoricoStorie")]
        [SwaggerOperation(Summary = "API that allows you to take the history of your stories.")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetStoricoStorie()
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
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
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

        [HttpGet("GetStorieUtente")]
        [SwaggerOperation(Summary = "API that allows to get all stories by a user")]
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
                storiaList = await _dbStoria.GetAllAsync(u => u.fk_user == user.Id && u.DataPubblicazione > DateTime.Now.AddHours(-24));
                _response.Result = new StoriaDTORest() { User = new UsernameAndImageDTO() { UsernamePubblicante = user.UserName, ImmagineDiProfiloUser = user.ImmagineProfilo }, listaStorie = _mapper.Map<List<StoriaDTO>>(storiaList) };
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

        [HttpGet("GetStoriaById")]
        [SwaggerOperation(Summary = "API that allows to get a story from id")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetStoriaById(int Id)
        {
            try
            {
                Storia storia = await _dbStoria.GetAsync(s => s.Id == Id);
                if (storia==null)
                {
                    return NotFound();
                }
                _response.Result = _mapper.Map<StoriaDTO>(storia);
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

        [HttpGet("GetStorieUtentiSeguiti")]
        [SwaggerOperation(Summary = "API that allows to get all stories from all followed users")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetStorieUtentiSeguiti()
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
                            IEnumerable<Storia> storiaUs = await _dbStoria.GetAllAsync(s => s.fk_user == user.Id && s.DataPubblicazione > DateTime.Now.AddHours(-24));
                            List<StoriaDTORest> storie = new List<StoriaDTORest>();
                            if (storiaUs != null)
                            {
                                if (storiaUs.Count() != 0)
                                {
                                    storie.Add(new StoriaDTORest() { User = new UsernameAndImageDTO() { UsernamePubblicante = "la tua storia", ImmagineDiProfiloUser = user.ImmagineProfilo }, listaStorie = _mapper.Map<IEnumerable<StoriaDTO>>(storiaUs) });
                                }

                            }
                            IEnumerable<Segui> seguiti = await _dbSegui.GetAllAsync(s => s.Follower == user.Id);
                           
                            foreach (var item in seguiti)
                            {
                                IEnumerable<Storia> storiaUt = await _dbStoria.GetAllAsync(s => s.fk_user == item.Seguito && s.DataPubblicazione>DateTime.Now.AddHours(-24));
                               
                                if (storiaUt != null)
                                {
                                    if (storiaUt.Count()!=0)
                                    {
                                        ApplicationUser utente = await _dbUser.GetAsync(us => us.Id == item.Seguito);
                                        storie.Add(new StoriaDTORest() { User = new UsernameAndImageDTO() { UsernamePubblicante = utente.UserName, ImmagineDiProfiloUser = utente.ImmagineProfilo }, listaStorie = _mapper.Map<IEnumerable<StoriaDTO>>(storiaUt) });
                                    }
                                   
                                }
                            }
                            _response.Result = storie.AsEnumerable();
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
        [HttpPost("CreaStoria")]
        [SwaggerOperation(Summary = "API that allows to create new story")]
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
        [SwaggerOperation(Summary = "API that allows to delete a story", Description = "must be your story")]
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
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "API that allows to delete a story", Description = "Only admin")]
        [HttpDelete("EliminaStoriaAdmin")]
        public async Task<ActionResult<APIResponse>> DeleteStoriaAdmin(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var storia = await _dbStoria.GetAsync(u => u.Id == id);
                if (storia == null)
                {
                    return NotFound();
                }
                await _dbStoria.RemoveAsync(storia);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
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



    }
}
