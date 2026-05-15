using UnityEngine;
using SoulKnight.Weapons;

namespace SoulKnight.Player
{
    /// <summary>
    /// Handles weapon equipping, aiming, and shooting input.
    /// Supports mouse aim (PC) and right-joystick aim (mobile/gamepad).
    /// </summary>
    public class PlayerShooter : MonoBehaviour
    {
        [Header("Weapon Slots")]
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private int maxWeaponSlots = 2;

        private BaseWeapon[] weapons;
        private int currentWeaponIndex = 0;
        private Camera mainCamera;

        private void Awake()
        {
            weapons = new BaseWeapon[maxWeaponSlots];
            mainCamera = Camera.main;
        }

        private void Update()
        {
            AimWeapon();
            HandleShootInput();
            HandleWeaponSwitch();
        }

        private void AimWeapon()
        {
            if (weapons[currentWeaponIndex] == null) return;

            // Mouse aiming
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            Vector2 aimDir = (mouseWorld - transform.position).normalized;

            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            weaponHolder.rotation = Quaternion.Euler(0, 0, angle);

            // Flip weapon sprite based on aim direction
            Vector3 scale = weaponHolder.localScale;
            scale.y = aimDir.x < 0 ? -1f : 1f;
            weaponHolder.localScale = scale;
        }

        private void HandleShootInput()
        {
            if (weapons[currentWeaponIndex] == null) return;

            if (Input.GetButton("Fire1"))
                weapons[currentWeaponIndex].TryShoot();
        }

        private void HandleWeaponSwitch()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f) CycleWeapon(1);
            else if (scroll < 0f) CycleWeapon(-1);

            if (Input.GetKeyDown(KeyCode.Q)) CycleWeapon(-1);
            if (Input.GetKeyDown(KeyCode.E)) CycleWeapon(1);
        }

        private void CycleWeapon(int direction)
        {
            currentWeaponIndex = (currentWeaponIndex + direction + maxWeaponSlots) % maxWeaponSlots;
            RefreshWeaponVisibility();
        }

        public bool PickupWeapon(BaseWeapon newWeapon)
        {
            // Find empty slot first
            for (int i = 0; i < maxWeaponSlots; i++)
            {
                if (weapons[i] == null)
                {
                    EquipWeapon(newWeapon, i);
                    return true;
                }
            }

            // Replace current weapon
            DropCurrentWeapon();
            EquipWeapon(newWeapon, currentWeaponIndex);
            return true;
        }

        private void EquipWeapon(BaseWeapon weapon, int slot)
        {
            weapons[slot] = weapon;
            weapon.transform.SetParent(weaponHolder);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            RefreshWeaponVisibility();
        }

        private void DropCurrentWeapon()
        {
            if (weapons[currentWeaponIndex] == null) return;
            weapons[currentWeaponIndex].transform.SetParent(null);
            // TODO: spawn pickup item at player position
            weapons[currentWeaponIndex] = null;
        }

        private void RefreshWeaponVisibility()
        {
            for (int i = 0; i < maxWeaponSlots; i++)
            {
                if (weapons[i] != null)
                    weapons[i].gameObject.SetActive(i == currentWeaponIndex);
            }
        }

        public BaseWeapon GetCurrentWeapon() => weapons[currentWeaponIndex];
    }
}
