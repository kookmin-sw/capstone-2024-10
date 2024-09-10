using TMPro;

public class UI_RemainPerson : UI_Base
{
    // Start is called before the first frame update
    enum Texts
    {
        PersonCount,
        RemainPerson
    }

    // Update is called once per frame
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<TMP_Text>(typeof(Texts));

        return true;
    }

    private void Update()
    {
        if (Managers.SceneMng.CurrentScene.IsSceneType((int)Define.SceneType.ReadyScene))
        {
            Hide();
        }
        else
        {
            Show();
            GetText((int)Texts.PersonCount).text = $"{Managers.GameMng.GameEndSystem.CrewNum} / {Define.PLAYER_COUNT-1}";
        }
    }

    public void Hide() => gameObject.SetActive(false);

    public void Show() => gameObject.SetActive(true);
}
