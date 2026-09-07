// JavaScript xử lý tương tác riêng cho trang Đăng nhập (Login)
document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.querySelector("form[action*='Login']");
    if (loginForm) {
        loginForm.addEventListener("submit", function () {
            const submitBtn = loginForm.querySelector("button[type='submit']");
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.classList.add("opacity-75", "cursor-wait");
                submitBtn.innerText = "Logging in...";
            }
        });
    }
});
