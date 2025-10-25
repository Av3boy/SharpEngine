using System;
using System.Collections.Generic;
using System.Text;

namespace AssetStore.Database.Models;

public class Comment
{
    public int Id { get; set; }
    public Guid AssetId { get; set; }
    public Guid UserId { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public Comment? ParentComment { get; set; }
    public IReadOnlyList<Comment> Replies { get; set; } = [];
}
