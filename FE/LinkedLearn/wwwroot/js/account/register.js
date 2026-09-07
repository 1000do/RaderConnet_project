// JavaScript xử lý tương tác riêng cho trang Đăng ký (Register)
document.addEventListener("DOMContentLoaded", function () {
    const registerForm = document.querySelector("form[action*='Register']");
    if (registerForm) {
        registerForm.addEventListener("submit", function () {
            const submitBtn = registerForm.querySelector("button[type='submit']");
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.classList.add("opacity-75", "cursor-wait");
                submitBtn.innerText = "Creating account...";
            }
        });
    }
});
