namespace JSN.Shared.Setting
{
    public class JwtSetting
    {
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string Token { get; set; }
        public int TokenValidityInMinutes { get; set; }
        public int RefreshTokenValidityInDays { get; set; }
    }
}
