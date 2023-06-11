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
using System.Collections;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace SocialAPI.Controllers.v1
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/FollowAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class SegnalazioneController : ControllerBase
    {
        protected APIResponse _response;
        private readonly ISegnalazioneRepository _dbSegnala;
        private readonly IUserRepository _dbUser;
        private readonly IMapper _mapper;
        public SegnalazioneController(ISegnalazioneRepository dbSegnala, IUserRepository dbuser, IMapper mapper)
        {
            _dbSegnala = dbSegnala;
            _mapper = mapper;
            _dbUser = dbuser;
            _response = new();
        }
        [Authorize(Roles = "admin")]
        [HttpGet("GetReportUtenteAdmin")]
        [SwaggerOperation(Summary = "API that allows you to get all the reports referring to a user.", Description="Only admin")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetReportUtenteAdmin(string username)
        {
            try
            {
                ApplicationUser user = await _dbUser.GetAsync(u => u.UserName == username);
                if (user == null)
                {
                    return NotFound();
                }
                IEnumerable<Segnalazione> segnalazioneList;
                segnalazioneList = await _dbSegnala.GetAllAsync(u => u.fk_UtenteSegnalato == user.Id);
                List<SegnalazioneDTO> listaSegnalazioni = new List<SegnalazioneDTO>();
                foreach (var item in segnalazioneList)
                {
                    ApplicationUser userSegnalante = await _dbUser.GetAsync(u => u.Id == item.fk_UtenteRichiedente);
                    listaSegnalazioni.Add(new SegnalazioneDTO() { Id=item.Id, Motivazione=item.Motivazione, utenteRichiedente= userSegnalante.UserName, utenteSegnalato=user.UserName});
                };
                _response.Result = (listaSegnalazioni);
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
        [HttpPost("SegnalaUtente")]
        [SwaggerOperation(Summary = "API that allows you to report a user.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> SegnalaUtente(string utenteDaSegnalare, CreateSegnalazioneDTO segnalazioneDTO)
        {
            try
            {
                if (utenteDaSegnalare == null)
                {
                    return BadRequest();
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
                            ApplicationUser userCheSegnala = await _dbUser.GetAsync(u => u.UserName == Username);
                            ApplicationUser userdaSegnalare = await _dbUser.GetAsync(u => u.UserName == utenteDaSegnalare);
                            Segnalazione segnalato = await _dbSegnala.GetAsync(s => s.fk_UtenteRichiedente == userCheSegnala.Id && s.fk_UtenteSegnalato == userdaSegnalare.Id);
                            if (segnalato == null)
                            {
                                Segnalazione segnalazione = new Segnalazione() { fk_UtenteRichiedente= userCheSegnala.Id, fk_UtenteSegnalato=userdaSegnalare.Id, Motivazione= segnalazioneDTO.Motivazione };
                                await _dbSegnala.CreateAsync(segnalazione);
                                _response.Result = new SegnalazioneDTO() { Id= segnalazione.Id, Motivazione=segnalazione.Motivazione, utenteRichiedente= userCheSegnala.UserName, utenteSegnalato= userdaSegnalare.UserName};
                                _response.StatusCode = HttpStatusCode.Created;
                                return Ok(_response);
                            }
                            else
                            {
                                return Ok("segnalazione già mandata");
                            }
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

        [Authorize(Roles ="admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "API that allows you to delete a report reffering to a user.", Description ="Only admin")]
        [HttpDelete("ElimnaSegnalazioneAdmin")]
        public async Task<ActionResult<APIResponse>> ElimnaSegnalazioneAdmin(int id)
        {
            try
            {
                Segnalazione segnalazione = await _dbSegnala.GetAsync(s=>s.Id==id);
                if (segnalazione==null)
                {
                    return NotFound();
                }
                await _dbSegnala.RemoveAsync(segnalazione);
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
