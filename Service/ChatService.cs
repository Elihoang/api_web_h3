using API_WebH3.DTO.Chat;
using API_WebH3.DTO.Message;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class ChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;
    
    public ChatService(IChatRepository chatRepository, IUserRepository userRepository)
    {
        _chatRepository = chatRepository;
        _userRepository = userRepository;
    }
    
    public async Task<IEnumerable<ChatDto>> GetAllAsync()
    {
        var chats = await _chatRepository.GetAllAsync();
        return chats.Select(c => new ChatDto
        {
            Id = c.Id,
            User1Id = c.User1Id,
            User2Id = c.User2Id,
            CreatedAt = c.CreatedAt,
            Messages = c.Messages?.Select(m => new MessageDto
            {
                Id = m.Id,
                ChatId = m.ChatId,
                SenderId = m.SenderId,
                Content = m.Content,
                IsRead = m.IsRead,
                SentAt = m.SentAt
            }).ToList() ?? new List<MessageDto>()
        });
    }
    public async Task<ChatDto> GetByIdAsync(Guid id)
    {
        var chat = await _chatRepository.GetByIdAsync(id);
        if (chat == null)
        {
            return null;
        }
        return new ChatDto
        {
            Id = chat.Id,
            User1Id = chat.User1Id,
            User2Id = chat.User2Id,
            CreatedAt = chat.CreatedAt,
            Messages = chat.Messages?.Select(m => new MessageDto
            {
                Id = m.Id,
                ChatId = m.ChatId,
                SenderId = m.SenderId,
                Content = m.Content,
                IsRead = m.IsRead,
                SentAt = m.SentAt
            }).ToList() ?? new List<MessageDto>()
        };
    }
    
    public async Task<IEnumerable<ChatDto>> GetByUserIdAsync(Guid userId)
    {
        var chats = await _chatRepository.GetByUserIdAsync(userId);
        return chats.Select(c => new ChatDto
        {
            Id = c.Id,
            User1Id = c.User1Id,
            User2Id = c.User2Id,
            CreatedAt = c.CreatedAt,
            Messages = c.Messages?.Select(m => new MessageDto
            {
                Id = m.Id,
                ChatId = m.ChatId,
                SenderId = m.SenderId,
                Content = m.Content,
                IsRead = m.IsRead,
                SentAt = m.SentAt
            }).ToList() ?? new List<MessageDto>()
        });
    }
    
    public async Task<ChatDto> CreateAsync(CreateChatDto createChatDto)
    {
        var user1 = await _userRepository.GetByIdAsync(createChatDto.User1Id);
        if (user1 == null)
        {
            AppLogger.LogError("User1 không tìm thấy.");
        }

        var user2 = await _userRepository.GetByIdAsync(createChatDto.User2Id);
        if (user2 == null)
        {
            AppLogger.LogError("Tài này này vô hiệu hóa");
        }

        if (createChatDto.User1Id == createChatDto.User2Id)
        {
            AppLogger.LogError("User1 và User2 phải khác nhau.");
        }

        var existingChat = await _chatRepository.GetByUsersAsync(createChatDto.User1Id, createChatDto.User2Id);
        if (existingChat != null)
        {
            AppLogger.LogError("Cuộc trò chuyện giữa những người dùng này đã tồn tại.");
        }

        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            User1Id = createChatDto.User1Id,
            User2Id = createChatDto.User2Id,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };

        await _chatRepository.AddAsync(chat);

        return new ChatDto
        {
            Id = chat.Id,
            User1Id = chat.User1Id,
            User2Id = chat.User2Id,
            CreatedAt = chat.CreatedAt,
            Messages = new List<MessageDto>()
        };
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var chat = await _chatRepository.GetByIdAsync(id);
        if (chat == null)
        {
            return false;
        }
        await _chatRepository.DeleteAsync(id);
        return true;
    }
}