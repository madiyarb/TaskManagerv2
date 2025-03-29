using TaskManager.Domain.Enums;

namespace TaskManager.Domain.DomainModels;

public class TaskDomainModel
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Phone { get; private set; }
    public Statuses Status { get; private set; }

    private TaskDomainModel()
    {
        
    }

    public TaskDomainModel(string name, string phone, Statuses status)
    {
        Name = name;
        Phone = phone;
        Status = status;
    }
}