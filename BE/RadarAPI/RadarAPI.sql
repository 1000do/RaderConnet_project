-- Kích hoạt extension PostGIS (BẮT BUỘC ĐỂ DÙNG TÍNH NĂNG RADAR QUÉT BÁN KÍNH GPS)
CREATE EXTENSION IF NOT EXISTS postgis;

-- 1. Bảng Categories (Danh mục Sở thích)
CREATE TABLE IF NOT EXISTS "Categories" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Icon" VARCHAR(255) NULL
);

-- Thêm vài dữ liệu mẫu
INSERT INTO "Categories" ("Name", "Icon") VALUES 
('Cà phê', '☕'),
('Cầu lông', '🏸'),
('Boardgame', '🎲'),
('Làm việc/Học tập', '💻')
ON CONFLICT DO NOTHING;

-- 2. Bảng Activities (Lưu Kèo / Bài viết)
CREATE TABLE IF NOT EXISTS "Activities" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "HostId" VARCHAR(255) NOT NULL,
    "HostName" VARCHAR(255) NOT NULL,          -- Lưu bóng tên để load lẹ
    "HostAvatarUrl" TEXT NULL,                 -- Lưu bóng hình để load lẹ
    "Title" VARCHAR(255) NOT NULL,
    "Description" TEXT NULL,
    "CategoryId" INTEGER NOT NULL REFERENCES "Categories"("Id"),
    
    -- TRÁI TIM CỦA RADAR: Cột lưu Tọa độ địa lý chuẩn WGS 84 (SRID 4326)
    "Location" GEOGRAPHY(Point, 4326) NOT NULL,
    
    "MaxParticipants" INTEGER NOT NULL,
    "CurrentParticipants" INTEGER NOT NULL DEFAULT 0,
    
    -- Trạng thái: Open (Đang mở), Full (Đã đủ người), Expired (Hết hạn), Cancelled (Hủy)
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Open',
    
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "ExpiresAt" TIMESTAMP WITH TIME ZONE NOT NULL
);

-- Tạo Index không gian (Spatial Index) siêu tốc độ cho cột Location
CREATE INDEX IF NOT EXISTS "IX_Activities_Location" ON "Activities" USING GIST ("Location");

-- 3. Bảng ActivityParticipants (Thành viên xin tham gia)
CREATE TABLE IF NOT EXISTS "ActivityParticipants" (
    "ActivityId" UUID NOT NULL REFERENCES "Activities"("Id") ON DELETE CASCADE,
    "UserId" VARCHAR(255) NOT NULL,
    "UserName" VARCHAR(255) NOT NULL,          -- Lưu bóng
    "UserAvatarUrl" TEXT NULL,                 -- Lưu bóng
    
    -- Trạng thái: Pending (Chờ duyệt), Approved (Đã duyệt), Rejected (Bị từ chối)
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Pending',
    
    "RequestedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "JoinedAt" TIMESTAMP WITH TIME ZONE NULL,
    
    PRIMARY KEY ("ActivityId", "UserId")
);
