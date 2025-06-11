using API_WebH3.DTO.Message;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class MessageService
{
    private readonly IMessageRepository _messageRepository; 
    private readonly IChatRepository _chatRepository; 
    private readonly IUserRepository _userRepository;
    
    public MessageService(IMessageRepository messageRepository, IChatRepository chatRepository, IUserRepository userRepository)
    {
        _messageRepository = messageRepository;
        _chatRepository = chatRepository;
        _userRepository = userRepository;
    }
    
    public async Task<IEnumerable<MessageDto>> GetAllAsync()
    {
        var messages = await _messageRepository.GetAllAsync();
        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            ChatId = m.ChatId,
            SenderId = m.SenderId,
            Content = m.Content,
            IsRead = m.IsRead,
            SentAt = m.SentAt
        });
    }
    public async Task<MessageDto> GetByIdAsync(Guid id)
    {
        var message = await _messageRepository.GetByIdAsync(id);
        if (message == null)
        {
            return null;
        }
        return new MessageDto
        {
            Id = message.Id,
            ChatId = message.ChatId,
            SenderId = message.SenderId,
            Content = message.Content,
            IsRead = message.IsRead,
            SentAt = message.SentAt
        };
    }
    
    public async Task<IEnumerable<MessageDto>> GetByChatIdAsync(Guid chatId)
    {
        var messages = await _messageRepository.GetByChatIdAsync(chatId);
        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            ChatId = m.ChatId,
            SenderId = m.SenderId,
            Content = m.Content,
            IsRead = m.IsRead,
            SentAt = m.SentAt
        });
    }
    public async Task<MessageDto> CreateAsync(CreateMessageDto createMessageDto)
    {
        var chat = await _chatRepository.GetByIdAsync(createMessageDto.ChatId);
        if (chat == null)
        {
            AppLogger.LogError("Chat not found.");
        }

        var sender = await _userRepository.GetByIdAsync(createMessageDto.SenderId);
        if (sender == null)
        {
            AppLogger.LogError("Sender not found.");
        }

        if (createMessageDto.SenderId != chat.User1Id && createMessageDto.SenderId != chat.User2Id)
        {
            AppLogger.LogError("Sender must be a participant in the chat.");
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = createMessageDto.ChatId,
            SenderId = createMessageDto.SenderId,
            Content = createMessageDto.Content,
            IsRead = false,
            SentAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };

        await _messageRepository.AddAsync(message);

        return new MessageDto
        {
            Id = message.Id,
            ChatId = message.ChatId,
            SenderId = message.SenderId,
            Content = message.Content,
            IsRead = message.IsRead,
            SentAt = message.SentAt
        };
    }
    public async Task<MessageDto> UpdateAsync(Guid id, UpdateMessageDto updateMessageDto)
    {
        var message = await _messageRepository.GetByIdAsync(id);
        if (message == null)
        {
            return null;
        }

        message.IsRead = updateMessageDto.IsRead;

        await _messageRepository.UpdateAsync(message);

        return new MessageDto
        {
            Id = message.Id,
            ChatId = message.ChatId,
            SenderId = message.SenderId,
            Content = message.Content,
            IsRead = message.IsRead,
            SentAt = message.SentAt
        };
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var message = await _messageRepository.GetByIdAsync(id);
        if (message == null)
        {
         return false;   
        }
        await _messageRepository.DeleteAsync(id);
        return true;
    }
}