public class ScrapedData
{
    public int Id { get; set; }

    public string HospitalName { get; set; } = null!;

    public int WaitTimeMinutes { get; set; }

    public DateTime ScrapedAtUtc { get; set; }
}