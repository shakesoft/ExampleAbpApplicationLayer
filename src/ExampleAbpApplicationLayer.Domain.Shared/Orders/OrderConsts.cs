namespace ExampleAbpApplicationLayer.Orders
{
    public static class OrderConsts
    {
        private const string DefaultSorting = "{0}CreationTime desc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Order." : string.Empty);
        }

    }
}