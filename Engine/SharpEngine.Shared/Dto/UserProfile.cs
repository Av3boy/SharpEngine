using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEngine.Shared.Dto;

public class UserProfile
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password
    {
        get => _password;
        set => SetPassword(value);
    }

    private string _password;

    public List<SocialMedia> LinkedSocialMedia { get; set; } = [];

    public List<Project> Projects { get; set; } = [];

    public List<Achievement> Achievements { get; set; } = [];

    private void SetPassword(string pwd)
    {
        // TODO: #60 Figure out a good way to handle encryption
        _password = pwd;
    }
}

public class SocialMedia
{
    public string Name { get; set; }
    public string Url { get; set; }
}

public class Achievement
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? DateAchieved { get; set; }

    public bool IsAchieved => DateAchieved.HasValue;
}
