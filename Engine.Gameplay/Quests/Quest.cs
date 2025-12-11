namespace Engine.Gameplay.Quests;

public sealed class Quest
{
    public string Id { get; }
    public string Title { get; }
    public int KillTarget { get; }
    public QuestStatus Status { get; private set; }
    public int Kills { get; private set; }

    public Quest(string id, string title, int killTarget)
    {
        Id = id;
        Title = title;
        KillTarget = killTarget;
        Status = QuestStatus.NotStarted;
    }

    public void Start()
    {
        if (Status == QuestStatus.NotStarted)
        {
            Status = QuestStatus.InProgress;
        }
    }

    public void OnKill()
    {
        if (Status != QuestStatus.InProgress) return;
        Kills++;
        if (Kills >= KillTarget)
        {
            Status = QuestStatus.Completed;
        }
    }

    public void TurnIn()
    {
        if (Status == QuestStatus.Completed)
        {
            Status = QuestStatus.TurnedIn;
        }
    }
}
