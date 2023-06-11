using AutoMapper;
using SocialAPI.Data;
using SocialAPI.Models;
using SocialAPI.Models.Dto;
using SocialAPI.Repository.IRepostiory;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using System.Security.Claims;
using System.Collections;

namespace SocialAPI.Controllers.v1
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/ChatApi")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ChatController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IChatRepository _dbchat;
        private readonly IMessaggioRepository _dbmess;
        private readonly IUserRepository _dbUser;
        private readonly IMapper _mapper;
        public ChatController(IChatRepository dbchat, IUserRepository dbuser, IMessaggioRepository dbmess, IMapper mapper)
        {
            _dbchat = dbchat;
            _mapper = mapper;
            _dbUser = dbuser;
            _dbmess = dbmess;
            _response = new();
        }
        [Authorize]
        [HttpGet("GetMessaggiChat")]
        [SwaggerOperation(Summary = "API that allows to get chat messages", Description = "API that allows you to receive all messages exchanged with a user.")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetMessaggiChat(string usernameDest)
        {
            try
            {
                if (usernameDest == null)
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
                            ApplicationUser userAtt = await _dbUser.GetAsync(u => u.UserName == Username);
                            ApplicationUser userDest = await _dbUser.GetAsync(u => u.UserName == usernameDest);
                            if (userAtt == null || userDest == null)
                            {
                                return BadRequest();
                            }
                            Chat chat = await _dbchat.GetAsync(c => (c.UtenteA == userAtt.Id && c.UtenteB == userDest.Id) || (c.UtenteB == userAtt.Id && c.UtenteA == userDest.Id));
                            if (chat == null)
                            {
                                _response.Result = null;
                            }
                            else
                            {
                                IEnumerable<Messaggio> messaggi = await _dbmess.GetAllAsync(m => m.fk_chat == chat.Id);
                                List<MessaggioDTO> messaggiiDTO = new List<MessaggioDTO>();
                                foreach (var item in messaggi)
                                {
                                    bool mittente = false;
                                    if (item.fk_Mittente == userAtt.Id)
                                    {
                                        mittente = true;
                                    }
                                    messaggiiDTO.Add(new MessaggioDTO() { Id = item.Id, Contenuto = item.Contenuto, Mittente = mittente });
                                }
                                _response.Result = messaggiiDTO;
                            }
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

        [Authorize]
        [HttpGet("GetChat")]
        [SwaggerOperation(Summary = "API that allows to get chats", Description = "API that allows you to receive all the users you wrote with. ")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetChat()
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
                            ApplicationUser userAtt = await _dbUser.GetAsync(u => u.UserName == Username);
                            if (userAtt == null)
                            {
                                return BadRequest();
                            }
                            List<Chat> chat = await _dbchat.GetAllAsync(c => (c.UtenteA == userAtt.Id) || (c.UtenteB == userAtt.Id));
                            List<UsernameAndImageDTO> users = new List<UsernameAndImageDTO>();
                            if (chat == null)
                            {
                                _response.Result = null;
                            }
                            else
                            {
                                foreach (var item in chat)
                                {
                                    bool mittente = false;
                                    if (item.UtenteA != userAtt.Id)
                                    {
                                        ApplicationUser userDest = await _dbUser.GetAsync(u => u.Id == item.UtenteA);
                                        users.Add(new UsernameAndImageDTO() { UsernamePubblicante = userDest.UserName, ImmagineDiProfiloUser = userDest.ImmagineProfilo });
                                    }
                                    else
                                    {
                                        ApplicationUser userDest = await _dbUser.GetAsync(u => u.Id == item.UtenteB);
                                        users.Add(new UsernameAndImageDTO() { UsernamePubblicante = userDest.UserName, ImmagineDiProfiloUser = userDest.ImmagineProfilo });
                                    }
                                }
                                _response.Result = new ChatViewDTO() { listaChat = users, MioUsername = userAtt.UserName };
                            }
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
        [Authorize]
        [SwaggerOperation(Summary = "API that allows to create new message", Description = "API that allows you to create a new message for a specific user. You need to insert the message in the body and pass the recipient's user in the url.")]
        [HttpPost("MandaMessaggio")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Segui(string usernameDestinatario, CreateMessaggioDTO mess)
        {
            try
            {
                if (usernameDestinatario == null)
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
                            ApplicationUser userAtt = await _dbUser.GetAsync(u => u.UserName == Username);
                            ApplicationUser userDest = await _dbUser.GetAsync(u => u.UserName == usernameDestinatario);
                            if (userAtt == null || userDest == null)
                            {
                                return BadRequest();
                            }
                            Chat chat = await _dbchat.GetAsync(c => (c.UtenteA == userAtt.Id && c.UtenteB == userDest.Id) || (c.UtenteB == userAtt.Id && c.UtenteA == userDest.Id));
                            if (chat == null)
                            {
                                chat = new Chat() { UtenteA = userAtt.Id, UtenteB = userDest.Id };
                                await _dbchat.CreateAsync(chat);
                            }
                            Messaggio mex = new Messaggio() { Contenuto = mess.Contenuto, fk_chat = chat.Id, fk_Mittente = userAtt.Id };
                            await _dbmess.CreateAsync(mex);
                            return Ok(new MessaggioDTO() { Id = mex.Id, Contenuto = mex.Contenuto, Mittente = true });

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
        [SwaggerOperation(Summary = "API that allows to delete a message", Description = "API that allows you to delete a message by passing it the id. You must be the sender of the message.")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("EliminaMessaggio")]
        public async Task<ActionResult<APIResponse>> EliminaMessaggio(int id)
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
                            ApplicationUser userAtt = await _dbUser.GetAsync(u => u.UserName == Username);
                            if (userAtt == null)
                            {
                                return BadRequest();
                            }
                            Messaggio mess = await _dbmess.GetAsync(m=>m.Id==id && m.fk_Mittente==userAtt.Id);
                            if (userAtt == null)
                            {
                                return NotFound();
                            }
                            await _dbmess.RemoveAsync(mess);
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
    }
}
