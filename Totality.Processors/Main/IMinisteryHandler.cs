using Totality.Model;

namespace Totality.Handlers.Main
{
    interface IMinisteryHandler
    {
        OrderResult ProcessOrder(Order order);
    }
}
