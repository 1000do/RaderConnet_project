-- 1. Bảng ROLES
CREATE TABLE IF NOT EXISTS roles (
    role_id SERIAL PRIMARY KEY,
    role_name TEXT NOT NULL UNIQUE
);

-- 2. Bảng USERS (Thông tin đăng nhập & Trạng thái)
CREATE TABLE IF NOT EXISTS users (
    user_id SERIAL PRIMARY KEY,
    email TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    status INT DEFAULT 1, -- 1: Active, 0: Inactive, 2: Banned
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 3. Bảng PROFILES (Thông tin cá nhân - 1:1 với Users)
CREATE TABLE IF NOT EXISTS profiles (
    user_id INT PRIMARY KEY,
    username TEXT, 
    full_name TEXT,
    phone_number TEXT,
    avatar_url TEXT,
    bio TEXT,
    is_instructor BOOLEAN DEFAULT FALSE,

    -- 🔥 CÁC TRƯỜNG PRIVACY MỚI
    is_public_email BOOLEAN DEFAULT FALSE, -- Mặc định ẩn email
    is_public_phone BOOLEAN DEFAULT FALSE, -- Mặc định ẩn SĐT
    is_public_profile BOOLEAN DEFAULT TRUE, -- Cho phép tìm thấy Profile hay không
    
    CONSTRAINT fk_profile_user
        FOREIGN KEY (user_id)
        REFERENCES users(user_id)
        ON DELETE CASCADE
);

-- 4. Bảng USER_ROLES (Phân quyền người dùng)
CREATE TABLE IF NOT EXISTS user_roles (
    user_id INT,
    role_id INT,
    PRIMARY KEY (user_id, role_id),
    CONSTRAINT fk_userrole_user
        FOREIGN KEY (user_id)
        REFERENCES users(user_id)
        ON DELETE CASCADE,
    CONSTRAINT fk_userrole_role
        FOREIGN KEY (role_id)
        REFERENCES roles(role_id)
        ON DELETE CASCADE
);

-- 5. INSERT Dữ liệu mẫu ban đầu
INSERT INTO roles (role_name) VALUES 
('learner'), ('Instructor'), ('staff'), ('admin')
ON CONFLICT (role_name) DO NOTHING;