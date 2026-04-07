using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;


namespace ExamPdpRasmBot.Data.Apiwork;

public class ApiResponse
{
    public class PexelsResponse
    {
        [JsonPropertyName("src")]
        public List<Photo> Photos { get; set; }
    }

    public class Photo
    {
        [JsonPropertyName("src")]
        public ImageSource Src { get; set; }
    }

    public class ImageSource
    {
        [JsonPropertyName("large")]
        public string Large { get; set; }
    }
}
