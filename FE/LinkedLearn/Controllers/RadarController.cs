using LinkedLearn.Models.RadarVM;
using LinkedLearn.Service.IService;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LinkedLearn.Controllers
{
    public class RadarController : Controller
    {
        private readonly IRadarGatewayFrontendService _radarService;
        private readonly IHttpClientFactory _httpClientFactory;

        public RadarController(IRadarGatewayFrontendService radarService, IHttpClientFactory httpClientFactory)
        {
            _radarService = radarService;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            // Trả về giao diện trống trước. 
            // View sẽ dùng Javascript (navigator.geolocation) để lấy tọa độ và gọi AJAX về hàm Search bên dưới
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Search(double longitude, double latitude)
        {
            var activities = await _radarService.SearchRadarAsync(longitude, latitude);
            // Trả về PartialView hoặc JSON. Ở đây trả về PartialView để render thẳng HTML cho lẹ
            return PartialView("_RadarListPartial", activities);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateActivityViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (content, statusCode) = await _radarService.CreateActivityAsync(model);
                if (statusCode == 200 || statusCode == 201)
                {
                    TempData["SuccessMessage"] = "Tạo kèo thành công!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Lỗi từ server: " + content);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAddress(double lat, double lon)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("User-Agent", "LinkedLearn/1.0");
                var url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={lat}&lon={lon}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement>(json);
                    var displayName = data.GetProperty("display_name").GetString();
                    return Json(new { address = displayName });
                }
            }
            catch { }
            return Json(new { address = (string?)null });
        }
    }
}
