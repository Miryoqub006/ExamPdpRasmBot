using ExamPdpRasmBot.Model;
using System.Text.Json;
using System.Text.Json.Serialization;
using static ExamPdpRasmBot.Data.Apiwork.ApiResponse;

namespace ExamPdpRasmBot.Data
{
    public class UserDatas
    {
        private const string _filePath = "users.json";
        private static readonly HttpClient _httpClient = new HttpClient();


        private const string PexelsApiKey = "BZ7ePfqQhH5Ft9cKmJ2wXrL8nY3vB6dG9aZ1sM0q";



        public static List<UserModel> GetUsers()
        {
            if (!File.Exists(_filePath)) return new List<UserModel>();
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<UserModel>>(json) ?? new List<UserModel>();
        }



        public static void SaveUser(UserModel user)
        {
            var users = GetUsers();
            users.RemoveAll(u => u.ChatId == user.ChatId);
            users.Add(user);
            File.WriteAllText(_filePath, JsonSerializer.Serialize(users));
        }



        // Pexels API'dan rasmlarni olish
        public static async Task<List<string>> GetImagesByKeyword(string keyword)
        {
            var images = new List<string>();

            try
            {
                string apiUrl = $"https://api.pexels.com/v1/search?query={keyword}&per_page=3";

                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("Authorization", PexelsApiKey);

                // 3. API'ga so'rov yuboramiz
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    var data = JsonSerializer.Deserialize<PexelsResponse>(jsonResponse);

                    if (data?.Photos != null)
                    {
                        foreach (var photo in data.Photos.Take(3))
                        {
                            if (!string.IsNullOrEmpty(photo.Src?.Large))
                            {
                                images.Add(photo.Src.Large);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Rasm qidiruvda xatolik: {ex.Message}");
            }

            
            return images;
        }

        
    }
}