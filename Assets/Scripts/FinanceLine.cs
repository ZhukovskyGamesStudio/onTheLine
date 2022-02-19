using UnityEngine;
using UnityEngine.UI;

public class FinanceLine : MonoBehaviour
{
    public Text nameText, priceText;
    public Button button;
    public GameObject CrossedLine;
    public void SetValues(string name, int price)
    {
        this.nameText.text = name;
        this.priceText.text = price.ToString() + " P";
    }

    public void SetCrossed(bool isCrossed)
    {
        CrossedLine.SetActive(isCrossed);
        if (isCrossed)
            CrossedLine.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 2) == 1 ? 180 : 0);
    }
}
