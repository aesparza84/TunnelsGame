using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [Header("Dialogue TextMeshPro")]
    [SerializeField] private TextMeshProUGUI _DialogueText;

    [Header("Dialogue Stuff")]
    [SerializeField] private string[] NeedCheeseDial;
    [SerializeField] private string[] HasCheeseDial;
    [SerializeField] private string AttackDial;

    private void Start()
    {
        AdjustRoom.ExitNoCheese += OnNoCheeseExit;
        CheeseItem.CheesePickedUp += OnCheesePickedUp;
        PlayerController.OnAttackStatic += OnAttackDialogue;
    }

    private void OnAttackDialogue(object sender, System.EventArgs e)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayText(AttackDial));
    }

    private void OnCheesePickedUp(object sender, System.EventArgs e)
    {
        StopAllCoroutines();

        int n = UnityEngine.Random.Range(0, HasCheeseDial.Length);
        StartCoroutine(DisplayText(HasCheeseDial[n]));
    }

    private void OnNoCheeseExit(object sender, System.EventArgs e)
    {
        StopAllCoroutines();
        
        int n = UnityEngine.Random.Range(0, NeedCheeseDial.Length);
        StartCoroutine(DisplayText(NeedCheeseDial[n]));
    }

    private IEnumerator DisplayText(string txt)
    {
        _DialogueText.text = txt;
        _DialogueText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3);
        _DialogueText.gameObject.SetActive(false);

        yield return null;
    }

    private void OnDisable()
    {
        AdjustRoom.ExitNoCheese -= OnNoCheeseExit;
        CheeseItem.CheesePickedUp -= OnCheesePickedUp;

    }
}
