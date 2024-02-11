namespace BillOMat.BlazorUI
{
    public class BillOMatApiClient(HttpClient httpClient)
    {
        public async Task<Patient[]> GetPatientsAsync()
        {
            return await httpClient.GetFromJsonAsync<Patient[]>("api/patients") ?? [];
        }

        public record Patient(int Id, string Firstname, string Lastname, string Nickname);
    }
}
