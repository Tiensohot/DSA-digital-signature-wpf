1. Mở đầu – Giới thiệu đề tài

Trong phần trình bày này, tôi giới thiệu ứng dụng minh họa chữ ký số DSA triển khai bằng C#/.NET 8 với giao diện WPF theo mô hình MVVM.
Mục tiêu chính của hệ thống là hiện thực trọn vẹn quy trình sinh khóa – ký – xác thực chữ ký số đối với văn bản, đồng thời tổ chức mã nguồn theo kiến trúc tách lớp rõ ràng giữa lớp mật mã và lớp giao diện người dùng, phù hợp bối cảnh các môn Mật mã học, An toàn thông tin và Lập trình .NET.


2. Bối cảnh và cơ sở lý thuyết tóm tắt

Về mặt lý thuyết, DSA – Digital Signature Algorithm – là một thuật toán chữ ký số khóa công khai dựa trên độ khó của bài toán logarit rời rạc. Hệ thống sử dụng các tham số (p,q,g)(p,q,g)(p,q,g), với khóa bí mật xxx và khóa công khai y=gx mod py = g^x \bmod py=gxmodp.
Khi ký một thông điệp mmm, dữ liệu trước hết được băm bởi một hàm băm mật mã; trong cài đặt này là SHA-256. Thuật toán DSA tạo ra cặp số (r,s)(r,s)(r,s) đóng vai trò chữ ký số. Bên kiểm tra, với thông điệp, chữ ký và khóa công khai, sẽ tính lại một giá trị vvv; nếu vvv trùng với rrr thì chữ ký được xem là hợp lệ, đảm bảo đồng thời tính xác thực chủ thể và tính toàn vẹn nội dung.
Trong chương trình, các bước toán học này được hiện thực thông qua lớp System.Security.Cryptography.DSA và các phương thức SignData và VerifyData của .NET.


3. Kiến trúc hệ thống

Về kiến trúc, dự án được tách thành hai phần chính.
Thứ nhất, thư viện mật mã CryptoLib:
– Lớp DsaKeyPair mô tả cấu trúc cặp khóa, bao gồm khóa công khai, khóa bí mật dưới dạng mảng byte và độ dài khóa.
– Interface IDsaService xác định ba thao tác cốt lõi: GenerateKey để sinh khóa, Sign để ký văn bản và Verify để xác thực chữ ký.
– Lớp DsaService hiện thực interface trên, sử dụng DSA.Create() để sinh khóa, ExportSubjectPublicKeyInfo và ExportPkcs8PrivateKey để xuất khóa theo các chuẩn X.509 và PKCS#8, đồng thời dùng SignData/VerifyData với HashAlgorithmName.SHA256.
Thứ hai, ứng dụng giao diện DsaWpfApp:
– Lớp MainViewModel đóng vai trò ViewModel trung tâm trong mô hình MVVM, lưu trữ trạng thái cần thiết cho giao diện: danh sách độ dài khóa, khóa đang sử dụng, nội dung cần ký, chữ ký dạng Base64, thông điệp kết quả xác thực và trạng thái hệ thống.
– Các thao tác của người dùng như “Generate Key”, “Sign”, “Verify” được ánh xạ sang các ICommand thông qua lớp RelayCommand, giúp tách logic xử lý khỏi code-behind và tuân thủ mô hình MVVM.
– Giao diện MainWindow tổ chức thành ba tab: Khóa DSA, Ký văn bản và Xác thực chữ ký, toàn bộ điều khiển (ComboBox, TextBox, Button) được liên kết dữ liệu (data binding) tới các thuộc tính trong MainViewModel.


4. Quy trình vận hành của ứng dụng

Quy trình sử dụng ứng dụng có thể tóm tắt như sau.
Bước 1 – Sinh khóa:
Người dùng chọn độ dài khóa, ví dụ 2048 bit, sau đó kích hoạt lệnh Generate Key. ViewModel gọi IDsaService.GenerateKey, nhận về đối tượng DsaKeyPair, chuyển khóa công khai và khóa bí mật thành chuỗi Base64 và hiển thị trên tab “Khóa DSA”.
Bước 2 – Ký văn bản:
Tại tab “Ký văn bản”, người dùng nhập nội dung cần ký. Khi nhấn Ký, ViewModel gọi IDsaService.Sign, trong đó dữ liệu được mã hóa UTF-8, băm bằng SHA-256 và ký bằng khóa bí mật hiện hành. Chữ ký dạng mảng byte được mã hóa Base64 và hiển thị để dễ sao chép hoặc lưu trữ.
Bước 3 – Xác thực chữ ký:
Tại tab “Xác thực chữ ký”, người dùng cung cấp lại nội dung văn bản và chữ ký Base64. Lệnh Verify sẽ giải mã chữ ký Base64 về dạng nhị phân, gọi IDsaService.Verify với khóa công khai tương ứng. Kết quả xác thực được phản ánh trực quan qua thông báo “Chữ ký HỢP LỆ” hoặc “Chữ ký KHÔNG hợp lệ”, kèm mã màu để dễ quan sát. Mọi thay đổi trạng thái được cập nhật tự động nhờ cơ chế INotifyPropertyChanged trong ViewModel.


5. Nhận xét và hướng mở rộng

Về mặt kỹ thuật, hệ thống hiện thực đầy đủ các thao tác cơ bản của chữ ký số DSA trên văn bản thuần, đồng thời minh họa rõ cách tích hợp thư viện mật mã .NET vào một ứng dụng WPF sử dụng MVVM. Cấu trúc tách riêng CryptoLib giúp việc thay thế hoặc mở rộng sang các thuật toán khác như RSA hoặc ECDSA trở nên tương đối thuận lợi.
Hạn chế hiện tại là ứng dụng mới tập trung vào dữ liệu dạng văn bản, chưa xử lý trực tiếp các định dạng tài liệu như PDF hoặc DOCX, và chưa triển khai các cơ chế quản lý vòng đời khóa như bảo vệ khóa bí mật bằng mật khẩu hoặc thu hồi khóa. Hướng phát triển tiếp theo có thể bao gồm: hỗ trợ ký file, lưu–tải khóa và chữ ký dưới dạng file chuẩn, cũng như bổ sung các chỉ số so sánh hiệu năng giữa DSA và các thuật toán chữ ký số khác.


6. Kết luận

Tổng kết lại, hệ thống cho thấy việc kết hợp DSA trong System.Security.Cryptography với kiến trúc MVVM trên WPF cho phép xây dựng một công cụ minh họa chữ ký số có cấu trúc rõ ràng, dễ mở rộng và phù hợp bối cảnh học thuật. Ứng dụng có thể được sử dụng như một công cụ trực quan hỗ trợ giảng dạy và tự học về chữ ký số, cũng như làm nền tảng cho các nghiên cứu và bài toán thực tế phức tạp hơn trong lĩnh vực an toàn thông tin.
