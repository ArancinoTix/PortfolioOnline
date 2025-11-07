namespace VisualInspector
{
    /// <summary>
    ///     OrderAttribute can be used to change order of drawing for all members.
    /// </summary>
    /// <remarks>
    ///     Script in MonoBehaviours have order of "-10000" by default. If you want to draw something before script, use order smaller than "-10000".
    ///     Fields have order are drawn first (in no particular order)
    ///     Properties are drawn second (in no particular order)
    ///     Methods are drawn last (in no particular order)
    ///     All members have order of 0 by default, Order of 0 will not change order of drawing.
    /// </remarks>
    public class OrderAttribute : VisualAttribute
    {
        public int Order { get; set; }
        
        public OrderAttribute(int order)
        {
            Order = order;
        }
    }
}