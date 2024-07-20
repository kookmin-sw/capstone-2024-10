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

        GetText(Texts.Main_Title_text).SetText("Restore Power Of Facility!");
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
        _batteryChargePlan.SetObjectiveText($"Insert Batteries in Battery Charger ({count}/{Define.BATTERY_CHARGE_GOAL})", true);
        if (count >= Define.BATTERY_CHARGE_GOAL)
        {
            _batteryChargePlan.SetObjectiveText($"Insert Batteries in Battery Charger ({count}/{Define.BATTERY_CHARGE_GOAL})", true, true);

            StartCoroutine(OnBatteryChargeFinished());
        }
    }
    ///
    /// Plan A: 엘리베이터 조작 컴퓨터에 USBKey 일정 개수 사용 -> 엘리베이터 조작 키패드 상호작용 -> 엘리베이터로 탈출
    ///
    public void UpdateUSBKeyCount(int count)
    {
        _planA.SetObjectiveText($"Insert USB Keys in Elevator Control Computer ({count}/{Define.USBKEY_INSERT_GOAL})", true);
        if (count >= Define.USBKEY_INSERT_GOAL)
        {
            _planA.SetObjectiveText($"Insert USB Keys in Elevator Control Computer ({count}/{Define.USBKEY_INSERT_GOAL})", true, true);

            StartCoroutine(_planA.CompleteObjective("Activate the Elevator and Escape!",""));
        }
    }
    ///
    /// Plan B: 중앙통제실 컴퓨터에 Cardkey사용 -> 중앙통제실 컴퓨터 상호작용 -> 화물통제 컴퓨터 상호작용 -> 화물 통로로 탈출 (화물 통로 문 자동 열림)
    ///
    public void OnCardkeyUsed()
    {
        _planB.SetObjectiveText($"Use Card Key on Central Control Computer", true, true);

        StartCoroutine(_planB.CompleteObjective("Use Central Control Computer",
            MakeHintFromSectorName(new [] { Define.SectorName.CentralControlRoom })));
    }

    public void OnCentralControlComputerWorkFinished()
    {
        _planB.SetObjectiveText($"Use Central Control Computer", true, true);

        StartCoroutine(_planB.CompleteObjective("Use Cargo Control Computer To Open Cargo Gate",
            MakeHintFromSectorName(new[] { Define.SectorName.CargoControlRoom, Define.SectorName.DirectorOffice})));
    }

    public void OnCargoPassageOpen()
    {
        _planB.SetObjectiveText($"Use Cargo Control Computer to Open Cargo Gate", true, true);

        StartCoroutine(_planB.CompleteObjective("Escape Through the Cargo Gate!", ""));
    }

    private IEnumerator OnBatteryChargeFinished()
    {
        var titleText = GetText(Texts.Main_Title_text);

        yield return StartCoroutine(_batteryChargePlan.CompleteObjective("", ""));
        yield return titleText.DOFade(0, 1f).WaitForCompletion();

        _batteryChargePlan.gameObject.SetActive(false);
        titleText.SetText("Complete Any Plan to Escape!");
        titleText.DOFade(1, 1f);

        _planA.gameObject.SetActive(true);
        _planA.SetObjectiveText($"Insert USB Keys in Elevator Control Computer (0/{Define.USBKEY_INSERT_GOAL})", false);
        _planA.SetHintText(MakeHintFromSectorName(new[] { Define.SectorName.PowerRoom, Define.SectorName.VisitingRoom }));

        _planB.gameObject.SetActive(true);
        _planB.SetObjectiveText($"Use Card Key on Central Control Computer", false);
        _planB.SetHintText(MakeHintFromSectorName(new[] {Define.SectorName.CentralControlRoom }));

        if (Managers.GameMng.GameEndSystem.CrewNum == 1)
        {
            Managers.GameMng.PlanSystem.EnablePlanC();
        }
    }

    public void EnablePlanC()
    {
        _planC.gameObject.SetActive(true);
        _planC.SetObjectiveText($"Use Emergency Control Device to Open Panic Room", false);
        _planC.SetHintText(MakeHintFromSectorName(new[] { Define.SectorName.Oratory, Define.SectorName.SampleRoom }));
    }

    public void OnPanicRoomActivated()
    {
        _planC.SetObjectiveText("Use Emergency Control Device to Open Panic Room", true, true);

        StartCoroutine(_planC.CompleteObjective("Find Open Panic Room and Escape!", "Hint: Only Two Panic Rooms are Open"));
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

