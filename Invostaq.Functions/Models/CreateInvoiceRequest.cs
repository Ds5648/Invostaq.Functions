namespace InvoStaq.Functions.Models;

public class CreateInvoiceRequest
{
    public Guid? Id { get; set; }           // optional — auto-generated if omitted
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}