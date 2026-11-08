using TriangleLab.Models;

namespace TriangleLab.Services;

public class UserStore
{
    private readonly object gate = new();
    private readonly List<User> users = new()
    {
        new() { Id = 1, Name = "Olena", Email = "olena@example.com" },
        new() { Id = 2, Name = "Taras", Email = "taras@example.com" }
    };

    public List<User> All()
    {
        lock (gate) return users.Select(Copy).ToList();
    }

    public User? Find(int id)
    {
        lock (gate) return users.FirstOrDefault(u => u.Id == id) is { } user ? Copy(user) : null;
    }

    public bool Add(User user)
    {
        lock (gate)
        {
            if (users.Any(u => u.Id == user.Id))
                return false;
            users.Add(Copy(user));
            return true;
        }
    }

    private static User Copy(User user) => new() { Id = user.Id, Name = user.Name, Email = user.Email };
}
