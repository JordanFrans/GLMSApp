using System.Diagnostics.Contracts;

public class ServiceRequest
{
    public int Id { get; set; }
    public string Description { get; set; }
    public decimal Cost { get; set; }
    public string Status { get; set; }

    public int ContractId { get; set; }
    public Contract Contract { get; set; }
}