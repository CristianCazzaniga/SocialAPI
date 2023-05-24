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
using Microsoft.Extensions.Hosting;

namespace SocialAPI.Controllers.v1
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/LikeAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class LikeController : ControllerBase
    {
        protected APIResponse _response;
        private readonly ILikeRepostitory _dbLike;
        private readonly IPostRepostitory _dbPost;
        private readonly ICommentoRepostitory _dbCommento;
        private readonly IStoriaRepostitory _dbStoria;
        private readonly IUserRepository _dbUser;
        private readonly IMapper _mapper;
        public LikeController(ILikeRepostitory dbLike, IPostRepostitory dbPost, ICommentoRepostitory dbCommento, IStoriaRepostitory dbStoria, IUserRepository dbuser, IMapper mapper)
        {
            _dbLike = dbLike;
            _dbPost = dbPost;
            _dbCommento = dbCommento;
            _dbStoria= dbStoria;
            _mapper = mapper;
            _dbUser = dbuser;
            _response = new();
        }
        [HttpGet("GetLikeStoria")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetLikeStoria(int id)
        {
            try
            {
                IEnumerable<Like> likes = await _dbLike.GetAllAsync(l => l.fk_storia == id && l.TipoDestinazione=="storia");
                List<string> Usernames = new List<string>();
                foreach (Like lik in likes)
                {
                    ApplicationUser user = await _dbUser.GetAsync(u => u.Id == lik.fk_user);
                    if (user !=null)
                    {
                        Usernames.Add(user.UserName);
                    }
                }
                _response.Result = (Usernames);
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
        [HttpGet("GetLikePost")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetLikePost(int id)
        {
            try
            {
                IEnumerable<Like> likes = await _dbLike.GetAllAsync(l => l.fk_post == id && l.TipoDestinazione == "post");
                List<string> Usernames = new List<string>();
                foreach (Like lik in likes)
                {
                    ApplicationUser user = await _dbUser.GetAsync(u => u.Id == lik.fk_user);
                    if (user != null)
                    {
                        Usernames.Add(user.UserName);
                    }
                }
                _response.Result = (Usernames);
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
        [HttpGet("GetLikeCommenti")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetLikeCommenti(int id)
        {
            try
            {
                IEnumerable<Like> likes = await _dbLike.GetAllAsync(l => l.fk_commento == id && l.TipoDestinazione == "commento");
                List<string> Usernames = new List<string>();
                foreach (Like lik in likes)
                {
                    ApplicationUser user = await _dbUser.GetAsync(u => u.Id == lik.fk_user);
                    if (user != null)
                    {
                        Usernames.Add(user.UserName);
                    }
                }
                _response.Result = (Usernames);
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
        [HttpPost("LikeStoria")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> LikeStoria(int idStoria)
        {
            try
            {
                if (idStoria == null)
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
                            if (user ==null)
                            {
                                return BadRequest();
                            }
                            Like likes = await _dbLike.GetAsync(p => p.fk_storia == idStoria && p.TipoDestinazione == "storia" && p.fk_user == user.Id);
                            if (likes != null)
                            {
                                return Ok();
                            }
                            Like like = new Like() { fk_storia= idStoria, TipoDestinazione="storia", fk_user=user.Id };
                                await _dbLike.CreateAsync(like);
                                _response.Result = new LikeDTO(){Id=like.Id, Username=user.UserName, TipoDestinazione="storia", IdFk=idStoria};
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
        [HttpPost("LikePost")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> LikePost(int idPost)
        {
            try
            {
                if (idPost == null)
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
                            if (user == null)
                            {
                                return BadRequest();
                            }
                            Like likes = await _dbLike.GetAsync(p=>p.fk_post == idPost && p.TipoDestinazione == "post" && p.fk_user == user.Id);
                            if (likes!=null)
                            {
                                return Ok();
                            }
                            Like like = new Like() { fk_post = idPost, TipoDestinazione = "post", fk_user = user.Id };
                            await _dbLike.CreateAsync(like);
                            _response.Result = new LikeDTO() { Id = like.Id, Username = user.UserName, TipoDestinazione = "post", IdFk = idPost };
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
        [HttpPost("LikeCommento")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> LikeCommento(int idCommento)
        {
            try
            {
                if (idCommento == null)
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
                            if (user == null)
                            {
                                return BadRequest();
                            }
                            Like likes = await _dbLike.GetAsync(p => p.fk_commento == idCommento && p.TipoDestinazione == "commento" && p.fk_user == user.Id);
                            if (likes != null)
                            {
                                return Ok();
                            }
                            Like like = new Like() { fk_commento = idCommento, TipoDestinazione = "commento", fk_user = user.Id };
                            await _dbLike.CreateAsync(like);
                            _response.Result = new LikeDTO() { Id = like.Id, Username = user.UserName, TipoDestinazione = "commento", IdFk = idCommento };
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
        [HttpDelete("DeleteLikeStoria")]
        public async Task<ActionResult<APIResponse>> DeleteLikeStoria(int idStoria)
        {
            try
            {
                if (idStoria == null)
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
                            if (user == null)
                            {
                                return BadRequest();
                            }
                            Like likes = await _dbLike.GetAsync(p => p.fk_storia == idStoria && p.TipoDestinazione == "storia" && p.fk_user == user.Id);
                            if (likes == null)
                            {
                                return NotFound();
                            }
                            await _dbLike.RemoveAsync(likes);
                            _response.StatusCode = HttpStatusCode.NoContent;
                            _response.IsSuccess = true;
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
        [HttpDelete("DeleteLikePost")]
        public async Task<ActionResult<APIResponse>> DeleteLikePost(int idPost)
        {
            try
            {
                if (idPost == null)
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
                            if (user == null)
                            {
                                return BadRequest();
                            }
                            Like likes = await _dbLike.GetAsync(p => p.fk_post == idPost && p.TipoDestinazione == "post" && p.fk_user == user.Id);
                            if (likes == null)
                            {
                                return NotFound();
                            }
                            await _dbLike.RemoveAsync(likes);
                            _response.StatusCode = HttpStatusCode.NoContent;
                            _response.IsSuccess = true;
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
        [HttpDelete("DeleteLikeCommento")]
        public async Task<ActionResult<APIResponse>> DeleteLikeCommento(int idCommento)
        {
            try
            {
                if (idCommento == null)
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
                            if (user == null)
                            {
                                return BadRequest();
                            }
                            Like likes = await _dbLike.GetAsync(p => p.fk_commento == idCommento && p.TipoDestinazione == "commento" && p.fk_user == user.Id);
                            if (likes == null)
                            {
                                return NotFound();
                            }
                            await _dbLike.RemoveAsync(likes);
                            _response.StatusCode = HttpStatusCode.NoContent;
                            _response.IsSuccess = true;
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
    }
}
