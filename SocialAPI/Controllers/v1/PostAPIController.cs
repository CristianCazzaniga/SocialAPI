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
using System.ComponentModel;

namespace SocialAPI.Controllers.v1
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/PostAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class PostAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IPostRepostitory _dbPost;
        private readonly ISeguiRepository _dbSegui;
        private readonly ICommentoRepostitory _dbcommenti;
        private readonly ILikeRepostitory _dblike;
        private readonly IUserRepository _dbUser;
        private readonly IMapper _mapper;
        public PostAPIController(IPostRepostitory dbPost, ISeguiRepository dbSegui, IUserRepository dbuser,ILikeRepostitory dblike, ICommentoRepostitory dbcommenti, IMapper mapper)
        {
            _dbPost = dbPost;
            _dbSegui = dbSegui;
            _dbcommenti= dbcommenti;
            _dblike = dblike;
            _mapper = mapper;
            _dbUser = dbuser;
            _response = new();
        }

        [HttpGet("GetPostUtente")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetPosts(string username)
        {
            try
            {
                ApplicationUser user = await _dbUser.GetAsync(u => u.UserName == username);
                if (user == null)
                {
                    return NotFound();
                }
                IEnumerable<Post> postList;
                postList = await _dbPost.GetAllAsync(u => u.fk_user == user.Id);
                _response.Result = _mapper.Map<List<PostDTO>>(postList);
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


        [HttpGet("GetPostUtentiSeguiti")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetPostUtenti()
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
                            IEnumerable<Segui> seguiti = await _dbSegui.GetAllAsync(s => s.Follower == user.Id);
                            List<Post> posts = new List<Post>();
                            foreach (var item in seguiti)
                            {
                                IEnumerable<Post> postUt = await _dbPost.GetAllAsync(s => s.fk_user == item.Seguito);
                                posts.AddRange(postUt);
                            }
                            _response.Result = _mapper.Map<IEnumerable<PostDTO>>(posts);
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

        [HttpGet("GetPostUtentiSeguitiInfo")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetPostUtentiSeguitiInfo()
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
                            IEnumerable<Segui> seguiti = await _dbSegui.GetAllAsync(s => s.Follower == user.Id);
                            List<Post> posts = new List<Post>();
                            foreach (var item in seguiti)
                            {
                                IEnumerable<Post> postUt = await _dbPost.GetAllAsync(s => s.fk_user == item.Seguito);
                                posts.AddRange(postUt);
                            }
                            List<PostInfoDTO> listaPostiInfo = new List<PostInfoDTO>();
                            foreach (var post in posts)
                            {
                                IEnumerable<Like> likes = await _dblike.GetAllAsync(l => l.fk_post == post.Id && l.TipoDestinazione == "post");
                                List<string> Likes = new List<string>();
                                foreach (Like lik in likes)
                                {
                                    ApplicationUser us = await _dbUser.GetAsync(u => u.Id == lik.fk_user);
                                    if (us != null)
                                    {
                                        Likes.Add(us.UserName);
                                    }
                                }
                                IEnumerable<Commento> commentiList;
                                commentiList = await _dbcommenti.GetAllAsync(u => u.fk_post == post.Id);
                                List<CommentoDTO> listaCommOut = new List<CommentoDTO>();
                                foreach (var item in commentiList)
                                {
                                    try
                                    {
                                        ApplicationUser Utente = await _dbUser.GetAsync(u => u.Id == item.fk_user);
                                        if (Utente != null)
                                        {
                                            listaCommOut.Add(new CommentoDTO() { Id = item.Id, Contenuto = item.Contenuto, Username = Utente.UserName, DataPubblicazione = item.DataPubblicazione, DataModifica = item.DataModifica });
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                                ApplicationUser utentePubb = await _dbUser.GetAsync(u => u.Id == post.fk_user);
                                listaPostiInfo.Add(new PostInfoDTO() { Id = post.Id, commenti = listaCommOut, Contenuto = post.Contenuto, DataPubblicazione = post.DataPubblicazione, likes = Likes, Media = post.Media, UsernamePubblicante = utentePubb.UserName, DataModifica = post.UpdatedDate });
                            }
                            _response.Result = listaPostiInfo;
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
        [HttpPost("CreaPost")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreatePost([FromBody] PostCreateDTO createDTO)
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
                            Post post = new Post() { Contenuto = createDTO.Contenuto, Media = createDTO.Media, fk_user = user.Id, DataPubblicazione = DateTime.Now };
                            await _dbPost.CreateAsync(post);
                            _response.Result = _mapper.Map<PostDTO>(post);
                            _response.StatusCode = HttpStatusCode.Created;
                            return Ok(_response);

                        }
                    }

                }


                if (createDTO == null)
                {
                    return BadRequest(createDTO);
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
        [HttpDelete("EliminaPost")]
        public async Task<ActionResult<APIResponse>> DeletePost(int id)
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
                            var post = await _dbPost.GetAsync(u => u.Id == id && u.fk_user == user.Id);
                            if (post == null)
                            {
                                return NotFound();
                            }
                            await _dbPost.RemoveAsync(post);
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

        [Authorize("admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("EliminaPostAdmin")]
        public async Task<ActionResult<APIResponse>> DeletePostAdmin(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var post = await _dbPost.GetAsync(u => u.Id == id);
                if (post == null)
                {
                    return NotFound();
                }
                await _dbPost.RemoveAsync(post);
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

        [Authorize]
        [HttpPut("AggiornaPost")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdatePost(int id, [FromBody] PostUpdateDTO updateDTO)
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
                            Post post = await _dbPost.GetAsync(u => u.Id == id && u.fk_user == user.Id);
                            if (post != null)
                            {
                                if (updateDTO == null)
                                {
                                    return BadRequest();
                                }
                                post.Contenuto = updateDTO.Contenuto;

                                await _dbPost.UpdateAsync(post);
                                _response.StatusCode = HttpStatusCode.NoContent;
                                _response.IsSuccess = true;
                                return Ok(_response);
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
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
    }
}
