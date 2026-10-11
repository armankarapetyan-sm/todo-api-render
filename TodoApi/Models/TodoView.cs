namespace TodoApi.Models;

public class TodoView
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Owner { get; set; } = "";
    public string Title { get; set; } = "";
    public bool Done { get; set; }

    public static TodoView From(TodoItem item, string owner)
    {
        return new TodoView
        {
            Id = item.Id,
            UserId = item.UserId,
            Owner = owner,
            Title = item.Title,
            Done = item.Done
        };
    }
}
