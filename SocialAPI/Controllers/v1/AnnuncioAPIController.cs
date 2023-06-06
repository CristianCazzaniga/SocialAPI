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
    [Route("api/v{version:apiVersion}/AnnuncioAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AnnuncioAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IAnnuncioRepository _dbannuncio;
        public AnnuncioAPIController(IAnnuncioRepository dbannuncio)
        {
            _dbannuncio = dbannuncio;
            _response = new();
        }

        [HttpGet("GetAnnunci")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetAnnunci()
        {
            try
            {
                _response.Result = await _dbannuncio.GetAllAsync();
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




        [HttpPost("CreaAnnuncio")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreatePost([FromBody] Annuncio annuncio)
        {
            try
            {

                await _dbannuncio.CreateAsync(annuncio);
                _response.Result = annuncio;
                _response.StatusCode = HttpStatusCode.Created;
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
