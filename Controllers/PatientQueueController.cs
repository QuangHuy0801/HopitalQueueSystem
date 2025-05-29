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
    public class PatientQueueController : ControllerBase
    {
        private readonly PatientQueueRepository _repository;

        public PatientQueueController(PatientQueueRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("take-number")]
        public async Task<IActionResult> TakeNumber([FromBody] PatientQueueRequestDto dto)
        {
            var (newQueueNumber, patientId, roomId) = await _repository.TakeQueueNumberAsync(dto);
            return Ok(new
            {
                QueueNumber = newQueueNumber,
                PatientId = patientId,
                RoomId=roomId
            });
        }

    }
}