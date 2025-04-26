using API_WebH3.DTO.Chat;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;


[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{ 
    private readonly ChatService _chatService;
    public ChatController(ChatService chatService)
    {
        _chatService = chatService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatDto>>> GetChats()
    {
        var chats = await _chatService.GetAllAsync();
        return Ok(chats);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<ChatDto>> GetChat(Guid id)
    {
        var chat = await _chatService.GetByIdAsync(id);
        if (chat == null)
        {
            return NotFound();
        }
        return Ok(chat);
    }
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<ChatDto>>> GetChatsByUser(Guid userId)
    {
        var chats = await _chatService.GetByUserIdAsync(userId);
        return Ok(chats);
    }
    
    [HttpPost]
    public async Task<ActionResult<ChatDto>> CreateChat(CreateChatDto createChatDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var chatDto = await _chatService.CreateAsync(createChatDto);
        return CreatedAtAction(nameof(GetChat), new { id = chatDto.Id }, chatDto);

    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChat(Guid id)
    {
        var result = await _chatService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}