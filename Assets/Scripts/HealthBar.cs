using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Text _healthText;

    void Start()
    {

        _slider.maxValue = SceneManager.Instance.Player.GetHp();
        _slider.value = SceneManager.Instance.Player.GetHp();
    }

    void Update()
    {
        _healthText.text = SceneManager.Instance.Player.GetHp().ToString();
        _slider.value = SceneManager.Instance.Player.GetHp();
    }
}
