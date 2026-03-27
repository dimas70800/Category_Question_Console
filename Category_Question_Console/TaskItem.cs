namespace Category_Question_Console
{
    internal class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<SubTask> SubTasks { get; set; } = new List<SubTask>();
        public string Category { get; set; }
    }

    internal class SubTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}