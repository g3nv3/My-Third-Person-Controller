using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour
{
    [SerializeField] private Image[] _staminaBatImages;
    [SerializeField] private PlayerController _playerController;
     void Start()
    {
        
    }

    void Update()
    {
        EditStaminaBar();
    }

    private void EditStaminaBar()
    {
        if (_playerController.Stamina == _playerController.StartStamina)
            foreach (var image in _staminaBatImages)
                image.gameObject.SetActive(false);
        else
            foreach (var image in _staminaBatImages)
            {
                image.gameObject.SetActive(true);
                image.fillAmount = _playerController.Stamina / 100f;
            }
                
    }
}
