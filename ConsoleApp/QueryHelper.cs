using ConsoleApp.Model;
using ConsoleApp.Model.Enum;
using ConsoleApp.OutputTypes;

namespace ConsoleApp;

public class QueryHelper : IQueryHelper
{
    /// <summary>
    /// Отримує доставки, які були оплачено (де PaymentId не є null)
    /// </summary>
    public IEnumerable<Delivery> Paid(IEnumerable<Delivery> deliveries) =>
        deliveries.Where(d => d.PaymentId != null); // Завдання 1

    /// <summary>
    /// Отримує доставки, що зараз в обробці системою (не скасовані та не виконані)
    /// </summary>
    public IEnumerable<Delivery> NotFinished(IEnumerable<Delivery> deliveries) =>
        deliveries.Where(d => d.Status != DeliveryStatus.Cancelled && d.Status != DeliveryStatus.Done); // Завдання 2

    /// <summary>
    /// Повертає коротку інформацію про доставки певного клієнта
    /// </summary>
    public IEnumerable<DeliveryShortInfo> DeliveryInfosByClient(IEnumerable<Delivery> deliveries, string clientId) =>
        deliveries.Where(d => d.ClientId == clientId)
                  .Select(d => new DeliveryShortInfo
                  {
                      Id = d.Id,
                      StartCity = d.Direction.Origin.City,
                      EndCity = d.Direction.Destination.City,
                      ClientId = d.ClientId,
                      Type = d.Type,
                      LoadingPeriod = d.LoadingPeriod,
                      ArrivalPeriod = d.ArrivalPeriod,
                      Status = d.Status,
                      CargoType = d.CargoType
                  }); // Завдання 3

    /// <summary>
    /// Повертає перші 10 доставок певного типу, що починаються з певного міста
    /// </summary>
    public IEnumerable<Delivery> DeliveriesByCityAndType(IEnumerable<Delivery> deliveries, string cityName, DeliveryType type) =>
     deliveries.Where(d => d.Type == type && d.Direction.Origin.City == cityName);    // Завдання 4

    /// <summary>
    /// Сортує доставки за статусом, а якщо статуси однакові — за часом початку завантаження
    /// </summary>
    public IEnumerable<Delivery> OrderByStatusThenByStartLoading(IEnumerable<Delivery> deliveries) =>
        deliveries.OrderBy(d => d.Status).ThenBy(d => d.LoadingPeriod.Start); // Завдання 5

    /// <summary>
    /// Підраховує кількість унікальних типів вантажів
    /// </summary>
    public int CountUniqCargoTypes(IEnumerable<Delivery> deliveries) =>
        deliveries.Select(d => d.CargoType).Distinct().Count(); // Завдання 6

    /// <summary>
    /// Групує доставки за їх статусом та підраховує кількість доставок у кожній групі
    /// </summary>
    public Dictionary<DeliveryStatus, int> CountsByDeliveryStatus(IEnumerable<Delivery> deliveries) =>
        deliveries.GroupBy(d => d.Status)
                  .ToDictionary(g => g.Key, g => g.Count()); // Завдання 7

    /// <summary>
    /// Групує доставки за парами міст «старт-фініш» та обчислює середній проміжок між кінцем завантаження та початком прибуття (в хвилинах)
    /// </summary>
    public IEnumerable<AverageGapsInfo> AverageTravelTimePerDirection(IEnumerable<Delivery> deliveries) =>
        deliveries
            .Where(d => d.ArrivalPeriod.Start.HasValue && d.LoadingPeriod.End.HasValue)
            .GroupBy(d => (d.Direction.Origin.City, d.Direction.Destination.City))
            .Select(g => new AverageGapsInfo
            {
                StartCity = g.Key.Item1, // Місто відправлення
                EndCity = g.Key.Item2,   // Місто прибуття
                AverageGap = g.Average(d => (d.ArrivalPeriod.Start.Value - d.LoadingPeriod.End.Value).TotalMinutes)
            }); // Завдання 8

    /// <summary>
    /// Метод для пагінації — повертає елементи відповідно до сторінки та кількості на сторінці
    /// </summary>
    public IEnumerable<TElement> Paging<TElement, TOrderingKey>(IEnumerable<TElement> elements,
        Func<TElement, TOrderingKey> ordering,
        Func<TElement, bool>? filter = null,
        int countOnPage = 100,
        int pageNumber = 1) =>
        elements.Where(filter ?? (_ => true))
                .OrderBy(ordering)
                .Skip((pageNumber - 1) * countOnPage)
                .Take(countOnPage); // Завдання 9 
}
