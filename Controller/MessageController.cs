using API_WebH3.DTO.Message;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly MessageService _messageService;
    public MessageController(MessageService messageService)
    {
        _messageService = messageService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages()
    {
        var messages = await _messageService.GetAllAsync();
        return Ok(messages);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<MessageDto>> GetMessage(Guid id)
    {
        var message = await _messageService.GetByIdAsync(id);
        if (message == null)
        {
            return NotFound();
        }
        return Ok(message);
    }
    
    [HttpGet("chat/{chatId}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesByChat(Guid chatId)
    {
        var messages = await _messageService.GetByChatIdAsync(chatId);
        return Ok(messages);
    }
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var messageDto = await _messageService.CreateAsync(createMessageDto);
        return CreatedAtAction(nameof(GetMessage), new { id = messageDto.Id }, messageDto);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<MessageDto>> UpdateMessage(Guid id, UpdateMessageDto updateMessageDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var messageDto = await _messageService.UpdateAsync(id, updateMessageDto);
        if (messageDto == null)
        {
            return NotFound();
        }
        return Ok(messageDto);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessage(Guid id)
    {
        var result = await _messageService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}