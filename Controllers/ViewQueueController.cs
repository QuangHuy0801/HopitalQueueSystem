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
        [HttpGet("room/{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var room = await _repository.GetRoomById(id);
            if (room == null)
            {
                return NotFound();
            }
            return Ok(room);
        }
        [HttpPost("next/{id}")]
        public async Task<IActionResult> UpdateNextQueueByRoomId(int id)
        {
            var success = await _repository.UpdateNextPatientInQueueAsync(id);
            if (!success)
            {
                return NotFound("Hàng chờ rỗng");
            }

            return Ok(true);
        }
        [HttpGet("queue/{roomId}")]
        public async Task<IActionResult> GetQueueByRoomId(int roomId)
        {
            var result = await _repository.GetQueueByIdRoom(roomId);
            return Ok(result);
        }

    }
}