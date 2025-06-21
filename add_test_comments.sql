-- Thêm bình luận test cho chapter 11
INSERT INTO Comments (Content, CreatedAt, UserId, UserName, ChapterId) VALUES
('Truyện này hay quá! Rất thích tình tiết phần này 😍', GETDATE(), 'test-user-1', 'Nguyễn Văn A', 11),
('Hồi hộp quá, chờ chap tiếp theo!', GETDATE(), 'test-user-2', 'Trần Thị B', 11),
('Art đẹp, story cũng hay nữa. 5 sao ⭐⭐⭐⭐⭐', GETDATE(), 'test-user-3', 'Lê Văn C', 11),
('Phần này hơi buồn nhưng cảm động 😢', GETDATE(), 'test-user-4', 'Hoàng Thị D', 11);

-- Kiểm tra kết quả
SELECT * FROM Comments WHERE ChapterId = 11 ORDER BY CreatedAt DESC;
