// Enhanced Reading Experience Script
document.addEventListener('DOMContentLoaded', function() {
    // Reading progress bar
    const progressBar = document.createElement('div');
    progressBar.className = 'reading-progress';
    document.body.appendChild(progressBar);
    
    // Update progress bar on scroll
    window.addEventListener('scroll', function() {
        const winScroll = document.body.scrollTop || document.documentElement.scrollTop;
        const height = document.documentElement.scrollHeight - document.documentElement.clientHeight;
        const scrolled = (winScroll / height) * 100;
        progressBar.style.width = scrolled + '%';
        
        // Update sticky header style
        const stickyHeader = document.querySelector('.sticky-top');
        if (stickyHeader) {
            if (window.scrollY > 100) {
                stickyHeader.classList.add('scrolled');
            } else {
                stickyHeader.classList.remove('scrolled');
            }
        }
    });
    
    // Create floating navigation
    const floatingNav = document.createElement('div');
    floatingNav.className = 'floating-nav';
    
    // Scroll to top button
    const scrollTopBtn = document.createElement('button');
    scrollTopBtn.className = 'btn scroll-to-top';
    scrollTopBtn.innerHTML = '<i class="fas fa-arrow-up"></i>';
    scrollTopBtn.title = 'Lên đầu trang';
    scrollTopBtn.onclick = function() {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };
    
    // Scroll to comments button
    const scrollCommentsBtn = document.createElement('button');
    scrollCommentsBtn.className = 'btn';
    scrollCommentsBtn.innerHTML = '<i class="fas fa-comments"></i>';
    scrollCommentsBtn.title = 'Đến phần bình luận';
    scrollCommentsBtn.onclick = function() {
        const commentsSection = document.getElementById('comments-section');
        if (commentsSection) {
            commentsSection.scrollIntoView({ behavior: 'smooth' });
        }
    };
    
    floatingNav.appendChild(scrollTopBtn);
    floatingNav.appendChild(scrollCommentsBtn);
    document.body.appendChild(floatingNav);
    
    // Show/hide scroll to top button
    window.addEventListener('scroll', function() {
        if (window.scrollY > 300) {
            scrollTopBtn.classList.add('show');
        } else {
            scrollTopBtn.classList.remove('show');
        }
    });
    
    // Image lazy loading with loading animation
    const images = document.querySelectorAll('.chapter-image');
    images.forEach(function(img, index) {
        // Add loading delay for smooth appearance
        setTimeout(function() {
            img.style.animationDelay = (index * 0.1) + 's';
            img.setAttribute('loaded', 'true');
        }, 100);
        
        img.addEventListener('load', function() {
            this.style.opacity = '1';
        });
        
        img.addEventListener('error', function() {
            this.style.opacity = '0.5';
            this.title = 'Không thể tải hình ảnh';
        });
    });
    
    // Keyboard navigation
    document.addEventListener('keydown', function(e) {
        // Left arrow - Previous chapter
        if (e.key === 'ArrowLeft' && !e.ctrlKey && !e.shiftKey) {
            const prevBtn = document.querySelector('a[href*="chapter"]:has(.fa-chevron-left)');
            if (prevBtn && !prevBtn.classList.contains('btn-outline-secondary')) {
                window.location.href = prevBtn.href;
            }
        }
        
        // Right arrow - Next chapter
        if (e.key === 'ArrowRight' && !e.ctrlKey && !e.shiftKey) {
            const nextBtn = document.querySelector('a[href*="chapter"]:has(.fa-chevron-right)');
            if (nextBtn && !nextBtn.classList.contains('btn-outline-secondary')) {
                window.location.href = nextBtn.href;
            }
        }
        
        // C key - Focus on comment box
        if (e.key === 'c' || e.key === 'C') {
            const commentBox = document.querySelector('textarea[name="content"]');
            if (commentBox) {
                commentBox.focus();
                e.preventDefault();
            }
        }
        
        // Esc key - Clear comment box
        if (e.key === 'Escape') {
            const commentBox = document.querySelector('textarea[name="content"]');
            if (commentBox && document.activeElement === commentBox) {
                commentBox.blur();
            }
        }
    });
    
    // Auto-save comment draft
    const commentTextarea = document.querySelector('textarea[name="content"]');
    if (commentTextarea) {
        const storageKey = 'comment_draft_' + window.location.pathname;
        
        // Load saved draft
        const savedDraft = localStorage.getItem(storageKey);
        if (savedDraft) {
            commentTextarea.value = savedDraft;
        }
        
        // Save draft on input
        commentTextarea.addEventListener('input', function() {
            localStorage.setItem(storageKey, this.value);
        });
        
        // Clear draft on successful submit
        const commentForm = commentTextarea.closest('form');
        if (commentForm) {
            commentForm.addEventListener('submit', function() {
                localStorage.removeItem(storageKey);
            });
        }
    }
    
    // Enhanced chapter selection
    const chapterSelect = document.querySelector('select[onchange*="chapter"]');
    if (chapterSelect) {
        chapterSelect.addEventListener('change', function() {
            // Add loading state
            this.disabled = true;
            this.style.opacity = '0.6';
        });
    }
    
    // Smooth transitions for dynamic content
    const observer = new MutationObserver(function(mutations) {
        mutations.forEach(function(mutation) {
            if (mutation.type === 'childList') {
                mutation.addedNodes.forEach(function(node) {
                    if (node.nodeType === 1 && node.classList) {
                        node.style.opacity = '0';
                        node.style.transform = 'translateY(20px)';
                        setTimeout(function() {
                            node.style.transition = 'all 0.3s ease';
                            node.style.opacity = '1';
                            node.style.transform = 'translateY(0)';
                        }, 10);
                    }
                });
            }
        });
    });
    
    observer.observe(document.body, {
        childList: true,
        subtree: true
    });
    
    // Touch gestures for mobile
    let touchStartX = null;
    let touchStartY = null;
    
    document.addEventListener('touchstart', function(e) {
        touchStartX = e.touches[0].clientX;
        touchStartY = e.touches[0].clientY;
    });
    
    document.addEventListener('touchend', function(e) {
        if (!touchStartX || !touchStartY) return;
        
        const touchEndX = e.changedTouches[0].clientX;
        const touchEndY = e.changedTouches[0].clientY;
        
        const deltaX = touchStartX - touchEndX;
        const deltaY = touchStartY - touchEndY;
        
        // Horizontal swipe (chapter navigation)
        if (Math.abs(deltaX) > Math.abs(deltaY) && Math.abs(deltaX) > 50) {
            if (deltaX > 0) {
                // Swipe left - next chapter
                const nextBtn = document.querySelector('a[href*="chapter"]:has(.fa-chevron-right)');
                if (nextBtn && !nextBtn.classList.contains('btn-outline-secondary')) {
                    window.location.href = nextBtn.href;
                }
            } else {
                // Swipe right - previous chapter
                const prevBtn = document.querySelector('a[href*="chapter"]:has(.fa-chevron-left)');
                if (prevBtn && !prevBtn.classList.contains('btn-outline-secondary')) {
                    window.location.href = prevBtn.href;
                }
            }
        }
        
        touchStartX = null;
        touchStartY = null;
    });
    
    // Add tooltips to buttons
    const buttons = document.querySelectorAll('.btn[title]');
    buttons.forEach(function(btn) {
        btn.addEventListener('mouseenter', function() {
            const tooltip = document.createElement('div');
            tooltip.className = 'btn-tooltip';
            tooltip.textContent = this.title;
            tooltip.style.cssText = `
                position: absolute;
                bottom: 100%;
                left: 50%;
                transform: translateX(-50%);
                background: rgba(0, 0, 0, 0.9);
                color: white;
                padding: 5px 10px;
                border-radius: 5px;
                font-size: 12px;
                white-space: nowrap;
                z-index: 1000;
                pointer-events: none;
                margin-bottom: 5px;
            `;
            this.style.position = 'relative';
            this.appendChild(tooltip);
        });
        
        btn.addEventListener('mouseleave', function() {
            const tooltip = this.querySelector('.btn-tooltip');
            if (tooltip) {
                tooltip.remove();
            }
        });
    });
    
    console.log('🌟 Enhanced reading experience loaded successfully!');
});

// Reading Sidebar Functions
let isAutoScrolling = false;
let autoScrollInterval;
let isVerticalReading = false;
let isFollowing = false;

// Chapter navigation - will be set by the view
let prevChapterUrl = null;
let nextChapterUrl = null;

function setPrevChapterUrl(url) {
    prevChapterUrl = url;
}

function setNextChapterUrl(url) {
    nextChapterUrl = url;
}

function previousChapter() {
    if (prevChapterUrl) {
        window.location.href = prevChapterUrl;
    }
}

function nextChapter() {
    if (nextChapterUrl) {
        window.location.href = nextChapterUrl;
    }
}

// Toggle comments section
function toggleComments() {
    const commentsSection = document.getElementById('comments-section');
    if (commentsSection) {
        commentsSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
        // Highlight comments section briefly
        commentsSection.style.boxShadow = '0 0 20px rgba(79, 195, 247, 0.5)';
        setTimeout(() => {
            commentsSection.style.boxShadow = '';
        }, 2000);
    }
}

// Toggle follow status
function toggleFollow() {
    isFollowing = !isFollowing;
    const followBtn = document.querySelector('.sidebar-btn[onclick="toggleFollow()"]');
    const followIcon = followBtn.querySelector('i');
    const followText = followBtn.querySelector('span');
    
    if (isFollowing) {
        document.body.classList.add('following');
        followIcon.className = 'fas fa-bookmark';
        followText.textContent = 'Đã Theo Dõi';
        
        // Show notification
        showNotification('Đã thêm vào danh sách theo dõi!', 'success');
    } else {
        document.body.classList.remove('following');
        followIcon.className = 'far fa-bookmark';
        followText.textContent = 'Theo Dõi';
        
        showNotification('Đã bỏ theo dõi!', 'info');
    }
}

// Show comic details
function showComicDetails() {
    window.location.href = '@Url.Action("Details", "Comics", new { id = comic.Id })';
}

// Report error
function reportError() {
    const modal = document.createElement('div');
    modal.className = 'modal fade';
    modal.innerHTML = `
        <div class="modal-dialog">
            <div class="modal-content bg-dark text-white">
                <div class="modal-header">
                    <h5 class="modal-title">Báo Lỗi Chapter</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <form id="errorReportForm">
                        <div class="mb-3">
                            <label class="form-label">Loại lỗi:</label>
                            <select class="form-select bg-dark text-white" name="errorType">
                                <option value="missing-images">Thiếu hình ảnh</option>
                                <option value="wrong-order">Sai thứ tự trang</option>
                                <option value="low-quality">Chất lượng thấp</option>
                                <option value="other">Khác</option>
                            </select>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Mô tả chi tiết:</label>
                            <textarea class="form-control bg-dark text-white" name="description" rows="3" 
                                      placeholder="Mô tả lỗi bạn gặp phải..."></textarea>
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
                    <button type="button" class="btn btn-primary" onclick="submitErrorReport()">Gửi Báo Cáo</button>
                </div>
            </div>
        </div>
    `;
    
    document.body.appendChild(modal);
    const bsModal = new bootstrap.Modal(modal);
    bsModal.show();
    
    modal.addEventListener('hidden.bs.modal', () => {
        document.body.removeChild(modal);
    });
}

function submitErrorReport() {
    showNotification('Cảm ơn bạn đã báo cáo! Chúng tôi sẽ kiểm tra và xử lý sớm nhất.', 'success');
    const modal = bootstrap.Modal.getInstance(document.querySelector('.modal'));
    modal.hide();
}

// Toggle vertical reading mode
function toggleVerticalReading() {
    isVerticalReading = !isVerticalReading;
    const body = document.body;
    const verticalBtn = document.querySelector('.sidebar-btn[onclick="toggleVerticalReading()"]');
    const verticalText = verticalBtn.querySelector('span');
    
    if (isVerticalReading) {
        body.classList.add('vertical-reading');
        verticalText.textContent = 'Đọc theo chiều ngang';
        showNotification('Chế độ đọc dọc đã bật', 'info');
    } else {
        body.classList.remove('vertical-reading');
        verticalText.textContent = 'Đọc theo chiều dọc';
        showNotification('Chế độ đọc ngang đã bật', 'info');
    }
}

// Toggle auto scroll
function toggleAutoScroll() {
    isAutoScrolling = !isAutoScrolling;
    const autoScrollBtn = document.querySelector('.sidebar-btn[onclick="toggleAutoScroll()"]');
    const autoScrollText = autoScrollBtn.querySelector('span');
    const autoScrollIcon = autoScrollBtn.querySelector('i');
    
    if (isAutoScrolling) {
        document.body.classList.add('auto-scroll-active');
        autoScrollIcon.className = 'fas fa-pause-circle';
        autoScrollText.textContent = 'Tự động cuộn';
        
        // Start auto scrolling
        autoScrollInterval = setInterval(() => {
            window.scrollBy(0, 2);
            
            // Stop when reaching bottom
            if (window.innerHeight + window.scrollY >= document.body.offsetHeight - 100) {
                toggleAutoScroll();
            }
        }, 50);
        
        showNotification('Tự động cuộn đã bật', 'success');
    } else {
        document.body.classList.remove('auto-scroll-active');
        autoScrollIcon.className = 'fas fa-play-circle';
        autoScrollText.textContent = 'Tự động cuộn Tắt';
        
        if (autoScrollInterval) {
            clearInterval(autoScrollInterval);
        }
        
        showNotification('Tự động cuộn đã tắt', 'info');
    }
}

// Scroll to top
function scrollToTop() {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}

// Show notification
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `alert alert-${type} notification-toast`;
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9999;
        min-width: 300px;
        opacity: 0;
        transform: translateX(100%);
        transition: all 0.3s ease;
    `;
    notification.innerHTML = `
        <div class="d-flex align-items-center">
            <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-circle' : 'info-circle'} me-2"></i>
            ${message}
        </div>
    `;
    
    document.body.appendChild(notification);
    
    // Animate in
    setTimeout(() => {
        notification.style.opacity = '1';
        notification.style.transform = 'translateX(0)';
    }, 100);
    
    // Animate out and remove
    setTimeout(() => {
        notification.style.opacity = '0';
        notification.style.transform = 'translateX(100%)';
        setTimeout(() => {
            if (notification.parentNode) {
                document.body.removeChild(notification);
            }
        }, 300);
    }, 3000);
}

// Initialize sidebar functionality
document.addEventListener('DOMContentLoaded', function() {
    // Add smooth scrolling for better reading experience
    document.documentElement.style.scrollBehavior = 'smooth';
    
    // Add keyboard shortcuts
    document.addEventListener('keydown', function(e) {
        switch(e.key) {
            case 'ArrowLeft':
                if (e.ctrlKey) {
                    e.preventDefault();
                    previousChapter();
                }
                break;
            case 'ArrowRight':
                if (e.ctrlKey) {
                    e.preventDefault();
                    nextChapter();
                }
                break;
            case ' ':
                e.preventDefault();
                toggleAutoScroll();
                break;
            case 'Home':
                e.preventDefault();
                scrollToTop();
                break;
        }
    });
});
