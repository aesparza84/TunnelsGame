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

    [Header("Setting Lerp Speed")]
    [SerializeField] private float SettingSwitchSpeed;

    //Voluem component add-on
    private VhsVol _vhsPostProcess;
    private VHS_SETTING _vhsSetting;

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

        EnterVHS_State(VHS_SETTING.FAR);
    }

    private void LateUpdate()
    {
        VHS_EnemyCheck();

        if (SwitchVHS)
        {
            LerpVHSSetting();
        }
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
        if (_vhsSetting == newSetting)
            return;

        _vhsSetting = newSetting;
        SwitchVHS = true;

        switch (_vhsSetting)
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
}
