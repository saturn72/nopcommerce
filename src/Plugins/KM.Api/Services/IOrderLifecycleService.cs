﻿
namespace KedemMarket.Api.Services
{
    public interface IOrderLifecycleService
    {
        Task CancelOrderAsync(string orderId, params string[] cancellationReasons);
    }
}
