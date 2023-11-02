using JSN.Shared.Utilities;
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
            ValidAudience = ConvertHelper.ToString(ConfigurationBuilder["JWT:ValidAudience"]),
            ValidIssuer = ConvertHelper.ToString(ConfigurationBuilder["JWT:ValidIssuer"]),
            Token = ConvertHelper.ToString(ConfigurationBuilder["JWT:Token"]),
            TokenValidityInMinutes = ConvertHelper.ToInt32(ConfigurationBuilder["JWT:TokenValidityInMinutes"]),
            RefreshTokenValidityInDays = ConvertHelper.ToInt32(ConfigurationBuilder["JWT:RefreshTokenValidityInDays"])
        };

        SqlSettings = new List<SqlSetting>();
        var index = 0;
        var sqlName = ConvertHelper.ToString(ConfigurationBuilder.GetSection($"SQL:{index}:Name")
            .Value);
        while (!string.IsNullOrEmpty(sqlName))
        {
            SqlSettings.Add(new SqlSetting
            {
                Name = sqlName,
                ConnectString = ConvertHelper.ToString(ConfigurationBuilder.GetSection($"SQL:{index}:ConnectString")
                    .Value)
            });
            index++;
            sqlName = ConvertHelper.ToString(ConfigurationBuilder.GetSection($"SQL:{index}:Name")
                .Value);
        }

        DefaultSqlSetting = SqlSettings.FirstOrDefault();

        RedisSetting = new RedisSetting
        {
            Servers = ConvertHelper.ToString(ConfigurationBuilder["Redis:Servers"]),
            SentinelMasterName = ConvertHelper.ToString(ConfigurationBuilder["Redis:SentinelMasterName"]),
            DbNumber = ConvertHelper.ToInt32(ConfigurationBuilder["Redis:DbNumber"]),
            AuthPass = ConvertHelper.ToString(ConfigurationBuilder["Redis:AuthPass"]),
            IsSentinel = ConvertHelper.ToBoolean(ConfigurationBuilder["Redis:IsSentinel"]),
            IsUseRedisLazy = ConvertHelper.ToBoolean(ConfigurationBuilder["Redis:IsUseRedisLazy"]),
            ConnectTimeout = ConvertHelper.ToInt32(ConfigurationBuilder["Redis:MaxPoolSize"]),
            ConnectRetry = ConvertHelper.ToInt32(ConfigurationBuilder["Redis:MaxPoolSize"])
        };

        var articlePageSize = ConvertHelper.ToInt32(ConfigurationBuilder["ArticlePageSize"]);
        ArticlePageSize = articlePageSize != 0 ? articlePageSize : ArticlePageSize;
    }
}