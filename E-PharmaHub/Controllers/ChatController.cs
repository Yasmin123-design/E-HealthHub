using E_PharmaHub.Dtos;
using E_PharmaHub.Services.ChatServ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("start-with-pharmacist")]
        public async Task<IActionResult> StartConversation([FromQuery] int pharmacistId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var thread = await _chatService.StartConversationWithPharmacistAsync(userId, pharmacistId);
            return Ok(thread);
        }

        [HttpPost("start-with-doctor")]
        public async Task<IActionResult> StartConversationWithDoctor([FromQuery] int doctorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var thread = await _chatService.StartConversationWithDoctorAsync(userId, doctorId);
            return Ok(thread);
        }


        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var message = await _chatService.SendMessageAsync(dto.ThreadId, senderId, dto.Text);
            return Ok(message);
        }

        [HttpGet("{threadId}/messages")]
        public async Task<IActionResult> GetMessages(int threadId)
        {
            var messages = await _chatService.GetMessagesAsync(threadId);
            return Ok(messages);
        }

        [HttpGet("my-threads")]
        public async Task<IActionResult> GetMyThreads()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var threads = await _chatService.GetUserThreadsAsync(userId);
            return Ok(threads);
        }
    }
}

