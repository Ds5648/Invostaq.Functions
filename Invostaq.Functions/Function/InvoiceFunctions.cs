using System.Net;
using System.Text.Json;
using InvoStaq.Functions.Data;
using InvoStaq.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InvoStaq.Functions.Functions;

public class InvoiceFunctions
{
    private readonly InvoiceDbContext _db;
    private readonly ILogger<InvoiceFunctions> _logger;

    public InvoiceFunctions(InvoiceDbContext db, ILogger<InvoiceFunctions> logger)
    {
        _db = db;
        _logger = logger;
    }

    // POST /invoice
    [Function("CreateInvoice")]
    public async Task<HttpResponseData> CreateInvoice(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "invoice")] HttpRequestData req)
    {
        _logger.LogInformation("POST /invoice triggered");

        CreateInvoiceRequest? body;
        try
        {
            body = await JsonSerializer.DeserializeAsync<CreateInvoiceRequest>(req.Body);
        }
        catch (JsonException)
        {
            var badReq = req.CreateResponse(HttpStatusCode.BadRequest);
            string errorJson = JsonSerializer.Serialize(new { error = "Invalid JSON body." });
            await badReq.WriteStringAsync(errorJson);
            badReq.Headers.Add("Content-Type", "application/json");
            return badReq;
        }

        if (body is null || body.Amount <= 0 || body.Date == default)
        {
            var badReq = req.CreateResponse(HttpStatusCode.BadRequest);
            string errorJson = JsonSerializer.Serialize(new { error = "Amount (> 0) and Date are required." });
            await badReq.WriteStringAsync(errorJson);
            badReq.Headers.Add("Content-Type", "application/json");
            return badReq;
        }

        var invoice = new Invoice
        {
            Id = body.Id ?? Guid.NewGuid(),
            Amount = body.Amount,
            Date = body.Date
        };

        if (await _db.Invoices.AnyAsync(i => i.Id == invoice.Id))
        {
            var conflict = req.CreateResponse(HttpStatusCode.Conflict);
            string errorJson = JsonSerializer.Serialize(new { error = $"Invoice '{invoice.Id}' already exists." });
            await conflict.WriteStringAsync(errorJson);
            conflict.Headers.Add("Content-Type", "application/json");
            return conflict;
        }

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Invoice {Id} saved", invoice.Id);

        var response = req.CreateResponse(HttpStatusCode.Created);
        string json = JsonSerializer.Serialize(invoice);
        await response.WriteStringAsync(json);
        response.Headers.Add("Content-Type", "application/json");
        return response;
    }

    // GET /invoice/{id}
    [Function("GetInvoice")]
    public async Task<HttpResponseData> GetInvoice(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "invoice/{id:guid}")] HttpRequestData req,
        Guid id)
    {
        _logger.LogInformation("GET /invoice/{Id} triggered", id);

        var invoice = await _db.Invoices.FindAsync(id);

        if (invoice is null)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            string errorJson = JsonSerializer.Serialize(new { error = $"Invoice '{id}' not found." });
            await notFound.WriteStringAsync(errorJson);
            notFound.Headers.Add("Content-Type", "application/json");
            return notFound;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        string json = JsonSerializer.Serialize(invoice);
        await response.WriteStringAsync(json);
        response.Headers.Add("Content-Type", "application/json");
        return response;
    }
}