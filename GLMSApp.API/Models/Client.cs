using System.Diagnostics.Contracts;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ContactDetails { get; set; }
    public string Region { get; set; }

    public List<Contract> Contracts { get; set; }
}
