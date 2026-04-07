using System;
using System.Collections.Generic;
using System.Text;

namespace ExamPdpRasmBot.Model;

public class UserModel
{
    public long ChatId { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsAuthenticated { get; set; }
}
