using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CrewPlan : UI_Base
{
    private UI_PlanPanel _batteryChargePlan;
    private UI_PlanPanel _planA;
    private UI_PlanPanel _planB;
    private UI_PlanPanel _planC;

    enum GameObjects
    {
        Plan_BatteryCharge,
        PlanA,
        PlanB,
        PlanC,
    }

    enum Texts
    {
        Main_Title_text
    }

    private class UI_PlanPanel : UI_Base
    {
        private Tweener _colorTweener;
        private TMP_Text _objectiveText;
        private TMP_Text _hintText;

        enum Texts_PlanPanel
        {
            Objective_Text,
            Hint_Text,
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Bind<TMP_Text>(typeof(Texts_PlanPanel));
            _objectiveText = GetText(Texts_PlanPanel.Objective_Text);
            _hintText = GetText(Texts_PlanPanel.Hint_Text);
            gameObject.SetActive(false);
            return true;
        }

        public void SetObjectiveText(string text, bool isProgress, bool isComplete = false)
        {
            KillTween();
            _objectiveText.text = text;
            if (isProgress)
            {
                _colorTweener = _objectiveText.DOColor(Color.green, 0.1f).OnComplete(() =>
                {
                    if (isComplete) return;
                    _colorTweener = _objectiveText.DOColor(Color.white, 2f);
                });
            }
        }

        public void SetHintText(string text)
        {
            _hintText.text = text;
            _objectiveText.margin = string.IsNullOrEmpty(text) ? new Vector4(0, 0, 0, 0) : new Vector4(0, 0, 0, 35);
        }

        public IEnumerator CompleteObjective(string nextObjective, string nextHint)
        {
            yield return _colorTweener.WaitForCompletion();
            yield return new WaitForSeconds(1f);
            _objectiveText.fontStyle = FontStyles.Strikethrough;

            yield return _objectiveText.DOColor(Color.gray, 1f).WaitForCompletion();

            if (string.IsNullOrEmpty(nextObjective)) yield break;

            yield return new WaitForSeconds(1f);

            yield return _objectiveText.DOFade(0f, 0.5f).WaitForCompletion();

            _objectiveText.DOFade(1f, 1f);
            _objectiveText.fontStyle = FontStyles.Normal;
            _objectiveText.color = Color.white;
    
            
            SetObjectiveText(nextObjective, false);
            SetHintText(nextHint);

        }

        private void KillTween()
        {
            _colorTweener.Kill();
            _colorTweener = null;
        }

    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<GameObject>(typeof(GameObjects));
        Bind<TMP_Text>(typeof(Texts));

        GetText(Texts.Main_Title_text).SetText("Restore power of laboratory!");
        _batteryChargePlan = GetObject(GameObjects.Plan_BatteryCharge).GetOrAddComponent<UI_PlanPanel>();
        _planA = GetObject(GameObjects.PlanA).GetOrAddComponent<UI_PlanPanel>();
        _planB = GetObject(GameObjects.PlanB).GetOrAddComponent<UI_PlanPanel>();
        _planC = GetObject(GameObjects.PlanC).GetOrAddComponent<UI_PlanPanel>();

        return true;
    }

    public void EnableBatteryChargePlan()
    {
        _batteryChargePlan.gameObject.SetActive(true);
        UpdateBatteryCount(0);
        _batteryChargePlan.SetHintText(MakeHintFromSectorName(new[] {Define.SectorName.ContainmentControlRoom, Define.SectorName.StaffAccommodation, Define.SectorName.Cafeteria}));
    }

    public void UpdateBatteryCount(int count)
    {
        _batteryChargePlan.SetObjectiveText($"Insert batteries in battery charger ({count}/{Define.BATTERY_CHARGE_GOAL})", true);
        if (count >= Define.BATTERY_CHARGE_GOAL)
        {
            _batteryChargePlan.SetObjectiveText($"Insert batteries in battery charger ({count}/{Define.BATTERY_CHARGE_GOAL})", true, true);

            StartCoroutine(OnBatteryChargeFinished());
        }
    }
    /// 
    /// Plan A: 엘리베이터 조작 컴퓨터에 USBKey 일정 개수 사용 -> 엘리베이터 조작 키패드 상호작용 -> 엘리베이터로 탈출
    /// 
    public void UpdateUSBKeyCount(int count)
    {
        _planA.SetObjectiveText($"Insert USB Keys in elevator control computer ({count}/{Define.USBKEY_INSERT_GOAL})", true);
        if (count >= Define.USBKEY_INSERT_GOAL)
        {
            _planA.SetObjectiveText($"Insert USB Keys in elevator control computer ({count}/{Define.USBKEY_INSERT_GOAL})", true, true);

            StartCoroutine(_planA.CompleteObjective("Activate the elevator and escape!",""));
        }
    }
    /// 
    /// Plan B: 중앙통제실 컴퓨터에 Cardkey사용 -> 중앙통제실 컴퓨터 상호작용 -> 화물통제 컴퓨터 상호작용 -> 화물 통로로 탈출 (화물 통로 문 자동 열림)
    /// 
    public void OnCardkeyUsed()
    {
        _planB.SetObjectiveText($"Use cardkey on central control computer", true, true);

        StartCoroutine(_planB.CompleteObjective("Use central control computer",
            MakeHintFromSectorName(new [] { Define.SectorName.CentralControlRoom })));
    }

    public void OnCentralControlComputerWorkFinished()
    {
        _planB.SetObjectiveText($"Use central control computer", true, true);

        StartCoroutine(_planB.CompleteObjective("Use cargo control computer to open cargo passage gate",
            MakeHintFromSectorName(new[] { Define.SectorName.CargoControlRoom, Define.SectorName.DirectorOffice})));
    }

    public void OnCargoPassageOpen()
    {
        _planB.SetObjectiveText($"Use cargo control computer to open cargo passage gate", true, true);

        StartCoroutine(_planB.CompleteObjective("Escape through the cargo passage gate!", ""));
    }

    private IEnumerator OnBatteryChargeFinished()
    {
        var titleText = GetText(Texts.Main_Title_text);

        yield return StartCoroutine(_batteryChargePlan.CompleteObjective("", ""));
        yield return titleText.DOFade(0, 1f).WaitForCompletion();

        _batteryChargePlan.gameObject.SetActive(false);
        titleText.SetText("Complete any plan to escape!");
        titleText.DOFade(1, 1f);

        _planA.gameObject.SetActive(true);
        _planA.SetObjectiveText($"Insert USB Keys in elevator control computer (0/{Define.USBKEY_INSERT_GOAL})", false);
        _planA.SetHintText(MakeHintFromSectorName(new[] { Define.SectorName.PowerRoom, Define.SectorName.VisitingRoom }));

        _planB.gameObject.SetActive(true);
        _planB.SetObjectiveText($"Use cardkey on central control computer", false);
        _planB.SetHintText(MakeHintFromSectorName(new[] {Define.SectorName.CentralControlRoom }));
    }

    private static string MakeHintFromSectorName(Define.SectorName[] loc)
    {
        string text = $"Loc: {loc[0]}";

        for (int i = 1; i < loc.Length; i++)
        {
            text += $", {loc[i]}";
        }

        return text;
    }
}

