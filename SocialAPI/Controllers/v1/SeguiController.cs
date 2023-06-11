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

namespace SocialAPI.Controllers.v1
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/FollowAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class SeguiController : ControllerBase
    {
        protected APIResponse _response;
        private readonly ISeguiRepository _dbSegui;
        private readonly IUserRepository _dbUser;
        private readonly IMapper _mapper;
        public SeguiController(ISeguiRepository dbSegui, IUserRepository dbuser, IMapper mapper)
        {
            _dbSegui = dbSegui;
            _mapper = mapper;
            _dbUser = dbuser;
            _response = new();
        }
        [HttpGet("GetFollower")]
        [SwaggerOperation(Summary = "API that allows you to get your followers.")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetFollowerUtente(string username)
        {
            try
            {
                ApplicationUser user = await _dbUser.GetAsync(u => u.UserName == username);
                if (user == null)
                {
                    return NotFound();
                }
                IEnumerable<Segui> seguiList;
                seguiList = await _dbSegui.GetAllAsync(u => u.Seguito == user.Id);
                List<string> ListaFollower = new List<string>();
                foreach (var item in seguiList)
                {
                    ApplicationUser userFollower = await _dbUser.GetAsync(u => u.Id == item.Follower);
                    ListaFollower.Add(userFollower.UserName);
                };
                _response.Result = (ListaFollower);
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

        [HttpGet("GetSeguiti")]
        [SwaggerOperation(Summary = "API that allows you to get everyone you follow.")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetSeguitiUtente(string username)
        {
            try
            {
                ApplicationUser user = await _dbUser.GetAsync(u => u.UserName == username);
                if (user == null)
                {
                    return NotFound();
                }
                IEnumerable<Segui> seguiList;
                seguiList = await _dbSegui.GetAllAsync(u => u.Follower == user.Id);
                List<string> ListaSeguiti = new List<string>();
                foreach (var item in seguiList)
                {
                    ApplicationUser userSeguito = await _dbUser.GetAsync(u => u.Id == item.Seguito);
                    ListaSeguiti.Add(userSeguito.UserName);
                };
                _response.Result = (ListaSeguiti);
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
        [HttpPost("SeguiUtente")]
        [SwaggerOperation(Summary = "API that allows you to follow a user.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Segui(string utenteDaSeguire)
        {
            try
            {
                if (utenteDaSeguire == null)
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
                            ApplicationUser userCheSegue = await _dbUser.GetAsync(u => u.UserName == Username);
                            ApplicationUser userSeguito = await _dbUser.GetAsync(u => u.UserName == utenteDaSeguire);
                            Segui seguito = await _dbSegui.GetAsync(s => s.Follower == userCheSegue.Id && s.Seguito == userSeguito.Id);
                            if (seguito == null)
                            {
                                Segui follow = new Segui() { Follower = userCheSegue.Id, Seguito = userSeguito.Id };
                                await _dbSegui.CreateAsync(follow);
                                _response.Result = new SeguiDTO() { FollowerNome = userCheSegue.UserName, SeguitoNome = userSeguito.Name };
                                _response.StatusCode = HttpStatusCode.Created;
                                return Ok(_response);
                            }
                            return Ok();

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
        [SwaggerOperation(Summary = "API that allows you to unfollow a user.")]
        [HttpDelete("UnfollowUtente")]
        public async Task<ActionResult<APIResponse>> DeleteSegui(string UtenteDaUnfolloware)
        {
            try
            {
                if (UtenteDaUnfolloware == null)
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
                            ApplicationUser user = await _dbUser.GetAsync(u => u.UserName == Username);
                            ApplicationUser userUn = await _dbUser.GetAsync(u => u.UserName == UtenteDaUnfolloware);
                            Segui segui = await _dbSegui.GetAsync(s => s.Follower == user.Id && s.Seguito == userUn.Id);

                            if (segui == null)
                            {
                                return NotFound();
                            }
                            await _dbSegui.RemoveAsync(segui);
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
