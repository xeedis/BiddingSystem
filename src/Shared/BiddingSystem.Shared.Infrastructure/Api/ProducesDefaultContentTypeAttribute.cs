using Microsoft.AspNetCore.Mvc;

namespace BiddingSystem.Shared.Infrastructure.Api;

public class ProducesDefaultContentTypeAttribute : ProducesAttribute
{
    public ProducesDefaultContentTypeAttribute(Type type) : base(type)
    {
    }

    public ProducesDefaultContentTypeAttribute(params string[] additionalContentTypes) 
        : base("application/json", additionalContentTypes)
    {
    }
}