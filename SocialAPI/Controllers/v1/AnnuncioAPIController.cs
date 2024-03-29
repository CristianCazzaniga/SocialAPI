﻿using AutoMapper;
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
        [SwaggerOperation(Summary = "API that allows to get the ads", Description = "API that allows you to receive 8 random advertisements among all those present in the database.")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetAnnunci()
        {
            try
            {
                _response.Result = await _dbannuncio.GetAnnunciCasual();
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
        [SwaggerOperation(Summary = "API that allows to create the ads", Description = "API that allows you to create an advertisements inserting an object of type ad in the body.")]
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

        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "API that allows to delete the ads (only for admin)", Description = "API that allows you to delete an advertisements.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("EliminaAnnuncioAdmin")]
        public async Task<ActionResult<APIResponse>> EliminaAnnuncioAdmin(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var annuncio = await _dbannuncio.GetAsync(u => u.Id == id);
                if (annuncio == null)
                {
                    return NotFound();
                }
                await _dbannuncio.RemoveAsync(annuncio);
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
