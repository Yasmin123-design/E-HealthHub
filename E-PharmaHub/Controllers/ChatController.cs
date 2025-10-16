using E_PharmaHub.Dtos;
using E_PharmaHub.Services;
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

        [HttpPost("start")]
        public async Task<IActionResult> StartConversation([FromQuery] string pharmacistId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var thread = await _chatService.StartConversationAsync(userId, pharmacistId);
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

