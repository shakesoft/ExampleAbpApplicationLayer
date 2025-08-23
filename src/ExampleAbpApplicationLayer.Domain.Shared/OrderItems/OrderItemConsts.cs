namespace ExampleAbpApplicationLayer.OrderItems
{
    public static class OrderItemConsts
    {
        private const string DefaultSorting = "{0}CreationTime desc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "OrderItem." : string.Empty);
        }

    }
}