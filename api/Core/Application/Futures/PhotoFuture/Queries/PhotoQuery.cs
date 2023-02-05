using Application.Interfaces;
using Domain.Entities;
using Domain.Responses;
using FluentValidation;
using MediatR;

namespace Application.Futures.PhotoFuture.Queries;

public class PhotoValidator : AbstractValidator<PhotoRequest>
{
    public PhotoValidator()
    {
    }
}

public class PhotoRequest : IRequest<IRestResponseResult<List<Photo>>>
{
    public int? id { get; set; }
    public int? start { get; set; }
    public int? count { get; set; }
}

public class PhotoRequestHandler : IRequestHandler<PhotoRequest, IRestResponseResult<List<Photo>>>
{
    private readonly IPhotoRepository photoRepository;

    public PhotoRequestHandler(IPhotoRepository photoRepository)
    {
        this.photoRepository = photoRepository;
    }

    public async Task<IRestResponseResult<List<Photo>>> Handle(PhotoRequest request, CancellationToken cancellationToken)
    {
        var photos = new List<Photo>();

        if (request.id.HasValue)
        {
            if (await this.photoRepository.GetPhotoByIdAsync(request.id.Value) is Photo photo)
            {
                photos.Add(photo);
            }
        }
        else if (request.start.HasValue && request.count.HasValue)
        {
            photos = await this.photoRepository.GetPhotosAsync(request.start.Value, request.count.Value);
        }
        else
        {
            photos = await this.photoRepository.GetPhotosAsync();
        }

        return RestResponseResult<List<Photo>>.Success(photos);
    }
}