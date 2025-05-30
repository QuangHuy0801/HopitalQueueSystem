using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViewQueueController : ControllerBase
    {
        private readonly ViewQueueRepository _repository;

        public ViewQueueController(ViewQueueRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("rooms")]
        public async Task<IActionResult> GetRoom()
        {
            var rooms = await _repository.GetRoom();
            return Ok(rooms);
        }

    }
}