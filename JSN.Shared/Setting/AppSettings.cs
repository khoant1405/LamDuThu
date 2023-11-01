using Microsoft.Extensions.Configuration;

namespace JSN.Shared.Setting;

public static class AppSettings
{
    private static IConfiguration _configurationBuilder;

    public static IConfiguration ConfigurationBuilder
    {
        get => _configurationBuilder;
        set
        {
            _configurationBuilder = value;
            LoadConfig();
        }
    }

    public static int ArticlePageSize { get; set; } = 20;
    public static JwtSetting JwtSetting { get; set; }
    public static List<SqlSetting> SqlSettings { get; set; }
    public static SqlSetting? DefaultSqlSetting { get; set; }
    public static RedisSetting RedisSetting { get; set; }

    public static void LoadConfig()
    {
        JwtSetting = new JwtSetting
        {
            ValidAudience = ConfigurationBuilder["JWT:ValidAudience"],
            ValidIssuer = ConfigurationBuilder["JWT:ValidIssuer"],
            Token = ConfigurationBuilder["JWT:Token"],
            TokenValidityInMinutes = Convert.ToInt32(ConfigurationBuilder["JWT:TokenValidityInMinutes"]),
            RefreshTokenValidityInDays = Convert.ToInt32(ConfigurationBuilder["JWT:RefreshTokenValidityInDays"])
        };

        SqlSettings = new List<SqlSetting>();
        var index = 0;
        var sqlName = ConfigurationBuilder.GetSection($"SQL:{index}:Name")
            .Value;
        while (!string.IsNullOrEmpty(sqlName))
        {
            SqlSettings.Add(new SqlSetting
            {
                Name = sqlName,
                ConnectString = ConfigurationBuilder.GetSection($"SQL:{index}:ConnectString")
                    .Value
            });
            index++;
            sqlName = ConfigurationBuilder.GetSection($"SQL:{index}:Name")
                .Value;
        }

        DefaultSqlSetting = SqlSettings.FirstOrDefault();

        RedisSetting = new RedisSetting
        {
            Servers = ConfigurationBuilder["Redis:Servers"],
            SentinelMasterName = ConfigurationBuilder["Redis:SentinelMasterName"],
            DbNumber = int.Parse(ConfigurationBuilder["Redis:DbNumber"]),
            AuthPass = ConfigurationBuilder["Redis:AuthPass"],
            IsSentinel = bool.Parse(ConfigurationBuilder["Redis:IsSentinel"]),
            WaitBeforeForcingMasterFailover = int.Parse(ConfigurationBuilder["Redis:WaitBeforeForcingMasterFailover"]),
            MaxPoolSize = int.Parse(ConfigurationBuilder["Redis:MaxPoolSize"]),
            ClientName = ConfigurationBuilder["Redis:ClientName"],
            IsUseRedisLazy = bool.Parse(ConfigurationBuilder["Redis:IsUseRedisLazy"]),
            ConnectTimeout = int.Parse(ConfigurationBuilder["Redis:MaxPoolSize"]),
            ConnectRetry = int.Parse(ConfigurationBuilder["Redis:MaxPoolSize"])
        };

        var articlePageSize = Convert.ToInt32(ConfigurationBuilder["ArticlePageSize"]);
        ArticlePageSize = articlePageSize != 0 ? articlePageSize : ArticlePageSize;
    }
}