namespace P3AddNewFunctionalityDotNetCore.Data.Models.Entities
{
    public class CartLine
    {
        public int OrderLineId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
