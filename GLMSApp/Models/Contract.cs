public class Contract
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; }

    public string FilePath { get; set; }

    public List<ServiceRequest> ServiceRequests { get; set; }
}