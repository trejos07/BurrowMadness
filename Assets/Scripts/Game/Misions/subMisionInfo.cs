[System.Serializable]
public class subMisionInfo
{
    public string ItemName;
    public int Amount;
    public int Redward;
    public bool completed;

    public subMisionInfo()
    {
    }

    public subMisionInfo(string ItemName, int Amount, int Redward ,bool completed)
    {
        this.Amount = Amount;
        this.ItemName = ItemName;
        this.Redward = Redward;
        this.completed = completed;
}
}
