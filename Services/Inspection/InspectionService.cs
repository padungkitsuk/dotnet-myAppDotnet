using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Encodings.Web;
using MyBackend.Services.Inspection;
using MyBackend.Models.Inspection;

namespace MyBackend.Services.Inspection;

public class InspectionService : IInspectionService
{
    private readonly ILogger<InspectionService> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true };
    //private readonly IProductRepository _repository;

    public InspectionService(ILogger<InspectionService> logger//, IProductRepository repository
    )
    {
        _logger = logger;
       // _repository = repository;
    }

    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "data.json");

    public async Task<IEnumerable<InspectionTransaction>> GetAllAsync()
    {
        var result = new List<InspectionTransaction>();
        try
        {
            if (File.Exists(filePath))
            {
                // ใช้ ReadAllTextAsync เพื่อไม่ให้ Block Thread
                string jsonFileContent = await File.ReadAllTextAsync(filePath);

                using (JsonDocument doc = JsonDocument.Parse(jsonFileContent))
                {
                    if (doc.RootElement.TryGetProperty("inpectionList", out JsonElement jsonStringElement))
                    {
                        string innerJsonArray = jsonStringElement.GetString() ?? "[]";
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var trans = JsonSerializer.Deserialize<List<InspectionTransaction>>(innerJsonArray, options);
                        if (trans != null)
                        {
                            result = trans; 
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("ไม่พบไฟล์ที่: " + filePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"เกิดข้อผิดพลาด: {ex.Message}");
            // อาจจะ throw ต่อ หรือ return list ว่างก็ได้ครับ
        }

        return result;
    }

    public async Task<InspectionDetail> GetByIdAsync(int id)
    {
       var result = new InspectionDetail();
        try
        {
            if (File.Exists(filePath))
            {
                // ใช้ ReadAllTextAsync เพื่อไม่ให้ Block Thread
                string jsonFileContent = await File.ReadAllTextAsync(filePath);

                using (JsonDocument doc = JsonDocument.Parse(jsonFileContent))
                {
                    if (doc.RootElement.TryGetProperty("inpectionDetail", out JsonElement jsonStringElement))
                    {
                        string innerJsonArray = jsonStringElement.GetString() ?? "[]";
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var trans = JsonSerializer.Deserialize<InspectionDetail>(innerJsonArray, options);
                        if (trans != null)
                        {
                            result = trans; 
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("ไม่พบไฟล์ที่: " + filePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"เกิดข้อผิดพลาด: {ex.Message}");
            // อาจจะ throw ต่อ หรือ return list ว่างก็ได้ครับ
        }

        return result;
    }
}