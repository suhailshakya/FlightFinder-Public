namespace Vacation.Services
{
    public class SharedDataService
    {
        public string SharedToken { get; set; }
        public Task<List<string>> SharedIataData { get; set; }
        public DateTime departDate { get; set; }
        public DateTime returnDate { get; set; }
        public string departCode { get; set; }
        public string arrivalCity { get; set; }
        public string emailAddress { get; set; }
        public string userCred = "";

        public string getCorrectUser()
        {
            ReadFile rf = new ReadFile();
            userCred = rf.ReadFileCreds("user");
            return userCred;
        }
    }
}
