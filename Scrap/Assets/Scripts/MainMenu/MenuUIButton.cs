using UnityEngine;

namespace Menu.UI
{
    public class MenuUIButton : MonoBehaviour
    {
        public bool showHoverUIOnPC = false;
        [SerializeField] private GameObject hoverUIObject;

        public void OnButtonHover()
        {
            if (hoverUIObject != null)
            {
                if (MenuManager.Instance.isKeyboardAndMouse)
                {
                    if (showHoverUIOnPC)
                        hoverUIObject.SetActive(true);
                }
                else
                {
                    hoverUIObject.SetActive(true);
                }
            }
            MenuManager.Instance.PlayButtonSoundOnce(false);
        }

        public void OnStopHover()
        {
            if (hoverUIObject != null)
                hoverUIObject.SetActive(false);
        }
    }
}