using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.VirtualTexturing;
using VolFx;
public enum VHS_SETTING { FAR, NEAR, POINTBLANK }
public class VHSSwitcher : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private Volume _Volume;

    [Header("VHS SO")]
    [SerializeField] private VHSSO MaxVHSSettings;
    [SerializeField] private VHSSO EntranceExitVHSSettings;
    [SerializeField] private VHSSO DeathVHSSettings;
    [SerializeField] private VHSSO GameOverVHSSettings;
    [SerializeField] private float LerpSpeed;
    [SerializeField] private float TargetRegularWeight;

    [Header("Target Weight Percent")]
    [Range(0f, 1f)]
    [SerializeField] private float Far_VHS;
    [SerializeField] private float Far_Distance;
    [Range(0f, 1f)]
    [SerializeField] private float Near_VHS;
    [SerializeField] private float Near_Distance;
    [Range(0f, 1f)]
    [SerializeField] private float PointBlank_VHS;
    [SerializeField] private float PointBlank_Distance;
    private float currentVHSWeight;
    private float targetWeight;
    private bool SwitchVHS;

    private bool ActiveComponent;

    [Header("Setting Lerp Speed")]
    [SerializeField] private float SettingSwitchSpeed;

    //Voluem component add-on
    private VhsVol _vhsPostProcess;
    private VHS_SETTING _vhsDistanceSetting;

    [Header("Player Object")]
    [SerializeField] private GameObject PlayerObj;

    private const int EnemyMask = (1 << 7);
    private const int CheckRadius = 10;
    private Collider[] _enemyColliders;

    private void Start()
    {
        _enemyColliders = new Collider[5];

        if (_Volume.profile.TryGet<VhsVol>(out VhsVol v))
        {
            _vhsPostProcess = v;
        }

        LevelMessanger.LevelFinished += OnExitTransition;
        LevelMessanger.LevelStart += OnEntranceTransition;
        LevelMessanger.GameLoopStopped += SwitchToDeathVHS;
        RatKillEvent.KilledAnimFinished += SwitchToGameOverVHS;

        ConfigureNewVHSSettings(MaxVHSSettings);
    }

    private void SwitchToGameOverVHS(object sender, System.EventArgs e)
    {
        InstantSwitchSettings(GameOverVHSSettings);
    }

    private void SwitchToDeathVHS(object sender, System.EventArgs e)
    {
        ActiveComponent = false;
        InstantSwitchSettings(DeathVHSSettings);
    }

    private void OnEntranceTransition(object sender, System.EventArgs e)
    {
        StopCoroutine(VHS_Exit());
        StartCoroutine(VHS_Entrance());
    }

    private void OnExitTransition(object sender, System.EventArgs e)
    {
        StartCoroutine(VHS_Exit());
    }

    private IEnumerator VHS_Entrance()
    {
        currentVHSWeight = _vhsPostProcess._weight.value;

        while (currentVHSWeight > 0)
        {
            currentVHSWeight = Mathf.MoveTowards(currentVHSWeight, 0, LerpSpeed * Time.deltaTime);
            _vhsPostProcess._weight.value = currentVHSWeight;
            yield return null;
        }

        ConfigureNewVHSSettings(MaxVHSSettings);

        while (currentVHSWeight < TargetRegularWeight)
        {
            currentVHSWeight = Mathf.MoveTowards(currentVHSWeight, TargetRegularWeight, LerpSpeed * Time.deltaTime);
            _vhsPostProcess._weight.value = currentVHSWeight;
            yield return null;
        }

        ActiveComponent = true;
        EnterVHS_State(VHS_SETTING.FAR);

        yield return null;
    }
    private IEnumerator VHS_Exit()
    {
        ActiveComponent = false;

        _vhsPostProcess._weight.value = 0.0f;
        ConfigureNewVHSSettings(EntranceExitVHSSettings);

        currentVHSWeight = _vhsPostProcess._weight.value;
        while (currentVHSWeight < 1)
        {
            currentVHSWeight = Mathf.MoveTowards(currentVHSWeight, 1, LerpSpeed * Time.deltaTime);
            _vhsPostProcess._weight.value = currentVHSWeight;
            yield return null;
        }

        yield return null;
    }

    private void LateUpdate()
    {
        if (!ActiveComponent)
        {
            return;
        }

        VHS_EnemyCheck();

        if (SwitchVHS)
        {
            LerpVHSSetting();
        }
    }

    private void ConfigureNewVHSSettings(VHSSO settings)
    {
        _vhsPostProcess._bleed.value = settings.BleedVal;
        _vhsPostProcess._rocking.value = settings.RockingVal;
        _vhsPostProcess._tape.value = settings.TapeVal;
        _vhsPostProcess._noise.value = settings.NoiseVal;
        _vhsPostProcess._flickering.value = settings.FlickeringVal;
        _vhsPostProcess._glitch.value = settings.GlitchColor;
    }

    /// <summary>
    /// Adjusts VHS intensity based on how close an 'enemy' can be to the player 
    /// </summary>
    private void VHS_EnemyCheck()
    {
        if (PlayerObj == null)
            return;

        int count = Physics.OverlapSphereNonAlloc(PlayerObj.transform.position, CheckRadius, _enemyColliders, EnemyMask);

        if (count > 0)
        {
            float lowestDist = 100;

            for (int i = 0; i < count; i++)
            {
                float dist = Vector3.Distance(PlayerObj.transform.position, _enemyColliders[i].transform.position);

                if (dist < lowestDist)
                {
                    lowestDist = dist;
                }
            }

            if (lowestDist <= PointBlank_Distance)
            {
                EnterVHS_State(VHS_SETTING.POINTBLANK);

            }
            else if (lowestDist <= Near_Distance)
            {
                EnterVHS_State(VHS_SETTING.NEAR);

            }
            else if (lowestDist <= Far_Distance)
            {
                EnterVHS_State(VHS_SETTING.FAR);
            }

        }
    }

    private void EnterVHS_State(VHS_SETTING newSetting)
    {
        if (_vhsDistanceSetting == newSetting)
            return;

        _vhsDistanceSetting = newSetting;
        SwitchVHS = true;

        switch (_vhsDistanceSetting)
        {
            case VHS_SETTING.FAR:
                targetWeight = Far_VHS;

                break;
            case VHS_SETTING.NEAR:
                targetWeight = Near_VHS;

                break;
            case VHS_SETTING.POINTBLANK:
                targetWeight = PointBlank_VHS;

                break;
            default:
                break;
        }
    }

    private void LerpVHSSetting()
    {
        if (currentVHSWeight != targetWeight)
        {
            currentVHSWeight = Mathf.MoveTowards(currentVHSWeight, targetWeight, SettingSwitchSpeed * Time.deltaTime);
            _vhsPostProcess._weight.value = currentVHSWeight;

        }
        else
        {
            //Switch off lerp
            SwitchVHS = false;
        }
    }

    private void InstantSwitchSettings(VHSSO setting)
    {
        ConfigureNewVHSSettings(setting);
        _vhsPostProcess._weight.value = 1;
    }


    private void OnDisable()
    {
        LevelMessanger.LevelFinished -= OnExitTransition;
        LevelMessanger.LevelStart -= OnEntranceTransition;
        LevelMessanger.GameLoopStopped -= SwitchToDeathVHS;
        RatKillEvent.KilledAnimFinished -= SwitchToGameOverVHS;
    }
}
