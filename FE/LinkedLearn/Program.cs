using LinkedLearn.Service;
using LinkedLearn.Service.IService;

namespace LinkedLearn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Thêm dịch vụ Controllers và Views
            builder.Services.AddControllersWithViews();

            // 🔥 2. ĐĂNG KÝ CÁC DỊCH VỤ CƠ SỞ (SỬA LỖI Ở ĐÂY)
            // Cần dòng này để UserGatewayFrontendService đọc được Cookie từ Browser
            builder.Services.AddHttpContextAccessor();

            // Cần dòng này để khởi tạo HttpClient gọi sang Gateway
            builder.Services.AddHttpClient();

            // 3. Đăng ký Session (Để lưu UserName hiển thị trên Layout)
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // 4. Đăng ký Gateway Service (Tầng nghiệp vụ gọi API)
            builder.Services.AddScoped<IUserGatewayFrontendService, UserGatewayFrontendService>();

            var app = builder.Build();

            // --- CẤU HÌNH MIDDLEWARE ---
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Lưu ý: Nếu test local ko có HTTPS thì có thể comment dòng này
            // app.UseHttpsRedirection(); 

            app.UseStaticFiles();

            // 5. Kích hoạt Session (Phải đặt trước UseRouting và UseAuthorization)
            app.UseSession();

            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}