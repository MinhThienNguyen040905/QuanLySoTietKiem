--CREATE DATABASE QuanLySoTietKiem
--GO
--USE QuanLySoTietKiem
--GO

INSERT INTO Interest_Rates (Term, Rate)
VALUES 
	(1,3.55),
	(3,3.80),
	(6,4.35),
	(12,4.75),
	(18,5.25),
	(24,5.75)

INSERT INTO Users (Username, Password, Fullname, Email, Money, Gender, Dob, Address, Phone, Avatar, Identity_Card)
VALUES 
	('Buivankhoa1998', 'BuiVanKhoa1998@', N'Bùi Văn Khoa', 'buivankhoa01031998@gmail.com', 16500000, N'Nam', '1998-01-03', N'Số 606, Đường Phan Đình Phùng, Thành phố Đà Lạt, Lâm Đồng', '0706232150', 'https://res.cloudinary.com/dpnvyfwnp/image/upload/v1735444323/yhrrp3yxft1iksg4z732.png', '068098665120'), -- Nhan vat demo
	('VanAnhNguyen111990', 'NguyenVanAnh!19900101', N'Nguyễn Vân Anh', 'nguyenvananh1190@gmail.com', 1000000, N'Nữ', '1990-01-01', N'Số 123, Đường Láng, Quận Đống Đa, Hà Nội', '0901234567', 'https://res.cloudinary.com/dpnvyfwnp/image/upload/v1735568288/profile-1_qhopy1.png', '001190125360'),
	('TranthiBinh92', 'TranThiBinh1992#0202', N'Trần Thị Bình', 'tranthibinh19920202@gmail.com', 20000000, N'Nữ', '1992-02-02', N'Số 456, Đường Nguyễn Trãi, Quận 5, TP. Hồ Chí Minh', '0902345678', 'https://res.cloudinary.com/dpnvyfwnp/image/upload/v1735568309/profile-2_qpboe9.png', '07919223235'),
	('NgoVanGiang97', '%NgoVanGiang19971106', N'Ngô Văn Giang', 'ngovangiang1997116@gmail.com', 4500000, N'Nam', '1996-07-07', N'Số 404, Đường Hùng Vương, Thành phố Huế, Thừa Thiên Huế', '0907890123', 'https://res.cloudinary.com/dpnvyfwnp/image/upload/v1735568336/profile-4_wmn5bs.png', '046097128955')

INSERT INTO Group_Savings_Accounts (NAME,Creating_Date,Money,Target,Status,Description,Interest_Rate_ID)
VALUES 
     (N'Sổ tiết kiệm mua nhà','2024-05-31',13018000,150000000,N'Đang hoạt động',N'Tiết kiệm mua căn nhà mới',2),
	 (N'Sổ tiết kiệm mua ô tô','2024-09-02',8000000,100000000,N'Đang hoạt động',N'Tiết kiệm mua xe ô tô',5),
	 ----
     (N'Sổ tiết kiệm học phí con', '2024-12-01', 50000000, 100000000, N'Đang hoạt động', N'Tiết kiệm tiền học phí con', 4),
    (N'Sổ tiết kiệm du lịch Nhật Bản', '2024-12-15', 20000000, 70000000, N'Đang hoạt động', N'Trống', 3),

	 (N'Sổ tiết kiệm kinh doanh','2024-12-16',100000000,150000000,N'Đang hoạt động',N'Tiết kiệm kinh doanh',3)
	 


	 -- Nhom



INSERT INTO Group_Details(Saving_ID,Username,Total_Money,Is_Owner)
VALUES 
     (1,'Buivankhoa1998',5006000,1),
	 (1,'VanAnhNguyen111990',10006000,0),
	 (1,'TranthiBinh92',7006000,0),
	 (2,'Buivankhoa1998',5000000,1),
	 (2,'NgoVanGiang97',3000000,0),

	 --
	 (3, 'Buivankhoa1998', 30000000, 1),
    (3, 'VanAnhNguyen111990', 20000000, 0),
    (4, 'TranthiBinh92', 15000000, 1),
    (4, 'NgoVanGiang97', 5000000, 0),
	(5,'NgoVanGiang97',1000000,1)
	

INSERT INTO Group_Transactions_Information (Transaction_Date,Money,Description,Saving_ID,Username)
VALUES
       -- tạo sổ mua nhà
      ('2024-5-31 8:04:12',10000000,N'Tạo sổ tiết kiệm',1,'Buivankhoa1998'),
	  -- Anh nạp 10M
	  ('2024-5-31 9:15:18',10000000,N'Góp 10 triệu',1,'VanAnhNguyen111990'),
	  --Bình nạp 10 M
	  ('2024-5-31 10:47:04',10000000,N'Trống',1,'TranthiBinh92'),
	  -- ngày đáo hạn của sổ mua nhà
	  ('2024-8-31 0:0:0',18000,N'Trả lãi',1,null),
	  --tạo sổ tiết kiệm ô tô
	  ('2024-9-02 15:30:28',5000000,N'Tạo sổ tiết kiệm',2,'Buivankhoa1998'),
	  -- Giang nộp 3 triệu vào sổ tiết kiệm ô tô
	  ('2024-9-10 7:26:12',3000000,N'Trống',2,'NgoVanGiang97'),
	  --Khoa rút tiền từ sổ mua nhà
	  ('2024-10-11 11:02:10',-5000000,N'Mượn 5 triệu mua điện thoại',1,'Buivankhoa1998'),
	  --Bình rút 3 triệu từ sổ mua nhà
	  ('2024-10-25 7:03:10',-3000000,N'Lấy tiền sửa TV',1,'TranthiBinh92'),
	  ----
 -- Giao dịch tạo sổ học phí con
    ('2024-12-01 10:00:00', 30000000, N'Tạo sổ tiết kiệm', 3, 'Buivankhoa1998'),
    ('2024-12-01 13:30:00', 20000000, N'Góp tiền', 3, 'VanAnhNguyen111990'),

    -- Giao dịch tạo sổ du lịch Nhật Bản
    ('2024-12-15 14:00:00', 15000000, N'Tạo sổ tiết kiệm', 4, 'TranthiBinh92'),
    ('2024-12-15 14:30:00', 5000000, N'Góp tiền', 4, 'NgoVanGiang97'),

    -- Lãi suất
    ('2025-12-01 00:00:00', 2375000, N'Sổ tiết kiệm đến ngày đáo hạn', 3, NULL),
    ('2025-06-15 00:00:00', 435000,  N'Sổ tiết kiệm đến ngày đáo hạn', 4, NULL),

    -- Rút tiền
    ('2025-01-10 09:00:00', -10000000, N'Rút tiền học phí', 3, 'Buivankhoa1998'),
    ('2025-07-01 10:00:00', -5000000, N'Rút tiền du lịch', 4, 'TranthiBinh92'),

	--tạo sổ kinh doanh
	('2024-12-16 07:15:20',10000000,N'Tạo sổ tiết kiệm kinh doanh',5,'NgoVanGiang97'),

	--Bình nạp vào sổ tiết kiệm kinh doanh 5 triệu
	('2024-12-17 07:14:20',5000000,N'Tôi đóng góp 5 trẹo',5,'TranthiBinh92'),

	--Khoa chuyển từ số mua Iphone sang sổ kinh doanh
	('2024-12-20 9:34:34',10000000,N'Tôi phải lấy số tiền mua điện thoại cho mẹ để nạp vào sổ này',5,'Buivankhoa1998')
INSERT INTO Group_Notifications (Description,Type,Money,Username_Sender,Notification_Date,Saving_ID)
VALUES
      -- tạo sổ mua nhà
      (N'Kỳ hạn: 3 tháng-Lãi suất: 3.8%/năm-Tiết kiệm mua căn nhà mới',N'MờiYC',null,'Buivankhoa1998','2024-5-31 8:04:12',1),
	  -- Anh đồi ý vào sổ mua nhà, đồng ý
	  (N'Đồng ý',N'MờiPH',null,'VanAnhNguyen111990','2024-5-31 9:10:02',1),
	  -- Bình đồng ý vào sổ mua nhà, đồng ý
	  (N'Đồng ý',N'MờiPH',null,'TranthiBinh92','2024-5-31 10:38:00',1),
	  --Anh nạp 10 M vào sổ mua nhà, đồng ý
	  (N'Góp 10 triệu',N'Nạp',10000000,'VanAnhNguyen111990','2024-5-31 9:15:18',1),
	  --Bình Nạp 10 triệu vào sổ mua nhà
	  (N'Trống',N'Nạp',10000000,'TranthiBinh92','2024-5-31 10:47:04',1),
	  --Thông báo sổ mua nhà đến ngày đáo hạn
	  (N'Sổ tiết kiệm đã đến ngày đáo hạn',N'Lãi',285000,null,'2024-8-31 0:0:0',1),
	  -- tạo sổ mua ô tô
	  (N'Kỳ hạn: 18 tháng-Lãi suất: 5.25%/năm-Tiết kiệm mua xe ô tô',N'MờiYC',null,'Buivankhoa1998','2024-9-02 15:30:28',2),
	  -- Giang đồng ý vào sổ Ô tô
	  (N'Đồng ý',N'MờiPH',null,'NgoVanGiang97','2024-9-02 19:10:02',2),
	  -- Anh không đồng ý vào sổ ô tô
	  (N'Không đồng ý',N'MờiPH',null,'VanAnhNguyen111990','2024-9-02 20:16:03',2),
	  -- Giang nộp 3 triệu vào sổ tiết kiệm ô tô
	  (N'Trống',N'Nạp',3000000,'NgoVanGiang97','2024-9-10 7:26:12',2),

	  --Khoa rút 5 triệu từ sổ mua nhà
	  (N'Mượn 5 triệu mua điện thoại',N'Rút',-5000000,'Buivankhoa1998','2024-10-11 11:02:10',1),
	  -- Anh Gửi yêu cầu rút 3 triệu từ sổ mua nhà cho trưởng nhóm: Khoa
	  (N'Xin rút tiền',N'RútYC',3000000,'VanAnhNguyen111990','2024-10-15 20:02:10',1),
	  --Thông báo Rút PH việc Khoa từ chối việc rút YC của Anh
	  (N'Không đồng ý',N'RútPH',5000000,'Buivankhoa1998','2024-10-20 10:02:10',1),
	  --Bình muốn rút 3 triệu từ sổ Mua nhà
	  (N'Lấy tiền sửa TV',N'RútYC',3000000,'TranthiBinh92','2024-10-22 6:02:10',1),
	  --Thông báo RÚt PH Cho Bình biết Khoa đã chấp nhận rút YC
	  (N'Đồng ý',N'RútPH',3000000,'Buivankhoa1998','2024-10-25 7:03:10',1),

	  --Thông báo cho các thành viên còn lại trong mua nhà Bình rút 3 triệu
	  (N'Lấy tiền sửa TV',N'Rút',-3000000,'TranthiBinh92','2024-10-25 7:03:10',1),

	  ----

	  --17
	-- Thông báo tạo sổ học phí con
    (N'Kỳ hạn: 12 tháng-Lãi suất: 4.75%/năm-Tiết kiệm tiền học phí con', N'MờiYC', NULL, 'Buivankhoa1998', '2024-12-01 10:00:00', 3),
	--18
    -- Thông báo thành viên đồng ý hoặc không đồng ý
    (N'Đồng ý', N'MờiPH', NULL, 'VanAnhNguyen111990', '2024-12-01 12:00:00', 3),
	--19
	  ---thông báo nạp tiền học phí
	  (N'Nạp tiền vào sổ',N'Nạp',20000000,'VanAnhNguyen111990','2024-12-01 13:30:00',3),

	  --20
    -- Thông báo tạo sổ du lịch Nhật Bản
    (N'Kỳ hạn: 6 tháng-Lãi suất: 4.35%/năm-Trống', N'MờiYC', NULL, 'TranthiBinh92', '2024-12-15 14:00:00', 4),
	--21
	--thông báo mời phản hổi
	(N'Đồng ý',N'MờiPH',NULL,'NgoVanGiang97','2024-12-15 15:00:00',4),
	--22
	--Giang  góp tiền
	(N'Góp tiền',N'Nạp',5000000,'NgoVanGiang97','2024-12-15 14:30:00',4),

	--23
	--Thông báo lãi sổ 3
	(N'Sổ tiết kiệm đến này đáo hạn',N'Lãi',2375000,null,'2025-12-01 00:00:00',3),
	--24
	--thông báo lãi sổ 4
     (N'Sổ tiết kiệm đến này đáo hạn',N'Lãi',435000,null,'2025-06-15 00:00:00',4),

	 --25
	 --trưởng nhóm rút tiền
	 (N'Rút tiền trả nợ',N'Rút',-10000000,'Buivankhoa1998','2025-01-10 09:00:00',3),

	 --26
	 --trưởng nhóm rút tiền
	 (N'Rút tiền mua sắm',N'Rút',-5000000,'TranthiBinh92','2025-07-01 10:00:00',4),

	 --27
	 --Mời vào sổ kinh doanh
	 (N'Kỳ hạn: 18 tháng-Lãi suất: 5,25%/năm-Khởi nghiệp bán quần áo',N'MờiYC',10000000,'NgoVanGiang97','2024-12-16 07:15:20',5),

	 --28
	  --Khoa đồng ý vào sổ kinh doanh
	  (N'Đồng ý',N'MờiPH',null,'Buivankhoa1998','2024-12-16 07:30:20',5),

	  --29
	  --bình đồng ý vào nhóm
	  (N'Đồng ý',N'MờiPH',null,'TranthiBinh92','2024-12-16 08:30:20',5),

	  --30
	  --Bình nạp 5 trẹo vào kinh doanh
	  (N'Tôi đóng góp 5 trẹo',N'Nạp',5000000,'TranthiBinh92','2024-12-17 07:14:20',5),

	  --31
	  --Khoa chuyển tiền từ sổ mua điện thoại sang kinh doanh
	  (N'Tôi phải chuyển tiền mua điện thoại cho mẹ sang sổ này',N'Nạp',10000000,'Buivankhoa1998','2024-12-20 9:34:34',5)

INSERT INTO Group_Notifications_Details (Group_Notification_ID,Username,IS_Deleted,Status)
VALUES
      --Anh nhận thông báo mời YC STK mua nhà, đồng ý
      (1,'VanAnhNguyen111990',0,N'Đồng ý'),
	  --Bình nhận thông báo mời YC STK mua nhà, đồng ý
	  (1,'TranthiBinh92',0,N'Đồng ý'),
	  --Khoa nhận thông báo Mời PH từ Anh, đã đọc
	  (2,'Buivankhoa1998',0,N'Chưa đọc'),
	   --Khoa nhận thông báo Mời PH từ Bình, đã đọc
	  (3,'Buivankhoa1998',0,N'Đã đọc'),
	  -- Khoa nhận thông báo Anh nạp 10, triệu đã đọc
	  (4,'Buivankhoa1998',0,N'Đã đọc'),
	  -- Bình nhận thông báo Anh nạp 10 triệu, đã đọc
	  (4,'TranthiBinh92',0,N'Đã đọc'),
	  --Khoa nhận thông báo Bình nạp 10 triệu, đã đọc
	  (5,'Buivankhoa1998',0,N'Chưa đọc'),
	  --Anh nhận thông báo Bình nạp 1o trieuj, đã đọc
	  (5,'VanAnhNguyen111990',0,N'Chưa đọc'),
	  -- Khoa nhận thông báo sổ mua nhà đến ngày đáo hạn
	  (6,'Buivankhoa1998',0,N'Đã đọc'),
	  -- Anh nhận thông báo sổ mua nhà đến ngày đáo hạn
	  (6,'VanAnhNguyen111990',0,N'Chưa đọc'),
	  --Bình nhận thông báo sổ mua nhà đến ngày đáo hạn
	  (6,'TranthiBinh92',0,N'Đã đọc'),

	  --Mời Giang vào sổ mua ô tô, đồng ý
	   (7,'NgoVanGiang97',0,N'Đồng ý'),
	   --Mời Anh vào sổ mua ô tô, không đồng ý
	   (7,'VanAnhNguyen111990',0,N'Không đồng ý'),

	   --Giang đồng ý vào ô tô
	   (8,'VanAnhNguyen111990',0,N'Chưa đọc'),
	   --Anh không đồng ý vào ô tô
	   (9,'VanAnhNguyen111990',0,N'Đã đọc'),
	   --thông báo cho khoa về việc Giang nộp 3 triệu vào ô tô
	   (10,'Buivankhoa1998',0,N'Chưa đọc'),
	   --Thông báo cho Anh việc Khoa rút 5 triệu từ sổ mua  nhà
	   (11,'VanAnhNguyen111990',0,N'Đã đọc'),
	   --Thông báo cho Bình việc Khoa rút 5 triệu từ sổ mua nhà
	   (11,'TranthiBinh92',0,N'Đã đọc'),
	  --Khoa không đồng ý Rút YC 3 triệu của Anh
	    (12,'Buivankhoa1998',0,N'Không đồng ý'),
	  -- Thông báo Rút PH việc Khoa từ chối rút YC của Anh
	     (13,'VanAnhNguyen111990',0,N'Không đồng ý'),
	 --Thông báo cho khoa biết Bình muốn rút 3 triệu từ sổ Mua nhà, Khoa đồng ý
	    (14,'Buivankhoa1998',0,N'Đồng ý'),
	 --Thông báo Rút PH cho Bình biết Khoa đã chấp nhận Rút YC 3 triệu
	   (15,'TranthiBinh92',0,N'Chưa đọc'),
	   -- Thông báo Rút cho khoa, Bình rut 3 triệu
	   (16,'Buivankhoa1998',0,N'Chưa đọc'),
	   --Thông báo rút cho Anh, Bình rút 3 triệu
	   (16,'VanAnhNguyen111990',0,N'Đã đọc'),

	   ---
	 -- Thông báo sổ học phí con
    (17, 'VanAnhNguyen111990', 0, N'Đồng ý'),
	--phản hổi chấp nhận
	(18,'Buivankhoa1998',0,N'Đã đọc'),
		--nạp
	  (19,'Buivankhoa1998' , 0, N'Chưa đọc'),

	  --thông báo đến giang về sổ du lịch nhật bản
	  (20,'NgoVanGiang97',0,N'Đồng ý'),
	  --thông báo mời phan hổi
	  (21,'TranthiBinh92',0,N'Đã đọc'),
	  (22,'TranthiBinh92',0,N'Đã đọc'),
	  --thông báo lãi sổ 3
	   (23,'VanAnhNguyen111990',0,N'Đã đọc'),
	   (23,'Buivankhoa1998',0,N'Đã đọc'),
	   --thong báo lãi sổ 4
	   (24,'TranthiBinh92',0,N'Đã đọc'),
	   (24,'NgoVanGiang97',0,N'Đã đọc'),

	   (25,'VanAnhNguyen111990',0,N'Đã đọc'),

	   --thông báo rút
	   (26,'NgoVanGiang97',0,N'Đã đọc'),
	   

	   --mời khoa vào sổ kinh doanh, khoa đồng ý
	   (27,'Buivankhoa1998',0,N'Đồng ý'),

	   --Khoa đồng ý vào nhóm
	   (28,'NgoVanGiang97',0,N'Đồng ý'),

	  --Bình đồng ý vào kinh doanh
	  (29,'NgoVanGiang97',0,N'Đồng ý'),

	  --Thông báo cho giang việc bình đóng 5 trẹo
	
	  (30,'NgoVanGiang97',0,N'Chưa đọc'),

	  --thông báo cho khoa việc bình đóng 5 triệu
	  (30,'Buivankhoa1998',0,N'Đã đọc'),


	  --thông báo cho giang biết khoa đã nạp tiền
	  (31,'NgoVanGiang97',0,N'Đã đọc'),

	  --thông báo cho bình việc khoa đóng 5 triệu
	  (31,'TranthiBinh92',0,N'Đã đọc')

INSERT INTO Personal_Savings_Accounts (Name, Creating_Date, Money, Target, Status, Description, Username, Interest_Rate_ID)
VALUES 
	(N'Tiết kiệm mua xe', '2024-05-20', 13507075, 40000000, N'Đang hoạt động', N'Để dành tiền mua xe mới', 'Buivankhoa1998', 3), -- 7.500.000 + 500.000 + 1.900.000 + 1.000.000  + lai (237.075) + 320.000 + 1.000.000 + 1.050.000
	(N'Tiền sắm Tết', '2024-12-28', '1700000', null, N'Đang hoạt động', N'Tiết kiệm tiền chuẩn bị cho tết 2025', 'Buivankhoa1998', 1), -- 1.200.000 + 500.000 
	(N'Quỹ khẩn cấp', '2024-11-12', '2220519', null, N'Đang hoạt động', N'Dự phòng cho các tình huống bất ngờ', 'Buivankhoa1998', 1), -- 1.500.000 + 500.000 + lai (519) + 220.000
	(N'Mua vàng', '2024-12-12', 1200000, 50000000, N'Đang hoạt động', N'Trống', 'VanAnhNguyen111990', 2), -- 1200000
	(N'Tiết kiệm mua xe', '2025-01-01', 2000000, 60000000, N'Đang hoạt động', 'Trống', 'TranthiBinh92', 4), --2000000
	(N'Sổ tiết kiệm mua Iphone 16','2024-05-25',12000000,25000000,N'Đang hoạt động',N'Mua Iphone 16 tặng mẹ','Buivankhoa1998',3)

INSERT INTO Personal_Notifications (Money_Rate, Is_Read, Notification_Date, Is_Deleted, Saving_ID)
VALUES 
	('237075', 0, '2024-11-20 01:11:23', 0, 1),
	('519', 0, '2024-12-12 00:00:52', 0, 3),
	('950000',0, '2024-01-25 00:00:52',0,6)
INSERT INTO Personal_Transactions_Information (Saving_ID, Transaction_Date, Money, Description)
VALUES 
	(1, '2024-05-20 12:11:46', 7500000, N'Tạo sổ tiết kiệm'),
	(1, '2024-06-01 11:22:12', 500000, N'Trống'),
	(1, '2024-07-15 18:00:12', 1900000, N'Trích tiền lương'),
	(1, '2024-10-22 9:23:33', 1000000, N'Trống'),
	(1, '2024-11-20 00:00:03', 237075, N'Trả lãi'),
	(1, '2024-11-30 20:20:02', 320000, N'Tiền thưởng thêm'),
	(1, '2024-12-15 06:50:33', 1000000, N'Trích tiền lương'),
	(1, '2024-12-30 10:15:56', 1050000, N'Trống'),
	(2, '2024-12-28 11:23:33', 1200000, N'Tạo sổ tiết kiệm'),
	(2, '2025-01-01 12:20:25', 500000, N'Trống'),
	(3, '2024-11-12 15:21:39', 1500000, N'Tạo sổ tiết kiệm'),
	(3, '2024-11-30 12:12:21', 500000, N'Trống'),
	(3, '2024-12-12 00:00:01', 519, N'Trả lãi'),
	(3, '2024-12-15 06:48:23', 220000, N'Trích tiền lương'),
	(4, '2024-12-12 11:11:12', 1200000, N'Tạo sổ tiết kiệm'),
	(5, '2025-01-01 12:32:23', 2000000, N'Tạo sổ tiết kiệm'),
	(6, '2024-12-20 9:34:34',-10000000,N'Chuyển tiền đến Sổ tiết kiệm kinh doanh (nhóm)')
