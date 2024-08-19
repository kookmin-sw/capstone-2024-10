public class GameManager
{
    public MapSystem MapSystem { get; set; }
    public PlanSystem PlanSystem { get; set; }
    public RenderingSystem RenderingSystem { get; set; }
    public GameEndSystem GameEndSystem { get; set; }
    public SettingSystem SettingSystem { get; set; }
    public Define.GameResultType GameResult { get; set; } = Define.GameResultType.NotDecided;
}
