using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThanhMau : MonoBehaviour
{
    public Image _thanhmau;

    private void Start()
    {
        if (_thanhmau == null)
        {
            Debug.LogError("Thanh máu chưa được gán. Vui lòng kéo và thả đối tượng Image vào trường _thanhmau trong Inspector.");
        }
    }

    public void capNhatThanhMau(float luongmauhientai, float luongmautoida)
    {
        if (_thanhmau != null)
        {
            _thanhmau.fillAmount = luongmauhientai / luongmautoida;
        }
        else
        {
            Debug.LogWarning("Thanh máu không được gán, không thể cập nhật thanh máu.");
        }
    }
}