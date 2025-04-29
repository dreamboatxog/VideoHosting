using System;
using Microsoft.EntityFrameworkCore;
using VideoService.Domain.Entities;

namespace VideoService.Infrastructure;

public class VideoDbContext : DbContext 
{
    public VideoDbContext(DbContextOptions<VideoDbContext> options): base(options){}
    public DbSet<Video> Videos { get; set; }
}
