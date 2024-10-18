using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.VirtualTexturing;
using VolFx;
public enum VHS_SETTING { FAR, NEAR, POINTBLANK }
public class VHSSwitcher : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private Volume _Volume;

    [SerializeField] private Color healColor;
    [SerializeField] private Color critialColor;
    [SerializeField] private float VignettSpeed;
    private Vignette _VolVignete;

    [Header("VHS SO")]
    [SerializeField] private VHSSO MaxVHSSettings;
    [SerializeField] private VHSSO EntranceExitVHSSettings;
    [SerializeField] private VHSSO DeathVHSSettings;
    [SerializeField] private VHSSO GameOverVHSSettings;
    [SerializeField] private VHSSO EvilTransitionSettings;
    [SerializeField] private float LerpSpeed;
    [SerializeField] private float TargetRegularWeight;

    [Header("Audio")]
    [SerializeField] private bool UsingSound;
    [SerializeField] private Sound GlitchSound;
    [SerializeField] private Sound DeathGlitchSound;
    [SerializeField] private float MaxAudioDistance;
    private float currentVol;
    private AudioSource _audioSource;

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
    private bool EvilTransition;

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
        _audioSource = GetComponent<AudioSource>();

        _enemyColliders = new Collider[5];

        if (_Volume.profile.TryGet<VhsVol>(out VhsVol v))
        {
            _vhsPostProcess = v;
        }

        if (_Volume.profile.TryGet<Vignette>(out Vignette vi))
        {
            _VolVignete = vi;
        }

        LevelMessanger.LevelFinished += OnExitTransition;
        LevelMessanger.LevelStart += OnEntranceTransition;
        LevelMessanger.PlayerReset += IntroSound; 
        LevelMessanger.GameLoopStopped += SwitchToDeathVHS;
        LevelMessanger.DifficultyIncrease += DiffictultyVisualUpdate;
        RatKillEvent.KilledAnimFinished += SwitchToGameOverVHS;
        HealthComponent.OnHeal += HealVisuals;

        InstantSwitchSettings(EntranceExitVHSSettings);

    }

    private void IntroSound(object sender, System.EventArgs e)
    {
        ChangeSound(GlitchSound);
        _audioSource.volume = 1;
    }

    private void HealVisuals(object sender, System.EventArgs e)
    {
        _VolVignete.color.value = healColor;
        _VolVignete.active = true;
        _VolVignete.intensity.value = 0.47f;
    }

    private void DiffictultyVisualUpdate(object sender, System.EventArgs e)
    {
        EvilTransition = true;
    }

    private void SwitchToGameOverVHS(object sender, System.EventArgs e)
    {
        InstantSwitchSettings(GameOverVHSSettings);
    }

    private void SwitchToDeathVHS(object sender, System.EventArgs e)
    {
        ActiveComponent = false;
        _audioSource.volume = 1;
        ChangeSound(DeathGlitchSound);
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
            _audioSource.volume = currentVHSWeight;
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

        //Set vol to 0
        _audioSource.volume = 0;

        if (EvilTransition)
        {
            ConfigureNewVHSSettings(EvilTransitionSettings);
            _audioSource.volume = 1;
            ChangeSound(DeathGlitchSound);
        }
        else
        {
            ConfigureNewVHSSettings(EntranceExitVHSSettings);
        }

        currentVHSWeight = _vhsPostProcess._weight.value;
        while (currentVHSWeight < 1)
        {
            currentVHSWeight = Mathf.MoveTowards(currentVHSWeight, 1, LerpSpeed * Time.deltaTime);
            _audioSource.volume = Mathf.Lerp(_audioSource.volume, 1, LerpSpeed *Time.deltaTime);
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

        if (_VolVignete.intensity.value > 0)
        {
            _VolVignete.intensity.value = Mathf.MoveTowards(_VolVignete.intensity.value, 0, VignettSpeed * Time.deltaTime);
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
                //EnterVHS_State(VHS_SETTING.POINTBLANK);
                UpdateVHSSettings(lowestDist, PointBlank_Distance, PointBlank_VHS);

            }
            else if (lowestDist <= Near_Distance)
            {
                //EnterVHS_State(VHS_SETTING.NEAR);
                UpdateVHSSettings(lowestDist, Near_Distance, Near_VHS);

            }
            else if (lowestDist <= Far_Distance)
            {
                //EnterVHS_State(VHS_SETTING.FAR);
                UpdateVHSSettings(lowestDist, Far_Distance, Far_VHS);
            }

            if (UsingSound)
            {
                if (_audioSource != null)
                {
                    SetGlitchSound(lowestDist);
                }

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

    private void UpdateVHSSettings(float distance, float targetDist, float cap)
    {
        currentVHSWeight = Mathf.InverseLerp(Far_VHS, targetDist, distance);
        currentVHSWeight = Mathf.Clamp(currentVHSWeight, Far_VHS, cap);

        _vhsPostProcess._weight.value = currentVHSWeight;
    }

    private void SetGlitchSound(float distance)
    {
        //PointBlank_Distance = 2
        currentVol = Mathf.InverseLerp(MaxAudioDistance, PointBlank_Distance, distance);
        currentVol = Mathf.Clamp(currentVol, 0, 1);
        _audioSource.volume = currentVol;
    }

    private void ChangeSound(Sound s)
    {
        _audioSource.clip = s.soundClip;
        _audioSource.priority = s.Priority;
        _audioSource.volume = s.Volume;
        _audioSource.pitch = s.Pitch;
        _audioSource.panStereo = s.StereoPan;
        _audioSource.spatialBlend = s.SpatialBlend;
        _audioSource.minDistance = s.MinDistance;
        _audioSource.maxDistance = s.MaxDistance;
        _audioSource.Play();
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
        LevelMessanger.PlayerReset -= IntroSound;
        LevelMessanger.GameLoopStopped -= SwitchToDeathVHS;
        LevelMessanger.DifficultyIncrease -= DiffictultyVisualUpdate;
        RatKillEvent.KilledAnimFinished -= SwitchToGameOverVHS;
        HealthComponent.OnHeal -= HealVisuals;
    }
}
