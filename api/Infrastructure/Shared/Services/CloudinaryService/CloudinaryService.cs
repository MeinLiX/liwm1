using CloudinaryDotNet;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Domain.Models;
using CloudinaryDotNet.Actions;

namespace Shared.Services.CloudinaryService;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> configuration)
    {
        var account = new Account(configuration.Value.CloudName, configuration.Value.ApiKey, configuration.Value.ApiSecret);

        this.cloudinary = new Cloudinary(account);
    }

    public async Task<List<string>> GetPhotosAsync()
    {
        var photos = await this.cloudinary.ListResourcesAsync(new ListResourcesParams
        {
            ResourceType = ResourceType.Image,
            MaxResults = 100
        });

        var photoUrls = new List<string>(photos.Resources.Where(r => r.Url.OriginalString.Contains("avatars")).Select(r => r.Url.OriginalString));

        return photoUrls;
    }
}