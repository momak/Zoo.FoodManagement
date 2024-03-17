namespace Zoo.Services.Abstractions;

public interface IZooPriceService
{
    /// <summary>
    /// Calculate the total price per day
    /// </summary>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>total cost per day for a zoo</returns>
    Task<decimal> CalculatePrice(CancellationToken ct);
}