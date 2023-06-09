using SocialAPI.Models;
using SocialAPI.Models.Dto;
using SocialAPI.Repository.IRepostiory;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SocialAPI.Controllers
{
    [Route("api/v{version:apiVersion}/UsersAuth")]
    [ApiController]
    [ApiVersionNeutral]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly ICommentoRepostitory _commrepo;
        private readonly IChatRepository _chatrepo;
        private readonly ISeguiRepository _seguirepo;
        protected APIResponse _response;
        public UsersController(IUserRepository userRepo, ICommentoRepostitory commrepo, ISeguiRepository seguirepo, IChatRepository chatrepo)
        {
            _userRepo = userRepo;
            _response = new();
            _commrepo = commrepo;
            _chatrepo = chatrepo;
            _seguirepo = seguirepo;
        }
        [Authorize]
        [HttpGet("GetUtenteByToken")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetUtenteByToken()
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
                            ApplicationUser user = await _userRepo.GetAsync(s => s.UserName == Username);
                            if (user == null)
                            {
                                return NotFound();
                            }
                            _response.Result = new UserDTO() { ID = user.Id, Name = user.Name, UserName = user.UserName, ImmagineProfilo = user.ImmagineProfilo, dataDiNascita = user.dataDiNascita };
                            _response.StatusCode = HttpStatusCode.OK;
                            return Ok(_response);
                        }
                    }
                }
                return NotFound();

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpGet("GetUtenteByUser")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetUtenteByUser(string username)
        {
            try
            {
                ApplicationUser user = await _userRepo.GetAsync(s => s.UserName == username);
                if (user == null)
                {
                    return NotFound();
                }
                _response.Result = new UserDTO() { ID = user.Id, Name = user.Name, UserName = user.UserName, ImmagineProfilo = user.ImmagineProfilo, dataDiNascita = user.dataDiNascita };
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
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
        [HttpGet("GetUtenteBySearch")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetUtenteBySearch(string search)
        {
            try
            {
                List<ApplicationUser> user = await _userRepo.GetAllAsync();
                if (user == null)
                {
                    return NotFound();
                }
                List<string> utentiNome = new List<string>();
                foreach (var item in user)
                {
                    utentiNome.Add(item.UserName);
                }
                utentiNome = utentiNome.OrderBy(each => Compute(each, search)).Where(u=>u.ToLower().Contains(search.ToLower())).ToList();
                List<UsernameAndImageDTO> usl = new List<UsernameAndImageDTO>();
                foreach (var item in utentiNome)
                {
                    ApplicationUser ut = await _userRepo.GetAsync(u => u.UserName == item);
                    usl.Add(new UsernameAndImageDTO() { UsernamePubblicante = ut.UserName, ImmagineDiProfiloUser = ut.ImmagineProfilo });
                }

                _response.Result = usl.Take(6);
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
        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteUserFromIdAdmin")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> DeleteUserFromIdAdmin(string id)
        {
            try
            {
                ApplicationUser user = await _userRepo.GetAsync(s => s.Id == id);
                if (user == null)
                {
                    return NotFound();
                }
                List<Commento> comms = await _commrepo.GetAllAsync(c => c.fk_user == user.Id);
                foreach (var item in comms)
                {
                    await _commrepo.RemoveAsync(item);
                }
                List<Segui> seguis = await _seguirepo.GetAllAsync(c => c.Follower == user.Id || c.Seguito == user.Id);
                foreach (var item in seguis)
                {
                    await _seguirepo.RemoveAsync(item);
                }
                List<Chat> chats = await _chatrepo.GetAllAsync(c => c.UtenteA == user.Id || c.UtenteB == user.Id);
                foreach (var item in chats)
                {
                    await _chatrepo.RemoveAsync(item);
                }

                await _userRepo.RemoveAsync(user);
                _response.StatusCode = HttpStatusCode.OK;
                return NoContent();

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
        [HttpDelete("DeleteUserFromToken")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> DeleteUserFromToken()
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
                            ApplicationUser user = await _userRepo.GetAsync(s => s.UserName == Username);
                            if (user == null)
                            {
                                return NotFound();
                            }
                            List<Commento> comms = await _commrepo.GetAllAsync(c => c.fk_user == user.Id);
                            foreach (var item in comms)
                            {
                                await _commrepo.RemoveAsync(item);
                            }
                            List<Segui> seguis = await _seguirepo.GetAllAsync(c => c.Follower == user.Id || c.Seguito == user.Id);
                            foreach (var item in seguis)
                            {
                                await _seguirepo.RemoveAsync(item);
                            }
                            List<Chat> chats = await _chatrepo.GetAllAsync(c => c.UtenteA == user.Id || c.UtenteB == user.Id);
                            foreach (var item in chats)
                            {
                                await _chatrepo.RemoveAsync(item);
                            }

                            await _userRepo.RemoveAsync(user);
                            _response.StatusCode = HttpStatusCode.OK;
                            return NoContent();
                        }
                    }
                }
                return NotFound();

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _userRepo.Login(model);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username or password is incorrect");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
        {
            bool ifUserNameUnique = _userRepo.IsUniqueUser(model.UserName);
            bool ifEmailNameUnique = _userRepo.IsUniqueUser(model.Email);
            if (!ifUserNameUnique || !ifEmailNameUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username already exists");
                return BadRequest(_response);
            }

            var user = await _userRepo.Register(model);
            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while registering");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }
    }
}
