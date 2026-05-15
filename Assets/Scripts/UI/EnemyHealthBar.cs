using UnityEngine;
using UnityEngine.UI;

namespace SoulKnight.UI
{
    /// <summary>
    /// World-space health bar for enemies.
    /// Attach to an enemy child GameObject that has a Canvas (World Space).
    /// </summary>
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private GameObject barRoot;

        private Camera mainCam;

        private void Awake()
        {
            mainCam = Camera.main;
            if (barRoot) barRoot.SetActive(false);
        }

        public void Init(Enemies.BaseEnemy enemy)
        {
            enemy.OnHealthChanged.AddListener(UpdateBar);
        }

        private void UpdateBar(int current, int max)
        {
            if (barRoot) barRoot.SetActive(true);
            if (slider) slider.value = (float)current / max;

            if (current <= 0 && barRoot) barRoot.SetActive(false);
        }

        private void LateUpdate()
        {
            // Always face the camera
            if (mainCam) transform.rotation = mainCam.transform.rotation;
        }
    }
}
