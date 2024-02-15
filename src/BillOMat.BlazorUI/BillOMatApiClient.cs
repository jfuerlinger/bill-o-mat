namespace BillOMat.BlazorUI
{
    public class BillOMatApiClient(HttpClient httpClient)
    {
        public record Patient(int Id, string Firstname, string Lastname, string Nickname);
        public async Task<Patient[]> GetPatientsAsync()
        {
            return await httpClient.GetFromJsonAsync<Patient[]>("patients") ?? [];
        }

        public record Configuration(string GrafanaUrl);

        public async Task<Configuration?> GetConfiguration()
        {
            return await httpClient.GetFromJsonAsync<Configuration>("startup");
        }
    }
}
