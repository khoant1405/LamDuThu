﻿using JSN.Shared.Config;
using StackExchange.Redis;

namespace JSN.Redis.Helper;

public class RedisHelper
{
    public static ConnectionMultiplexer? GetConnectionMultiplexer()
    {
        var config = AppConfig.RedisConfig;

        if (config.IsUseRedisLazy)
        {
            return null;
        }

        if (config.IsSentinel != true)
        {
            return ConnectionMultiplexer.Connect(GetConfigRedis());
        }

        var sentinelOptions = new ConfigurationOptions
        {
            TieBreaker = "",
            CommandMap = CommandMap.Sentinel,
            AbortOnConnectFail = false
        };
        var configRedis = GetConfigRedis();
        foreach (var item in configRedis.EndPoints)
        {
            sentinelOptions.EndPoints.Add(item);
        }

        configRedis.EndPoints.Clear();
        var sentinelConnection = ConnectionMultiplexer.Connect(sentinelOptions);
        return sentinelConnection.GetSentinelMasterConnection(configRedis);
    }

    public static ConfigurationOptions GetConfigRedis()
    {
        var config = AppConfig.RedisConfig;

        var servers = config.Servers?.Split(",");
        var endPointCollection = new EndPointCollection();
        if (servers != null)
        {
            foreach (var server in servers)
            {
                endPointCollection.Add(server);
            }
        }

        var configurationOptions = new ConfigurationOptions
        {
            EndPoints = endPointCollection,
            DefaultDatabase = config.DbNumber,
            AbortOnConnectFail = false
        };

        if (config.IsSentinel == true)
        {
            configurationOptions.ServiceName = config.SentinelMasterName;
        }

        if (config.ConnectTimeout > 0)
        {
            configurationOptions.ConnectTimeout = config.ConnectTimeout;
        }

        if (config.ConnectRetry > 0)
        {
            configurationOptions.ConnectRetry = config.ConnectRetry;
        }

        if (!string.IsNullOrEmpty(config.AuthPass))
        {
            configurationOptions.Password = config.AuthPass;
        }

        return configurationOptions;
    }
}

#region explain

//1. `abortConnect ={ bool}`:
//   -Mặc định: true(false on Azure).
//   - Nếu true, Connect sẽ không tạo kết nối khi không có máy chủ nào khả dụng.

//2. `allowAdmin={bool}`:
//   -Mặc định: false.
//   - Kích hoạt một loạt các lệnh được xem xét là nguy hiểm.

//3. `channelPrefix={string}`:
//   -Mặc định: null.
//   - Tiền tố tùy chọn cho tất cả các hoạt động pub/sub.

//4. `checkCertificateRevocation={bool}`:
//   -Mặc định: true.
//   - Xác định xem danh sách thu hồi chứng chỉ được kiểm tra trong quá trình xác thực hay không.

//5. `connectRetry={int}`:
//   -Mặc định: 3.
//   - Số lần lặp lại thử kết nối trong quá trình kết nối ban đầu.

//6. `connectTimeout={int}`:
//   -Mặc định: 5000(ms).
//   - Thời gian chờ(ms) cho các thao tác kết nối.

//7. `configChannel={string}`:
//   -Mặc định: "__Booksleeve_MasterChanged".
//   - Tên kênh phát sóng để truyền thông thay đổi cấu hình.

//8. `configCheckSeconds={int}`:
//   -Mặc định: 60.
//   - Thời gian(giây) kiểm tra cấu hình.

//9. `defaultDatabase={int}`:
//   -Mặc định: null.
//   - Chỉ mục cơ sở dữ liệu mặc định, từ 0 đến số cơ sở dữ liệu - 1.

//10. `keepAlive={int}`:
//    -Mặc định: -1.
//    - Thời gian(giây) gửi một tin nhắn để giữ kết nối sống (60 giây mặc định).

//11. `name ={ string}`:
//    -Mặc định: null.
//    - Xác định cho kết nối trong Redis.

//12. `password={string}`:
//    -Mặc định: null.
//    - Mật khẩu cho máy chủ Redis.

//13. `user={string}`:
//    -Mặc định: null.
//    - Người dùng cho máy chủ Redis (đối với ACL trên Redis phiên bản 6 và cao hơn).

//14. `proxy ={ proxy type}`:
//    -Mặc định: Proxy.None.
//    - Loại proxy sử dụng (nếu có).

//15. `resolveDns ={ bool}`:
//    -Mặc định: false.
//    - Xác định rằng việc giải quyết DNS sẽ là cách rõ ràng và tích cực.

//16. `serviceName={string}`:
//    -Mặc định: null.
//    - Sử dụng cho việc kết nối đến dịch vụ chính của Sentinel.

//17. `ssl={bool}`:
//    -Mặc định: false.
//    - Chỉ định việc sử dụng mã hóa SSL.

//18. `sslHost={string}`:
//    -Mặc định: null.
//    - Bắt buộc định danh máy chủ SSL trên chứng chỉ máy chủ.

//19. `sslProtocols={enum}`:
//    -Mặc định: null.
//    - Phiên bản SSL/TLS được hỗ trợ khi sử dụng kết nối mã hóa.

//20. `syncTimeout={int}`:
//    -Mặc định: 5000(ms).
//    - Thời gian(ms) cho các thao tác đồng bộ.

//21. `asyncTimeout={int}`:
//    -Mặc định: SyncTimeout.
//    - Thời gian(ms) cho các thao tác không đồng bộ.

//22. `tiebreaker={string}`:
//    -Mặc định: "__Booksleeve_TieBreak".
//    - Khóa sử dụng để chọn máy chủ trong tình huống chính mơ hồ.

//23. `version={string}`:
//    -Mặc định: (4.0 in Azure, else 2.0).
//    -Cấp độ phiên bản Redis (hữu ích khi máy chủ không cung cấp điều này).

//24. `tunnel ={ string}`:
//    -Mặc định: null.
//    - Đường hầm cho các kết nối (sử dụng http:{ proxy url}
//cho máy chủ proxy dựa trên "connect").

//25. `setlib ={ bool}`:
//    -Mặc định: true.
//    - Xác định liệu CLIENT SETINFO có nên sử dụng để đặt tên/thư viện phiên bản trên kết nối.

//26. `protocol={string}`:
//    -Mặc định: null.
//    - Giao thức Redis để sử dụng.

#endregion