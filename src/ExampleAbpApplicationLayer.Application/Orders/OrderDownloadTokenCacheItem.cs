using System;

namespace ExampleAbpApplicationLayer.Orders;

public abstract class OrderDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}