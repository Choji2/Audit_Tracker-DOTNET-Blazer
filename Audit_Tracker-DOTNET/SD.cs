namespace Main_SD
{
    public class SD
    {

        public readonly Record_stat[] statuses =
        {
            new()
            {
                 code= 0,
                 desc = "NOT READY"
            },
            new()
            {
                 code= 1,
                 desc = "READY FOR TAG OFFICE"
            },new()
            {
                 code= 2,
                 desc = "COMPLETED"
            }
        };

        public readonly  string[] labels = { "NOT READY", "READY FOR TAG OFFICE", "COMPLETED" };

        public readonly List<string> AllowedSIDs = new(){
        "S-1-5-21-4127812034-820336945-2256232113-513"//Domain Users
    };

        public const string UserDomain = "DOM\\";
        public const string Master = "Master";
        public const string Auditor = "Auditor";


    }


    public class Record_stat
    {
        public int code { get; set; }
        public string desc { get; set; }
    }
}
