using Moq;
using Shared.Interfaces;
using Shared.Messages;
using VideoService.Application.Interfaces;
using VideoService.Domain.Entities;
using VideoService.Domain.Enums;

namespace VideoService.Tests;

public class VideoServiceTests
{
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IMessagePublisher> _messagePublisherMock;
    private readonly VideoService.Application.Services.VideoService _videoService;

    public VideoServiceTests()
    {
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _messagePublisherMock = new Mock<IMessagePublisher>();
        _videoService = new Application.Services.VideoService(_videoRepositoryMock.Object, _fileStorageServiceMock.Object, _messagePublisherMock.Object);
    }

    [Fact]
    public async Task DeleteVideoAsync_VideoNotFound_ReturnsFalse()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        _videoRepositoryMock.Setup(repo => repo.GetByIdAsync(videoId)).ReturnsAsync((Video?)null);

        // Act
        var result = await _videoService.DeleteVideoAsync(videoId);

        // Assert
        Assert.False(result);
        _fileStorageServiceMock.Verify(fs => fs.DeleteAsync(It.IsAny<string>()), Times.Never);
        _messagePublisherMock.Verify(mp => mp.Publish(It.IsAny<VideoDeleted>()), Times.Never);
        _videoRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Video>()), Times.Never);
    }

    [Fact]
    public async Task DeleteVideoAsync_VideoFoundWithFilePath_DeletesVideoAndPublishesMessage()
    {
        // Arrange
        var video = new Video
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Description = "Descr",
            Status = VideoStatus.Completed,
            Title = "awd",
            FilePath = "path/to/video.mp4"
        };
        _videoRepositoryMock.Setup(repo => repo.GetByIdAsync(video.Id)).ReturnsAsync(video);
        _fileStorageServiceMock.Setup(fs => fs.DeleteAsync(video.FilePath)).Returns(Task.CompletedTask);
        _videoRepositoryMock.Setup(repo => repo.DeleteAsync(video)).ReturnsAsync(true);

        // Act
        var result = await _videoService.DeleteVideoAsync(video.Id);

        // Assert
        Assert.True(result);
        _fileStorageServiceMock.Verify(fs => fs.DeleteAsync(video.FilePath), Times.Once);
        _messagePublisherMock.Verify(mp => mp.Publish(It.Is<VideoDeleted>(msg => msg.Id == video.Id)), Times.Once);
        _videoRepositoryMock.Verify(repo => repo.DeleteAsync(video), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_VideoNotFound_ReturnsNull()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        _videoRepositoryMock.Setup(repo => repo.GetByIdAsync(videoId)).ReturnsAsync((Video?)null);

        // Act
        var result = await _videoService.GetVideoByIdAsync(videoId);

        // Assert
        Assert.Null(result);
    }

        [Fact]
        public async Task GetByIdAsync_VideoFound_ReturnsVideo()
        {
            // Arrange
            var video = new Video
            {
                Id =  Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Description = "Descr",
                Status = VideoStatus.Completed,
                Title = "awd",
                FilePath = "path/to/video.mp4"
            };
            _videoRepositoryMock.Setup(repo => repo.GetByIdAsync(video.Id)).ReturnsAsync(video);

            // Act
            var result = await _videoService.GetVideoByIdAsync(video.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(video, result);
        }

        [Fact]
        public async Task ProcessVideoAsync_VideoNotFound_ReturnsFalse()
        {
            // Arrange
            var processedVideo = new VideoProcessed(Guid.NewGuid(), "path/to/thumbnail.jpg", new List<string?> { "path/to/processed/video.mp4" }, true);
            _videoRepositoryMock.Setup(repo => repo.GetByIdAsync(processedVideo.Id)).ReturnsAsync((Video?)null);


            // Act
            var result = await _videoService.ProcessVideoAsync(processedVideo);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ProcessVideoAsync_SuccessfulProcessing_SetsCompletedStatusAndPaths()
        {
            // Arrange
            var processedVideo = new VideoProcessed(Guid.NewGuid(), "path/to/thumbnail.jpg", new List<string?> { "path/to/processed/video.mp4" }, true);
            var video = new Video
            {
                Id = processedVideo.Id,
                CreatedAt = DateTime.UtcNow,
                Description = "Descr",
                Status = VideoStatus.Uploading,
                Title = "awd",
                FilePath = "path/to/video.mp4"
            };
            _videoRepositoryMock.Setup(repo => repo.GetByIdAsync(processedVideo.Id)).ReturnsAsync(video);
            _videoRepositoryMock.Setup(repo => repo.UpdateAsync(video)).ReturnsAsync(true);

            // Act
            var result = await _videoService.ProcessVideoAsync(processedVideo);

            // Assert
            Assert.True(result);
            Assert.Equal(VideoStatus.Completed, video.Status);
            Assert.Equal(processedVideo.ThumbnailPath, video.ThumbnailPath);
            Assert.Equal(processedVideo.ProcessedVideoPath.FirstOrDefault(), video.FilePath);
        }

          [Fact]
        public async Task ProcessVideoAsync_FailedProcessing_SetsFailedStatus()
        {
            // Arrange
            var processedVideo = new VideoProcessed(Guid.NewGuid(), "path/to/thumbnail.jpg", new List<string?> { "path/to/processed/video.mp4" }, false);
            var video = new Video
            {
                Id = processedVideo.Id,
                CreatedAt = DateTime.UtcNow,
                Description = "Descr",
                Status = VideoStatus.Uploading,
                Title = "awd",
                FilePath = "path/to/video.mp4"
            };
            _videoRepositoryMock.Setup(repo => repo.GetByIdAsync(processedVideo.Id)).ReturnsAsync(video);

            // Act
            var result = await _videoService.ProcessVideoAsync(processedVideo);

            // Assert
            Assert.False(result);
            Assert.Equal(VideoStatus.Failed, video.Status);
        }

        [Fact]
        public async Task ProcessVideoAsync_EmptyProcessedPaths_ReturnsFalse()
        {
            // Arrange
           var processedVideo = new VideoProcessed(Guid.NewGuid(), null , null, true);
            var video = new Video
            {
                Id = processedVideo.Id,
                CreatedAt = DateTime.UtcNow,
                Description = "Descr",
                Status = VideoStatus.Uploading,
                Title = "awd"
            };


            _videoRepositoryMock.Setup(repo => repo.GetByIdAsync(video.Id)).ReturnsAsync((Video?)null);

            // Act
            var result = await _videoService.ProcessVideoAsync(processedVideo);

            // Assert
            Assert.False(result);
            Assert.Equal(string.Empty, video.FilePath);
            Assert.Equal(string.Empty, video.ThumbnailPath);
        }

}
