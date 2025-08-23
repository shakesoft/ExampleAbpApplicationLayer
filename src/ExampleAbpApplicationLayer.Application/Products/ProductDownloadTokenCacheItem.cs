using System;

namespace ExampleAbpApplicationLayer.Products;

public abstract class ProductDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}