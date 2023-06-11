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
    [Route("api/v{version:apiVersion}/CommentoApi")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CommentoApiController : ControllerBase
    {
        protected APIResponse _response;
        private readonly ICommentoRepostitory _dbCommento;
        private readonly IPostRepostitory _dbPost;
        private readonly IUserRepository _dbUser;
        private readonly IMapper _mapper;
        public CommentoApiController(ICommentoRepostitory dbCommento, IPostRepostitory dbPost, IUserRepository dbuser, IMapper mapper)
        {
            _dbCommento = dbCommento;
            _dbPost = dbPost;
            _mapper = mapper;
            _dbUser = dbuser;
            _response = new();
        }

        [HttpGet("GetCommentiPost")]
        [SwaggerOperation(Summary = "API that allows to get the comments of a post", Description = "API that allows you to receive all the comments of a post passing the post id")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetCommentiPost(int id)
        {
            try
            {
                Post post = await _dbPost.GetAsync(p => p.Id == id);
                if (post == null)
                {
                    return NotFound();
                }
                IEnumerable<Commento> commentiList;
                commentiList = await _dbCommento.GetAllAsync(u => u.fk_post == id);
                List<CommentoDTO> listaCommOut = new List<CommentoDTO>();
                foreach (var item in commentiList)
                {
                    try
                    {
                        ApplicationUser Utente = await _dbUser.GetAsync(u => u.Id == item.fk_user);
                        if (Utente != null)
                        {
                            listaCommOut.Add(new CommentoDTO() {Id=item.Id, Contenuto = item.Contenuto, User = new UsernameAndImageDTO() { UsernamePubblicante = Utente.UserName, ImmagineDiProfiloUser = Utente.ImmagineProfilo }, DataPubblicazione = item.DataPubblicazione, DataModifica = item.DataModifica });
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                _response.Result = listaCommOut;
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
        [HttpPost("CreaCommento")]
        [SwaggerOperation(Summary = "API that allows to create new comment for a post", Description = "API that allows to create new comment for a post passing the post id and in the body the object.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateCommento(int idPost, [FromBody] CreateCommentoDTO createDTO)
        {
            try
            {

                if (createDTO == null || idPost == 0)
                {
                    return BadRequest(createDTO);
                }
                Post post = await _dbPost.GetAsync(p => p.Id == idPost);
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
                                return NotFound();
                            }
                            Commento commento = new Commento() { Contenuto = createDTO.Contenuto, fk_post = idPost, fk_user = user.Id, DataPubblicazione = DateTime.Now};
                            await _dbCommento.CreateAsync(commento);
                            _response.Result = new CommentoDTO() { User = new UsernameAndImageDTO() { UsernamePubblicante = user.UserName, ImmagineDiProfiloUser = user.ImmagineProfilo }, Contenuto = commento.Contenuto, DataPubblicazione = commento.DataPubblicazione, DataModifica = commento.DataModifica };
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
        [SwaggerOperation(Summary = "API that allows to delete comment of a post", Description = "API that allows to delete comment of a post passing the comment id. the comment must be yours.")]
        [HttpDelete("EliminaCommento")]
        public async Task<ActionResult<APIResponse>> DeleteCommento(int id)
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
                            var commento = await _dbCommento.GetAsync(u => u.Id == id && u.fk_user == user.Id);
                            if (commento == null)
                            {
                                return NotFound();
                            }
                            await _dbCommento.RemoveAsync(commento);
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
        [SwaggerOperation(Summary = "API that allows to delete comment of a post", Description = "API that allows to delete comment of a post passing the comment id. Only for admin")]

        [HttpDelete("EliminaCommentoAdmin")]
        public async Task<ActionResult<APIResponse>> DeleteCommentoAdmin(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var commento = await _dbCommento.GetAsync(u => u.Id == id);
                if (commento == null)
                {
                    return NotFound();
                }
                await _dbCommento.RemoveAsync(commento);
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
        [HttpPut("AggiornaCommento")]
        [SwaggerOperation(Summary = "API that allows to update comment of a post")]
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
                            Commento commento = await _dbCommento.GetAsync(u => u.Id == id && u.fk_user == user.Id);
                            if (commento != null)
                            {
                                if (updateDTO == null)
                                {
                                    return BadRequest();
                                }
                                commento.Contenuto = updateDTO.Contenuto;

                                await _dbCommento.UpdateAsync(commento);
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
