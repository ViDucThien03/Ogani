using System;
using System.Collections.Generic;

namespace OganiAdmin.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? ProductDesc { get; set; }

    public decimal? ProductPrice { get; set; }

    public int? ProductQuantity { get; set; }

    public DateOnly? ProductExp { get; set; }

    public string? ProductImg { get; set; }

    public DateOnly? ProductAdd { get; set; }

    public int? SellProduct { get; set; }

    public int? CateId { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category? Cate { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
