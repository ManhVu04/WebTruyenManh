// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Follow functionality
$(document).ready(function() {
    // Khởi tạo trạng thái follow cho tất cả các nút
    $('.follow-btn').each(function() {
        const btn = $(this);
        const comicId = btn.data('comic-id');
        
        if (comicId) {
            // Kiểm tra trạng thái follow hiện tại
            checkFollowStatus(btn, comicId);
        }
    });
    
    // Xử lý click nút follow
    $(document).on('click', '.follow-btn', function(e) {
        e.preventDefault();
        const btn = $(this);
        const comicId = btn.data('comic-id');
        
        if (!comicId) {
            showToast('Có lỗi xảy ra khi theo dõi truyện', 'error');
            return;
        }
        
        // Vô hiệu hóa nút tạm thời
        btn.prop('disabled', true);
        
        // Gọi API toggle follow
        $.ajax({
            url: '/Follow/Toggle',
            type: 'POST',
            data: {
                comicId: comicId,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function(response) {
                if (response.success) {
                    updateFollowButton(btn, response.isFollowing);
                    showToast(response.message, 'success');
                } else {
                    showToast(response.message || 'Có lỗi xảy ra', 'error');
                }
            },
            error: function() {
                showToast('Có lỗi xảy ra khi kết nối server', 'error');
            },
            complete: function() {
                btn.prop('disabled', false);
            }
        });
    });
});

// Kiểm tra trạng thái follow
function checkFollowStatus(btn, comicId) {
    $.ajax({
        url: '/Follow/CheckStatus',
        type: 'GET',
        data: { comicId: comicId },
        success: function(response) {
            if (response.success) {
                updateFollowButton(btn, response.isFollowing);
            }
        },
        error: function() {
            console.error('Không thể kiểm tra trạng thái follow cho comic', comicId);
        }
    });
}

// Cập nhật giao diện nút follow
function updateFollowButton(btn, isFollowing) {
    const icon = btn.find('i');
    const text = btn.find('.follow-text');
    
    if (isFollowing) {
        btn.removeClass('btn-outline-danger').addClass('btn-danger');
        icon.removeClass('far').addClass('fas');
        text.text('Bỏ theo dõi');
        btn.attr('title', 'Bỏ theo dõi');
    } else {
        btn.removeClass('btn-danger').addClass('btn-outline-danger');
        icon.removeClass('fas').addClass('far');
        text.text('Theo dõi');
        btn.attr('title', 'Theo dõi truyện');
    }
    
    btn.data('following', isFollowing);
}

// Hiển thị toast notification
function showToast(message, type = 'info') {
    // Tạo toast element
    const toastHtml = `
        <div class="toast align-items-center text-white bg-${type === 'success' ? 'success' : type === 'error' ? 'danger' : 'primary'} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-circle' : 'info-circle'} me-2"></i>
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;
    
    // Thêm vào container
    let toastContainer = $('.toast-container');
    if (toastContainer.length === 0) {
        $('body').append('<div class="toast-container position-fixed bottom-0 end-0 p-3" style="z-index: 1055;"></div>');
        toastContainer = $('.toast-container');
    }
    
    const toastElement = $(toastHtml);
    toastContainer.append(toastElement);
    
    // Hiển thị toast
    const toast = new bootstrap.Toast(toastElement[0], {
        autohide: true,
        delay: 3000
    });
    toast.show();
    
    // Xóa toast sau khi ẩn
    toastElement.on('hidden.bs.toast', function() {
        $(this).remove();
    });
}
