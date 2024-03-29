﻿using AutoMapper;
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
using Swashbuckle.AspNetCore.Annotations;

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
        private readonly IMessaggioRepository _dbMessaggio;
        private readonly IUserRepository _dbUser;
        private readonly IMapper _mapper;
        public LikeController(ILikeRepostitory dbLike, IPostRepostitory dbPost, IMessaggioRepository dbMessaggio, ICommentoRepostitory dbCommento, IStoriaRepostitory dbStoria, IUserRepository dbuser, IMapper mapper)
        {
            _dbLike = dbLike;
            _dbMessaggio= dbMessaggio;
            _dbPost = dbPost;
            _dbCommento = dbCommento;
            _dbStoria= dbStoria;
            _mapper = mapper;
            _dbUser = dbuser;
            _response = new();
        }
        [HttpGet("GetLikeStoria")]
        [SwaggerOperation(Summary = "API that allows to get the likes of a story")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetLikeStoria(int id)
        {
            try
            {
                Storia storia = await _dbStoria.GetAsync(p => p.Id == id);
                if (storia == null)
                {
                    return NotFound();
                }
                IEnumerable<Like> likes = await _dbLike.GetAllAsync(l => l.fk_storia == id && l.TipoDestinazione=="storia");
                List<UsernameAndImageDTO> Usernames = new List<UsernameAndImageDTO>();
                foreach (Like lik in likes)
                {
                    ApplicationUser user = await _dbUser.GetAsync(u => u.Id == lik.fk_user);
                    if (user != null)
                    {
                        Usernames.Add(new UsernameAndImageDTO() { UsernamePubblicante = user.UserName, ImmagineDiProfiloUser = user.ImmagineProfilo });
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
        [SwaggerOperation(Summary = "API that allows to get the likes of a post")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetLikePost(int id)
        {
            try
            {
                Post post = await _dbPost.GetAsync(p=>p.Id==id);
                if (post==null)
                {
                    return NotFound();
                }
                IEnumerable<Like> likes = await _dbLike.GetAllAsync(l => l.fk_post == id && l.TipoDestinazione == "post");
                List<UsernameAndImageDTO> Usernames = new List<UsernameAndImageDTO>();
                foreach (Like lik in likes)
                {
                    ApplicationUser user = await _dbUser.GetAsync(u => u.Id == lik.fk_user);
                    if (user != null)
                    {
                        Usernames.Add(new UsernameAndImageDTO() { UsernamePubblicante = user.UserName, ImmagineDiProfiloUser = user.ImmagineProfilo });
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

        [HttpGet("GetLikeMessaggio")]
        [SwaggerOperation(Summary = "API that allows to get the likes of a message")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetLikeMessaggio(int id)
        {
            try
            {
                Post post = await _dbPost.GetAsync(p => p.Id == id);
                if (post == null)
                {
                    return NotFound();
                }
                IEnumerable<Like> likes = await _dbLike.GetAllAsync(l => l.fk_messaggio == id && l.TipoDestinazione == "messaggio");
                List<UsernameAndImageDTO> Usernames = new List<UsernameAndImageDTO>();
                foreach (Like lik in likes)
                {
                    ApplicationUser user = await _dbUser.GetAsync(u => u.Id == lik.fk_user);
                    if (user != null)
                    {
                        Usernames.Add(new UsernameAndImageDTO() { UsernamePubblicante = user.UserName, ImmagineDiProfiloUser = user.ImmagineProfilo });
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
        [SwaggerOperation(Summary = "API that allows to get the likes of a comment")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetLikeCommenti(int id)
        {
            try
            {
                Commento commento = await _dbCommento.GetAsync(p => p.Id == id);
                if (commento == null)
                {
                    return NotFound();
                }
                IEnumerable<Like> likes = await _dbLike.GetAllAsync(l => l.fk_commento == id && l.TipoDestinazione == "commento");
                List<UsernameAndImageDTO> Usernames = new List<UsernameAndImageDTO>();
                foreach (Like lik in likes)
                {
                    ApplicationUser user = await _dbUser.GetAsync(u => u.Id == lik.fk_user);
                    if (user != null)
                    {
                        Usernames.Add(new UsernameAndImageDTO() { UsernamePubblicante = user.UserName, ImmagineDiProfiloUser = user.ImmagineProfilo });
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
        [SwaggerOperation(Summary = "API that allows to create new like for a story")]
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
                Storia storia = await _dbStoria.GetAsync(m => m.Id == idStoria);
                if (storia == null)
                {
                    return NotFound();
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
        [SwaggerOperation(Summary = "API that allows to create new like for a post")]
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
                Post post = await _dbPost.GetAsync(m => m.Id == idPost);
                if (post == null)
                {
                    return NotFound();
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
        [SwaggerOperation(Summary = "API that allows to create new like for a message")]
        [HttpPost("LikeMessaggio")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> LikeMessaggio(int idMessaggio)
        {
            try
            {
                if (idMessaggio == null)
                {
                    return BadRequest();
                }
                Messaggio msg = await _dbMessaggio.GetAsync(m=>m.Id == idMessaggio);
                if (msg == null)
                {
                    return NotFound();
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
                            Like likes = await _dbLike.GetAsync(p => p.fk_messaggio == idMessaggio && p.TipoDestinazione == "messaggio" && p.fk_user == user.Id);
                            if (likes != null)
                            {
                                return Ok();
                            }
                            Like like = new Like() { fk_messaggio = idMessaggio, TipoDestinazione = "messaggio", fk_user = user.Id };
                            await _dbLike.CreateAsync(like);
                            _response.Result = new LikeDTO() { Id = like.Id, Username = user.UserName, TipoDestinazione = "messaggio", IdFk = idMessaggio };
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
        [SwaggerOperation(Summary = "API that allows to create new like for a comment")]
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
                Commento commento = await _dbCommento.GetAsync(m => m.Id == idCommento);
                if (commento == null)
                {
                    return NotFound();
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
        [SwaggerOperation(Summary = "API that allows to delete a like of a story")]
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
        [SwaggerOperation(Summary = "API that allows to delete a like of a message")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("DeleteLikeMessaggio")]
        public async Task<ActionResult<APIResponse>> DeleteLikeMessaggio(int idMessaggio)
        {
            try
            {
                if (idMessaggio == null)
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
                            Like likes = await _dbLike.GetAsync(p => p.fk_messaggio == idMessaggio && p.TipoDestinazione == "messaggio" && p.fk_user == user.Id);
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
        [SwaggerOperation(Summary = "API that allows to delete a like of a post")]
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
        [SwaggerOperation(Summary = "API that allows to delete a like of a comment")]
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
