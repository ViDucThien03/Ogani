using System;
using System.Collections.Generic;

namespace OganiAdmin.Models;

public partial class Shipment
{
    public int ShipId { get; set; }

    public DateTime? ShipDate { get; set; }

    public string? ShipAddress { get; set; }

    public string? ShipPhone { get; set; }

    public string? ShipNote { get; set; }

    public decimal? ShipPrice { get; set; }

    public string? ShipMethod { get; set; }

    public string? ShipState { get; set; }

    public string? ShipCode { get; set; }

    public int? CusId { get; set; }

    public virtual Customer? Cus { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
