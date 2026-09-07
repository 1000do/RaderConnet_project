Mục tiêu dự án
Về mặt sản phẩm: Giải quyết bài toán kết nối tức thì (Hyperlocal & Ephemeral). Thay vì tạo sự kiện dài hạn, hệ thống chỉ tập trung vào các hoạt động diễn ra ngay trong ngày, trong bán kính gần, giúp người dùng tìm được bạn đồng hành mà không mất thời gian lên lịch phức tạp.

Về mặt kỹ thuật ghi điểm CV:

Chứng minh năng lực xử lý dữ liệu không gian địa lý (GIS/Spatial Data) bằng C# và SQL.

Làm chủ luồng dữ liệu thời gian thực (Real-time WebSockets với SignalR).

Thiết kế hệ thống tự dọn dẹp dữ liệu hết hạn (Lifecycle/TTL) và cơ chế khóa chống trùng slot khi nhiều người cùng bấm tham gia một lúc.

Các chức năng cốt lõi
1. Quản lý tài khoản & Hồ sơ tức thì (User & Profile)

Đăng ký, đăng nhập bằng Email/Password hoặc Google OAuth (trả về JWT kèm Refresh Token).

Hồ sơ tối giản: Avatar, tên hiển thị, điểm uy tín (Reputation Score), thẻ sở thích (Tags: Cầu lông, Cà phê, Chạy bộ, Boardgame).

Cấp quyền truy cập vị trí hiện tại (Trình duyệt gửi tọa độ Latitude & Longitude lên server).

2. Đăng "Kèo" tức thì (Create Ephemeral Activity)

Người tạo kèo (Host) điền thông tin:

Nội dung & Thể loại: Tiêu đề, mô tả ngắn, tag danh mục.

Địa điểm: Tọa độ GPS tự động lấy theo vị trí đứng hoặc ghim trên bản đồ (Leaflet / OpenStreetMap).

Số lượng người cần tuyển: Ví dụ cần 1 hoặc 2 người.

Thời gian sống của kèo: 2 tiếng, 4 tiếng hoặc mốc giờ cụ thể (hết giờ kèo tự đóng).

Hệ thống xác thực: Chỉ cho phép một user tạo tối đa 1–2 kèo còn hiệu lực cùng lúc để chống spam.

3. Bản đồ Radar & Tìm kiếm theo bán kính (Radar Exploration)

Hiển thị bản đồ với tâm là vị trí của người dùng hiện tại.

Lọc theo bán kính: Chọn thanh trượt 1km, 3km, 5km, 10km.

Lọc theo danh mục: Chọn xem riêng thể thao, ăn uống hoặc học tập.

Backend tính toán và chỉ trả về các kèo đang mở nằm trong bán kính đã chọn, sắp xếp theo khoảng cách từ gần đến xa.

4. Xin tham gia & Phê duyệt theo thời gian thực (Join & Match Engine)

Người tìm kèo bấm "Xin tham gia" (Request to Join).

Thông báo Real-time (SignalR): Host lập tức nhận được thông báo nổi trên màn hình kèm thông tin người xin vào.

Host bấm "Đồng ý" hoặc "Từ chối":

Nếu đồng ý: Slot được lấp đầy. Nếu đủ số lượng, trạng thái kèo tự động chuyển sang Full.

Xử lý bài toán Concurrency: Nếu chỉ còn 1 slot mà 2 người cùng bấm xin duyệt cùng lúc, backend dùng lock để chỉ 1 người thành công.

5. Nhóm chat tạm thời (Ephemeral Chat Room)

Khi Host duyệt thành viên, một phòng chat riêng giữa Host và các thành viên được mở ra qua SignalR.

Các bên trao đổi vị trí chính xác (số bàn, đặc điểm nhận dạng).

Tin nhắn chat tự động bị xóa toàn bộ khi kèo kết thúc để tối ưu dung lượng lưu trữ.

6. Kết thúc kèo & Đánh giá uy tín (Post-Meetup & Rating)

Host hoặc thành viên bấm "Hoàn thành buổi gặp".

Thả tim/vote uy tín cho nhau (Tăng/giảm điểm tín nhiệm của profile).

Tác vụ nền (Background Worker): Định kỳ mỗi 2 phút, worker quét các kèo quá thời hạn ExpiresAt mà chưa xong để tự động chuyển trạng thái sang Expired, ngắt kết nối phòng chat và dọn dẹp cache.

Backend: ASP.NET Core Web API (.NET 8/9).

Database: SQL Server (hỗ trợ geography) hoặc PostgreSQL kết hợp PostGIS.

Spatial Library: NetTopologySuite tích hợp trong Entity Framework Core.

Realtime: ASP.NET Core SignalR.

Background Jobs: IHostedService / BackgroundService tích hợp sẵn hoặc Hangfire.

Caching: Redis (lưu tạm tọa độ người dùng và danh sách active pins).

Frontend gợi ý: React hoặc Vue.js kết hợp thư viện bản đồ mã nguồn mở Leaflet (miễn phí, không cần thẻ tín dụng như Google Maps).