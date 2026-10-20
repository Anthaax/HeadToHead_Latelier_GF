namespace HeadToHead_Latelier.Common.Data
{
    public class Player
    {
        public int Id { get; set; }
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Shortname { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public Country Country { get; set; } = new();
        public string Picture { get; set; } = string.Empty;
        public PlayerData Data { get; set; } = new();
    }
}
