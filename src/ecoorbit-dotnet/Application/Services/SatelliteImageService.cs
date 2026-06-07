using ecoorbit_dotnet.Application.DTOs.SatelliteImage;
using ecoorbit_dotnet.Application.Interfaces;
using ecoorbit_dotnet.Domain.Entities;
using ecoorbit_dotnet.Infrastructure.Repositories.Interfaces;

namespace ecoorbit_dotnet.Application.Services;

public class SatelliteImageService : ISatelliteImageService
{
    private readonly ISatelliteImageRepository _repository;
    private readonly IUserRepository _userRepository;

    public SatelliteImageService(ISatelliteImageRepository repository, IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<SatelliteImageResponseDto>> GetAllAsync()
    {
        var images = await _repository.GetAllAsync();
        return images.Select(MapToDto);
    }

    public async Task<SatelliteImageResponseDto> GetByIdAsync(Guid id)
    {
        var image = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Satellite image {id} not found.");
        return MapToDto(image);
    }

    public async Task<IEnumerable<SatelliteImageResponseDto>> GetByUserIdAsync(Guid userId)
    {
        var images = await _repository.GetByUserIdAsync(userId);
        return images.Select(MapToDto);
    }

    public async Task<SatelliteImageResponseDto> CreateAsync(CreateSatelliteImageDto dto, Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User {userId} not found.");

        var image = new SatelliteImage
        {
            Id = Guid.NewGuid(),
            ImageUrl = dto.ImageUrl,
            Region = dto.Region,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            CapturedAt = dto.CapturedAt,
            UserId = userId
        };

        await _repository.AddAsync(image);
        image.User = user;
        return MapToDto(image);
    }

    public async Task DeleteAsync(Guid id)
    {
        var image = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Satellite image {id} not found.");
        await _repository.DeleteAsync(image);
    }

    private static SatelliteImageResponseDto MapToDto(SatelliteImage image) => new()
    {
        Id = image.Id,
        ImageUrl = image.ImageUrl,
        Region = image.Region,
        Latitude = image.Latitude,
        Longitude = image.Longitude,
        CapturedAt = image.CapturedAt,
        SubmittedAt = image.SubmittedAt,
        UserId = image.UserId,
        UserName = image.User?.Name ?? string.Empty,
        HasDetectionResult = image.DetectionResult is not null
    };
}