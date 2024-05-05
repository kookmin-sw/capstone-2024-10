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

        private bool _textLock;
        private bool _textChangedflag;
        private string _savedObjectiveText;
        enum Texts
        {
            Objective_Text,
        }

        public override bool Init()
        {
            if (base.Init() == false)
                return false;

            Bind<TMP_Text>(typeof(Texts));
            _objectiveText = GetText(Texts.Objective_Text);
            return true;
        }

        public void SetText(string text, bool isProgress, bool isComplete = false)
        {
            if (_textLock)
            {
                _savedObjectiveText = text;
                _textChangedflag = true;
                return;
            }
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


        public IEnumerator CompleteObjective(string nextObjective)
        {
            _textLock = true;
            yield return _colorTweener.WaitForCompletion();
            yield return new WaitForSeconds(1f);
            _objectiveText.fontStyle = FontStyles.Strikethrough;

            yield return _objectiveText.DOColor(Color.gray, 1f).WaitForCompletion();

            if(string.IsNullOrEmpty(nextObjective)) yield break;

            yield return new WaitForSeconds(1f);

            yield return _objectiveText.DOFade(0f, 0.5f).WaitForCompletion();

            _textLock = false;
            _objectiveText.DOFade(1f, 1f);
            _objectiveText.fontStyle = FontStyles.Normal;
            _objectiveText.color = Color.white;
            if (_textChangedflag)
            {   
                SetText(_savedObjectiveText, false);
                _textChangedflag = false;
            }
            else
            {
                SetText(nextObjective, false);
            }
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
        _planA.gameObject.SetActive(false);
        _planB.gameObject.SetActive(false);
        _planC.gameObject.SetActive(false);

        return true;
    }

    public void UpdateBatteryCount(int count)
    {
        _batteryChargePlan.SetText($"Find and insert batteries in battery charger ({count}/{Define.BATTERY_CHARGE_GOAL})", true);
        if (count >= Define.BATTERY_CHARGE_GOAL)
        {
            _batteryChargePlan.SetText($"Collect and charge the batteries ({count}/{Define.BATTERY_CHARGE_GOAL})", true, true);

            StartCoroutine(OnBatteryChargeFinished());
        }
    }

    public void UpdateUSBKeyCount(int count)
    {
        _planA.SetText($"Find and insert USB Keys in elevator control computer({count}/{Define.USBKEY_INSERT_GOAL})", true);
        if (count >= Define.USBKEY_INSERT_GOAL)
        {
            _planA.SetText($"Find and insert USB Keys in elevator control computer({count}/{Define.USBKEY_INSERT_GOAL})", true, true);

            StartCoroutine(_planA.CompleteObjective("Activate the elevator and escape!"));
        }
    }

    public void OnCardkeyUsed()
    {
        _planB.SetText($"Find and use cardkey on main control computer (1/1)", true, true);

        StartCoroutine(_planB.CompleteObjective("Use cargo control computer to unlock the cargo passage gate (0/1)"));
    }

    public void OnCargoComputerWorkFinished()
    {
        _planB.SetText($"Use cargo control computer to unlock the cargo passage gate (1/1)", true, true);

        StartCoroutine(_planB.CompleteObjective("Open the the cargo passage gate and escape!"));
    }

    private IEnumerator OnBatteryChargeFinished()
    {
        var titleText = GetText(Texts.Main_Title_text);

        StartCoroutine(_batteryChargePlan.CompleteObjective(""));
        yield return titleText.DOFade(0, 1f).WaitForCompletion();

        titleText.SetText("Complete any plan to escape!");
        titleText.DOFade(1, 1f);

        _planA.gameObject.SetActive(true);
        _planA.SetText($"Find and insert USB Keys in elevator control computer (0/{Define.USBKEY_INSERT_GOAL})", false);
        _planB.gameObject.SetActive(true);
        _planB.SetText($"Find and use cardkey on main control computer (0/1)", false);
    }

    public void OnGeneratorRestored()
    {
        _planA.SetText($"b. Restore the backup generator (1/1)", true, true);

        StartCoroutine(_planA.CompleteObjective("c. Escape through the elevator."));
    }
}

