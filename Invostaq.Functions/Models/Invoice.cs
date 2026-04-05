namespace InvoStaq.Functions.Models;

public class Invoice
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}