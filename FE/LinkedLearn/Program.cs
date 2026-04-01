using LinkedLearn.Service;

namespace LinkedLearn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Thêm dịch vụ Controllers và Views
            builder.Services.AddControllersWithViews();

            // 🔥 2. DÒNG QUAN TRỌNG NHẤT BỊ THIẾU: Đăng ký HttpClient Factory
            builder.Services.AddHttpClient();

            // 3. Đăng ký Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // 4. Đăng ký Gateway Service của bạn
            builder.Services.AddScoped<IUserGatewayFrontendService, UserGatewayFrontendService>();

            var app = builder.Build();

            // --- CẤU HÌNH MIDDLEWARE ---
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // 5. Kích hoạt Session (phải trước UseRouting)
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